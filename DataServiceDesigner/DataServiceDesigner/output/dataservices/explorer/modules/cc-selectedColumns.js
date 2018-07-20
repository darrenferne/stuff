define(["require", "exports", "knockout", "loglevel", "sprintf", "modules/bwf-explorer", "modules/bwf-metadata", "scripts/knockout-selection", "options", "modules/bwf-utilities"], function (require, exports, knockout, logLevel, sprintf, explorer, metadataService, selection, options, bwf) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var s = selection;
    var bwfId = 1000000000;
    var SelectedColumnsViewModel = /** @class */ (function () {
        function SelectedColumnsViewModel(params) {
            var _this = this;
            this.gridId = "selectedColumnsGrid";
            this.gridIsInEditMode = ko.observable(true);
            this.clearButtonText = options.resources["bwf_clear"];
            this.isCreate = false;
            this.rendered = knockout.observable(false);
            this.loaded = knockout.observable(false);
            this.ready = knockout.pureComputed(function () {
                var rendered = _this.rendered();
                var loaded = _this.loaded();
                return rendered && loaded;
            });
            this.enableColumnSelectionMove = knockout.pureComputed(function () { return _this.selectedRecords().length === 1; });
            this.enableColumnSelectionRemove = knockout.pureComputed(function () { return _this.selectedRecords().length > 0; });
            this.enableClear = knockout.pureComputed(function () { return _this.selectedGridPresentationColumns().length > 0; });
            this.parentGridId = params.grid;
            this.init = function () { return _this.syncFromRecord(); };
            this.metadata = knockout.observable(null);
            this.baseType = params.model.observables['BaseType'];
            this.dataService = params.model.observables['System'];
            this.typeForCurrentSelectedColumns = this.baseType();
            this.dataServiceForCurrentSelectedColumns = this.dataService();
            this.isCreate = params.model.state.isCreate;
            this.formDisabled = params.model.observables['formDisabled'];
            this.subscriptions = knockout.observableArray([]);
            this.data = {
                instanceName: 'cc-selectedColumns',
                baseType: this.baseType,
                dataService: this.dataService,
                formDisabled: this.formDisabled,
                isCreate: this.isCreate,
                renderedState: params.model.observables['__renderedState'],
                model: params.model
            };
            // make panel wide
            knockout.postbox.publish(params.grid + '-togglePanelWidth', true);
            params.model.observables['__renderedState'].push(this.ready);
            this.gridColumns = knockout.observableArray([]);
            this.selectedGridPresentationColumns = knockout.observableArray([]);
            this.recordColumns = params.model.observables[params.metadata.name];
            this.selectedRecords = knockout.pureComputed(function () {
                return _this.selectedGridPresentationColumns().filter(function (x) { return Object.keys(x.values).map(function (k) { return x.values[k]; }).some(function (v) { return v.isSelected(); }); });
            });
            this.areInvalidValues = ko.pureComputed(function () { return !_this.selectedGridPresentationColumns().every(function (x) { return x.values["Alias"].isValid() && x.values["Format"].isValid(); }); });
            this.subscriptions.push(this.areInvalidValues.subscribe(function (val) { return params.model.observables['customControlDisableSave'](val); }));
            this.subscriptions.push(this.availablePropertySelectedPostboxSubscription());
            this.subscriptions.push(this.baseTypeSubscription());
            this.subscriptions.push(this.dataServiceSubscription());
            this.getMetadata();
            var getTypePromise = metadataService.getType("explorer", "Property");
            getTypePromise.done(function (metadata) {
                var metadataClone = bwf.clone(metadata);
                // we set the name to DisplayName so that we show the correct name as set in syncFromRecord
                metadataClone.properties["Name"].name = "DisplayName";
                metadataClone.properties["Name"].isNotEditableInGrid = true;
                metadataClone.properties["Alias"].displayName = options.resources["bwf_alias"];
                metadataClone.properties["Format"].displayName = options.resources["bwf_format"];
                metadataClone.properties["Format"].customEditingCell = "ds-explorer-grid;cc-format";
                metadataClone.properties["Format"].useCustomEditingCell = true;
                var columnsMetadata = [
                    metadataClone.properties["Name"],
                    metadataClone.properties["Alias"],
                    metadataClone.properties["Format"]
                ];
                var explorerGridColumns = columnsMetadata.map(function (x, index) {
                    return new explorer.ExplorerGridColumn(x, x.name, index + 1);
                });
                _this.gridColumns(explorerGridColumns);
                _this.selectedGridPresentationColumns()
                    .forEach(function (x) { return _this.subscribeToItemProperties(x, explorerGridColumns); });
            });
        }
        SelectedColumnsViewModel.prototype.subscribeToItemProperties = function (item, columns) {
            var _this = this;
            item.configureColumns(columns);
            // we deal with the format validation in cc-format
            item.values["Alias"].extend({
                validAlias: {
                    message: "Alias cannot contain \\"
                }
            });
            Object.keys(item.values).map(function (y) { return item.values[y]; }).forEach(function (v) {
                _this.subscriptions.push(v.subscribe(function () { return _this.updateRecords(); }));
                // fixes validation & changes not being tracked correctly
                v.extend({ notify: 'always' });
            });
        };
        SelectedColumnsViewModel.prototype.dispose = function () {
            this.subscriptions().forEach(function (sub) { return sub.dispose(); });
        };
        SelectedColumnsViewModel.prototype.availablePropertySelectedPostboxSubscription = function () {
            var _this = this;
            return knockout.postbox.subscribe(this.data.instanceName + '-cc-available-property-selected', function (item) {
                var localColumns = _this.recordColumns();
                var alreadyExists = localColumns.some(function (local) { return local.Name === item.name || local.DisplayName === item.displayName; });
                if (alreadyExists) {
                    logLevel.warn(sprintf.sprintf("Tried to add item '%s' to selected columns when already selected", item.name));
                    return;
                }
                var maxPosition = localColumns.reduce(function (l, r) { return l.Position < r.Position ? r : l; }, { Position: 0 }).Position;
                var selectedColumn = { Name: item.name, DisplayName: _this.getDisplayName(item.name) };
                var newCol = {
                    BaseTypeName: _this.baseType(),
                    Data: selectedColumn,
                    TypeName: "Property",
                    Id: (item.id || bwfId++).toString(),
                    Position: maxPosition + 1
                };
                var gridItem = new explorer.ExplorerGridItem(newCol, _this.gridColumns(), _this.dataService(), {
                    disableChangeTracking: true,
                    disableValidateOnChange: true
                });
                gridItem.updateType("Added");
                var columns = _this.gridColumns();
                _this.subscribeToItemProperties(gridItem, columns);
                _this.selectedGridPresentationColumns.push(gridItem);
                _this.syncToRecord();
            });
        };
        SelectedColumnsViewModel.prototype.baseTypeSubscription = function () {
            var _this = this;
            return this.baseType.subscribe(function (bt) {
                if (bt) {
                    _this.getMetadata();
                    if (_this.typeForCurrentSelectedColumns && _this.typeForCurrentSelectedColumns !== bt) {
                        _this.clearSelectedColumns();
                        _this.typeForCurrentSelectedColumns = bt;
                        _this.syncToRecord();
                    }
                }
                else {
                    _this.clearSelectedColumns();
                    _this.typeForCurrentSelectedColumns = '';
                    _this.syncToRecord();
                }
            });
        };
        SelectedColumnsViewModel.prototype.dataServiceSubscription = function () {
            var _this = this;
            return this.dataService.subscribe(function (ds) {
                if (ds) {
                    _this.getMetadata();
                    if (_this.dataServiceForCurrentSelectedColumns && _this.dataServiceForCurrentSelectedColumns !== ds) {
                        _this.clearSelectedColumns();
                        _this.dataServiceForCurrentSelectedColumns = ds;
                        _this.syncToRecord();
                    }
                }
                else {
                    _this.clearSelectedColumns();
                    _this.dataServiceForCurrentSelectedColumns = '';
                    _this.syncToRecord();
                }
            });
        };
        SelectedColumnsViewModel.prototype.getDisplayName = function (propertyName) {
            var metadata = this.metadata();
            if (!metadata || !propertyName)
                return null;
            var isValue = metadataService.isPropertyPathValid(this.dataService(), metadata, propertyName);
            if (!isValue)
                return null;
            var p = metadataService.getPropertyWithPrefix(this.dataService(), metadata, propertyName);
            return p.displayName;
        };
        SelectedColumnsViewModel.prototype.updateRecords = function () {
            this.selectedGridPresentationColumns().forEach(function (x) {
                var alias = x.values["Alias"]();
                var format = x.values["Format"]();
                if (alias != null && alias.trim() !== "") {
                    x.record.Alias = alias.trim();
                }
                else {
                    x.record.Alias = null;
                }
                x.record.Format = (format == null || format.trim() === "") ? null : format.trim();
            });
            this.syncToRecord();
        };
        SelectedColumnsViewModel.prototype.getMetadata = function () {
            var _this = this;
            var bt = this.baseType();
            var ds = metadataService.getDataService(this.dataService());
            var local = this.selectedGridPresentationColumns();
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
                _this.loaded(true);
                if (_this.init) {
                    _this.init();
                    _this.init = null;
                }
            });
        };
        SelectedColumnsViewModel.prototype.gridConfiguration = function () {
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
                metadata: this.metadata,
                disableGridSorting: ko.observable(true),
                records: this.selectedGridPresentationColumns,
                recordsCount: ko.pureComputed(function () { return _this.selectedGridPresentationColumns().length; }),
                selectedColumns: knockout.pureComputed(function () { return _this.gridColumns(); }),
                selectedRecords: this.selectedRecords,
                inEditMode: this.gridIsInEditMode,
                postRender: function () { return _this.gridIsInEditMode(true); },
                validate: function () { return true; },
                embedded: true
            };
            return config;
        };
        //#region moving & ordering
        SelectedColumnsViewModel.prototype.moveColumnSelectionToTop = function () {
            this.selectedRecords()[0].queryPosition(-1);
            this.syncPositions();
        };
        SelectedColumnsViewModel.prototype.moveColumnSelectionUp = function () {
            var selected = this.selectedRecords()[0];
            selected.queryPosition(selected.queryPosition() - 1.5);
            this.syncPositions();
        };
        SelectedColumnsViewModel.prototype.moveColumnSelectionDown = function () {
            var selected = this.selectedRecords()[0];
            selected.queryPosition(selected.queryPosition() + 1.5);
            this.syncPositions();
        };
        SelectedColumnsViewModel.prototype.moveColumnSelectionToBottom = function () {
            this.selectedRecords()[0].queryPosition(999);
            this.syncPositions();
        };
        SelectedColumnsViewModel.prototype.removeColumnSelection = function () {
            var _this = this;
            ko.postbox.publish(this.gridId + "-delete-row", { callback: function () { return _this.syncPositions(); } });
        };
        SelectedColumnsViewModel.prototype.clearSelectedColumns = function () {
            this.selectedGridPresentationColumns([]);
            this.syncToRecord();
        };
        SelectedColumnsViewModel.prototype.syncPositions = function () {
            this.selectedGridPresentationColumns()
                .sort(function (l, r) { return l.queryPosition() - r.queryPosition(); })
                .forEach(function (record, i) { return record.queryPosition(i + 1); });
            this.selectedGridPresentationColumns.valueHasMutated();
            this.syncToRecord();
        };
        //#endregion
        SelectedColumnsViewModel.prototype.syncToRecord = function () {
            var mappedToRecord = this.selectedGridPresentationColumns()
                .map(function (column) {
                return {
                    Id: column.record.Id,
                    Name: column.record.Name,
                    DisplayName: column.record.Name,
                    Position: column.queryPosition(),
                    Alias: column.record.Alias,
                    Format: column.record.Format
                };
            });
            this.recordColumns(mappedToRecord);
        };
        SelectedColumnsViewModel.prototype.syncFromRecord = function () {
            var _this = this;
            var mappedToLocal = this.filterValidRecordColumns(this.recordColumns())
                .sort(function (l, r) { return l.Position - r.Position; })
                .map(function (column, index) {
                column.DisplayName = _this.getDisplayName(column.Name);
                var columnRecord = {
                    Id: column.Id.toString(),
                    Data: column,
                    BaseTypeName: _this.baseType(),
                    TypeName: "Property",
                    Position: index + 1
                };
                return new explorer.ExplorerGridItem(columnRecord, _this.gridColumns(), _this.dataService(), {
                    disableChangeTracking: true,
                    disableValidateOnChange: true
                });
            });
            this.selectedGridPresentationColumns(mappedToLocal);
            this.syncToRecord();
        };
        SelectedColumnsViewModel.prototype.filterValidRecordColumns = function (recordColumns) {
            var _this = this;
            if (!recordColumns)
                return [];
            var invalidColumns = recordColumns.filter(function (column) { return _this.getDisplayName(column.Name) == null; });
            if (invalidColumns.length > 0) {
                var message;
                if (invalidColumns.length > 1)
                    message = sprintf.sprintf("The selected column properties '%s' are invalid and will be removed when saving.", invalidColumns.map(function (y) { return y.Name; }).join("', '"));
                else
                    message = sprintf.sprintf("The selected column property '%s' is invalid and will be removed when saving.", invalidColumns[0].Name);
                console.warn(message);
                ko.postbox.publish(this.parentGridId + "-persistent-warning-message", message);
                return recordColumns.filter(function (x) { return invalidColumns.indexOf(x) < 0; });
            }
            return recordColumns;
        };
        return SelectedColumnsViewModel;
    }());
    exports.default = SelectedColumnsViewModel;
});
