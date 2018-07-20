define(["require", "exports", "knockout", "sprintf", "modules/bwf-explorer", "modules/bwf-metadata", "options", "modules/bwf-utilities", "scripts/knockout-selection"], function (require, exports, knockout, sprintf, explorer, metadataService, options, bwf) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var SelectedAggregationsViewModel = /** @class */ (function () {
        function SelectedAggregationsViewModel(params) {
            var _this = this;
            this.gridId = "selectedAggregationsGrid";
            this.gridRendered = ko.observable(false);
            this.clearButtonText = options.resources["bwf_clear"];
            this.isCreate = false;
            this.rendered = knockout.observable(false);
            this.loaded = knockout.observable(false);
            this.ready = knockout.pureComputed(function () {
                var rendered = _this.rendered();
                var loaded = _this.loaded();
                return rendered && loaded;
            });
            this.propertyClicked = function (item) {
                var selectedAggregation = {
                    Id: 0,
                    DisplayName: _this.getDisplayName(item.Name),
                    PropertyName: item.Name,
                    Function: null
                };
                var maxPosition = _this.selectedGridPresentationAggregations().length;
                var newRow = {
                    BaseTypeName: _this.baseType(),
                    Data: selectedAggregation,
                    TypeName: "Aggregation",
                    Position: maxPosition + 1
                };
                var gridItem = new explorer.ExplorerGridItem(newRow, _this.gridColumns(), _this.dataService(), {
                    disableChangeTracking: true,
                    disableValidateOnChange: true
                });
                // need a unique id for removing items correctly
                //gridItem.bwfId = bwf.getNextBwfId().toString();
                gridItem.updateType("Added");
                var columns = _this.gridColumns();
                _this.subscribeToItemProperties(gridItem, columns);
                _this.selectedGridPresentationAggregations.push(gridItem);
                _this.syncToRecord();
            };
            this.enableAggregationSelectionRemove = knockout.pureComputed(function () { return _this.selectedRecords().length > 0; });
            this.enableClear = knockout.pureComputed(function () { return _this.selectedGridPresentationAggregations().length > 0; });
            this.parentGridId = params.grid;
            this.init = function () { return _this.syncFromRecord(); };
            this.metadata = knockout.observable(null);
            this.aggregationMetaData = knockout.observable(null);
            this.isAggregationsSupported = knockout.observable(false);
            this.baseType = params.model.observables['BaseType'];
            this.dataService = params.model.observables['System'];
            this.availableProperties = params.model.observables['SelectedColumns'];
            this.typeForCurrentSelectedAggregations = this.baseType();
            this.dataServiceForCurrentSelectedAggregations = this.dataService();
            this.subscriptions = knockout.observableArray([]);
            this.validationMessage = params.model.validations.messages[params.metadata.name];
            this.topLabel = options.resources["bwf_top"];
            this.bottomLabel = options.resources["bwf_bottom"];
            this.topValue = { Text: this.topLabel, Value: "top" };
            this.bottomValue = { Text: this.bottomLabel, Value: "bottom" };
            this.aggregationsPositon = ko.observable(params.model.observables['AggregationsPosition']() === "top" ? this.topValue : this.bottomValue);
            params.model.observables['selectedAggregationsPosition'] = this.aggregationsPositon;
            // make panel wide
            knockout.postbox.publish(params.grid + '-togglePanelWidth', true);
            params.model.observables['__renderedState'].push(this.ready);
            this.gridColumns = knockout.observableArray([]);
            this.isCreate = params.model.state.isCreate;
            this.formDisabled = params.model.observables['formDisabled'];
            this.gridIsInEditMode = ko.pureComputed(function () { return _this.gridRendered() && !_this.formDisabled(); });
            this.selectedGridPresentationAggregations = knockout.observableArray([]);
            this.recordAggregations = params.model.observables[params.metadata.name];
            this.selectedRecords = knockout.pureComputed(function () {
                return _this.selectedGridPresentationAggregations().filter(function (x) { return Object.keys(x.values).map(function (k) { return x.values[k]; }).some(function (v) { return v.isSelected(); }); });
            });
            this.areInvalidValues = ko.pureComputed(function () { return !_this.selectedGridPresentationAggregations().every(function (x) { return x.values["Function"].isValid() && x.values["ExtraParameters"].isValid(); }); });
            this.subscriptions.push(this.areInvalidValues.subscribe(function (val) { return params.model.observables['customControlDisableSave'](val); }));
            this.subscriptions.push(this.baseTypeSubscription());
            this.subscriptions.push(this.dataServiceSubscription());
            this.subscriptions.push(this.availablePropertiesSubscription());
            this.getMetadata();
            var getTypePromise = metadataService.getType("explorer", "Aggregation");
            getTypePromise.done(function (metadata) {
                var metadataClone = bwf.clone(metadata);
                // we set the name to DisplayName so that we show the correct name as set in syncFromRecord
                metadataClone.properties["PropertyName"].name = "DisplayName";
                metadataClone.properties["PropertyName"].isNotEditableInGrid = true;
                metadataClone.properties["ExtraParameters"].customEditingCell = "ds-explorer-grid;cc-extraParameters";
                metadataClone.properties["ExtraParameters"].useCustomEditingCell = true;
                var columnsMetadata = [
                    metadataClone.properties["PropertyName"],
                    metadataClone.properties["Function"],
                    metadataClone.properties["ExtraParameters"]
                ];
                var explorerGridColumns = columnsMetadata.map(function (x, index) {
                    return new explorer.ExplorerGridColumn(x, x.name, index + 1);
                });
                _this.gridColumns(explorerGridColumns);
                _this.selectedGridPresentationAggregations()
                    .forEach(function (x) { return _this.subscribeToItemProperties(x, explorerGridColumns); });
                _this.aggregationMetaData(metadataClone);
            });
        }
        SelectedAggregationsViewModel.prototype.subscribeToItemProperties = function (item, columns) {
            var _this = this;
            item.configureColumns(columns);
            Object.keys(item.values).map(function (y) { return item.values[y]; }).forEach(function (v) {
                _this.subscriptions.push(v.subscribe(function () { return _this.updateRecords(); }));
                // fixes validation & changes not being tracked correctly
                v.extend({ notify: 'always' });
            });
        };
        SelectedAggregationsViewModel.prototype.dispose = function () {
            this.subscriptions().forEach(function (sub) { return sub.dispose(); });
        };
        SelectedAggregationsViewModel.prototype.baseTypeSubscription = function () {
            var _this = this;
            return this.baseType.subscribe(function (bt) {
                if (bt) {
                    _this.getMetadata();
                    if (_this.typeForCurrentSelectedAggregations && _this.typeForCurrentSelectedAggregations !== bt) {
                        _this.clearSelectedAggregations();
                        _this.typeForCurrentSelectedAggregations = bt;
                        _this.syncToRecord();
                    }
                }
                else {
                    _this.clearSelectedAggregations();
                    _this.typeForCurrentSelectedAggregations = '';
                    _this.syncToRecord();
                }
            });
        };
        SelectedAggregationsViewModel.prototype.dataServiceSubscription = function () {
            var _this = this;
            return this.dataService.subscribe(function (ds) {
                if (ds) {
                    _this.getMetadata();
                    if (_this.dataServiceForCurrentSelectedAggregations && _this.dataServiceForCurrentSelectedAggregations !== ds) {
                        _this.clearSelectedAggregations();
                        _this.dataServiceForCurrentSelectedAggregations = ds;
                        _this.syncToRecord();
                    }
                }
                else {
                    _this.clearSelectedAggregations();
                    _this.dataServiceForCurrentSelectedAggregations = '';
                    _this.syncToRecord();
                }
            });
        };
        SelectedAggregationsViewModel.prototype.availablePropertiesSubscription = function () {
            var _this = this;
            return this.availableProperties.subscribe(function (ap) {
                if (!!ap && ap.length > 0) {
                    var invalidAggregations_1 = _this.getInvalidAggregations(_this.recordAggregations(), ap);
                    if (invalidAggregations_1.length > 0) {
                        _this.recordAggregations(_this.recordAggregations().filter(function (x) { return invalidAggregations_1.indexOf(x) < 0; }));
                        _this.syncFromRecord();
                    }
                }
                else {
                    _this.clearSelectedAggregations();
                    _this.syncToRecord();
                }
            });
        };
        SelectedAggregationsViewModel.prototype.isValidPropertyPath = function (propertyName) {
            var metadata = this.metadata();
            if (!metadata || !propertyName)
                return false;
            return metadataService.isPropertyPathValid(this.dataService(), metadata, propertyName);
        };
        SelectedAggregationsViewModel.prototype.getDisplayName = function (propertyName) {
            var metadata = this.metadata();
            if (!metadata || !propertyName)
                return null;
            var isValid = metadataService.isPropertyPathValid(this.dataService(), metadata, propertyName);
            if (!isValid)
                return null;
            var p = metadataService.getPropertyWithPrefix(this.dataService(), metadata, propertyName);
            return p.displayName;
        };
        SelectedAggregationsViewModel.prototype.updateRecords = function () {
            var _this = this;
            this.selectedGridPresentationAggregations().forEach(function (x) {
                var aggFunction = x.values["Function"];
                var extraParameters = x.values["ExtraParameters"];
                x.record.Function = (aggFunction() == null) ? null : aggFunction();
                if ((!aggFunction() || aggFunction().Value !== "weightedaverage") && (extraParameters() || "").trim() !== "") {
                    extraParameters("");
                }
                x.record.ExtraParameters = (extraParameters() == null || extraParameters().trim() === "") ? null : extraParameters().trim();
                var validationMessage = sprintf.sprintf("Invalid parameter for %s(%s). The parameter must be a valid property path.", options.resources["bwf_weightedaverage"], x.record.DisplayName);
                if (!!aggFunction() && aggFunction().Value === "weightedaverage" && !_this.isValidPropertyPath(x.record.ExtraParameters)) {
                    extraParameters.isValid(false);
                    if (!extraParameters.validationMessages().some(function (m) { return m == validationMessage; }))
                        extraParameters.validationMessages.push(validationMessage);
                }
                else {
                    extraParameters.isValid(true);
                    extraParameters.validationMessages.remove(validationMessage);
                }
            });
            this.syncToRecord();
        };
        SelectedAggregationsViewModel.prototype.getMetadata = function () {
            var _this = this;
            var bt = this.baseType();
            var ds = metadataService.getDataService(this.dataService());
            var local = this.selectedGridPresentationAggregations();
            if (!bt || !ds) {
                if (this.isCreate) {
                    this.loaded(true);
                }
                return;
            }
            if (local.length > 0 && local.every(function (l) { return l.baseTypeName === bt; }))
                return;
            metadataService.getType(ds.name, bt)
                .done(function (m) {
                _this.metadata(m);
                var supports = m.supportsAggregations && !(m.useCombinedQuerier || m.usesTypesFromOtherDataServices);
                _this.isAggregationsSupported(supports);
                _this.loaded(true);
                if (_this.init) {
                    _this.init();
                    _this.init = null;
                }
            });
        };
        SelectedAggregationsViewModel.prototype.gridConfiguration = function () {
            var _this = this;
            var config = {
                createNewRecord: function () { return null; },
                canSelectIndividualCells: knockout.observable(!this.formDisabled()),
                disabled: this.formDisabled,
                disableInsertRecords: true,
                disableRemoveRecords: true,
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
                viewGridId: this.gridId,
                metadata: this.aggregationMetaData,
                disableGridSorting: ko.observable(true),
                records: this.selectedGridPresentationAggregations,
                recordsCount: ko.pureComputed(function () { return _this.selectedGridPresentationAggregations().length; }),
                selectedColumns: knockout.pureComputed(function () { return _this.gridColumns(); }),
                selectedRecords: this.selectedRecords,
                inEditMode: this.gridIsInEditMode,
                postRender: function () { return _this.gridRendered(true); },
                validate: function () { return true; },
                embedded: true
            };
            return config;
        };
        SelectedAggregationsViewModel.prototype.removeAggregationSelection = function () {
            ko.postbox.publish(this.gridId + "-delete-row");
            this.syncToRecord();
        };
        SelectedAggregationsViewModel.prototype.clearSelectedAggregations = function () {
            this.selectedGridPresentationAggregations([]);
            this.syncToRecord();
        };
        SelectedAggregationsViewModel.prototype.syncToRecord = function () {
            var mappedToRecord = this.selectedGridPresentationAggregations()
                .map(function (column) {
                return {
                    Id: column.record.Id,
                    DisplayName: column.record.PropertyName,
                    PropertyName: column.record.PropertyName,
                    Function: column.record.Function,
                    ExtraParameters: column.record.ExtraParameters
                };
            });
            this.recordAggregations(mappedToRecord);
        };
        SelectedAggregationsViewModel.prototype.syncFromRecord = function () {
            var _this = this;
            var mappedToLocal = this.filterValidRecordAggregations(this.recordAggregations())
                .sort(function (l, r) {
                var nameA = l.PropertyName.toUpperCase(); // ignore upper and lowercase
                var nameB = l.PropertyName.toUpperCase(); // ignore upper and lowercase
                if (nameA < nameB)
                    return -1;
                if (nameA > nameB)
                    return 1;
                return 0;
            })
                .map(function (column, index) {
                column.DisplayName = _this.getDisplayName(column.PropertyName);
                var columnRecord = {
                    Id: column.Id.toString(),
                    Data: column,
                    BaseTypeName: _this.baseType(),
                    TypeName: "Aggregation",
                    Position: index + 1
                };
                return new explorer.ExplorerGridItem(columnRecord, _this.gridColumns(), _this.dataService(), {
                    disableChangeTracking: true,
                    disableValidateOnChange: true
                });
            });
            this.selectedGridPresentationAggregations(mappedToLocal);
            this.syncToRecord();
        };
        SelectedAggregationsViewModel.prototype.getInvalidAggregations = function (recordAggregations, availableProperties) {
            var _this = this;
            return recordAggregations.filter(function (column) {
                return _this.getDisplayName(column.PropertyName) == null || !availableProperties.some(function (ap) { return ap.Name === column.PropertyName; });
            });
        };
        SelectedAggregationsViewModel.prototype.filterValidRecordAggregations = function (recordAggregations) {
            if (!recordAggregations)
                return [];
            var invalidAggregations = this.getInvalidAggregations(recordAggregations, this.availableProperties());
            if (invalidAggregations.length > 0) {
                var message = void 0;
                if (invalidAggregations.length > 1)
                    message = sprintf.sprintf("The aggregation properties '%s' are invalid and will be removed when saving.", invalidAggregations.map(function (y) { return y.PropertyName; }).join("', '"));
                else
                    message = sprintf.sprintf("The aggregation property '%s' is invalid and will be removed when saving.", invalidAggregations[0].PropertyName);
                console.warn(message);
                ko.postbox.publish(this.parentGridId + "-persistent-warning-message", message);
                return recordAggregations.filter(function (x) { return invalidAggregations.indexOf(x) < 0; });
            }
            return recordAggregations;
        };
        return SelectedAggregationsViewModel;
    }());
    exports.default = SelectedAggregationsViewModel;
});
