define(["require", "exports", "knockout", "loglevel", "modules/bwf-metadata", "scripts/sprintf", "options", "modules/bwf-utilities", "modules/bwf-explorer", "loglevel"], function (require, exports, knockout, logLevel, metadataService, sprintf, options, bwf, explorer, log) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var SelectedParameterViewModel = /** @class */ (function () {
        function SelectedParameterViewModel(params) {
            var _this = this;
            this.clearButtonText = options.resources["bwf_clear"];
            this.gridColumns = ko.observableArray([]);
            this.loaded = ko.observable(false);
            this.metadata = ko.observable(null);
            this.rendered = ko.observable(false);
            this.selectedParameters = ko.observableArray([]);
            this.parametersGridItems = ko.observableArray([]);
            this.subscriptions = ko.observableArray([]);
            this.viewGridId = "selectedParametersGrid";
            this.enableParameterSelectionMove = ko.pureComputed(function () { return _this.selectedParameterGridItems().length === 1; });
            this.enableParameterSelectionRemove = ko.pureComputed(function () { return _this.selectedParameterGridItems().length > 0; });
            this.enableClear = ko.pureComputed(function () { return _this.selectedParameters().length > 0; });
            // we need this as a closure so that the references to 'this'
            // work correctly when we are using the method as a callback
            this.updatePositions = function () {
                var parameterGridItems = _this.parametersGridItems();
                parameterGridItems.sort(function (l, r) { return l.queryPosition() - r.queryPosition(); });
                var position = 1;
                parameterGridItems.map(function (x, index) { return x.queryPosition(index); });
                _this.syncToRecord();
            };
            logLevel.debug('Data passed into selected parameter view model: ', params);
            this.baseType = params.model.observables['BaseType'];
            this.dataService = params.model.observables['System'] || params.model.observables['DataService'];
            this.isCreate = params.model.state.isCreate;
            this.formDisabled = params.model.observables['formDisabled'];
            this.gridId = params.grid;
            this.data = {
                instanceName: 'cc-selectedParameter',
                baseType: this.baseType,
                dataService: this.dataService,
                formDisabled: this.formDisabled,
                isCreate: this.isCreate,
                renderedState: params.model.observables['__renderedState'],
                model: params.model
            };
            this.propertyMetadata = params.metadata;
            var selectedParametersBase = params.model.observables[params.metadata.name]() || [];
            // computeds
            this.ready = knockout.pureComputed(function () { return _this.rendered() && _this.loaded(); });
            params.model.observables['__renderedState'].push(this.ready);
            this.selectedParameterGridItems = ko.pureComputed(function () {
                return _this.parametersGridItems().filter(function (x) { return Object.keys(x.values).map(function (k) { return x.values[k]; }).some(function (v) { return v.isSelected(); }); });
            });
            this.isThereValidationErrors = ko.pureComputed(function () { return !_this.parametersGridItems().every(function (x) { return x.values["Alias"].isValid(); }); }).extend({ notify: 'always' });
            // make panel wide
            knockout.postbox.publish(params.grid + '-togglePanelWidth', true);
            // subscriptions
            this.subscriptions.push(this.isThereValidationErrors.subscribe(function (val) { return params.model.observables['customControlDisableSave'](val); }));
            this.subscriptions.push(this.parametersGridItems.subscribe(function (x) { return x.forEach(function (y) {
                y.configureColumns(_this.gridColumns());
                y.values["Alias"].extend({
                    validAlias: {
                        message: "Alias cannot contain \\"
                    },
                    notify: 'always' // need this so that the savePropertyDisabled computed functions properly
                });
            }); }));
            this.subscriptions.push(this.dataService.subscribe(function (ds) { return _this.dataServiceSubscription(ds); }));
            this.subscriptions.push(this.baseType.subscribe(function (bt) { return _this.baseTypeSubscription(bt); }));
            this.subscriptions.push(this.addAvailablePropertyPostboxSubscription());
            // finish setting up parameters
            if (this.isCreate && (!this.dataService() || !this.baseType()))
                this.initialRefreshMetadataCallback(selectedParametersBase);
            else
                this.refreshMetadata(this.dataService(), this.baseType(), function () { return _this.initialRefreshMetadataCallback(selectedParametersBase); });
        }
        SelectedParameterViewModel.prototype.dataServiceSubscription = function (ds) {
            var md = this.metadata();
            if (md && ds) {
                var oldDataService = md.dataService;
                if (this.baseType())
                    this.refreshMetadata(ds, this.baseType());
                if (oldDataService && oldDataService !== ds) {
                    this.clearParameters();
                    this.syncToRecord();
                }
            }
            else {
                this.clearParameters();
                this.syncToRecord();
            }
        };
        SelectedParameterViewModel.prototype.baseTypeSubscription = function (bt) {
            var md = this.metadata();
            if (bt) {
                var oldType = md ? md.type : null;
                this.refreshMetadata(this.dataService(), bt);
                if (oldType !== bt) {
                    this.clearParameters();
                    this.syncToRecord();
                }
            }
            else {
                this.clearParameters();
                this.syncToRecord();
            }
        };
        SelectedParameterViewModel.prototype.addAvailablePropertyPostboxSubscription = function () {
            var _this = this;
            return knockout.postbox.subscribe(this.data.instanceName + '-cc-available-property-selected', function (item) { return _this.addProperty(item); });
        };
        SelectedParameterViewModel.prototype.addProperty = function (item) {
            var _this = this;
            logLevel.debug('Available property for addition to selected parameters:', item.name);
            logLevel.debug('Complete item:', item);
            var parameters = this.selectedParameters();
            var maxExistingPosition = 0;
            if (parameters.length !== 0) {
                maxExistingPosition = parameters.sort(function (l, r) { return l.Position - r.Position; })[parameters.length - 1].Position;
            }
            logLevel.debug('Max existing position', maxExistingPosition);
            var property = this.getProperty(item.name);
            var newItem = {
                Id: 0,
                Parameter: item.name,
                DisplayName: property.displayName,
                Position: maxExistingPosition + 1,
                Operator: "=",
                Title: property.description
            };
            this.selectedParameters.push(newItem);
            var columnRecord = {
                Id: newItem.Id.toString(),
                BaseTypeName: this.baseType(),
                Data: newItem,
                Position: newItem.Position,
                TypeName: "ViewParameter"
            };
            var gridItem = new explorer.ExplorerGridItem(columnRecord, this.gridColumns(), this.dataService(), {
                disableChangeTracking: true,
                disableValidateOnChange: true
            });
            gridItem.updateType("Added");
            // need a unique id for removing items correctly
            gridItem.bwfId = bwf.getNextBwfId().toString();
            // update the values on the record whenever they are changed
            Object.keys(gridItem.values).map(function (k) { return gridItem.values[k]; })
                .forEach(function (x) { return x.subscribe(function () { return _this.updateRecords(); }); });
            this.parametersGridItems.push(gridItem);
            this.syncToRecord();
        };
        SelectedParameterViewModel.prototype.initialRefreshMetadataCallback = function (selectedParametersBase) {
            var _this = this;
            this.selectedParameters(this.getParameters(selectedParametersBase));
            metadataService.getType("explorer", "ViewParameter").done(function (viewParameterMetadata) {
                var metadataClone = bwf.clone(viewParameterMetadata);
                metadataClone.properties['Parameter'].name = "DisplayName";
                metadataClone.properties['Parameter'].isNotEditableInGrid = true;
                metadataClone.properties['Operator'].useCustomControl = true;
                metadataClone.properties['Operator'].useCustomEditingCell = true;
                metadataClone.properties['Operator'].customEditingCell = "ds-explorer-grid;cc-operator";
                metadataClone.properties['Operator'].hasChoice = false;
                metadataClone.properties['Operator'].isNotEditableInGrid = false;
                metadataClone.properties['Alias'].displayName = options.resources["bwf_alias"];
                var columnsMetadata = [
                    metadataClone.properties['Parameter'],
                    metadataClone.properties['Operator'],
                    metadataClone.properties['Alias']
                ];
                _this.gridColumns(columnsMetadata.map(function (x, index) { return new explorer.ExplorerGridColumn(x, x.name, index + 1); }));
                _this.parametersGridItems(_this.selectedParameters().map(function (param) {
                    var columnRecord = {
                        Id: param.Id.toString(),
                        BaseTypeName: _this.baseType(),
                        Data: param,
                        Position: param.Position,
                        TypeName: "ViewParameter"
                    };
                    var gridItem = new explorer.ExplorerGridItem(columnRecord, _this.gridColumns(), _this.dataService(), {
                        disableChangeTracking: true,
                        disableValidateOnChange: true
                    });
                    if (gridItem.bwfId == "") {
                        gridItem.bwfId = bwf.getNextBwfId().toString();
                    }
                    return gridItem;
                }));
                _this.parametersGridItems().forEach(function (item) {
                    Object.keys(item.values).map(function (y) { return item.values[y]; }).forEach(function (v) {
                        _this.subscriptions.push(v.subscribe(function () { return _this.updateRecords(); }));
                        v.extend({ notify: 'always' });
                    });
                });
                if (_this.isCreate)
                    _this.loaded(true);
            });
        };
        SelectedParameterViewModel.prototype.clearParameters = function () {
            this.selectedParameters([]);
            this.parametersGridItems([]);
            this.syncToRecord();
        };
        SelectedParameterViewModel.prototype.dispose = function () {
            this.subscriptions().forEach(function (sub) { return sub.dispose(); });
        };
        SelectedParameterViewModel.prototype.updateRecords = function () {
            this.parametersGridItems().forEach(function (x) {
                var alias = x.values["Alias"]();
                if (alias != null && alias.trim() !== "") {
                    x.record.Alias = alias.trim();
                }
                else {
                    x.record.Alias = null;
                }
                x.record.Operator = x.values["Operator"]();
            });
            this.syncToRecord();
        };
        SelectedParameterViewModel.prototype.gridConfiguration = function () {
            var _this = this;
            var gridConfig = {
                createNewRecord: function () { return null; },
                disabled: this.formDisabled,
                disableInsertRecords: true,
                disableRemoveRecords: false,
                disableSoftDelete: true,
                header: {
                    enabled: knockout.observable(false),
                    name: null, config: null
                },
                footer: {
                    enabled: knockout.observable(false),
                    name: null, config: null
                },
                isView: false,
                metadata: this.metadata,
                viewGridId: this.viewGridId,
                disableGridSorting: ko.observable(true),
                canSelectIndividualCells: ko.observable(!this.formDisabled()),
                records: this.parametersGridItems,
                recordsCount: ko.pureComputed(function () { return _this.parametersGridItems().length; }),
                selectedColumns: knockout.pureComputed(function () { return _this.gridColumns(); }),
                selectedRecords: this.selectedParameterGridItems,
                inEditMode: ko.observable(true),
                postRender: function () { return; },
                validate: function () { return null; },
                embedded: true
            };
            return gridConfig;
        };
        SelectedParameterViewModel.prototype.getParameters = function (parameters) {
            var _this = this;
            var sortedParameters = parameters.sort(function (l, r) { return l.Position - r.Position; });
            var mappedParameters = sortedParameters.map(function (item, index) {
                var property = _this.getProperty(item.Parameter);
                if (property == null) {
                    log.warn("View parameter '" + item.Parameter + "' isn't valid for views of '" + _this.baseType() + "'");
                    return;
                }
                var element = {
                    Id: item.Id,
                    Parameter: item.Parameter,
                    Operator: item.Operator,
                    DisplayName: property.displayName,
                    Title: property.description,
                    Position: item.Position,
                    Alias: item.Alias
                };
                return element;
            });
            return mappedParameters.filter(function (p) { return p != null; });
        };
        SelectedParameterViewModel.prototype.getProperty = function (propertyName) {
            return metadataService.getPropertyWithPrefix(this.dataService(), this.metadata(), propertyName);
        };
        SelectedParameterViewModel.prototype.refreshMetadata = function (dataService, baseType, callback) {
            var _this = this;
            if (!dataService || !baseType) {
                logLevel.debug("Dataservice or basetype was invalid in refreshMetadata");
                this.metadata(null);
                this.selectedParameters([]);
                return;
            }
            metadataService.getType(dataService, baseType).done(function (metadata) {
                _this.metadata(metadata);
                _this.loaded(true);
                if (callback)
                    callback(metadata);
            }).fail(function () {
                logLevel.warn(sprintf.sprintf("Error occurred retrieving metadata for baseType '%s' (dataservice = '%s')", baseType, dataService));
                _this.metadata(null);
                _this.selectedParameters([]);
            });
        };
        SelectedParameterViewModel.prototype.syncToRecord = function () {
            var parameters = this.parametersGridItems().map(function (gridItem) {
                var record = gridItem.record;
                var selectedParam = {
                    Id: record.Id,
                    Parameter: record.Parameter,
                    Operator: record.Operator,
                    Position: gridItem.queryPosition(),
                    Alias: record.Alias
                };
                return selectedParam;
            });
            this.data.model.observables[this.propertyMetadata.name](parameters);
        };
        SelectedParameterViewModel.prototype.moveParameterSelectionToTop = function () {
            this.selectedParameterGridItems()[0].queryPosition(-1);
            this.updatePositions();
        };
        SelectedParameterViewModel.prototype.moveParameterSelectionUp = function () {
            var selected = this.selectedParameterGridItems()[0];
            selected.queryPosition(selected.queryPosition() - 1.5);
            this.updatePositions();
        };
        SelectedParameterViewModel.prototype.moveParameterSelectionDown = function () {
            var selected = this.selectedParameterGridItems()[0];
            selected.queryPosition(selected.queryPosition() + 1.5);
            this.updatePositions();
        };
        SelectedParameterViewModel.prototype.moveParameterSelectionToBottom = function () {
            this.selectedParameterGridItems()[0].queryPosition(9999);
            this.updatePositions();
        };
        SelectedParameterViewModel.prototype.removeSelectedParameters = function () {
            ko.postbox.publish(this.viewGridId + "-delete-row", { callback: this.updatePositions });
        };
        return SelectedParameterViewModel;
    }());
    exports.default = SelectedParameterViewModel;
});
