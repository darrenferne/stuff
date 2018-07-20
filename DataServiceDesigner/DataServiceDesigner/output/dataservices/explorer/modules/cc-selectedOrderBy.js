define(["require", "exports", "knockout", "modules/bwf-metadata", "scripts/knockout-selection", "sprintf", "options", "modules/bwf-explorer"], function (require, exports, ko, metadataService, selection, sprintf, options, explorer) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var s = selection;
    var SelectedOrderByViewModel = /** @class */ (function () {
        function SelectedOrderByViewModel(params) {
            var _this = this;
            this.metadata = ko.observable(null);
            this.orderByGridItems = ko.observableArray([]);
            this.gridColumns = ko.observableArray([]);
            this.subscriptions = ko.observableArray([]);
            this.clearButtonText = options.resources["bwf_clear"];
            this.isCreate = false;
            this.isDisabled = false;
            this.rendered = ko.observable(false);
            this.loaded = ko.observable(false);
            this.gridId = "orderByGrid";
            this.ready = ko.pureComputed(function () {
                var rendered = _this.rendered();
                var loaded = _this.loaded();
                return rendered && loaded;
            });
            this.enableOrderBySelectionMove = ko.pureComputed(function () { return _this.selectedRecords().length === 1; });
            this.enableOrderBySelectionRemove = ko.pureComputed(function () { return _this.selectedRecords().length > 0; });
            this.enableClear = ko.pureComputed(function () { return _this.orderByGridItems().length > 0; });
            this.enableColumnSelectionMove = ko.pureComputed(function () { return _this.selectedRecords().length === 1; });
            this.enableColumnSelectionRemove = ko.pureComputed(function () { return _this.selectedRecords().length > 0; });
            this.isCreate = params.model.state.isCreate;
            this.isDisabled = params.model.observables["formDisabled"]();
            params.model.observables['__renderedState'].push(this.ready);
            this.init = function () { return _this.syncFromRecord(); };
            this.baseType = params.model.observables['BaseType'];
            this.dataService = params.model.observables['System'];
            this.typeForCurrentOrderBy = this.baseType();
            this.dataServiceForCurrentOrderBy = this.dataService();
            this.recordOrderBys = params.model.observables[params.metadata.name];
            this.formDisabled = params.model.observables['formDisabled'];
            this.data = {
                instanceName: 'cc-selectedOrderBy',
                baseType: this.baseType,
                dataService: this.dataService,
                formDisabled: this.formDisabled,
                isCreate: this.isCreate,
                renderedState: params.model.observables['__renderedState'],
                model: params.model
            };
            this.selectedRecords = ko.pureComputed(function () {
                return _this.orderByGridItems().filter(function (x) { return Object.keys(x.values).map(function (k) { return x.values[k]; }).some(function (v) { return v.isSelected(); }); });
            });
            this.subscriptions.push(this.orderByGridItems.subscribe(function (x) { return x.forEach(function (y) { return y.configureColumns(_this.gridColumns()); }); }));
            var parameterColumn = metadataService.getEmptyGridColumnMetadata();
            parameterColumn.abbreviatedName = "Parameter";
            parameterColumn.name = "DisplayName";
            parameterColumn.displayName = "Parameter";
            parameterColumn.type = "string";
            var directionColumn = {
                abbreviatedName: "Direction",
                additionalDescriptions: [],
                alignment: "left",
                customEditingCell: "ds-explorer-grid;cc-direction",
                useCustomEditingCell: true,
                defaultFormat: undefined,
                description: "Order by direction",
                displayName: "Direction",
                hasChoice: true,
                isNotEditableInGrid: false,
                isMandatoryInEditMode: false,
                isNullable: false,
                isDisabledInCreateMode: false,
                isDisabledInEditMode: false,
                name: "Direction",
                type: "string",
                useCustomDisplayCell: false,
            };
            var columns = [parameterColumn, directionColumn];
            var gridColumns = columns.map(function (x, index) { return new explorer.ExplorerGridColumn(x, x.name, index + 1); });
            this.gridColumns(gridColumns);
            this.parentGridId = params.grid;
            this.subscriptions.push(ko.postbox.subscribe(this.data.instanceName + '-cc-available-property-selected', function (item) {
                var orderByGridItems = _this.orderByGridItems();
                var alreadyExists = orderByGridItems.some(function (local) { return local.record.Name === item.name; });
                if (alreadyExists)
                    return;
                var maxPosition = orderByGridItems.length;
                var orderByItem = {
                    Direction: '',
                    DisplayName: _this.getDisplayName(_this.metadata(), item.name),
                    Name: item.name,
                    Position: maxPosition + 1,
                    forType: _this.baseType()
                };
                var newGridItem = _this.createOrderByGridItem(orderByItem, "Added");
                // subscribe to value changes and sync to record
                Object.keys(newGridItem.values).map(function (k) { return newGridItem.values[k]; })
                    .forEach(function (x) { return x.subscribe(function () { return _this.syncToRecord(); }); });
                _this.orderByGridItems.push(newGridItem);
                _this.syncToRecord();
            }));
            this.subscriptions.push(this.baseType.subscribe(function (bt) {
                if (bt) {
                    _this.getMetadata();
                    if (_this.typeForCurrentOrderBy && _this.typeForCurrentOrderBy !== bt) {
                        _this.clearOrderBys();
                        _this.typeForCurrentOrderBy = bt;
                        _this.syncToRecord();
                    }
                }
                else {
                    _this.clearOrderBys();
                    _this.typeForCurrentOrderBy = '';
                    _this.syncToRecord();
                }
            }));
            this.subscriptions.push(this.dataService.subscribe(function (ds) {
                if (ds) {
                    _this.getMetadata();
                    if (_this.dataServiceForCurrentOrderBy && _this.dataServiceForCurrentOrderBy !== ds) {
                        _this.clearOrderBys();
                        _this.dataServiceForCurrentOrderBy = ds;
                        _this.syncToRecord();
                    }
                }
                else {
                    _this.clearOrderBys();
                    _this.dataServiceForCurrentOrderBy = '';
                    _this.syncToRecord();
                }
            }));
            this.getMetadata();
        }
        SelectedOrderByViewModel.prototype.dispose = function () {
            this.subscriptions().forEach(function (sub) { return sub.dispose(); });
        };
        SelectedOrderByViewModel.prototype.getDisplayName = function (metadata, property) {
            var isValid = metadataService.isPropertyPathValid(this.dataService(), metadata, property);
            if (!isValid)
                return null;
            var p = metadataService.getPropertyWithPrefix(this.dataService(), metadata, property);
            return p.displayName;
        };
        SelectedOrderByViewModel.prototype.getMetadata = function () {
            var _this = this;
            var bt = this.baseType();
            var ds = metadataService.getDataService(this.dataService());
            var local = this.orderByGridItems();
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
                if (_this.init) {
                    _this.init();
                    _this.init = null;
                }
            });
        };
        SelectedOrderByViewModel.prototype.getOrderByString = function (orderBys) {
            return orderBys.sort(function (l, r) { return l.queryPosition() - r.queryPosition(); })
                .map(function (item) { return item.values["Direction"]() !== '' ?
                item.record.Name + ' ' + item.values["Direction"]() :
                item.record.Name; })
                .join(',');
        };
        SelectedOrderByViewModel.prototype.moveOrderBySelectionToTop = function () {
            var selected = this.selectedRecords()[0];
            selected.queryPosition(-1);
            this.updatePositions();
        };
        SelectedOrderByViewModel.prototype.moveOrderBySelectionUp = function () {
            var selected = this.selectedRecords()[0];
            selected.queryPosition(selected.queryPosition() - 1.5);
            this.updatePositions();
        };
        SelectedOrderByViewModel.prototype.moveOrderBySelectionDown = function () {
            var selected = this.selectedRecords()[0];
            selected.queryPosition(selected.queryPosition() + 1.5);
            this.updatePositions();
        };
        SelectedOrderByViewModel.prototype.moveOrderBySelectionToBottom = function () {
            var selected = this.selectedRecords()[0];
            selected.queryPosition(9999);
            this.updatePositions();
        };
        SelectedOrderByViewModel.prototype.removeOrderBySelection = function () {
            ko.postbox.publish(this.gridId + "-delete-row", { callback: this.updatePositions() });
        };
        SelectedOrderByViewModel.prototype.clearOrderBys = function () {
            this.orderByGridItems([]);
            this.syncToRecord();
        };
        SelectedOrderByViewModel.prototype.updatePositions = function () {
            this.orderByGridItems()
                .sort(function (l, r) { return l.queryPosition() - r.queryPosition(); })
                .forEach(function (x, index) { return x.queryPosition(index); });
            this.syncToRecord();
        };
        SelectedOrderByViewModel.prototype.gridConfiguration = function () {
            var _this = this;
            var config = {
                createNewRecord: function () { return null; },
                disabled: this.formDisabled,
                disableInsertRecords: true,
                disableRemoveRecords: true,
                disableSoftDelete: true,
                canSelectIndividualCells: ko.observable(!this.formDisabled()),
                header: {
                    enabled: ko.observable(false),
                    name: null, config: null
                },
                footer: {
                    enabled: ko.observable(false),
                    name: null, config: null
                },
                isView: false,
                viewGridId: this.gridId,
                metadata: this.metadata,
                records: this.orderByGridItems,
                recordsCount: ko.pureComputed(function () { return _this.orderByGridItems().length; }),
                disableGridSorting: ko.observable(true),
                selectedColumns: ko.pureComputed(function () { return _this.gridColumns(); }),
                selectedRecords: this.selectedRecords,
                inEditMode: ko.observable(true),
                postRender: function () { return null; },
                validate: function () { return null; },
                embedded: true
            };
            return config;
        };
        SelectedOrderByViewModel.prototype.createOrderByGridItem = function (item, type) {
            var _this = this;
            if (type === void 0) { type = null; }
            var columnRecord = {
                BaseTypeName: this.baseType(),
                TypeName: "OrderBy",
                Position: item.Position,
                Id: item.Position.toString(),
                Data: item
            };
            var gridItem = new explorer.ExplorerGridItem(columnRecord, this.gridColumns(), this.dataService(), {
                disableChangeTracking: true,
                disableValidateOnChange: true
            });
            if (type !== null)
                gridItem.updateType(type);
            // sync to record on value change
            Object.keys(gridItem.values).map(function (x) { return gridItem.values[x]; }).forEach(function (v) {
                v.subscribe(function () { return _this.syncToRecord(); });
                // fixes issue with change tracking not showing properly in grid
                v.extend({ notify: 'always' });
            });
            return gridItem;
        };
        SelectedOrderByViewModel.prototype.syncToRecord = function () {
            var gridItems = this.orderByGridItems();
            gridItems.forEach(function (x) { return x.record["Direction"] = x.values["Direction"](); });
            var orderByString = this.getOrderByString(gridItems);
            this.recordOrderBys(orderByString);
        };
        SelectedOrderByViewModel.prototype.syncFromRecord = function () {
            var _this = this;
            var metadata = this.metadata();
            var bt = this.baseType();
            var ds = this.dataService();
            var recordOrderBys = this.recordOrderBys();
            if (!bt || !ds || !metadata)
                return;
            if (recordOrderBys == null) {
                this.clearOrderBys();
                this.loaded(true);
                return;
            }
            var orderByElements = recordOrderBys.split(',').filter(function (f) { return f.length > 0; });
            var orderBys = this.filterOrderBys(orderByElements)
                .map(function (item, index) {
                var orderBy = {
                    Name: item[0],
                    Direction: item[1],
                    DisplayName: _this.getDisplayName(metadata, item[0]),
                    Position: index + 1,
                    forType: _this.baseType()
                };
                return orderBy;
            });
            var orderByGridItems = orderBys.map(function (x) { return _this.createOrderByGridItem(x); });
            this.orderByGridItems(orderByGridItems);
            this.loaded(true);
            this.syncToRecord();
        };
        SelectedOrderByViewModel.prototype.filterOrderBys = function (orderBys) {
            var _this = this;
            var invalidOrderBys = [];
            var validOrderBys = [];
            orderBys.forEach(function (item) {
                var fragments = item.split(' ').filter(function (f) { return f != null && f.length > 0; });
                if (fragments.length === 0 || fragments.length > 2 || fragments[0] == null) {
                    invalidOrderBys.push(item);
                    return;
                }
                var direction = (fragments.length === 1 || fragments[1].indexOf('asc') >= 0) ? '' : 'desc';
                var displayName = _this.getDisplayName(_this.metadata(), fragments[0]);
                if (displayName == null) {
                    invalidOrderBys.push(item);
                    return;
                }
                validOrderBys.push([fragments[0], direction]);
            });
            if (invalidOrderBys.length > 0) {
                var message;
                if (invalidOrderBys.length > 1)
                    message = sprintf.sprintf("The order by properties '%s' are invalid and will be removed when saving.", invalidOrderBys.join("', '"));
                else
                    message = sprintf.sprintf("The order by property '%s' is invalid and will be removed when saving.", invalidOrderBys[0]);
                ko.postbox.publish(this.parentGridId + "-persistent-warning-message", message);
            }
            return validOrderBys;
        };
        return SelectedOrderByViewModel;
    }());
    exports.default = SelectedOrderByViewModel;
});
