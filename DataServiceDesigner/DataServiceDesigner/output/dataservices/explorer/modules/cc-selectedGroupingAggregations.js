define(["require", "exports", "knockout", "sprintf", "modules/bwf-explorer", "modules/bwf-metadata", "options", "modules/bwf-utilities", "scripts/knockout-selection"], function (require, exports, knockout, sprintf, explorer, metadataService, options, bwf) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var SelectedAggregationsViewModel = /** @class */ (function () {
        function SelectedAggregationsViewModel(params) {
            var _this = this;
            this.gridId = "selectedGroupingAggregationsGrid";
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
            this.enableClear = knockout.pureComputed(function () { return _this.selectedAggregationProperties().length > 0; });
            this.parentGridId = params.grid;
            this.init = function () { return _this.syncFromRecord(); };
            this.metadata = knockout.observable(null);
            this.groupingPropertyMetadata = knockout.observable(null);
            this.baseType = params.model.observables['BaseType'];
            this.dataService = params.model.observables['System'];
            this.typeForCurrentSelectedProperties = this.baseType();
            this.dataServiceForCurrentSelectedProperties = this.dataService();
            this.isCreate = params.model.state.isCreate;
            this.formDisabled = params.model.observables['formDisabled'];
            this.subscriptions = knockout.observableArray([]);
            this.data = {
                instanceName: 'cc-selectedGroupingAggregations',
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
            this.gridIsInEditMode = ko.observable(!this.formDisabled());
            this.selectedAggregationProperties = knockout.observableArray([]);
            this.recordProperties = params.model.observables[params.metadata.name];
            this.selectedRecords = knockout.pureComputed(function () {
                return _this.selectedAggregationProperties().filter(function (x) { return Object.keys(x.values).map(function (k) { return x.values[k]; }).some(function (v) { return v.isSelected(); }); });
            });
            this.areInvalidValues = ko.pureComputed(function () { return !_this.selectedAggregationProperties().every(function (x) {
                return x.values["Function"].isValid() &&
                    x.values["Options"].isValid() &&
                    x.values["Alias"].isValid() &&
                    x.values["ExtraParameters"].isValid() &&
                    x.values["Format"].isValid();
            }); });
            this.subscriptions.push(this.areInvalidValues.subscribe(function (val) { return params.model.observables['customControlDisableSave'](val); }));
            this.subscriptions.push(this.availablePropertySelectedPostboxSubscription());
            this.subscriptions.push(this.baseTypeSubscription());
            this.subscriptions.push(this.dataServiceSubscription());
            var getTypePromise = metadataService.getType("explorer", "GroupingAggregation");
            getTypePromise.done(function (metadata) {
                var metadataClone = bwf.clone(metadata);
                // we set the name to DisplayName so that we show the correct name as set in syncFromRecord
                metadataClone.properties["Name"].name = "DisplayName";
                metadataClone.properties["Name"].isNotEditableInGrid = true;
                metadataClone.properties["Function"].isNullable = false;
                var columnsMetadata = [
                    metadataClone.properties["Name"],
                    metadataClone.properties["Function"],
                    metadataClone.properties["Options"],
                    metadataClone.properties["ExtraParameters"],
                    metadataClone.properties["Alias"],
                    metadataClone.properties["Format"]
                ];
                var explorerGridColumns = columnsMetadata.map(function (x, index) {
                    return new explorer.ExplorerGridColumn(x, x.name, index + 1);
                });
                _this.gridColumns(explorerGridColumns);
                _this.groupingPropertyMetadata(metadataClone);
            });
            $.when(this.getMetadata(), getTypePromise).done(function () {
                if (_this.init) {
                    _this.init();
                    _this.init = null;
                }
                _this.selectedAggregationProperties()
                    .forEach(function (x) { return _this.subscribeToItemProperties(x, _this.gridColumns()); });
            });
        }
        SelectedAggregationsViewModel.prototype.subscribeToItemProperties = function (item, columns) {
            var _this = this;
            item.configureColumns(columns);
            Object.keys(item.values).forEach(function (y) {
                var v = item.values[y];
                _this.subscriptions.push(v.subscribe(function () { return _this.syncToRecord(); }));
                // fixes validation & changes not being tracked correctly
                v.extend({ notify: 'always' });
            });
        };
        SelectedAggregationsViewModel.prototype.dispose = function () {
            this.subscriptions().forEach(function (sub) { return sub.dispose(); });
        };
        SelectedAggregationsViewModel.prototype.availablePropertySelectedPostboxSubscription = function () {
            var _this = this;
            return knockout.postbox.subscribe(this.data.instanceName + '-cc-available-property-selected', function (item) {
                var localColumns = _this.recordProperties();
                var maxPosition = localColumns.reduce(function (l, r) { return l.Position < r.Position ? r : l; }, { Position: 0 }).Position;
                var selectedAggregation = {
                    Name: item.name,
                    DisplayName: _this.getDisplayName(item.name),
                    Options: { Text: "Exclude Nulls", Value: "excludenulls" }
                };
                var newRow = {
                    BaseTypeName: _this.baseType(),
                    Data: selectedAggregation,
                    TypeName: "GroupingAggregation",
                    Position: maxPosition + 1
                };
                var gridItem = new explorer.ExplorerGridItem(newRow, _this.gridColumns(), _this.dataService(), {
                    disableChangeTracking: true,
                    disableValidateOnChange: true
                });
                gridItem.updateType("Added");
                _this.subscribeToItemProperties(gridItem, _this.gridColumns());
                _this.selectedAggregationProperties.push(gridItem);
                _this.syncPositions();
            });
        };
        SelectedAggregationsViewModel.prototype.baseTypeSubscription = function () {
            var _this = this;
            return this.baseType.subscribe(function (bt) {
                if (bt) {
                    _this.getMetadata();
                    if (_this.typeForCurrentSelectedProperties && _this.typeForCurrentSelectedProperties !== bt) {
                        _this.clearSelectedColumns();
                        _this.typeForCurrentSelectedProperties = bt;
                        _this.syncToRecord();
                    }
                }
                else {
                    _this.clearSelectedColumns();
                    _this.typeForCurrentSelectedProperties = '';
                    _this.syncToRecord();
                }
            });
        };
        SelectedAggregationsViewModel.prototype.dataServiceSubscription = function () {
            var _this = this;
            return this.dataService.subscribe(function (ds) {
                if (ds) {
                    _this.getMetadata();
                    if (_this.dataServiceForCurrentSelectedProperties && _this.dataServiceForCurrentSelectedProperties !== ds) {
                        _this.clearSelectedColumns();
                        _this.dataServiceForCurrentSelectedProperties = ds;
                        _this.syncToRecord();
                    }
                }
                else {
                    _this.clearSelectedColumns();
                    _this.dataServiceForCurrentSelectedProperties = '';
                    _this.syncToRecord();
                }
            });
        };
        SelectedAggregationsViewModel.prototype.getDisplayName = function (propertyName) {
            var metadata = this.metadata();
            if (!metadata || !propertyName)
                return null;
            var isValue = metadataService.isPropertyPathValid(this.dataService(), metadata, propertyName);
            if (!isValue)
                return null;
            var p = metadataService.getPropertyWithPrefix(this.dataService(), metadata, propertyName);
            return p.displayName;
        };
        SelectedAggregationsViewModel.prototype.getMetadata = function () {
            var _this = this;
            var bt = this.baseType();
            var ds = metadataService.getDataService(this.dataService());
            var local = this.selectedAggregationProperties();
            if (!bt || !ds) {
                if (this.isCreate) {
                    this.loaded(true);
                }
                return;
            }
            if (local.length > 0 && local.every(function (l) { return l.baseTypeName === bt; }))
                return;
            return metadataService.getType(ds.name, bt)
                .done(function (m) {
                _this.metadata(m);
                _this.loaded(true);
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
                metadata: this.groupingPropertyMetadata,
                disableGridSorting: ko.observable(true),
                records: this.selectedAggregationProperties,
                recordsCount: ko.pureComputed(function () { return _this.selectedAggregationProperties().length; }),
                selectedColumns: knockout.pureComputed(function () { return _this.gridColumns(); }),
                selectedRecords: this.selectedRecords,
                inEditMode: this.gridIsInEditMode,
                postRender: function () { return _this.gridIsInEditMode(!_this.formDisabled()); },
                validate: function () { return true; },
                embedded: true
            };
            return config;
        };
        //#region moving & ordering
        SelectedAggregationsViewModel.prototype.moveColumnSelectionToTop = function () {
            this.selectedRecords()[0].queryPosition(-1);
            this.syncPositions();
        };
        SelectedAggregationsViewModel.prototype.moveColumnSelectionUp = function () {
            var selected = this.selectedRecords()[0];
            selected.queryPosition(selected.queryPosition() - 1.5);
            this.syncPositions();
        };
        SelectedAggregationsViewModel.prototype.moveColumnSelectionDown = function () {
            var selected = this.selectedRecords()[0];
            selected.queryPosition(selected.queryPosition() + 1.5);
            this.syncPositions();
        };
        SelectedAggregationsViewModel.prototype.moveColumnSelectionToBottom = function () {
            this.selectedRecords()[0].queryPosition(999);
            this.syncPositions();
        };
        SelectedAggregationsViewModel.prototype.removeColumnSelection = function () {
            var _this = this;
            ko.postbox.publish(this.gridId + "-delete-row", { callback: function () { return _this.syncPositions(); } });
        };
        SelectedAggregationsViewModel.prototype.clearSelectedColumns = function () {
            this.selectedAggregationProperties([]);
            this.syncToRecord();
        };
        SelectedAggregationsViewModel.prototype.syncPositions = function () {
            this.selectedAggregationProperties()
                .sort(function (l, r) { return l.queryPosition() - r.queryPosition(); })
                .forEach(function (record, i) {
                record.queryPosition(i + 1);
            });
            this.selectedAggregationProperties.valueHasMutated();
            this.syncToRecord();
        };
        //#endregion
        SelectedAggregationsViewModel.prototype.syncToRecord = function () {
            var mappedToRecord = this.selectedAggregationProperties()
                .map(function (column) {
                column.record.Function = column.values["Function"]();
                column.record.Options = column.values["Options"]();
                column.record.ExtraParameters = column.values["ExtraParameters"]();
                var alias = column.values["Alias"]();
                column.record.Alias = (alias == null || alias.trim() === "") ? null : alias.trim();
                var format = column.values["Format"]();
                column.record.Format = (format == null || format.trim() === "") ? null : format.trim();
                return {
                    Id: column.record.Id,
                    Name: column.record.Name,
                    DisplayName: column.record.Name,
                    Position: column.queryPosition(),
                    Function: column.record.Function,
                    Options: column.record.Options,
                    ExtraParameters: column.record.ExtraParameters,
                    Alias: column.record.Alias,
                    Format: column.record.Format
                };
            });
            this.recordProperties(mappedToRecord);
        };
        SelectedAggregationsViewModel.prototype.syncFromRecord = function () {
            var _this = this;
            var mappedToLocal = this.filterValidRecordColumns(this.recordProperties())
                .sort(function (l, r) { return l.Position - r.Position; })
                .map(function (column, index) {
                column.DisplayName = _this.getDisplayName(column.Name);
                var columnRecord = {
                    Id: column.Id.toString(),
                    Data: column,
                    BaseTypeName: _this.baseType(),
                    TypeName: "GroupingAggregation",
                    Position: index + 1
                };
                var gridItem = new explorer.ExplorerGridItem(columnRecord, _this.gridColumns(), _this.dataService(), {
                    disableChangeTracking: true,
                    disableValidateOnChange: true
                });
                _this.subscribeToItemProperties(gridItem, _this.gridColumns());
                return gridItem;
            });
            this.selectedAggregationProperties(mappedToLocal);
            this.syncPositions();
        };
        SelectedAggregationsViewModel.prototype.filterValidRecordColumns = function (recordGroupBys) {
            var _this = this;
            if (!recordGroupBys)
                return [];
            var invalidColumns = recordGroupBys.filter(function (column) { return _this.getDisplayName(column.Name) == null; });
            if (invalidColumns.length > 0) {
                var message;
                if (invalidColumns.length > 1)
                    message = sprintf.sprintf("The selected aggregation columns '%s' are invalid and will be removed when saving.", invalidColumns.map(function (y) { return y.Name; }).join("', '"));
                else
                    message = sprintf.sprintf("The selected aggregation column '%s' is invalid and will be removed when saving.", invalidColumns[0].Name);
                console.warn(message);
                ko.postbox.publish(this.parentGridId + "-persistent-warning-message", message);
                return recordGroupBys.filter(function (x) { return invalidColumns.indexOf(x) < 0; });
            }
            return recordGroupBys;
        };
        return SelectedAggregationsViewModel;
    }());
    exports.default = SelectedAggregationsViewModel;
});
