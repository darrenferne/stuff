var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "modules/bwf-explorer", "modules/grid/bwf-viewGrid-base", "modules/bwf-metadata", "modules/bwf-valueParser", "modules/bwf-utilities", "knockout", "options", "sprintf", "loglevel"], function (require, exports, explorer, vgBase, metadataService, valueParser, utils, ko, options, sprintf_m, log) {
    "use strict";
    var resources = options.resources;
    var sprintf = sprintf_m.sprintf;
    var ExplorerGridHeaderConfiguration = /** @class */ (function () {
        function ExplorerGridHeaderConfiguration(viewGrid) {
            this.applyChanges = viewGrid.applyChanges;
            this.cancelEditMode = viewGrid.exitEditMode;
            this.canApplyChanges = viewGrid.canApplyChanges;
            this.canDeleteRows = viewGrid.canDeleteRows;
            this.canInsertRows = viewGrid.canInsertRows;
            this.canEnterEditMode = viewGrid.canEnterEditMode;
            this.customButtons = viewGrid.customButtons;
            this.dataServiceActions = viewGrid.dataServiceActions;
            this.enterEditMode = viewGrid.enterEditMode;
            this.editModeAvailable = viewGrid.editModeAvailable;
            this.inEditMode = viewGrid.flags.inEditMode;
            this.isEnteringEditMode = viewGrid.isEnteringEditMode;
            this.showActionButtons = ko.observable(true);
            this.title = viewGrid.viewName;
            this.recordsCount = viewGrid.recordsCount;
            this.recordTypeActions = viewGrid.recordTypeActions;
            this.showTitle = ko.pureComputed({
                read: function () { return viewGrid.flags.showViewTitle() && !viewGrid.inGroupingMode(); },
                write: function (value) { return viewGrid.flags.showViewTitle(value); }
            });
            this.supportsEditMode = viewGrid.flags.supportsEditMode;
            this.canSelectIndividualCells = viewGrid.canSelectIndividualCells;
            this.deleteRow = function () { return ko.postbox.publish(viewGrid.viewGridId + '-delete-row'); };
            this.insertRow = function (above) {
                if (above === void 0) { above = true; }
                if (typeof above !== 'boolean') {
                    above = true;
                }
                ko.postbox.publish(viewGrid.viewGridId + '-insert-row', { insertAbove: above });
            };
            this.viewGridId = viewGrid.viewGridId;
        }
        return ExplorerGridHeaderConfiguration;
    }());
    var ExplorerGridConfiguration = /** @class */ (function () {
        function ExplorerGridConfiguration(viewGrid) {
            this.createNewRecord = null;
            this.flags = null;
            this.footer = null;
            this.header = null;
            this.inEditMode = null;
            this.isView = true;
            this.metadata = null;
            this.orderedBy = null;
            this.records = null;
            this.selectedColumns = null;
            this.selectedRecords = null;
            this.updateDirtyRecordWithLatestValues = null;
            this.validate = null;
            this.viewGridId = null;
            this.aggregates = viewGrid.aggregates;
            this.aggregatesPosition = ko.pureComputed(function () {
                if (viewGrid.aggregatesAtBottom) {
                    return 'bottom';
                }
                if (viewGrid.aggregatesAtTop) {
                    return 'top';
                }
                return viewGrid.aggregatesPosition();
            });
            this.createNewRecord = viewGrid.createNewRecord;
            this.inEditMode = viewGrid.flags.inEditMode;
            this.flags = viewGrid.flags;
            this.viewGridId = viewGrid.viewGridId;
            this.metadata = viewGrid.metadata;
            this.orderedBy = viewGrid.selectedOrderBys;
            this.records = viewGrid.records;
            this.recordsCount = viewGrid.recordsCount;
            this.selectedColumns = viewGrid.selectedColumns;
            this.selectedRecords = viewGrid.selectedRecords;
            this.validate = viewGrid.validateRecord;
            this.disableGridSorting = viewGrid.disableGridSorting;
            this.canSelectIndividualCells = viewGrid.canSelectIndividualCells;
            this.updateDirtyRecordWithLatestValues = viewGrid.updateDirtyRecordWithLatestValues;
            this.header = {
                enabled: ko.observable(true),
                config: new ExplorerGridHeaderConfiguration(viewGrid),
                name: 'grid/bwf-gridHeader'
            };
            var footerConfig = {
                autoUpdatesEnabled: viewGrid.flags.autoUpdatesEnabled,
                explorerDataService: viewGrid.explorerDataService,
                exportToExcel: viewGrid.exportToExcel,
                canEditCurrentView: viewGrid.canEditCurrentView,
                canOpenPowerBiPane: viewGrid.canOpenPowerBiPane,
                inEditMode: viewGrid.flags.inEditMode,
                isView: true,
                orderedBy: viewGrid.selectedOrderBys,
                paging: viewGrid.paging,
                disableGridSorting: viewGrid.disableGridSorting,
                viewId: viewGrid.viewId,
                viewGridId: viewGrid.viewGridId,
                exportToExcelEnabled: viewGrid.exportToExcelEnabled
            };
            this.footer = {
                enabled: ko.observable(true),
                config: footerConfig,
                name: 'grid/bwf-gridFooter'
            };
        }
        return ExplorerGridConfiguration;
    }());
    var GridQueryStrategy = /** @class */ (function () {
        function GridQueryStrategy(viewGrid) {
            var _this = this;
            this.query = ko.pureComputed(function () {
                if (_this.viewGrid.flags.loadingView() || !_this.viewGrid.flags.enableQuerying()) {
                    return null;
                }
                // so we don't query before we've loaded the parameter values
                if (!_this.viewGrid.flags.parameterBarRendered() && !_this.viewGrid.flags.disableParamsBar()) {
                    return null;
                }
                var skip = _this.viewGrid.paging.skip();
                var top = _this.viewGrid.paging.top();
                var queryBuild = _this.viewGrid.buildQuery();
                if (queryBuild == null) {
                    return null;
                }
                return sprintf("%ss?$skip=%d&$top=%d%s%s%s%s", _this.viewGrid.baseType(), skip, top, queryBuild.combinedFilter, queryBuild.expands, queryBuild.orderBy, queryBuild.aggregates);
            });
            this.viewGrid = viewGrid;
        }
        ;
        GridQueryStrategy.prototype.getQueryManager = function () {
            var dataService = metadataService.getDataService(this.viewGrid.dataService());
            return new explorer.GridQueryManager(dataService, this.viewGrid.viewGridId, this.viewGrid.aggregates, this.viewGrid.flags.autoUpdatesEnabled, this.viewGrid.flags.inEditMode, this.viewGrid.totalCount, this.query, this.viewGrid.records, this.viewGrid.selectedColumns, this.viewGrid.flags.querying, this.viewGrid.flags.enableQuerying);
        };
        ;
        GridQueryStrategy.prototype.queryForExcel = function () {
            var queryBuild = this.viewGrid.buildQuery();
            if (!queryBuild) {
                return null;
            }
            var query = sprintf("%ss?%s%s%s%s", queryBuild.baseType, queryBuild.combinedFilter, queryBuild.expands, queryBuild.orderBy, queryBuild.aggregates);
            return query;
        };
        ;
        return GridQueryStrategy;
    }());
    var GroupingGridQueryStrategy = /** @class */ (function () {
        function GroupingGridQueryStrategy(viewGrid) {
            var _this = this;
            this.query = ko.pureComputed(function () {
                if (_this.viewGrid.flags.loadingView() || !_this.viewGrid.flags.enableQuerying()) {
                    return null;
                }
                // so we don't query before we've loaded the parameter values
                if (!_this.viewGrid.flags.parameterBarRendered() && !_this.viewGrid.flags.disableParamsBar()) {
                    return null;
                }
                var skip = _this.viewGrid.paging.skip();
                var top = _this.viewGrid.paging.top();
                var queryBuild = _this.viewGrid.buildGroupingQuery();
                if (queryBuild == null) {
                    return null;
                }
                return sprintf("%ss/flattened?$skip=%d&$top=%d%s%s%s%s%s", _this.viewGrid.baseType(), skip, top, queryBuild.dataFilter, queryBuild.groupBy, queryBuild.aggregations, queryBuild.includeSubtotals, queryBuild.includeTotals);
            });
            this.viewGrid = viewGrid;
        }
        ;
        GroupingGridQueryStrategy.prototype.getQueryManager = function () {
            var dataService = metadataService.getDataService(this.viewGrid.dataService());
            this.viewGrid.aggregates([]);
            return new explorer.GroupingGridQueryManager(dataService, this.viewGrid.viewGridId, this.viewGrid.flags.autoUpdatesEnabled, this.viewGrid.flags.inEditMode, this.viewGrid.totalCount, this.query, this.viewGrid.records, this.viewGrid.selectedColumns, this.viewGrid.flags.querying, this.viewGrid.currentGroupingLevel, this.viewGrid.flags.enableQuerying);
        };
        GroupingGridQueryStrategy.prototype.queryForExcel = function () {
            var queryBuild = this.viewGrid.buildGroupingQuery();
            if (!queryBuild) {
                return null;
            }
            var query = sprintf("%ss?%s%s%s%s%s", queryBuild.baseType, queryBuild.dataFilter, queryBuild.groupBy, queryBuild.aggregations, queryBuild.includeSubtotals, queryBuild.includeTotals);
            return query;
        };
        ;
        return GroupingGridQueryStrategy;
    }());
    var GridStrategyFactory = /** @class */ (function () {
        function GridStrategyFactory(viewGrid) {
            var _this = this;
            this.gridQueryStrategy = new GridQueryStrategy(viewGrid);
            this.groupingGridQueryStrategy = new GroupingGridQueryStrategy(viewGrid);
            this.queryStrategy = ko.pureComputed(function () {
                if (viewGrid.inGroupingMode() && !viewGrid.isAtMaxGroupingLevel()) {
                    return _this.groupingGridQueryStrategy;
                }
                else {
                    return _this.gridQueryStrategy;
                }
            });
        }
        return GridStrategyFactory;
    }());
    var ViewGrid = /** @class */ (function (_super) {
        __extends(ViewGrid, _super);
        function ViewGrid(moduleConfig) {
            var _this = _super.call(this, moduleConfig) || this;
            // properties
            _this.addedColumnsForEditMode = ko.observableArray([]);
            _this.aggregates = ko.observableArray([]);
            _this.aggregatesPosition = ko.observable('bottom');
            _this.aggregationFunctions = ko.observableArray([]);
            _this.baseType = ko.observable('');
            _this.currentGroupingLevel = ko.observable(null);
            _this.disableGridSorting = ko.observable(false);
            _this.explorerDataService = ko.observable(null);
            _this.gridPresentation = ko.observable(null);
            _this.grouping = ko.observable(null);
            _this.groupingAggregations = ko.observableArray([]);
            _this.includeGroupingTotals = ko.observable(null);
            _this.includeGroupingSubtotals = ko.observableArray([]);
            _this.inGroupingMode = ko.observable(false);
            _this.invalidColumnsOrderByMessages = ko.observableArray([]);
            _this.isEnteringEditMode = ko.observable(false);
            _this.groupedByFilter = ko.observableArray([]);
            _this._selectedColumns = ko.observableArray([]);
            _this.selectedOrderBys = ko.observableArray([]);
            _this.selectedParameters = ko.observableArray([]);
            _this.selectedParameterValues = ko.observableArray([]);
            _this.selectionFilter = ko.observable('');
            _this.selectedAggregations = ko.observableArray([]);
            _this.urlAggregates = ko.observableArray([]);
            _this.validationsInProgress = {};
            // computed properties for the grid query
            _this.aggregatesQueryFragment = ko.pureComputed(function () {
                if (_this.urlAggregates().length > 0) {
                    return _this.urlAggregates()
                        .map(function (aggregate) {
                        return sprintf("%s(%s%s)", aggregate.Method, aggregate.Property, aggregate.Property2 ? ';' + aggregate.Property2 : '');
                    }).join(',');
                }
                else {
                    return _this.selectedAggregations()
                        .map(function (aggregate) {
                        return sprintf("%s(%s%s)", aggregate.Function.Value, aggregate.PropertyName, aggregate.ExtraParameters ? ';' + aggregate.ExtraParameters : '');
                    }).join(',');
                }
            });
            _this.aggregationsQueryFragment = ko.pureComputed(function () {
                return _this.groupingAggregations()
                    .map(function (aggregation) {
                    return _this.getAggregationQueryString(aggregation);
                }).join(',');
            });
            _this.expandsQueryFragment = ko.pureComputed(function () {
                var metadata = _this.metadata();
                var selectedcolumns = _this._selectedColumns();
                if (metadata == null) {
                    return null;
                }
                var identityProperties = metadata.identityProperties;
                var required = [];
                selectedcolumns
                    .map(function (c) { return c.path; })
                    .forEach(function (path) {
                    var seperator = path.lastIndexOf('/');
                    if (seperator === -1) {
                        return;
                    }
                    var type = path.substring(0, seperator);
                    required.push(type);
                });
                required = required
                    .concat(identityProperties)
                    .concat(metadata.expandsRequiredForEdit)
                    .filter(function (expand) { return !!expand; })
                    .filter(function (value, index, source) { return source.indexOf(value) === index; });
                return required.join(',');
            });
            _this.exportToExcelEnabled = ko.pureComputed(function () {
                var filter = _this.parameterFilter();
                var expands = _this.expandsQueryFragment();
                return !(filter == null || expands == null);
            });
            _this.isAtMaxGroupingLevel = ko.pureComputed(function () {
                return _this.currentGroupingLevel() > _this.grouping().GroupByColumns[_this.grouping().GroupByColumns.length - 1].Level;
            });
            _this.buildQuery = ko.pureComputed(function () {
                var orderBy = _this.orderByQueryFragment();
                var expands = _this.expandsQueryFragment();
                var aggregates = _this.aggregatesQueryFragment();
                var parameterFilter = _this.parameterFilter();
                var selectionFilter = _this.selectionFilter();
                var groupingFilter = _this.groupingFilter();
                // if it is null instead of empty string then something has gone
                // wrong so we shouldn't query anything
                if (parameterFilter == null || expands == null) {
                    return null;
                }
                // these properties don't include the keyword in case we want to display
                // the value somewhere. This definitely happens for orderbys, others may
                // be used in future. Either way, treating them consistently is good.
                if (expands.length > 0)
                    expands = sprintf("&$expand=%s", expands);
                if (orderBy.length > 0)
                    orderBy = sprintf("&$orderby=%s", orderBy);
                if (aggregates.length > 0)
                    aggregates = sprintf("&$aggregate=%s", aggregates);
                // filter combining is done here instead of in a computed observable
                // so we can do the check above.
                var combinedFilters = [];
                var combinedFilter = "";
                if (selectionFilter != null && selectionFilter.length > 0) {
                    combinedFilters.push(selectionFilter);
                }
                if (parameterFilter != null && parameterFilter.length > 0) {
                    combinedFilters.push(parameterFilter);
                }
                if (groupingFilter != null && groupingFilter.length > 0) {
                    combinedFilters.push(groupingFilter);
                }
                if (combinedFilters.length === 1) {
                    combinedFilter = combinedFilters[0];
                }
                else if (combinedFilters.length > 1) {
                    combinedFilter = "(" + combinedFilters.join(") and (") + ")";
                }
                if (combinedFilter.length > 0) {
                    combinedFilter = sprintf("&$filter=%s", combinedFilter);
                }
                return {
                    baseType: _this.baseType(),
                    combinedFilter: combinedFilter,
                    expands: expands,
                    orderBy: orderBy,
                    aggregates: aggregates
                };
            });
            _this.buildGroupingQuery = ko.pureComputed(function () {
                var groupBy = _this.orderByQueryFragment();
                var aggregations = _this.aggregationsQueryFragment();
                var parameterFilter = _this.parameterFilter();
                var selectionFilter = _this.selectionFilter();
                var groupingFilter = _this.groupingFilter();
                var includeSubtotals = _this.includeGroupingSubtotals().join(',');
                var includeTotals = _this.includeGroupingTotals();
                // if it is null instead of empty string then something has gone
                // wrong so we shouldn't query anything
                if (parameterFilter == null || groupBy == null || aggregations == null || groupingFilter == null)
                    return null;
                // these properties don't include the keyword in case we want to display
                // the value somewhere. This definitely happens for orderbys, others may
                // be used in future. Either way, treating them consistently is good.
                if (groupBy.length > 0)
                    groupBy = sprintf("&$groupby=%s", groupBy);
                if (aggregations.length > 0)
                    aggregations = sprintf("&$aggregations=%s", aggregations);
                if (includeSubtotals.length > 0)
                    includeSubtotals = sprintf("&$includesubtotals=%s", includeSubtotals);
                includeTotals = sprintf("&$includetotals=%s", includeTotals || false);
                // filter combining is done here instead of in a computed observable
                // so we can do the check above.
                var dataFilters = [];
                var dataFilter = "";
                if (selectionFilter != null && selectionFilter.length > 0)
                    dataFilters.push(selectionFilter);
                if (parameterFilter != null && parameterFilter.length > 0)
                    dataFilters.push(parameterFilter);
                if (groupingFilter != null && groupingFilter.length > 0)
                    dataFilters.push(groupingFilter);
                if (dataFilters.length === 1) {
                    dataFilter = dataFilters[0];
                }
                else if (dataFilters.length > 1) {
                    dataFilter = "(" + dataFilters.join(") and (") + ")";
                }
                if (dataFilter.length > 0)
                    dataFilter = sprintf("&$datafilter=%s", dataFilter);
                return {
                    baseType: _this.baseType(),
                    dataFilter: dataFilter,
                    groupBy: groupBy,
                    aggregations: aggregations,
                    includeSubtotals: includeSubtotals,
                    includeTotals: includeTotals
                };
            });
            _this.orderByQueryFragment = ko.pureComputed(function () {
                return _this.selectedOrderBys().join(',');
            });
            _this.groupingFilter = ko.pureComputed(function () {
                var metadata = _this.metadata();
                if (!_this.inGroupingMode() || metadata == null)
                    return null;
                var filter = _this.groupedByFilter()
                    .map(function (g) {
                    var values = g.groupedBy.map(function (v) {
                        var propertyMetadata = metadataService.getProperty(metadata, v.Key);
                        var value = valueParser.getRawValue(propertyMetadata.type, v.Value);
                        if (value === null) {
                            return v.Key + " isnull";
                        }
                        else {
                            return utils.UrlUtilities.getEncodedValue(propertyMetadata.type, v.Key, "=", value);
                        }
                    });
                    if (values.some(function (v) { return v == null; })) {
                        _this.showIncorrectQueryUrlParams();
                        return null;
                    }
                    return values.join(' and ');
                });
                if (filter.some(function (f) { return !f; })) {
                    // null to signify an invalid filter and that we should not query for records
                    return null;
                }
                return filter.join(' and ');
            });
            _this.parameterFilter = ko.pureComputed(function () {
                var selectedParameterValues = _this.selectedParameterValues();
                var grouped = selectedParameterValues
                    .reduce(function (acc, n) {
                    if (acc[n.field] === undefined)
                        acc[n.field] = [];
                    acc[n.field].push(n);
                    return acc;
                }, {});
                var parameterFilters = Object.keys(grouped).map(function (key) {
                    var group = grouped[key];
                    var includeEmpty = group.some(function (g) { return g.includeEmpty; });
                    var filterPieces = group.map(function (spv) {
                        var values = spv.values.map(function (v) { return utils.UrlUtilities.getEncodedValue(spv.type, spv.field, spv.operator, v); });
                        if (values.some(function (v) { return v == null; })) {
                            _this.showIncorrectQueryUrlParams();
                            return null;
                        }
                        var joiningOperator = '';
                        switch (spv.operator) {
                            case '<=':
                            case '<':
                            case '>':
                            case '>=':
                            case '!=':
                            case 'notLike':
                                joiningOperator = ' and ';
                                break;
                            default:
                                joiningOperator = ' or ';
                                break;
                        }
                        return values.join(joiningOperator);
                    }).filter(function (f) { return f.length !== 0; });
                    var filter;
                    if (filterPieces.length > 0) {
                        filter = filterPieces
                            .map(function (f) { return "(" + f + ")"; })
                            .join(' and ');
                    }
                    var emptyOperator = includeEmpty ? 'or' : 'and';
                    var empty = sprintf(includeEmpty
                        ? '%s isnull'
                        : '%s isnotnull', group[0].field);
                    if (filterPieces.length > 0) {
                        return includeEmpty
                            ? "((" + filter + ") " + emptyOperator + " " + empty + ")"
                            : "(" + filter + ")";
                    }
                    else {
                        return empty;
                    }
                });
                if (parameterFilters.some(function (pF) { return !pF; })) {
                    // null to signify an invalid filter and that we should not query for records
                    return null;
                }
                return parameterFilters.join(' and ');
            });
            // other computed properties
            _this.canApplyChanges = ko.pureComputed(function () {
                var noValidationErrors = !_this.hasAnyValidationErrors();
                var inEditMode = _this.flags.inEditMode();
                var isApplyInProgress = _this.flags.applyInProgress();
                var canApplyChanges = inEditMode || !isApplyInProgress;
                return noValidationErrors && canApplyChanges;
            });
            _this.hasAnyValidationErrors = ko.pureComputed(function () {
                return _this.records().filter(function (r) { return r._destroy !== true; })
                    .map(function (r) { return r.hasValidationErrors(); })
                    .some(function (ve) { return ve; });
            });
            _this.canEnterEditMode = ko.pureComputed(function () {
                var isEnteringEditMode = _this.isEnteringEditMode();
                var inEditMode = _this.flags.inEditMode();
                var supportsEditMode = _this.editModeAvailable();
                return (supportsEditMode && !(inEditMode || isEnteringEditMode));
            });
            _this.canInsertRows = ko.pureComputed(function () {
                var inEditMode = _this.flags.inEditMode();
                var metadata = _this.metadata();
                return inEditMode && metadata && metadata.insertableInEditMode;
            });
            _this.canDeleteRows = ko.pureComputed(function () {
                var inEditMode = _this.flags.inEditMode();
                var metadata = _this.metadata();
                return inEditMode && metadata && metadata.deletableInEditMode;
            });
            _this.canOpenPowerBiPane = ko.pureComputed(function () { return !_this.inGroupingMode(); });
            _this.editModeAvailable = ko.pureComputed(function () {
                var supportsEditMode = _this.flags.supportsEditMode();
                var disableEditMode = _this.flags.disableEditMode();
                return supportsEditMode && !disableEditMode && (!_this.inGroupingMode() || _this.isAtMaxGroupingLevel());
            });
            _this.enableParameters = ko.pureComputed(function () { return !_this.flags.disableParamsBar(); });
            _this.showParameters = ko.pureComputed({
                read: function () {
                    var length = _this.selectedParameters().length;
                    var show = _this.flags.showParameterBar();
                    return show && length > 0 && !_this.flags.inEditMode();
                },
                write: function (v) { return _this.flags.showParameterBar(!v); }
            });
            _this.selectedColumns = ko.pureComputed(function () {
                var toReturn = _this._selectedColumns();
                if (_this.flags.inEditMode()) {
                    var columnsToAdd = _this.missingPropertyColumns();
                    toReturn = toReturn.concat(columnsToAdd);
                }
                return toReturn;
            });
            _this.missingPropertyColumns = ko.computed(function () {
                var metadata = _this.metadata();
                if (!metadata)
                    return [];
                var missingProperties = [];
                var requiredProperties = Object.keys(metadata.properties)
                    .map(function (key) { return metadata.properties[key]; })
                    .filter(function (y) { return y.isMandatoryInEditMode; });
                var selectedColumns = _this._selectedColumns();
                missingProperties = requiredProperties
                    .filter(function (x) { return selectedColumns
                    .filter(function (y) { return y.path.split("/")[0] === x.name; }).length === 0; });
                var missingPropertyColumns = missingProperties
                    .map(function (x, index) { return new explorer.ExplorerGridColumn(x, x.name, _this._selectedColumns().length + index + 1); });
                _this.records().forEach(function (x) { return x.configureColumns(missingPropertyColumns); });
                return missingPropertyColumns;
            });
            _this.applyChanges = function () {
                if (!_this.canApplyChanges())
                    return;
                _this.flags.applyInProgress(true);
                _this.clearSelected();
                var records = _this.records();
                records
                    .filter(function (x) { return !x._destroy; })
                    .forEach(function (x) { return _this.updateDirtyRecordWithLatestValues(x, _this.selectedColumns()); });
                var anyValidationErrors = records.some(function (r) { return Object.keys(r.values).some(function (key) { return !r.values[key].isValid(); }); }) ||
                    records.some(function (x) { return x.modelValidations().length > 0; });
                if (anyValidationErrors) {
                    _this.flags.applyInProgress(false);
                    return;
                }
                var request = _this.applyChangesRequest(records);
                request.fail(function (failure) {
                    log.error(failure.responseJSON["fullException"]);
                    _this.errorMessage.showError("Error occurred", failure.responseJSON["message"]);
                    _this.flags.applyInProgress(false);
                });
                request.done(function (response) { return _this.processApplyChangesResponse(response); });
            };
            _this.applyChangesRequest = function (records) {
                records.forEach(function (r) {
                    r.modelValidations([]);
                    Object.keys(r.values).forEach(function (key) { return r.values[key].validationMessages([]); });
                });
                var toCreate = {};
                var toUpdate = {};
                var toDelete = records.filter(function (r) { return r._destroy; }).map(function (r) { return r.id; });
                records.filter(function (r) { return r.isNewRecord(); })
                    .forEach(function (r) { return toCreate[r.bwfId] = r.dirtyRecord; });
                records.filter(function (r) { return r.hasUnsavedChanges() && !r.isNewRecord(); })
                    .forEach(function (r) { return toUpdate[r.id] = r.dirtyRecord; });
                var changeSet = {
                    Create: toCreate,
                    Update: toUpdate,
                    Delete: toDelete
                };
                var changeSetUrl = sprintf("%s/changeset/%s", _this.dataServiceUrl(), _this.baseType);
                var request = $.ajax({
                    url: changeSetUrl,
                    xhrFields: {
                        withCredentials: true
                    },
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify(changeSet)
                });
                return request;
            };
            _this.createNewRecord = function () {
                var newRecord = {
                    Id: utils.getNextBwfId().toString(),
                    Position: 0,
                    Data: metadataService.generateNewRecord(_this.metadata()),
                    BaseTypeName: _this.metadata().type,
                    TypeName: _this.metadata().type
                };
                _this.initialiseNewRecord(newRecord.Data);
                var newRow = new explorer.ExplorerGridItem(newRecord, _this.selectedColumns(), _this.dataService(), { updateType: "Added" });
                return newRow;
            };
            _this.enterEditMode = function () {
                var canEdit = _this.canEnterEditMode();
                if (!canEdit)
                    return;
                _this.clearSelected();
                _this.isEnteringEditMode(true);
                _this.queryExecutor().refresh().done(function () {
                    _this.flags.inEditMode(true);
                    ko.postbox.publish(_this.viewGridId + '-resizeRequired');
                    var missingPropertyColumns = _this.missingPropertyColumns();
                    if (missingPropertyColumns.length > 0) {
                        var missingPropertiesString = missingPropertyColumns
                            .map(function (x) { return "'" + ko.unwrap(x.displayName) + "'"; })
                            .join(", ");
                        var message = missingPropertyColumns.length === 1
                            ? resources["bwf_mandatory_columns_message_single"]
                            : resources["bwf_mandatory_columns_message_multiple"];
                        ko.postbox.publish("bwf-transient-notification", {
                            message: sprintf("%s %s", missingPropertiesString, message),
                            styleClass: "alert-warning"
                        });
                    }
                    _this.isEnteringEditMode(false);
                });
            };
            _this.exitEditMode = function () {
                _this.flags.querying(true);
                if (_this.queryExecutor()) {
                    _this.queryExecutor().refresh()
                        .done(_this.cancelEditMode)
                        .always(function () {
                        _this.flags.querying(false);
                        _this.flags.applyInProgress(false);
                    });
                }
                else {
                    _this.cancelEditMode();
                    _this.flags.querying(false);
                    _this.flags.applyInProgress(false);
                }
            };
            _this.exportToExcel = function () {
                if (!_this.exportToExcelEnabled() || _this.flags.exportInProgress())
                    return null;
                _this.flags.exportInProgress(true);
                var query = _this.queryStrategy().queryForExcel();
                var ds = metadataService.getDataService(_this.dataService());
                var exportUrl = sprintf("%s/export/excel/%s", ds.url, query);
                var viewName = _this.viewName();
                // flatten the breadcrumbs
                var breadcrumbs = _this.breadcrumbConfiguration.breadcrumbs()
                    .filter(function (x) { return x.data != null; })
                    .map(function (x) { return x.data; });
                var args = {
                    Name: _this.viewName(),
                    BreadCrumbs: breadcrumbs,
                    InGroupingMode: _this.inGroupingMode() && !_this.isAtMaxGroupingLevel(),
                    SelectedColumns: _this._selectedColumns().map(function (c) {
                        return {
                            Field: c.path.replace(/\//g, '.'),
                            DisplayName: c.displayName() != null ? c.displayName() : c.metadata.abbreviatedName,
                            Format: _this.deriveFormat(c),
                            Alignment: c.metadata.alignment,
                            Type: c.metadata.type
                        };
                    })
                };
                $.ajax({
                    url: exportUrl,
                    xhrFields: {
                        withCredentials: true
                    },
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify(args)
                }).done(function (response) {
                    var url = sprintf("%s/api/generated/%s/%s", ds.hostUrl, response, viewName);
                    $("<iframe style='display: none' src='" + url + "'></iframe>").appendTo("body");
                    _this.flags.exportInProgress(false);
                }).fail(function (message) {
                    ko.postbox.publish("bwf-transient-notification", {
                        message: 'Error posting export to excel request:' + message.responseText,
                        styleClass: 'alert-danger',
                        requireDismissal: true
                    });
                    _this.flags.exportInProgress(false);
                });
            };
            _this.validateRecord = function (record, success, failure) {
                if (_this.validationsInProgress[record.bwfId])
                    _this.validationsInProgress[record.bwfId].abort();
                _this.updateDirtyRecordWithLatestValues(record, _this.selectedColumns());
                var ds = metadataService.getDataService(_this.dataService());
                var saveUrl = sprintf("%s/%s/%s/withoutpersistance", ds.url, record.typeName, (record.updateType() === "Added" ? "" : record.id));
                var data;
                if (record.updateType() === "Added" && (record.metadata.hasEditabilityToRoles || record.metadata.hasVisibilityToRoles)) {
                    data = JSON.stringify({
                        Record: record.dirtyRecord,
                        VisibleToRoles: [],
                        EditableByRoles: []
                    });
                }
                else {
                    data = JSON.stringify(record.dirtyRecord);
                }
                _this.validationsInProgress[record.bwfId] = $.ajax({
                    url: saveUrl,
                    xhrFields: {
                        withCredentials: true
                    },
                    type: record.updateType() === "Added" ? 'POST' : 'PUT',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: data
                });
                _this.validationsInProgress[record.bwfId].done(function (response) {
                    success(record, response);
                });
                _this.validationsInProgress[record.bwfId].fail(function (response) {
                    if (response.statusText !== 'abort')
                        failure(record, JSON.parse(response.responseText));
                });
            };
            _this.nextGroupingLevel = function (data) {
                _this.setGroupingLevel(_this.currentGroupingLevel() + 1, [data]);
            };
            _this.resetGroupingLevel = function () {
                _this.groupedByFilter([]);
                if (_this.inGroupingMode()) {
                    _this.setGroupingLevel(0);
                }
                else {
                    _this.currentGroupingLevel(null);
                }
            };
            _this.setGroupingLevel = function (newLevelStart, groupedBys) {
                var currentLevel = _this.currentGroupingLevel();
                groupedBys = groupedBys || [];
                if (newLevelStart !== 0 && groupedBys.length === 0 ||
                    newLevelStart > currentLevel + 1 ||
                    newLevelStart < 0) {
                    log.error("Failed to set grouping level. Invalid grouped by path from level " + currentLevel + " to " + newLevelStart + ".");
                    ko.postbox.publish('bwf-transient-notification', {
                        message: "Setting a grouping level failed",
                        styleClass: 'alert-warning',
                        requireDismissal: true
                    });
                    throw "Failed to set grouping level. Invalid grouped by path from level " + currentLevel + " to " + newLevelStart + ".";
                }
                var newLevel = newLevelStart === 0 ? 0 : newLevelStart + groupedBys.length - 1;
                _this.currentGroupingLevel(newLevel);
                // Stop querying
                var pauseQuerying = _this.queryExecutor() != null;
                if (pauseQuerying) {
                    _this.queryExecutor().dispose();
                    _this.queryExecutor(null);
                }
                // Remove old history
                _this.groupedByFilter.splice(newLevelStart - 1);
                (_a = _this.groupedByFilter).push.apply(_a, groupedBys);
                if (_this.isAtMaxGroupingLevel()) {
                    _this.disableGridSorting(_this.gridPresentation().DisableGridSorting);
                    _this.groupingAggregations([]);
                    _this.includeGroupingSubtotals([]);
                    _this.includeGroupingTotals(null);
                    _this.records([]);
                    _this.selectedOrderBys(_this.getOrderBys(_this.gridPresentation().OrderBy));
                    _this._selectedColumns(_this.getColumns(_this.gridPresentation()));
                    _this.selectedAggregations(_this.gridPresentation().Aggregations);
                    _this.aggregatesPosition(_this.gridPresentation().AggregationsPosition.Value);
                    _this.recordTypeActions(_this.recordTypeActionsResult
                        .map(function (action) { return new explorer.ActionButtonViewModel(action, _this); })
                        .sort(function (l, r) { return l.Position - r.Position; }));
                    // Re-enable querying
                    if (pauseQuerying) {
                        _this.queryExecutor(_this.queryStrategy().getQueryManager());
                    }
                }
                else {
                    _this.disableGridSorting(true);
                    _this.selectedAggregations([]);
                    _this.aggregatesPosition('bottom');
                    _this.records([]);
                    _this.groupingAggregations(_this.getGroupingAggregations(_this.grouping()));
                    _this.selectedOrderBys(_this.getGroupingOrderBys(_this.grouping()));
                    _this._selectedColumns(_this.getGroupingColumns(_this.grouping(), _this.gridPresentation()));
                    _this.includeGroupingSubtotals(_this.getGroupingSubtotals(_this.grouping()));
                    _this.includeGroupingTotals(_this.grouping().IncludeTotals);
                    _this.recordTypeActions(_this.getGroupingActions()
                        .map(function (action) { return new explorer.ActionButtonViewModel(action, _this); })
                        .sort(function (l, r) { return l.Position - r.Position; }));
                    // Re-enable querying
                    if (pauseQuerying)
                        _this.queryExecutor(_this.queryStrategy().getQueryManager());
                }
                var _a;
            };
            _this.setGroupingBreadcrumbs = function () {
                var metadata = _this.metadata();
                var breadcrumbs = _this.breadcrumbConfiguration.breadcrumbs;
                if (!_this.inGroupingMode() || metadata == null) {
                    breadcrumbs([]);
                }
                else {
                    var groupedByFilter_1 = _this.groupedByFilter();
                    breadcrumbs([{
                            displayName: _this.viewName(),
                            data: null,
                            onClick: _this.resetGroupingLevel,
                            url: options.explorerHostUrl + "/view/#open/" + _this.viewName(),
                            active: (groupedByFilter_1.length === 0)
                        }]);
                    breadcrumbs.push.apply(breadcrumbs, groupedByFilter_1
                        .map(function (item, index) {
                        var displayName = "";
                        var description = "";
                        var data = [];
                        item.groupedBy.forEach(function (g, i) {
                            var property = metadataService.getProperty(metadata, g.Key);
                            var value = valueParser.getDisplayValue(property.type, g.Value);
                            displayName += ((i === 0) ? "" : ", ") + value;
                            description += ((i === 0) ? "" : "\n") + property.displayName + ": " + value;
                            data.push({ "Title": property.displayName, "Value": value });
                        });
                        var isLastItem = (index + 1 === groupedByFilter_1.length);
                        return {
                            displayName: displayName,
                            description: description,
                            data: data,
                            onClick: !isLastItem ? (function () { _this.setGroupingLevel(item.level, [item]); }) : null,
                            active: isLastItem
                        };
                    }));
                }
            };
            _this.aggregatesAtTop = moduleConfig.aggregatesAtTop;
            _this.aggregatesAtBottom = moduleConfig.aggregatesAtBottom;
            metadataService.getDataServiceSafely("explorer")
                .done(function (dataService) {
                _this.explorerDataService(dataService);
            });
            _this.gridConfiguration = new ExplorerGridConfiguration(_this);
            var strategyFactory = new GridStrategyFactory(_this);
            _this.queryStrategy = strategyFactory.queryStrategy;
            _this.breadcrumbConfiguration = {
                breadcrumbs: ko.observableArray([]),
                disabled: _this.flags.inEditMode
            };
            _this.initialiseNewRecord = moduleConfig.initialiseNewRecord || (function () { return; });
            _this.setupSubscriptions(moduleConfig);
            return _this;
        }
        ViewGrid.prototype.getAggregationQueryString = function (aggregation) {
            return sprintf("%s(%s%s%s)", aggregation.Function, aggregation.Name, aggregation.ExtraParameters ? ";" + aggregation.ExtraParameters : "", aggregation.Options ? ":" + aggregation.Options.Value : "");
        };
        ViewGrid.prototype.getAggregationIdString = function (aggregation) {
            return sprintf("%s_%s_%s_%s", aggregation.Function, aggregation.Name, aggregation.ExtraParameters ? aggregation.ExtraParameters : "", aggregation.Options ? aggregation.Options.Value : "");
        };
        ViewGrid.prototype.updateDirtyRecordWithLatestValues = function (item, columns) {
            columns.filter(function (c) { return !c.metadata.hasChoice && !c.metadata.isNotEditableInGrid && !item.values[c.path].isReadonly(); })
                .forEach(function (c) {
                item.dirtyRecord[c.path] = item.values[c.path]();
            });
        };
        ViewGrid.prototype.processApplyChangesResponse = function (response) {
            this.records().forEach(function (r) { return r.applyChangeSetResult(response); });
            // the _destroy flag KO sets isnt an observable, so since processing the 
            // change set may have "undeleted" some objects we need to tell KO that the 
            // array has mutated so any undeleted records reappear in the grid
            this.records.valueHasMutated();
            var anyErrors = Object.keys(response.FailedCreates)
                .concat(Object.keys(response.FailedDeletions))
                .concat(Object.keys(response.FailedUpdates))
                .filter(function (key) { return key[0] !== '$'; })
                .length > 0;
            // remove records that have been deleted
            this.records.remove(function (r) { return response.SuccessfullyDeleted.some(function (d) { return r.id === d; }); });
            if (anyErrors) {
                this.flags.applyInProgress(false);
                return;
            }
            // refresh the grid and exit edit mode after the query is complete
            this.exitEditMode();
        };
        ViewGrid.prototype.loadView = function (data) {
            var _this = this;
            // Reset State
            this.resetView(data);
            this.invalidColumnsOrderByMessages([]);
            this.selectedParameters([]);
            this.selectedParameterValues([]);
            this._selectedColumns([]);
            this.selectedOrderBys([]);
            this.selectedAggregations([]);
            this.groupingAggregations([]);
            this.includeGroupingTotals(null);
            this.includeGroupingSubtotals([]);
            this.aggregates([]);
            this.aggregatesPosition('bottom');
            this.recordTypeActionsResult = [];
            // Setup New View
            this.customiseColumnHeaders = (typeof data.applyCustomColumnHeaders === "function")
                ? data.applyCustomColumnHeaders
                : function () { return; };
            this.flags.disableEditMode(!!data.disableEditMode);
            var buttons = (data.customButtons || [])
                .map(function (cb) { return new explorer.ActionButtonViewModel(cb, _this); });
            this.customButtons(buttons);
            this.urlAggregates(data.urlAggregates || []);
            this.urlFilteredBy(data.urlFilteredby || []);
            this.urlParameters(data.urlParameters || []);
            this.viewName(data.viewName);
            var viewQuery = options.explorerHostUrl + "/api/Explorer/Query/Views";
            var params = "$top=1" +
                "&$expand=System,Selection,Selection/System,Presentation,GridPresentation,GridPresentation/SelectedColumns,GridPresentation/Aggregations,SelectedParameters,Grouping,Grouping/GroupByColumns,Grouping/Aggregations" +
                ("&$filter=Name='" + this.viewName().replace(/'/g, "\\'") + "'");
            var getView = $.ajax({
                xhrFields: {
                    withCredentials: true
                },
                data: { q: params },
                url: viewQuery
            });
            var viewLoadErrorMessage = "The view '" + this.viewName() + "' cannot be opened. " +
                "This may be because it doesn't exist, or you do not have the required permissions to open it.";
            getView.fail(function () { return _this.showGridLoadError(viewLoadErrorMessage); });
            getView.done(function (result) {
                if (result.TotalCount === 0)
                    _this.showGridLoadError(viewLoadErrorMessage);
                var view = result.Records[0];
                _this.gridConfiguration.view = view;
                var ds = metadataService.getDataServiceSafely(view.DataServiceSystem);
                ds.done(function (dataService) { return _this.loadedView(view, dataService, data.enableQuerying); });
                ds.fail(function () { return _this.showGridLoadError("The view '" + _this.viewName() + "' cannot be opened. The dataservice '" + view.DataServiceSystem + "' cannot be found."); });
            });
        };
        ViewGrid.prototype.loadedView = function (view, ds, enableQuerying) {
            var _this = this;
            this.dataServiceUrl(ds.url);
            this.inGroupingMode(view.Grouping != null);
            var dataServiceUrl = sprintf("%s/api/Explorer/Query/DataServices", options.explorerHostUrl);
            var getActionsUrl = sprintf("%s/api/%s/AvailableActions", ds.hostUrl, view.DataServiceSystem);
            var recordActionsUrl = sprintf("%s/%s", getActionsUrl, view.BaseType);
            var requests = [];
            requests.push($.ajax({
                xhrFields: {
                    withCredentials: true
                },
                url: dataServiceUrl
            }));
            requests.push($.ajax({
                xhrFields: {
                    withCredentials: true
                },
                url: getActionsUrl
            }));
            requests.push($.ajax({
                xhrFields: {
                    withCredentials: true
                },
                url: recordActionsUrl
            }));
            requests.push(metadataService.getType(view.DataServiceSystem, view.BaseType));
            if (this.inGroupingMode()) {
                var aggregationFunctionsUrl = sprintf("%s/api/%s/AvailableAggregationFunctions", ds.hostUrl, view.DataServiceSystem);
                requests.push($.ajax({
                    xhrFields: {
                        withCredentials: true
                    },
                    url: aggregationFunctionsUrl
                }));
            }
            $.when.apply($, requests)
                .done(function (d, da, ra, m, af) {
                _this.loadedUniverse(view, d[0], da[0], ra[0], m, (af ? af[0] : null), enableQuerying);
            })
                .fail(function (message) {
                log.error("An error occurred when loading the view: ", message);
            });
        };
        ViewGrid.prototype.loadedUniverse = function (view, dataServicesResult, dataServiceActionsResult, recordTypeActionsResult, metadata, aggregationFunctions, enableQuerying) {
            var _this = this;
            if (enableQuerying === void 0) { enableQuerying = true; }
            this.flags.enableQuerying(enableQuerying);
            this.flags.autoUpdatesEnabled(metadata.autoUpdatesByDefault);
            this.flags.supportsEditMode(metadata.supportsEditMode);
            this.metadata(metadata);
            this.baseType(view.BaseType);
            this.dataService(dataServicesResult.Records
                .filter(function (ds) { return ds.System === view.DataServiceSystem; })[0].System);
            this.aggregationFunctions(aggregationFunctions);
            this.selectionFilter(view.Selection.Filter ? view.Selection.Filter : '');
            this.gridPresentation(view.GridPresentation);
            this.recordTypeActionsResult = recordTypeActionsResult;
            this.invalidColumnsOrderByMessages([]);
            if (this.inGroupingMode()) {
                this.updateGroupingLevels(view.Grouping.GroupByColumns);
                this.grouping(view.Grouping);
                this.resetGroupingLevel();
            }
            else {
                this.selectedOrderBys(this.getOrderBys(view.GridPresentation.OrderBy));
                this._selectedColumns(this.getColumns(view.GridPresentation));
                this.disableGridSorting(view.GridPresentation.DisableGridSorting);
                this.selectedAggregations(view.GridPresentation.Aggregations);
                this.aggregatesPosition(view.GridPresentation.AggregationsPosition.Value);
                this.recordTypeActions(this.recordTypeActionsResult
                    .map(function (action) { return new explorer.ActionButtonViewModel(action, _this); })
                    .sort(function (l, r) { return l.Position - r.Position; }));
                this.currentGroupingLevel(null);
            }
            this.customiseColumnHeaders(this._selectedColumns());
            this.selectedParameters(view.SelectedParameters || []);
            this.viewId(view.Id);
            if (this.invalidColumnsOrderByMessages().length > 0) {
                ko.postbox.publish("bwf-transient-notification", {
                    message: this.invalidColumnsOrderByMessages().join(". "),
                    styleClass: "alert-danger",
                    requireDismissal: true
                });
            }
            this.dataServiceActions(dataServiceActionsResult
                .map(function (action) { return new explorer.ActionButtonViewModel(action, _this); })
                .sort(function (l, r) { return l.Position - r.Position; }));
            if (this.queryExecutor())
                this.queryExecutor().dispose();
            this.queryExecutor(this.queryStrategy().getQueryManager());
            this.flags.loadingView(false);
        };
        ViewGrid.prototype.updateGroupingLevels = function (groupingProperties) {
            var currentLevel = -1;
            groupingProperties
                .sort(function (left, right) { return left.Position - right.Position; })
                .forEach(function (prop) {
                if (prop.NewLevel)
                    ++currentLevel;
                prop.Level = currentLevel;
            });
        };
        ViewGrid.prototype.getGroupingActions = function () {
            var actions = [];
            actions.push({
                Name: "NextGroupuingLevel",
                Position: 1,
                IconType: explorer.ActionIconType.FontAwesome,
                Icon: "fa-level-down",
                DisplayName: resources["bwf_drilldown"],
                Explanation: resources["bwf_drilldown_description"],
                InvokableFor: explorer.InvokableFor.One,
                InvokableForScript: "return selectedRecords[0].record.aggregationRowType.Value === 'Level';",
                TypeName: "BWF.DataServices.Metadata.Attributes.Actions.NextGroupingLevelAction"
            });
            return actions;
        };
        ViewGrid.prototype.getOrderBys = function (orderByString) {
            var _this = this;
            if (orderByString == null)
                return [];
            var orderBysArray = [];
            var orderBysSplit = orderByString.split(',');
            var invalidOrderBys = orderBysSplit.filter(function (ob) { return !metadataService.isOrderByValid(_this.metadata(), ob); });
            if (invalidOrderBys.length > 0) {
                var invalidOrderByString = sprintf(invalidOrderBys.length === 1 ?
                    resources["bwf_invalid_orderby_single"] :
                    resources["bwf_invalid_orderby_multiple"], invalidOrderBys.join("', '"));
                this.invalidColumnsOrderByMessages.push(invalidOrderByString);
            }
            orderBysArray = orderBysSplit.filter(function (x) { return invalidOrderBys.indexOf(x) < 0; });
            return orderBysArray;
        };
        ViewGrid.prototype.getGroupingOrderBys = function (grouping) {
            var _this = this;
            if (!grouping || this.currentGroupingLevel() == null)
                return [];
            var invalidOrderBys = [];
            var allGroupBys = grouping.GroupByColumns;
            var orderBysArray = allGroupBys
                .filter(function (column) { return column.Level === _this.currentGroupingLevel(); })
                .map(function (column) { return sprintf("%s %s", column.Name, column.Order.Value); })
                .filter(function (orderBy) {
                var valid = metadataService.isOrderByValid(_this.metadata(), orderBy);
                if (!valid) {
                    invalidOrderBys.push(orderBy);
                }
                return valid;
            });
            if (invalidOrderBys.length > 0) {
                var invalidOrderByString = sprintf(invalidOrderBys.length === 1 ?
                    resources["bwf_invalid_orderby_single"] :
                    resources["bwf_invalid_orderby_multiple"], invalidOrderBys.join("', '"));
                this.invalidColumnsOrderByMessages.push(invalidOrderByString);
            }
            return orderBysArray;
        };
        ViewGrid.prototype.getColumns = function (gridPresentation) {
            var _this = this;
            var invalidColumns = [];
            var columns = gridPresentation.SelectedColumns
                .sort(function (left, right) { return left.Position - right.Position; })
                .filter(function (column) {
                var valid = metadataService.isPropertyPathValid(_this.dataService(), _this.metadata(), column.Name);
                if (!valid) {
                    invalidColumns.push(column);
                }
                return valid;
            })
                .map(function (column, index) {
                var overrides = {};
                if (column.Format != null)
                    overrides["format"] = column.Format;
                var propertyMetadata = metadataService.getProperty(_this.metadata(), column.Name, overrides);
                var gridColumn = new explorer.ExplorerGridColumn(propertyMetadata, column.Name, index + 1);
                if (column.Alias != null)
                    gridColumn.displayName(column.Alias);
                return gridColumn;
            });
            if (invalidColumns.length > 0) {
                var message = invalidColumns.map(function (x) { return "'" + x.Name + "'"; }).join(", ");
                var alertMessage = invalidColumns.length === 1 ?
                    sprintf("%s %s %s", resources["bwf_column"], message, resources["bwf_column_doesnt_exist_single"]) :
                    sprintf("%s %s %s", resources["bwf_column"], message, resources["bwf_column_doesnt_exist_multiple"]);
                this.invalidColumnsOrderByMessages.push(alertMessage);
            }
            return columns;
        };
        ViewGrid.prototype.getGroupingSubtotals = function (grouping) {
            var _this = this;
            var currentLevel = this.currentGroupingLevel();
            return grouping.GroupByColumns
                .filter(function (groupBy) {
                if (!groupBy.IncludeSubtotals || groupBy.Level !== currentLevel)
                    return false;
                return metadataService.isPropertyPathValid(_this.dataService(), _this.metadata(), groupBy.Name);
            })
                .map(function (groupBy) {
                return groupBy.Name;
            });
        };
        ViewGrid.prototype.getGroupingColumns = function (grouping, presentation) {
            var _this = this;
            if (!grouping || this.currentGroupingLevel() == null)
                return [];
            var invalidColumns = [];
            var currentLevel = this.currentGroupingLevel();
            // Group Bys
            var columns = grouping.GroupByColumns
                .filter(function (groupBy) {
                if (groupBy.Level !== currentLevel)
                    return false;
                var valid = metadataService.isPropertyPathValid(_this.dataService(), _this.metadata(), groupBy.Name);
                if (!valid)
                    invalidColumns.push(groupBy.Name);
                return valid;
            })
                .map(function (groupBy, index) {
                var presentationColumn = presentation.SelectedColumns.filter(function (pCol) { return pCol.Name === groupBy.Name; })[0];
                var overrides = {};
                if (groupBy.Format != null) {
                    overrides["format"] = groupBy.Format;
                }
                else if (presentationColumn != null && presentationColumn.Format != null) {
                    overrides["format"] = presentationColumn.Format;
                }
                var propertyMetadata = metadataService.getProperty(_this.metadata(), groupBy.Name, overrides);
                var gridColumn = new explorer.GroupingExplorerGridColumn(propertyMetadata, groupBy.Name, index + 1);
                if (groupBy.Alias) {
                    gridColumn.displayName(groupBy.Alias);
                }
                else if (presentationColumn != null && presentationColumn.Alias != null) {
                    gridColumn.displayName(presentationColumn.Alias);
                }
                return gridColumn;
            });
            // Aggregations        
            columns = columns.concat(this.groupingAggregations()
                .map(function (aggregation, index) {
                var presentationColumn = presentation.SelectedColumns.filter(function (pCol) { return pCol.Name === aggregation.Name; })[0];
                var aggMetadata = _this.aggregationFunctions().filter(function (aggMeta) { return aggMeta.Name === aggregation.Function; })[0];
                var overrides = {};
                if (aggregation.Format != null) {
                    overrides["format"] = aggregation.Format;
                }
                else if (presentationColumn != null && presentationColumn.Format != null) {
                    overrides["format"] = presentationColumn.Format;
                }
                var aggPropertyMetadata = metadataService.getAggregationProperty(aggMetadata, _this.metadata(), aggregation.Name, overrides);
                var gridColumn = new explorer.GroupingExplorerGridColumn(aggPropertyMetadata, _this.getAggregationIdString(aggregation), index + 1 + columns.length);
                if (aggregation.Alias) {
                    gridColumn.displayName(aggregation.Alias);
                }
                else {
                    if (presentationColumn != null && presentationColumn.Alias != null)
                        gridColumn.displayName(presentationColumn.Alias);
                    gridColumn.displayName(sprintf("%s %s\n%s", aggMetadata.DisplayName, resources["bwf_of"], gridColumn.displayName()));
                }
                gridColumn.hoverTitleContent = (sprintf("%s %s %s%s\n[%s]", aggMetadata.DisplayName, resources["bwf_of"], gridColumn.hoverTitleContent, aggregation.ExtraParameters ? "\n" + resources["bwf_by"] + " " + aggregation.ExtraParameters : "", aggregation.Options.Text));
                return gridColumn;
            }));
            if (invalidColumns.length > 0) {
                var message = invalidColumns.map(function (x) { return "'" + x + "'"; }).join(", ");
                var alertMessage = invalidColumns.length === 1 ?
                    sprintf("%s %s %s", resources["bwf_column"], message, resources["bwf_column_doesnt_exist_single"]) :
                    sprintf("%s %s %s", resources["bwf_column"], message, resources["bwf_column_doesnt_exist_multiple"]);
                this.invalidColumnsOrderByMessages.push(alertMessage);
            }
            return columns;
        };
        ViewGrid.prototype.getGroupingAggregations = function (grouping) {
            var _this = this;
            if (!grouping || this.currentGroupingLevel() == null)
                return [];
            var currentLevel = this.currentGroupingLevel();
            var invalidColumns = [];
            var groupBysInHigherLevels = grouping.GroupByColumns.filter(function (groupBy) { return groupBy.Level <= currentLevel; });
            var aggregations = grouping.Aggregations
                .filter(function (agg) {
                if (groupBysInHigherLevels.some(function (groupBy) { return groupBy.Name === agg.Name; }))
                    return false;
                var valid = metadataService.isPropertyPathValid(_this.dataService(), _this.metadata(), agg.Name);
                if (!valid)
                    invalidColumns.push(agg.Name);
                return valid;
            })
                .sort(function (left, right) { return left.Position - right.Position; });
            if (invalidColumns.length > 0) {
                var message = invalidColumns.map(function (x) { return "'" + x + "'"; }).join(", ");
                var alertMessage = invalidColumns.length === 1 ?
                    sprintf("%s %s %s", resources["bwf_column"], message, resources["bwf_column_doesnt_exist_single"]) :
                    sprintf("%s %s %s", resources["bwf_column"], message, resources["bwf_column_doesnt_exist_multiple"]);
                this.invalidColumnsOrderByMessages.push(alertMessage);
            }
            return aggregations;
        };
        ViewGrid.prototype.parameterBarConfiguration = function () {
            return {
                disableParamsBar: this.flags.disableParamsBar,
                metadata: this.metadata,
                parameterBarRendered: this.flags.parameterBarRendered,
                forceQueryRefresh: this.refresh,
                enableQuerying: this.flags.enableQuerying,
                selectedParameters: this.selectedParameters,
                selectedParameterValues: this.selectedParameterValues,
                urlParameters: this.urlParameters(),
                urlFilteredBys: this.urlFilteredBy(),
                viewGridId: this.viewGridId,
                viewId: this.viewId(),
                explorerDataService: 'explorer',
                explorerHostUrl: options.explorerHostUrl
            };
        };
        ViewGrid.prototype.setupSubscriptions = function (moduleConfig) {
            var _this = this;
            _super.prototype.setupSubscriptions.call(this, moduleConfig);
            // Knockout Subscriptions
            this.subscribeKnockout(this._selectedColumns, function (newColumns) {
                _this.records().forEach(function (r) { return r.configureColumns(newColumns); });
            });
            this.subscriptions.push(ko.computed(this.setGroupingBreadcrumbs).extend({ deferred: true }));
            // Postbox Subscriptions        
            this.subscribePostbox("edit-" + this.viewGridId + "-GridPresentation-panelClosed", function () { return _this.refreshGridPresentation(); });
            this.subscribePostbox("edit-" + this.viewGridId + "-Grouping-panelClosed", function () { return _this.refreshGrouping(); });
            this.subscribePostbox("edit-" + this.viewGridId + "-Selection-panelClosed", function () { return _this.refreshSelection(); });
            this.subscribePostbox("edit-" + this.viewGridId + "-View-panelClosed", function () { return _this.refreshView(); });
            this.subscribePostbox(this.viewGridId + "-nextGroupingLevel", function (groupingData) { return _this.nextGroupingLevel(groupingData); });
        };
        ViewGrid.prototype.refreshView = function () {
            var _this = this;
            var queryRequest = $.ajax({
                url: options.explorerHostUrl + "/api/explorer/query/Views?$filter=Id=" + this.viewId() +
                    "&$expand=SelectedParameters,Selection,GridPresentation/SelectedColumns,GridPresentation/Aggregations,Presentation,Grouping,Grouping/GroupByColumns,Grouping/Aggregations",
                xhrFields: {
                    withCredentials: true
                }
            });
            queryRequest.done(function (reply) {
                var view = reply.Records[0];
                var selectionTypeHasChanged = view.Selection.BaseType !== _this.baseType() ||
                    view.Selection.DataServiceSystem !== _this.dataService();
                var gridPresentationTypeHasChanged = view.GridPresentation.BaseType !== _this.baseType() ||
                    view.GridPresentation.DataServiceSystem !== _this.dataService();
                var groupingTypeHasChanged = !!view.Grouping !== _this.inGroupingMode() ||
                    (view.Grouping && (view.Grouping.BaseType !== _this.baseType() ||
                        view.Grouping.DataServiceSystem !== _this.dataService()));
                var viewTypeHasChanged = view.BaseType !== _this.baseType() ||
                    view.DataServiceSystem !== _this.dataService();
                if (selectionTypeHasChanged || gridPresentationTypeHasChanged || groupingTypeHasChanged || viewTypeHasChanged) {
                    _this.reloadView(view.Name);
                }
                else {
                    _this.viewName(view.Name);
                    _this.selectedParameters(view.SelectedParameters);
                    _this.selectionFilter(view.Selection.Filter ? view.Selection.Filter : '');
                    _this.gridPresentation(view.GridPresentation);
                    if (_this.inGroupingMode()) {
                        _this.updateGroupingLevels(view.Grouping.GroupByColumns);
                        _this.grouping(view.Grouping);
                        _this.resetGroupingLevel();
                        _this.queryExecutor().refresh();
                    }
                    else {
                        _this.selectedOrderBys(_this.getOrderBys(view.GridPresentation.OrderBy));
                        _this._selectedColumns(_this.getColumns(view.GridPresentation));
                        _this.selectedAggregations(view.GridPresentation.Aggregations);
                        _this.aggregatesPosition(view.GridPresentation.AggregationsPosition.Value);
                    }
                    ko.postbox.publish(_this.viewGridId + '-changeViewName', view.Name);
                    ko.postbox.publish(_this.viewGridId + '-refreshParameters');
                }
            });
            queryRequest.fail(function (failure) {
                log.error("Failure occurred refreshing view", failure);
                ko.postbox.publish('bwf-transient-notification', {
                    message: "Refreshing view failed",
                    styleClass: 'alert-warning',
                    requireDismissal: true
                });
            });
        };
        ViewGrid.prototype.refreshGridPresentation = function () {
            var _this = this;
            var queryRequest = $.ajax({
                url: options.explorerHostUrl + "/api/explorer/query/Views?$filter=Id=" +
                    (this.viewId() + "&$expand=GridPresentation,GridPresentation/SelectedColumns,GridPresentation/Aggregations"),
                xhrFields: {
                    withCredentials: true
                }
            });
            queryRequest.done(function (reply) {
                var view = reply.Records[0];
                var gridPresentationTypeHasChanged = view.GridPresentation.BaseType !== _this.baseType() ||
                    view.GridPresentation.DataServiceSystem !== _this.dataService();
                if (gridPresentationTypeHasChanged) {
                    _this.reloadView(view.Name);
                }
                else {
                    _this.gridPresentation(view.GridPresentation);
                    if (!_this.inGroupingMode() || _this.isAtMaxGroupingLevel()) {
                        _this.selectedOrderBys(_this.getOrderBys(view.GridPresentation.OrderBy));
                        _this._selectedColumns(_this.getColumns(view.GridPresentation));
                        _this.selectedAggregations(view.GridPresentation.Aggregations);
                        _this.aggregatesPosition(view.GridPresentation.AggregationsPosition.Value);
                    }
                }
            });
            queryRequest.fail(function (failure) {
                log.error("Failure occurred refreshing grid presentation", failure);
                ko.postbox.publish('bwf-transient-notification', {
                    message: "Refreshing grid presentation failed",
                    styleClass: 'alert-warning',
                    requireDismissal: true
                });
            });
        };
        ViewGrid.prototype.refreshGrouping = function () {
            var _this = this;
            var queryRequest = $.ajax({
                url: options.explorerHostUrl + "/api/explorer/query/Views?$filter=Id=" +
                    (this.viewId() + "&$expand=Grouping,Grouping/GroupByColumns,Grouping/Aggregations"),
                xhrFields: {
                    withCredentials: true
                }
            });
            queryRequest.done(function (reply) {
                var view = reply.Records[0];
                var groupingTypeHasChanged = view.Grouping && (view.Grouping.BaseType !== _this.baseType() ||
                    view.Grouping.DataServiceSystem !== _this.dataService());
                if (groupingTypeHasChanged) {
                    _this.reloadView(view.Name);
                }
                else if (_this.inGroupingMode()) {
                    _this.updateGroupingLevels(view.Grouping.GroupByColumns);
                    _this.grouping(view.Grouping);
                    _this.resetGroupingLevel();
                }
            });
            queryRequest.fail(function (failure) {
                log.error("Failure occurred refreshing grid grouping", failure);
                ko.postbox.publish('bwf-transient-notification', {
                    message: "Refreshing grid grouping failed",
                    styleClass: 'alert-warning',
                    requireDismissal: true
                });
            });
        };
        ViewGrid.prototype.refreshSelection = function () {
            var _this = this;
            var queryRequest = $.ajax({
                url: options.explorerHostUrl + "/api/explorer/query/Views?$filter=Id=" +
                    (this.viewId() + "&$expand=Selection/System"),
                xhrFields: {
                    withCredentials: true
                }
            });
            queryRequest.done(function (reply) {
                var view = reply.Records[0];
                var selectionTypeHasChanged = view.Selection.BaseType !== _this.baseType() ||
                    view.Selection.DataServiceSystem !== _this.dataService();
                if (selectionTypeHasChanged)
                    _this.reloadView(view.Name);
                _this.selectionFilter(view.Selection.Filter ? view.Selection.Filter : '');
            });
            queryRequest.fail(function (failure) {
                log.error("Failure occurred refreshing selection", failure);
                ko.postbox.publish('bwf-transient-notification', {
                    message: "Refreshing selection failed",
                    styleClass: 'alert-warning',
                    requireDismissal: true
                });
            });
        };
        return ViewGrid;
    }(vgBase.ViewGridBase));
    return ViewGrid;
});
