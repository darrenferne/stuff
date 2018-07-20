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
define(["require", "exports", "knockout", "loglevel", "sprintf", "options", "modules/bwf-utilities", "modules/bwf-liveQuery", "modules/bwf-gridUtilities", "modules/bwf-propertyReader", "modules/bwf-metadata", "modules/bwf-knockout-validators", "modules/bwf-bindingHandlers"], function (require, exports, ko, log, sp, options, bwf, LiveQueryExecutor, gridUtil, propertyReader, metadataService, knockoutValidations, bindingHandlers) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var sprintf = sp.sprintf;
    var v = knockoutValidations;
    var bh = bindingHandlers;
    var ExplorerGridColumn = /** @class */ (function () {
        function ExplorerGridColumn(metadata, path, position) {
            var _this = this;
            this.accessor = function (gridItem) {
                var fragments = _this.path.split('/');
                if (fragments.length === 1) {
                    return gridItem.record[_this.path];
                }
                else {
                    var item = gridItem.record;
                    for (var i = 0; i < fragments.length - 1; i++) {
                        if (!item) {
                            break;
                        }
                        item = ko.unwrap(item[fragments[i]]);
                    }
                    if (item) {
                        item = item[fragments[fragments.length - 1]];
                    }
                    return item;
                }
            };
            this.colSpan = function (gridItem) { return 1; };
            this.displayName = ko.observable(metadata.abbreviatedName);
            this.metadata = metadata;
            this.path = path;
            this.position = ko.observable(position);
            var description = (metadata.description ? "\n\n" + metadata.description : "");
            var generateAdditionDescriptions = function (ad, d) {
                return ad + "\n" + d.Key + ": " + d.Value;
            };
            var additionalDescriptions = (metadata.additionalDescriptions ? metadata.additionalDescriptions.reduce(generateAdditionDescriptions, "") : "");
            this.hoverTitleContent = metadata.displayName + description + (additionalDescriptions.length > 0 ? "\n" + additionalDescriptions : "");
        }
        ExplorerGridColumn.prototype.displayClasses = function () {
            var classes = [];
            return classes.join(' ');
        };
        ExplorerGridColumn.prototype.formatter = function (v) {
            var value = ko.unwrap(v);
            var format = this.metadata.defaultFormat;
            if (this.metadata.type === 'date' && options.dateDisplayFormat) {
                format = options.dateDisplayFormat;
            }
            else if (this.metadata.type === 'time' && options.dateTimeDisplayFormat) {
                format = options.dateTimeDisplayFormat;
            }
            if (this.metadata.format != null) {
                format = this.metadata.format;
            }
            return gridUtil.formatValue(value, this.metadata.type, format);
        };
        return ExplorerGridColumn;
    }());
    exports.ExplorerGridColumn = ExplorerGridColumn;
    var GroupingExplorerGridColumn = /** @class */ (function (_super) {
        __extends(GroupingExplorerGridColumn, _super);
        function GroupingExplorerGridColumn() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.accessor = function (gridItem) {
                return (gridItem.totalCell && _this.path === gridItem.totalCell) ? null : gridItem.record[_this.path];
            };
            _this.colSpan = function (gridItem) {
                var isTotalCell = gridItem.totalCell && _this.path === gridItem.totalCell;
                var isSubTotalCell = gridItem.subtotalCell && _this.path === gridItem.subtotalCell;
                return (isTotalCell || isSubTotalCell) ? gridItem.totalColSpan : 1;
            };
            return _this;
        }
        return GroupingExplorerGridColumn;
    }(ExplorerGridColumn));
    exports.GroupingExplorerGridColumn = GroupingExplorerGridColumn;
    var BaseGridItem = /** @class */ (function () {
        function BaseGridItem(record, columns, dataService, optionalParameters) {
            var _this = this;
            this.modelValidations = ko.observableArray([]);
            this.queryPosition = ko.observable(0);
            this.values = {};
            this.hasValidationErrors = ko.pureComputed(function () {
                var valueObservables = Object.keys(_this.values).map(function (k) { return _this.values[k]; });
                var areModelValidationErrors = _this.modelValidations().length > 0;
                return areModelValidationErrors || valueObservables.some(function (vm) { return !vm.isValid(); });
            });
            this.getRowComponent = function (gridContext) { return ko.pureComputed(function () { return gridContext.inEditMode() ? 'grid/bwf-gridEditableRow' : 'grid/bwf-gridDisplayRow'; }); };
            this.getSelectedValues = function () {
                if (_this.selected())
                    return _this.values;
                else {
                    var selectedValues_1 = {};
                    Object.keys(_this.values).forEach(function (k) {
                        var value = _this.values[k];
                        if (_this.values[k].isInCopyOrPasteGroup() || _this.values[k].isSelected())
                            selectedValues_1[k] = value;
                    });
                    return selectedValues_1;
                }
            };
            // prevents null reference errors when accessing properties
            // when optionalParameters hasn't been set
            if (!optionalParameters) {
                optionalParameters = {};
            }
            this.bwfId = record.Id != null ? record.Id : bwf.getNextBwfId().toString();
            this.dataService = dataService;
            this.columns = columns;
            this.highlight = ko.observable(false);
            this.isChangeTrackingDisabled = optionalParameters.disableChangeTracking || false;
            this.isValidateOnChangeDisabled = optionalParameters.disableValidateOnChange || false;
            this.selected = ko.observable(false);
            this.updateTime = new Date().getTime();
            this.updateType = ko.observable(optionalParameters.updateType || 'None');
            this.isNewRecord = ko.pureComputed(function () { return _this.updateType() === 'Added'; });
            if (optionalParameters.doHighlight === true) {
                this.doHighlight();
            }
        }
        BaseGridItem.prototype.dispose = function () {
            if (this.highlightTimeout) {
                clearTimeout(this.highlightTimeout);
                this.highlightTimeout = null;
            }
        };
        BaseGridItem.prototype.applyChangeSetResult = function (result) { return; };
        BaseGridItem.prototype.applyLiveChanges = function (changes, columns) {
            var _this = this;
            if (columns != null)
                this.columns = columns;
            if (changes.ResultType === "Added") {
                this.record = this.createRecord(changes.Record);
                this.updateTime = new Date().getTime();
                this.updateType(changes.ResultType);
                this.configureColumns(this.columns);
                return;
            }
            var positionChanged = changes.ChangedProperties.filter(function (p) { return p === '.Position'; });
            if (positionChanged)
                this.queryPosition(changes.Record.Position);
            var changedProperties = changes.ChangedProperties
                .filter(function (p) { return p !== '.Position'; })
                .map(function (p) { return p.substring(6).replace(/\./g, '/'); });
            if (changedProperties.length === 0) {
                this.updateType('None');
                return;
            }
            this.record = this.createRecord(changes.Record);
            this.updateTime = new Date().getTime();
            this.updateType(changes.ResultType);
            Object.keys(this.values)
                .forEach(function (key) {
                var c = _this.columns.filter(function (c) { return c.path === key; })[0];
                _this.values[key](c.accessor(_this));
            });
        };
        BaseGridItem.prototype.configureChangeTracking = function () {
            var _this = this;
            if (this.isChangeTrackingDisabled) {
                this.hasUnsavedChanges = ko.pureComputed(function () { return false; });
                return;
            }
            this.hasUnsavedChanges = ko.pureComputed(function () {
                if (_this.isNewRecord())
                    return true;
                var dirtyFlags = Object.keys(_this.values)
                    .map(function (key) { return _this.values[key]; })
                    .filter(function (obs) { return ko.isObservable(obs); })
                    .filter(function (obs) { return obs.isDirty != null; })
                    .map(function (obs) { return obs.isDirty(); });
                return dirtyFlags.some(function (f) { return f; });
            });
        };
        BaseGridItem.prototype.configureColumns = function (columns) {
            var _this = this;
            columns.forEach(function (c) {
                if (_this.values[c.path] == null) {
                    _this.values[c.path] = ko.observable(c.accessor(_this));
                    _this.values[c.path].isSelected = ko.observable(false);
                    _this.values[c.path].isInCopyOrPasteGroup = ko.observable(false);
                    _this.values[c.path].isReadonly = ko.observable(false);
                    _this.values[c.path].colSpan = c.colSpan(_this);
                    if (c.path.indexOf('/') === -1) {
                        switch (c.metadata.type) {
                            case 'numeric':
                                _this.values[c.path].extend({
                                    validNumeric: {
                                        allowNull: c.metadata.isNullable,
                                        message: sprintf("'%s' must be a valid number", c.metadata.name)
                                    }
                                });
                                break;
                            case 'integer':
                                _this.values[c.path].extend({
                                    validInteger: {
                                        allowNull: c.metadata.isNullable,
                                        message: sprintf("'%s' must be a valid integer", c.metadata.name)
                                    }
                                });
                                break;
                            default:
                                _this.values[c.path].isValid = ko.observable(true);
                                _this.values[c.path].validationMessages = ko.observableArray([]);
                        }
                    }
                    else {
                        _this.values[c.path].isValid = ko.observable(true);
                        _this.values[c.path].validationMessages = ko.observableArray([]);
                    }
                }
                else {
                    _this.values[c.path](c.accessor(_this));
                }
                if (!_this.isChangeTrackingDisabled) {
                    if (!c.metadata.hasChoice) {
                        _this.setupChangeTracking(_this.values[c.path], _this.updateType() !== 'None');
                    }
                    else {
                        // choice cells are weird, we gotta handle it in the component manually
                        _this.values[c.path].isDirty = ko.observable(false);
                        _this.values[c.path].resetIsDirty = function () { return; };
                    }
                }
            });
        };
        BaseGridItem.prototype.createItem = function (record) {
            this.record = this.createRecord(record);
            this.baseTypeName = record.BaseTypeName;
            this.dirtyRecord = bwf.clone(this.record);
            this.position = ko.observable(record.Position);
            this.queryPosition(record.Position);
            this.typeName = record.TypeName;
            this.configureColumns(this.columns);
            this.configureChangeTracking();
        };
        BaseGridItem.prototype.doHighlight = function () {
            var _this = this;
            if (this.highlightTimeout)
                clearTimeout(this.highlightTimeout);
            this.highlight(true);
            this.highlightTimeout = setTimeout(function () {
                _this.highlight(false);
                _this.highlightTimeout = null;
            }, 1600);
        };
        BaseGridItem.prototype.setupChangeTracking = function (obs, isInitiallyDirty) {
            var _this = this;
            if (obs.isDirty)
                obs.isDirty.dispose();
            var initialState = ko.observable(ko.toJSON(this.translateChangeTrackObservable(obs)));
            var initiallyDirty = ko.observable(isInitiallyDirty);
            obs.isDirty = ko.computed(function () { return initiallyDirty() || initialState() !== ko.toJSON(_this.translateChangeTrackObservable(obs)); });
            obs.resetIsDirty = function () {
                initialState(ko.toJSON(_this.translateChangeTrackObservable(obs)));
                initiallyDirty(false);
            };
        };
        BaseGridItem.prototype.translateChangeTrackObservable = function (obs) {
            var returnValue = obs();
            if (typeof returnValue === "number" ||
                typeof returnValue === "boolean")
                returnValue = obs().toString();
            // this stops items that are null to start with from showing as 
            // changed if a value is changed and then returned to empty
            if (returnValue === null)
                returnValue = "";
            return returnValue;
        };
        BaseGridItem.prototype.resetValidation = function () {
            var _this = this;
            this.modelValidations([]);
            Object.keys(this.values).forEach(function (key) {
                _this.values[key].validationMessages([]);
                _this.values[key].isValid(true);
            });
        };
        return BaseGridItem;
    }());
    exports.BaseGridItem = BaseGridItem;
    var ExplorerGridItem = /** @class */ (function (_super) {
        __extends(ExplorerGridItem, _super);
        function ExplorerGridItem(record, columns, dataService, optionalParameters) {
            var _this = _super.call(this, record, columns, dataService, optionalParameters) || this;
            metadataService.getType(dataService, record.BaseTypeName)
                .done(function (metadata) {
                _this.metadata = metadata;
                _this.createItem(record);
                _this.id = record.Data[metadata.identityProperties[0]];
                _this.editable = metadata.supportsEditMode;
            });
            return _this;
        }
        ExplorerGridItem.prototype.createRecord = function (record) { return record.Data; };
        ExplorerGridItem.prototype.applyChangeSetResult = function (result) {
            var _this = this;
            if (this.isNewRecord()) {
                if (result.SuccessfullyCreated[this.bwfId]) {
                    this.record = bwf.clone(this.dirtyRecord);
                    this.id = result.SuccessfullyCreated[this.bwfId];
                    if (this.values["Id"])
                        this.values["Id"](this.id.toString());
                    this.record["Id"] = this.id;
                    Object.keys(this.values).forEach(function (key) { return _this.values[key].resetIsDirty(); });
                    this.updateType('None');
                }
                else {
                    this.applyValidation(result.FailedCreates[this.bwfId]);
                }
            }
            else if (this.updateType() === 'Deleted') {
                if (result.SuccessfullyDeleted.indexOf(this.id) === -1) {
                    this.applyValidation(result.FailedDeletions[this.id]);
                    delete this['_destroy'];
                }
            }
            else if (this.hasUnsavedChanges()) {
                if (result.SuccessfullyUpdated.indexOf(this.id) > -1) {
                    this.record = bwf.clone(this.dirtyRecord);
                    Object.keys(this.values).forEach(function (key) { return _this.values[key].resetIsDirty(); });
                    this.modelValidations([]);
                    this.updateType('None');
                }
                else {
                    this.applyValidation(result.FailedUpdates[this.id]);
                }
            }
        };
        ExplorerGridItem.prototype.applyValidation = function (validationResult) {
            var _this = this;
            if (validationResult['message']) {
                log.warn(validationResult.message);
                return;
            }
            this.resetValidation();
            if (validationResult.Type === "MessageSet") {
                this.modelValidations(validationResult.Messages);
                return;
            }
            var validation = validationResult;
            Object.keys(validation.PropertyValidations)
                .filter(function (key) { return key[0] !== '$'; })
                .forEach(function (key) {
                if (key in _this.values) {
                    _this.values[key].validationMessages
                        .push(validation.PropertyValidations[key]);
                    _this.values[key].isValid(false);
                }
                else {
                    // either a type property or a property not in the presentation
                    if (key in _this.dirtyRecord) {
                        // not in the presentation
                        validation.ModelValidations.push(sprintf("%s: %s", key, validation.PropertyValidations[key]));
                    }
                    else {
                        // a type
                        var valueField = _this.metadata.properties[key].valueFieldInEditorChoice;
                        var subFieldKey = sprintf('%s/%s', key, valueField);
                        // since this must be a dropdown, we have to have a field that is either
                        // the value field of of the object, or the name field as the source
                        if (subFieldKey in _this.values) {
                            var typeKey = subFieldKey;
                        }
                        else {
                            var typeKey = sprintf('%s/%s', key, 'Name');
                        }
                        if (!(typeKey in _this.values))
                            log.error(sprintf("Unknown property '%s'", key));
                        _this.values[typeKey].validationMessages
                            .push(validation.PropertyValidations[key]);
                        _this.values[typeKey].isValid(false);
                    }
                }
            });
            this.modelValidations(validation.ModelValidations);
        };
        return ExplorerGridItem;
    }(BaseGridItem));
    exports.ExplorerGridItem = ExplorerGridItem;
    var GroupingExplorerGridItem = /** @class */ (function (_super) {
        __extends(GroupingExplorerGridItem, _super);
        function GroupingExplorerGridItem(record, columns, dataService, groupingLevel, optionalParameters) {
            var _this = _super.call(this, record, columns, dataService, optionalParameters) || this;
            _this.groupingLevel = groupingLevel;
            _this.id = record.Id;
            _this.editable = false;
            metadataService.getType(_this.dataService, record.BaseTypeName)
                .done(function (metadata) {
                _this.metadata = metadata;
                _this.createItem(record);
            });
            return _this;
        }
        GroupingExplorerGridItem.prototype.applyValidation = function (validationResult) {
        };
        GroupingExplorerGridItem.prototype.createRecord = function (record) {
            var data = {
                aggregationRowType: record.AggregationRowType,
                groupedBy: record.GroupedBy,
                aggregations: record.Aggregations,
                level: this.groupingLevel + 1
            };
            record.GroupedBy.forEach(function (g) { return data[g.Key] = g.Value; });
            record.Aggregations.forEach(function (a) { return data[a.Key] = a.Value; });
            if (data.aggregationRowType.Value === AggregationRowType.Subtotal.Value ||
                data.aggregationRowType.Value === AggregationRowType.Total.Value) {
                this.getRowComponent = function (gridContext) {
                    return ko.pureComputed(function () { return 'grid/bwf-gridTotalsDisplayRow'; });
                };
            }
            this.totalColSpan = this.columns.length - record.Aggregations.length;
            if (record.AggregationRowType.Value === AggregationRowType.Total.Value) {
                this.totalCell = this.columns[0].path;
            }
            else if (record.AggregationRowType.Value === AggregationRowType.Subtotal.Value) {
                this.subtotalCell = record.GroupedBy[record.GroupedBy.length - 1].Key;
                this.totalColSpan -= this.columns.map(function (x) { return x.path; }).indexOf(this.subtotalCell);
            }
            return data;
        };
        return GroupingExplorerGridItem;
    }(BaseGridItem));
    exports.GroupingExplorerGridItem = GroupingExplorerGridItem;
    var BaseGridQueryManager = /** @class */ (function () {
        function BaseGridQueryManager(dataService, viewGridId, autoUpdatesEnabled, inEditMode, totalCount, query, records, columns, querying, liveQueryApi, enableQuerying) {
            var _this = this;
            this.collectedChanges = [];
            this.disposed = false;
            this.subscriptions = [];
            this.createQueryExecutor = function () {
                if (!_this.queryingEnabled()) {
                    _this.querying(false);
                    return;
                }
                if (_this.executor)
                    _this.executor.stop();
                if (_this.currentQuery == null) {
                    return;
                }
                var initialDone = function (r) {
                    _this.initialQueryDone(r);
                    if (_this.deferred) {
                        _this.deferred.resolve();
                        _this.deferred = null;
                    }
                };
                _this.querying(true);
                _this.executor = new LiveQueryExecutor(_this.dataService.hostUrl, _this.dataService.name, _this.autoUpdatesEnabled(), _this.liveQueryApi, _this.currentQuery, 1000, initialDone, function (r) { return _this.updatesQueryDone(r); }, _this.initialQueryFail);
            };
            this.initialQueryFail = function (response) {
                if (response != null) {
                    ko.postbox.publish("bwf-transient-notification", {
                        message: response.message,
                        styleClass: "alert-danger",
                        requireDismissal: true
                    });
                }
                _this.querying(false);
            };
            this.autoUpdatesEnabled = autoUpdatesEnabled;
            this.columns = columns;
            this.currentQuery = enableQuerying ? query() : null;
            this.dataService = dataService;
            this.inEditMode = inEditMode;
            this.liveQueryApi = liveQueryApi;
            this.querying = querying;
            this.queryingEnabled = enableQuerying;
            this.records = records;
            this.totalCount = totalCount;
            this.viewGridId = viewGridId;
            this.refresh = function () {
                if (_this.deferred)
                    _this.deferred.reject();
                _this.deferred = $.Deferred();
                _this.createQueryExecutor();
                return _this.deferred.promise();
            };
            this.subscriptions.push(query.subscribe(function (newQueryUrl) {
                if (newQueryUrl === null)
                    _this.executor.stop();
                if (newQueryUrl === _this.currentQuery)
                    return;
                _this.currentQuery = newQueryUrl;
                _this.createQueryExecutor();
            }));
            this.subscriptions.push(autoUpdatesEnabled.subscribe(function (update) {
                if (_this.executor) {
                    _this.executor.stop();
                    _this.executor.watchChanges = update;
                }
                if (update)
                    _this.createQueryExecutor();
            }));
            this.subscriptions.push(ko.postbox.subscribe(this.viewGridId + "-querying-enabled", function (enable) {
                if (enable === void 0) { enable = true; }
                return _this.queryingEnabled(enable);
            }));
            this.createQueryExecutor();
        }
        BaseGridQueryManager.prototype.dispose = function () {
            this.disposed = true;
            if (this.deferred)
                this.deferred.reject();
            if (this.executor)
                this.executor.stop();
            this.subscriptions.forEach(function (s) { return s.dispose(); });
            this.collectedChanges = null;
            this.dataService = null;
            this.records = null;
            this.querying = null;
            this.currentQuery = null;
            this.refresh = null;
        };
        BaseGridQueryManager.prototype.updatesQueryDone = function (result) {
            if (this.disposed || result.Case === 'None')
                return;
            // due to how json.net serialises f# union types the value of a Some type
            // will always be the first element here (and there can be no other elements)
            var cacheItem = result.Fields[0];
            this.collectedChanges.push(cacheItem.Fields[0]);
            if (!this.inEditMode())
                this.processLiveChanges();
        };
        return BaseGridQueryManager;
    }());
    exports.BaseGridQueryManager = BaseGridQueryManager;
    var GridQueryManager = /** @class */ (function (_super) {
        __extends(GridQueryManager, _super);
        function GridQueryManager(dataService, viewGridId, aggregates, autoUpdatesEnabled, inEditMode, totalCount, query, records, columns, querying, enableQuerying) {
            var _this = _super.call(this, dataService, viewGridId, autoUpdatesEnabled, inEditMode, totalCount, query, records, columns, querying, "query", enableQuerying) || this;
            _this.aggregates = aggregates || ko.observableArray([]);
            return _this;
        }
        GridQueryManager.prototype.initialQueryDone = function (result) {
            var _this = this;
            if (this.disposed)
                return;
            this.totalCount(result.TotalCount);
            var gridItems = result.Records.map(function (r) {
                return new ExplorerGridItem(r, _this.columns(), _this.dataService.name);
            });
            var aggregates = result.Aggregates.filter(function (e) { return e.Key !== "$type"; });
            this.postAggregates(aggregates);
            this.aggregates(aggregates);
            this.records(gridItems);
            this.querying(false);
        };
        GridQueryManager.prototype.postAggregates = function (aggregates) {
            var toPost = aggregates.reduce(function (c, aggregate) {
                c[aggregate.Key] = aggregate.Values.map(function (v) {
                    return { Method: v.Function, Value: v.TotalAggregate };
                });
                return c;
            }, {});
            ko.postbox.publish(this.viewGridId + '-aggregateRecord', toPost);
        };
        GridQueryManager.prototype.processLiveChanges = function () {
            var _this = this;
            this.collectedChanges.forEach(function (changes) {
                _this.totalCount(changes.TotalCount);
                var aggregatesFromChanges = changes.AggregateChanges.map(function (ac) { return ac.Aggregate; });
                var keysFromChanges = aggregatesFromChanges.map(function (a) { return a.Key; });
                var toSave = aggregatesFromChanges.concat(_this.aggregates().filter(function (r) { return keysFromChanges.indexOf(r.Key) === -1; }));
                _this.aggregates(toSave);
                _this.postAggregates(toSave);
                if (changes.ReplaceAll)
                    _this.records.removeAll();
                changes.RecordChanges.forEach(function (change) {
                    switch (change.ResultType) {
                        case 'Added':
                            _this.records.push(new ExplorerGridItem(change.Record, _this.columns(), _this.dataService.name, { doHighlight: true }));
                            break;
                        case 'Removed':
                            _this.records.remove(function (r) { return r.bwfId === change.Record.Id; });
                            break;
                        case 'Updated':
                            var existing = _this.records()
                                .filter(function (r) { return r.bwfId === change.Record.Id; })[0];
                            existing.applyLiveChanges(change);
                            break;
                    }
                });
            });
            this.collectedChanges = [];
        };
        return GridQueryManager;
    }(BaseGridQueryManager));
    exports.GridQueryManager = GridQueryManager;
    var GroupingGridQueryManager = /** @class */ (function (_super) {
        __extends(GroupingGridQueryManager, _super);
        function GroupingGridQueryManager(dataService, viewGridId, autoUpdatesEnabled, inEditMode, totalCount, query, records, columns, querying, currentGroupingLevel, enableQuerying) {
            var _this = _super.call(this, dataService, viewGridId, autoUpdatesEnabled, inEditMode, totalCount, query, records, columns, querying, "aggregationquery", enableQuerying) || this;
            _this.currentGroupingLevel = currentGroupingLevel;
            return _this;
        }
        GroupingGridQueryManager.prototype.initialQueryDone = function (result) {
            var _this = this;
            if (this.disposed)
                return;
            this.totalCount(result.TotalCount);
            var gridItems = result.Aggregations.map(function (r) {
                return new GroupingExplorerGridItem(r, _this.columns(), _this.dataService.name, _this.currentGroupingLevel());
            });
            this.records(gridItems);
            this.querying(false);
        };
        GroupingGridQueryManager.prototype.processLiveChanges = function () {
            var _this = this;
            this.collectedChanges.forEach(function (changes) {
                _this.totalCount(changes.TotalCount);
                if (changes.ReplaceAll)
                    _this.records.removeAll();
                changes.AggregationChanges.forEach(function (change) {
                    switch (change.ResultType) {
                        case 'Added':
                            _this.records.push(new GroupingExplorerGridItem(change.Record, _this.columns(), _this.dataService.name, _this.currentGroupingLevel(), { doHighlight: true }));
                            break;
                        case 'Removed':
                            _this.records.remove(function (r) { return r.bwfId === change.Record.Id; });
                            break;
                        case 'Updated':
                            var existing = _this.records()
                                .filter(function (r) { return r.bwfId === change.Record.Id; })[0];
                            existing.applyLiveChanges(change);
                            break;
                    }
                });
            });
            this.collectedChanges = [];
        };
        return GroupingGridQueryManager;
    }(BaseGridQueryManager));
    exports.GroupingGridQueryManager = GroupingGridQueryManager;
    var ActionArgs = /** @class */ (function () {
        function ActionArgs(model, subtype) {
            if (subtype != null) {
                this.baseType = subtype;
            }
            else if (model.Parent.selectedRecords().length === 1) {
                this.baseType = model.Parent.selectedRecords()[0].typeName;
            }
            else {
                this.baseType = model.Parent.baseType();
            }
            this.action = model.Name.toLowerCase();
            this.dataService = model.Parent.dataService();
            this.dataServiceUrl = model.Parent.dataServiceUrl();
            this.metadata = model.Parent.metadata();
            this.selectedParameterValues = model.Parent.selectedParameterValues;
            switch (this.action) {
                case 'delete':
                    this.component = 'bwf-delete';
                    break;
                case 'import':
                    this.component = 'bwf-import';
                    break;
                case 'export':
                    this.component = 'bwf-export';
                    break;
                case 'lock':
                    this.component = 'bwf-lock';
                    break;
                case 'assign':
                    this.component = 'bwf-massAssign';
                    break;
                default:
                    this.component = 'bwf-panel-editor';
                    break;
            }
            switch (this.action) {
                case 'create':
                    break;
                case 'edit':
                case 'view':
                    this.data = { Id: ko.unwrap(model.Parent.selectedRecords()[0].record.Id) };
                    break;
                case 'copy':
                    this.data = JSON.parse(ko.toJSON(model.Parent.selectedRecords()[0].record));
                    break;
                default:
                    this.data = model.Parent.selectedRecords().map(function (sr) { return sr.record; });
            }
        }
        return ActionArgs;
    }());
    function setActionTypeAndAdditionalFields(vm, action) {
        switch (action.TypeName) {
            case 'BWF.DataServices.Metadata.Attributes.Actions.CreateAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.EditAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.CopyAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.ViewAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.LockAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.UnlockAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.DeleteAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.ImportAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.PermissionedImportAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.ExportAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.PermissionedExportAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.OpenViewAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.UnlockUserAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.OpenDashboardAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.MassAssignAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.NextGroupingLevelAction':
                vm.ActionType = ActionType.Predefined;
                break;
            case 'BWF.DataServices.Metadata.Attributes.Actions.AuditTrailAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.PermissionedAuditTrailAction':
                vm.ActionType = ActionType.Predefined;
                vm.AuditType = action.AuditType;
                break;
            case 'BWF.DataServices.Metadata.Attributes.Actions.JavascriptAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.PermissionedJavascriptAction':
                vm.ActionType = ActionType.Javascript;
                vm.ScriptModule = action.ScriptModule;
                break;
            case 'BWF.DataServices.Metadata.Attributes.Actions.RedirectAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.PermissionedRedirectAction':
                vm.ActionType = ActionType.Redirect;
                vm.Url = action.Url;
                break;
            case 'BWF.DataServices.Metadata.Attributes.Actions.HashAction':
            case 'BWF.DataServices.Metadata.Attributes.Actions.PermissionedHashAction':
                vm.ActionType = ActionType.Hash;
                vm.Url = action.Url;
                break;
        }
    }
    var UrlReplaceModel = /** @class */ (function () {
        function UrlReplaceModel(replaceString, lookupString, matchIndex) {
            this.replaceString = replaceString;
            this.lookupString = lookupString;
            this.matchIndex = matchIndex;
        }
        return UrlReplaceModel;
    }());
    var ActionButtonViewModel = /** @class */ (function () {
        function ActionButtonViewModel(action, scope) {
            var _this = this;
            this.isActionLink = ko.observable(false);
            this.customButtonIsVisible = ko.pureComputed(function () { return !_this.isHidden(); });
            this.enabled = ko.pureComputed(function () {
                if (_this.isDisabled && _this.isDisabled())
                    return false;
                var isInvokableFor = _this.isInvokableFor();
                if (isInvokableFor && _this.InvokableForScript != null) {
                    return _this.runInvokableForScript();
                }
                return isInvokableFor;
            });
            this.ButtonHref = ko.pureComputed(function () {
                if (!_this.enabled())
                    return undefined;
                if (_this.isActionLink())
                    return _this.ActionLink();
                if (_this.ActionType === ActionType.Hash)
                    return "#" + _this.updateUrl(_this.Url);
                if (_this.Url != null)
                    return _this.updateUrl(_this.Url);
                return undefined; // removes href attribute from html
            });
            this.Parent = scope;
            this.Position = action.Position;
            this.DisplayName = action.DisplayName;
            this.Explanation = action.Explanation;
            this.Icon = action.Icon;
            this.InvokableFor = action.InvokableFor || InvokableFor.All;
            this.InvokableForScript = action.InvokableForScript;
            this.explorerHostUrl = scope['explorerHostUrl'];
            this.isEmbedded = scope['flags'].isEmbedded;
            this.isVisible = !this.isEmbedded;
            this.viewGrid = scope['viewGridId'];
            this.invokableForScriptFunction = new Function("selectedRecords", this.InvokableForScript);
            if (action.Name != null) {
                this.initialiseFromDataServiceAction(action, scope);
            }
            else {
                this.initialiseFromCustomButton(action);
            }
        }
        ActionButtonViewModel.prototype.initialiseFromCustomButton = function (button) {
            var _this = this;
            this.displayAsMenu = button.ButtonType === "Drop";
            this.PostboxMessage = button.PostboxMessage;
            this.IconType = ActionIconType.FontAwesome;
            this.id = button.Id;
            this.isBootstrapGlyphIcon = false;
            this.isCustomIcon = false;
            this.isFontAwesomeIcon = true;
            this.isDisabled = (typeof button.Disabled === 'function') ? button.Disabled : ko.observable(false);
            this.isHidden = (typeof button.Hidden === 'function') ? button.Hidden : ko.observable(false);
            this.menuItems = (button.Options || '')
                .split(',')
                .map(function (pair) {
                var fragments = pair.split('|');
                return {
                    DisplayName: fragments[0],
                    Explanation: fragments[0],
                    Value: fragments[1]
                };
            });
            this.actionClick = this.displayAsMenu
                ? function (a) { return _this.customButtonClicked('-OptionClicked-', a.Value); }
                : function () { return _this.customButtonClicked('-ButtonClicked-'); };
        };
        ActionButtonViewModel.prototype.initialiseFromDataServiceAction = function (action, scope) {
            var _this = this;
            this.Name = action.Name;
            this.IconType = action.IconType;
            this.CustomIconUrl = action.CustomIconUrl;
            this.Type = action.Type;
            this.TypeName = action.TypeName;
            var subTypes = action.IncludeSubTypes || [];
            this.displayAsMenu = subTypes.length > 0;
            this.isBootstrapGlyphIcon = this.IconType === ActionIconType.BootstrapGlyph;
            this.isCustomIcon = this.IconType === ActionIconType.Custom;
            this.isFontAwesomeIcon = this.IconType === ActionIconType.FontAwesome;
            this.menuItems = subTypes.map(function (subType, index) {
                var displayName = _this.DisplayName;
                if (action.IncludeSubTypes.length === action.SubTypeDisplayNames.length) {
                    displayName = action.SubTypeDisplayNames[index];
                }
                return {
                    DisplayName: displayName,
                    Explanation: 'Create a new ' + displayName,
                    Value: action.IncludeSubTypes[index]
                };
            });
            setActionTypeAndAdditionalFields(this, action);
            var doAction = this.viewGrid + '-doAction';
            if (this.ActionType === ActionType.Predefined) {
                switch (action.Name) {
                    case 'Create':
                    case 'View':
                    case 'Copy':
                    case 'Edit':
                    case 'Import':
                    case 'Export':
                    case 'Lock':
                    case 'Delete':
                    case 'Assign':
                        this.actionClick = function (model) {
                            var subtype = undefined;
                            if (model && model["Value"])
                                subtype = model.Value;
                            ko.postbox.publish(doAction, new ActionArgs(_this, subtype));
                        };
                        break;
                    case 'Unlock':
                        this.actionClick = function () { return _this.redirectToUnlock(); };
                        break;
                    case 'Unlock User':
                        this.actionClick = function () { return _this.unlockUser(); };
                        break;
                    case 'AuditTrail':
                        this.isActionLink(true);
                        this.ActionLink = this.auditTrailUrl;
                        this.actionClick = function () { return true; };
                        break;
                    case 'OpenView':
                        if (this.isEmbedded) {
                            this.actionClick = function () { return _this.openEmbeddedView(); };
                        }
                        else {
                            this.isActionLink(true);
                            this.actionClick = function () { return true; };
                            this.ActionLink = this.openViewUrl;
                        }
                        break;
                    case 'OpenDashboard':
                        this.isActionLink(true);
                        this.ActionLink = this.openDashboardUrl;
                        this.actionClick = function () { return true; };
                        break;
                    case 'NextGroupuingLevel':
                        this.actionClick = function () { return _this.openNextGroupingLevel(); };
                        break;
                    default:
                        log.error('Predefined action not known: ' + this.Name);
                        break;
                }
            }
            if (this.ActionType === ActionType.Javascript) {
                this.actionClick = function () { return eval(_this.ScriptModule); };
            }
            if (this.ActionType === ActionType.Redirect) {
                this.actionClick = function () { return _this.redirectToUrl(_this.Url); };
            }
            if (this.ActionType === ActionType.Hash) {
                this.actionClick = function () { return _this.setHashFromUrl(); };
            }
        };
        ActionButtonViewModel.prototype.isInvokableFor = function () {
            var selectedRecords = this.Parent.selectedRecords();
            var count = selectedRecords.length;
            if (this.InvokableFor === InvokableFor.All)
                return true;
            if (this.InvokableFor & InvokableFor.None &&
                count === 0)
                return true;
            if (this.InvokableFor & InvokableFor.One &&
                count === 1)
                return true;
            if (this.InvokableFor & InvokableFor.Many &&
                count > 1)
                return true;
            var cellCount = selectedRecords.reduce(function (accumulator, record) {
                return accumulator + Object.keys(record.getSelectedValues()).length;
            }, 0);
            if (this.InvokableFor & InvokableFor.OneCell &&
                cellCount === 1)
                return true;
            if (this.InvokableFor & InvokableFor.ManyCells &&
                cellCount > 1)
                return true;
            return false;
        };
        ActionButtonViewModel.prototype.runInvokableForScript = function () {
            try {
                return this.invokableForScriptFunction(this.Parent.selectedRecords());
            }
            catch (ex) {
                console.warn(sprintf("Custom script in bwf-explorer.js (action name '%s') caused an exception.", this.DisplayName), ex);
                return false;
            }
        };
        // actions
        ActionButtonViewModel.prototype.auditTrailUrl = function () {
            if (this.Parent != null && this.Parent.selectedRecords() != null && this.Parent.selectedRecords()[0] != null) {
                var dataService = metadataService.getDataService(this.Parent.dataService());
                var item = this.Parent.selectedRecords()[0];
                return sprintf("%s/view/#default/%s/%s?parameters=Id=%d", dataService.hostUrl, dataService.name, this.AuditType, bwf.filterEncode(item.record.Id));
            }
            return null;
        };
        ;
        ActionButtonViewModel.prototype.openViewUrl = function () {
            if (this.Parent != null && this.Parent.selectedRecords() != null && this.Parent.selectedRecords()[0] != null) {
                var name = ko.unwrap(this.Parent.selectedRecords()[0].record.Name);
                return sprintf("%s/view/#open/%s", options.explorerHostUrl, name);
            }
            return null;
        };
        ;
        ActionButtonViewModel.prototype.openDashboardUrl = function () {
            if (this.Parent != null && this.Parent.selectedRecords() != null && this.Parent.selectedRecords()[0] != null) {
                var id = ko.unwrap(this.Parent.selectedRecords()[0].record.Id);
                return sprintf("%s/dashboard/open/%s", options.explorerHostUrl, id);
            }
            return null;
        };
        ActionButtonViewModel.prototype.customButtonClicked = function (buttonType, value) {
            ko.postbox.publish(this.viewGrid + buttonType + this.PostboxMessage, value);
        };
        ActionButtonViewModel.prototype.openNextGroupingLevel = function () {
            var record = this.Parent.selectedRecords()[0].record;
            ko.postbox.publish(this.viewGrid + "-nextGroupingLevel", record);
        };
        ActionButtonViewModel.prototype.openEmbeddedView = function () {
            var name = ko.unwrap(this.Parent.selectedRecords()[0].record.Name);
            location.hash = 'open/' + name;
        };
        ActionButtonViewModel.prototype.redirectToUnlock = function () {
            var records = this.Parent.selectedRecords();
            var ids = records.map(function (r) { return bwf.filterEncode(r.id); }).join(';');
            var typeName = records[0].typeName;
            var parameters = sprintf("EntityId=%s and EntityType='%s'", ids, typeName);
            var url = sprintf("%s/view/#default/%s/BwfRecordLock?parameters=%s", options.explorerHostUrl, this.Parent.dataService(), parameters);
            this.redirectToUrl(url);
        };
        ActionButtonViewModel.prototype.unlockUser = function () {
            var userId = ko.unwrap(this.Parent.selectedRecords()[0].record.Id);
            var url = sprintf("/ext/membership/user/%s/unlock", userId);
            var request = $.ajax({
                url: url,
                xhrFields: {
                    withCredentials: true
                },
                type: 'POST'
            });
            request.done(function () {
                var message = options.resources['bwf_user_unlocked'];
                ko.postbox.publish('bwf-transient-notification', message);
            });
            request.fail(function (message) {
                if (message.status === 403) {
                    var reason = "You do not have the neccessary permission to unlock users.";
                    ko.postbox.publish("bwf-transient-notification", reason);
                }
                else {
                    ko.postbox.publish("bwf-transient-notification", message.responseText);
                }
            });
        };
        // TODO: understand and simplify this
        ActionButtonViewModel.prototype.updateUrl = function (url) {
            var hostPlaceholder = "[DataServiceHostUrl]", regexp = /{([^}]*)}/g;
            if (url.indexOf(hostPlaceholder) === 0) {
                var dataService = metadataService.getDataService(this.Parent.dataService());
                url = dataService.hostUrl + url.substr(hostPlaceholder.length);
            }
            var urlToSearch = url, match = null, urlItemsToReplace = [];
            while ((match = regexp.exec(urlToSearch)) != null) {
                var urlReplaceItem = new UrlReplaceModel(match[0], match[1], match.index);
                urlItemsToReplace.push(urlReplaceItem);
            }
            var selectedRecords = this.Parent.selectedRecords();
            urlItemsToReplace.forEach(function (replaceItem) {
                var newValues = selectedRecords.map(function (r) { return propertyReader.getPropertyValue(replaceItem.lookupString, r.record); });
                urlToSearch = urlToSearch.replace(replaceItem.replaceString, newValues.join(";"));
            });
            return urlToSearch;
        };
        ActionButtonViewModel.prototype.redirectToUrl = function (url) {
            var updatedUrl = this.updateUrl(url);
            window.location.href = updatedUrl;
        };
        ;
        ActionButtonViewModel.prototype.setHashFromUrl = function () {
            var updatedHash = this.updateUrl(this.Url);
            location.hash = updatedHash;
        };
        ;
        return ActionButtonViewModel;
    }());
    exports.ActionButtonViewModel = ActionButtonViewModel;
    var ActionIconType;
    (function (ActionIconType) {
        ActionIconType[ActionIconType["FontAwesome"] = 0] = "FontAwesome";
        ActionIconType[ActionIconType["BootstrapGlyph"] = 1] = "BootstrapGlyph";
        ActionIconType[ActionIconType["Custom"] = 2] = "Custom";
    })(ActionIconType = exports.ActionIconType || (exports.ActionIconType = {}));
    var ActionType;
    (function (ActionType) {
        ActionType[ActionType["Predefined"] = 0] = "Predefined";
        ActionType[ActionType["Javascript"] = 1] = "Javascript";
        ActionType[ActionType["Redirect"] = 2] = "Redirect";
        ActionType[ActionType["Hash"] = 3] = "Hash";
        ActionType[ActionType["Custom"] = 999] = "Custom";
    })(ActionType = exports.ActionType || (exports.ActionType = {}));
    var InvokableFor;
    (function (InvokableFor) {
        InvokableFor[InvokableFor["None"] = 1] = "None";
        InvokableFor[InvokableFor["One"] = 2] = "One";
        InvokableFor[InvokableFor["Many"] = 4] = "Many";
        InvokableFor[InvokableFor["OneCell"] = 8] = "OneCell";
        InvokableFor[InvokableFor["ManyCells"] = 16] = "ManyCells";
        InvokableFor[InvokableFor["All"] = 31] = "All";
    })(InvokableFor = exports.InvokableFor || (exports.InvokableFor = {}));
    var AggregationRowType = /** @class */ (function () {
        function AggregationRowType() {
        }
        AggregationRowType.Level = { Text: "Level", Value: "Level" };
        AggregationRowType.Subtotal = { Text: "Subtotal", Value: "Subtotal" };
        AggregationRowType.Total = { Text: "Total", Value: "Total" };
        return AggregationRowType;
    }());
    exports.AggregationRowType = AggregationRowType;
    function readOnlyGridConfiguration(gridId, metadata, columns, records, selectIndividualCells, disabled) {
        var canSelectIndividualCells = ko.observable(selectIndividualCells);
        var config = {
            createNewRecord: function () { return null; },
            disabled: disabled || ko.observable(false),
            disableInsertRecords: true,
            disableRemoveRecords: true,
            disableSoftDelete: true,
            canSelectIndividualCells: canSelectIndividualCells,
            header: {
                enabled: ko.observable(false),
                name: null, config: null
            },
            footer: {
                enabled: ko.observable(false),
                name: null, config: null
            },
            isView: false,
            viewGridId: gridId,
            metadata: ko.observable(metadata),
            records: records,
            recordsCount: ko.pureComputed(function () { return records().length; }),
            disableGridSorting: ko.observable(true),
            selectedColumns: ko.pureComputed(function () { return columns; }),
            selectedRecords: ko.pureComputed(function () {
                if (canSelectIndividualCells()) {
                    return records().filter(function (x) {
                        var filtered = Object.keys(x.values).filter(function (k) {
                            return x.values[k].isInCopyOrPasteGroup() || x.values[k].isSelected();
                        });
                        return filtered.length > 0;
                    });
                }
                else {
                    return records().filter(function (r) { return r.selected(); });
                }
            }),
            inEditMode: ko.observable(false),
            postRender: function () { return null; },
            validate: function () { return null; },
            embedded: true,
        };
        return config;
    }
    exports.readOnlyGridConfiguration = readOnlyGridConfiguration;
    var DummyQueryManager = /** @class */ (function () {
        function DummyQueryManager(dataServiceHostUrl, dataService, baseType, grid, aggregates, autoUpdatesEnabled, inEditMode, query, records, columns, querying, enableQuerying) {
            if (enableQuerying === void 0) { enableQuerying = true; }
            this.dataService = dataService;
            this.columns = columns;
            this.records = records;
            this.baseType = baseType;
        }
        DummyQueryManager.prototype.setData = function (nakedRecords, derivedTypeName) {
            var _this = this;
            var baseType = derivedTypeName || this.baseType;
            var columns = this.columns();
            var transformed = nakedRecords.map(function (r, i) {
                var dsr = {
                    BaseTypeName: baseType,
                    TypeName: _this.baseType,
                    Data: r,
                    Id: bwf.getNextBwfId().toString(),
                    Position: i + 1
                };
                return new ExplorerGridItem(dsr, columns, _this.dataService);
            });
            this.records(transformed);
        };
        return DummyQueryManager;
    }());
    exports.DummyQueryManager = DummyQueryManager;
    function generateIdentificationSummaryGridConfiguration(gridId, metadata, disabled) {
        return generateReadOnlyGridConfiguration(metadata.identificationSummaryFields, {}, gridId, metadata, disabled);
    }
    exports.generateIdentificationSummaryGridConfiguration = generateIdentificationSummaryGridConfiguration;
    function generateReadOnlyGridConfiguration(properties, displayNames, gridId, metadata, disabled) {
        var gridItems = ko.observableArray([]);
        var columns = generateGridColumnsFromPropertyNames(properties, displayNames, metadata);
        var manager = new DummyQueryManager(null, metadata.dataService, metadata.type, null, null, null, null, null, gridItems, function () { return columns; }, null);
        var config = readOnlyGridConfiguration(gridId, metadata, columns, gridItems, false, disabled);
        return {
            configuration: config,
            gridItems: gridItems,
            setRecords: function (r) { return manager.setData(r, metadata.type); }
        };
    }
    exports.generateReadOnlyGridConfiguration = generateReadOnlyGridConfiguration;
    var BasicGridColumnMetadata = /** @class */ (function () {
        function BasicGridColumnMetadata(type, name, abbreviatedName, displayName) {
            if (abbreviatedName === void 0) { abbreviatedName = name; }
            if (displayName === void 0) { displayName = abbreviatedName; }
            this.additionalDescriptions = [];
            this.defaultFormat = null;
            this.description = null;
            this.isMandatoryInEditMode = false;
            this.isNotEditableInGrid = false;
            this.isNullable = true;
            this.useCustomDisplayCell = false;
            this.useCustomEditingCell = false;
            this.dataService = "";
            this.displayFieldInEditorChoice = "";
            this.filteredOn = [];
            this.valueFieldInEditorChoice = "";
            this.populateChoiceUrl = "";
            this.populateChoiceQuery = "";
            this.refreshChoiceOnChangesTo = "";
            this.isDisabledInEditMode = false;
            this.isDisabledInCreateMode = false;
            this._clrType = "";
            this._isType = false;
            // IMetadataProperty - added these to support type property choices but probably just use metadata directly or add explicit support for choice values not coming from BWF queries, so can probably remove these
            this.copyBehaviour = null;
            this.customCopyScript = null;
            this.customControl = null;
            this.customControlHeight = null;
            this.isCustomControlHeightAuto = false;
            this.customControlParameter = null;
            this.customDisplayCell = null;
            this.customEditingCell = null;
            this.defaultValue = null;
            this.editingName = null;
            this.format = null;
            this.fullName = null;
            this.heightInLines = null;
            this.identityProperties = [];
            this.isFreeFormat = false;
            this.isHidden = false;
            this.isHiddenInEditor = false;
            this.isNotEditableInPanel = false;
            this.isNotCreatableInPanel = false;
            this.parameterAllowNullOrEmpty = false;
            this.parameterAvailableOperators = [];
            this.parameterDefaultValue = null;
            this.parameterDisplayProperty = null;
            this.parameterQuery = null;
            this.parameterQueryDataService = null;
            this.positionInEditor = null;
            this.isSelectorField = false;
            this.useCustomControl = false;
            this.isHiddenInEditMode = false;
            this.isHiddenInCreateMode = false;
            this._abbreviatedWasEmpty = false;
            this._isCollection = false;
            this.name = name;
            this.abbreviatedName = abbreviatedName;
            this.displayName = displayName;
            this.type = type.toLowerCase();
            this.isEnum = this.type === "enum";
            this.hasChoice = this.isEnum;
            // grid seems to ignore alignment, so don't bother?
            switch (this.type) {
                case 'boolean':
                    this.alignment = 'center';
                    break;
                case 'date':
                case 'time':
                case 'measure':
                case 'integer':
                case 'numeric':
                    this.alignment = 'right';
                    break;
                default:
                    this.alignment = 'left';
            }
        }
        return BasicGridColumnMetadata;
    }());
    exports.BasicGridColumnMetadata = BasicGridColumnMetadata;
    var BasicRecord = /** @class */ (function () {
        function BasicRecord(position, data) {
            this.BaseTypeName = null;
            this.TypeName = null;
            this.Id = bwf.getNextBwfId().toString();
            this.Position = position;
            this.Data = data;
        }
        return BasicRecord;
    }());
    exports.BasicRecord = BasicRecord;
    var BasicGridItem = /** @class */ (function (_super) {
        __extends(BasicGridItem, _super);
        function BasicGridItem(record, columns) {
            var _this = _super.call(this, record, columns, "") || this;
            _this.createItem(record);
            return _this;
        }
        BasicGridItem.prototype.createRecord = function (record) {
            return record.Data;
        };
        BasicGridItem.prototype.applyValidation = function (validationResult) {
            var _this = this;
            if (validationResult['message']) {
                log.warn(validationResult.message);
                return;
            }
            this.resetValidation();
            if (validationResult.Type === "MessageSet") {
                this.modelValidations(validationResult.Messages);
                return;
            }
            var validation = validationResult;
            Object.keys(validation.PropertyValidations)
                .filter(function (key) { return key[0] !== '$'; })
                .forEach(function (key) {
                if (key in _this.values) {
                    _this.values[key].validationMessages
                        .push(validation.PropertyValidations[key]);
                    _this.values[key].isValid(false);
                }
                else {
                    // either a type property or a property not in the presentation
                    if (key in _this.dirtyRecord) {
                        // not in the presentation
                        validation.ModelValidations.push(sprintf("%s: %s", key, validation.PropertyValidations[key]));
                    }
                    else {
                        // a type
                        var valueField = _this.metadata.properties[key].valueFieldInEditorChoice;
                        var subFieldKey = sprintf('%s/%s', key, valueField);
                        // since this must be a dropdown, we have to have a field that is either
                        // the value field of of the object, or the name field as the source
                        if (subFieldKey in _this.values) {
                            var typeKey = subFieldKey;
                        }
                        else {
                            var typeKey = sprintf('%s/%s', key, 'Name');
                        }
                        if (!(typeKey in _this.values))
                            log.error(sprintf("Unknown property '%s'", key));
                        _this.values[typeKey].validationMessages
                            .push(validation.PropertyValidations[key]);
                        _this.values[typeKey].isValid(false);
                    }
                }
            });
            this.modelValidations(validation.ModelValidations);
        };
        return BasicGridItem;
    }(BaseGridItem));
    exports.BasicGridItem = BasicGridItem;
    function generateBasicGridItem(record, position, columns) {
        return new BasicGridItem(new BasicRecord(position, record), columns);
    }
    exports.generateBasicGridItem = generateBasicGridItem;
    function generateBasicGridItems(records, columns) {
        return records.map(function (r, i) { return generateBasicGridItem(r, i + 1, columns); });
    }
    exports.generateBasicGridItems = generateBasicGridItems;
    function generateGridConfiguration(items, columns, metadata, gridId, editable) {
        var records = ko.observableArray(items);
        var config = readOnlyGridConfiguration(gridId, metadata, columns, records, editable);
        if (editable)
            config.inEditMode = ko.observable(true);
        config.updateDirtyRecordWithLatestValues = (function (item, columns) {
            return columns.filter(function (c) { return !c.metadata.hasChoice && !c.metadata.isNotEditableInGrid && !item.values[c.path].isReadonly(); })
                .forEach(function (c) {
                var record = item.dirtyRecord;
                var paths = c.path.split('/');
                var i = 0;
                while (i < paths.length - 1)
                    record = record[paths[i++]];
                record[paths[i]] = item.values[c.path]();
            });
        });
        return config;
    }
    exports.generateGridConfiguration = generateGridConfiguration;
    function generateGridColumnsFromPropertyNames(properties, displayNames, metadata) {
        return properties.map(function (p, i) {
            var column = new ExplorerGridColumn(metadataService.getProperty(metadata, p, {}), p, i + 1);
            var displayName = displayNames[p];
            if (displayName)
                column.displayName(displayName);
            return column;
        });
    }
    exports.generateGridColumnsFromPropertyNames = generateGridColumnsFromPropertyNames;
    function generateBasicGridConfiguration(items, columns, gridId, editable) {
        var choiceProperties = columns
            .filter(function (v) { return v.metadata.hasChoice; })
            .reduce(function (props, col) {
            var pieces = col.path.split('/');
            if (pieces.length === 2)
                props[pieces[0]] = col.metadata;
            return props;
        }, {});
        var metadata = {
            autoUpdatesByDefault: true,
            dataService: "",
            displayName: "",
            expandsRequiredForEdit: [],
            identityProperties: [],
            identificationSummaryFields: [],
            hasCompositeId: false,
            hasEditabilityToRoles: false,
            hasVisibilityToRoles: false,
            pluralisedDisplayName: "",
            properties: choiceProperties,
            selectorFields: [],
            supportsEditMode: false,
            supportsAggregations: false,
            insertableInEditMode: false,
            deletableInEditMode: false,
            type: "",
            resourcefulRoute: "",
            usesTypesFromOtherDataServices: false,
            useCombinedQuerier: false
        };
        return generateGridConfiguration(items, columns, metadata, gridId, editable);
    }
    exports.generateBasicGridConfiguration = generateBasicGridConfiguration;
});
