define(["require", "exports", "options", "jquery", "modules/bwf-liveQuery", "modules/bwf-metadata", "modules/bwf-queryBuilder"], function (require, exports, options, $, LiveQueryExecutor, metadataService, queryBuilder) {
    "use strict";
    var DataServiceClient = /** @class */ (function () {
        function DataServiceClient(defaultDataService) {
            this.liveQueryExecutors = {};
            this.liveQueryPollingIntervalMilliseconds = 1000;
            this.recordLockTypeString = "BwfRecordLock";
            this.defaultDataService = defaultDataService;
        }
        DataServiceClient.prototype.getDataService = function (dataServiceName) {
            if (dataServiceName == null) {
                if (!this.defaultDataService)
                    throw "No default data service has been specified.";
                return this.defaultDataService;
            }
            return dataServiceName;
        };
        DataServiceClient.prototype.startLiveQuery = function (query, changesCallback, dataService) {
            var _this = this;
            var deferred = $.Deferred();
            metadataService.getDataServiceSafely(this.getDataService(dataService))
                .done(function (ds) {
                var executor = new LiveQueryExecutor(ds.hostUrl, ds.name, true, "query", query, _this.liveQueryPollingIntervalMilliseconds, function (r) { return deferred.resolve({ QueryId: executor.queryId, Records: r.Records.map(function (x) { return x.Data; }) }); }, function (r) {
                    var result = r;
                    var changes = result.Fields[0].Fields[0];
                    var recordChanges = changes.RecordChanges
                        .filter(function (x) { return !(x.ChangedProperties.length == 1 && x.ChangedProperties[0] == ".Position"); })
                        .map(function (x) { return { ResultType: x.ResultType, Record: x.Record.Data }; });
                    changesCallback(recordChanges, changes.ReplaceAll);
                });
                _this.liveQueryExecutors[executor.queryId] = executor;
            })
                .fail(deferred.reject);
            return deferred.promise();
        };
        DataServiceClient.prototype.killLiveQuery = function (queryId) {
            var executor = this.liveQueryExecutors[queryId];
            if (executor) {
                delete this.liveQueryExecutors[queryId];
                executor.stop();
            }
        };
        DataServiceClient.prototype.dispose = function () {
            var executors = this.liveQueryExecutors;
            this.liveQueryExecutors = {};
            for (var e in executors)
                executors[e].stop();
        };
        DataServiceClient.prototype.create = function (item, type, dataService, requeryCreated) {
            var _this = this;
            if (dataService === void 0) { dataService = null; }
            if (requeryCreated === void 0) { requeryCreated = true; }
            var deferred = $.Deferred();
            this.getMetadataForType(type, this.getDataService(dataService)).done(function (metadata) {
                var giveRoles = metadata.hasEditabilityToRoles || metadata.hasVisibilityToRoles;
                var submissionData = giveRoles ? {
                    EditableByRoles: [],
                    VisibleToRoles: [],
                    Record: item
                } : item;
                metadataService.getDataServiceSafely(_this.getDataService(dataService)).done(function (ds) {
                    var createUrl = ds.url + "/" + type;
                    if (!requeryCreated)
                        createUrl = createUrl + "/withoutrequery";
                    $.ajax({
                        type: 'POST',
                        url: createUrl,
                        xhrFields: { withCredentials: true },
                        contentType: 'application/json',
                        data: JSON.stringify(submissionData)
                    }).done(deferred.resolve).fail(deferred.reject);
                }).fail(deferred.reject);
            }).fail(deferred.reject);
            return deferred.promise();
        };
        DataServiceClient.prototype.update = function (item, type, dataService, requeryUpdated) {
            if (dataService === void 0) { dataService = null; }
            if (requeryUpdated === void 0) { requeryUpdated = false; }
            var deferred = $.Deferred();
            metadataService.getDataServiceSafely(this.getDataService(dataService)).done(function (ds) {
                $.ajax({
                    type: 'PUT',
                    url: ds.url + "/" + type + "/" + item['Id'],
                    xhrFields: { withCredentials: true },
                    contentType: 'application/json',
                    data: JSON.stringify(item)
                }).done(deferred.resolve).fail(deferred.reject);
            }).fail(deferred.reject);
            return deferred.promise();
        };
        DataServiceClient.prototype.delete = function (id, type, dataService) {
            if (dataService === void 0) { dataService = null; }
            var deferred = $.Deferred();
            metadataService.getDataServiceSafely(this.getDataService(dataService)).done(function (ds) {
                $.ajax({
                    type: 'DELETE',
                    url: ds.url + "/" + type + "/" + id,
                    xhrFields: { withCredentials: true }
                }).done(deferred.resolve).fail(deferred.reject);
            }).fail(deferred.reject);
            return deferred.promise();
        };
        DataServiceClient.prototype.processChangeSet = function (changeSet, type, dataService) {
            if (dataService === void 0) { dataService = null; }
            var deferred = $.Deferred();
            metadataService.getDataServiceSafely(this.getDataService(dataService)).done(function (ds) {
                return $.ajax({
                    type: 'POST',
                    url: ds.hostUrl + "/api/" + ds.name + "/changeset/" + type,
                    xhrFields: { withCredentials: true },
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify(changeSet)
                }).done(deferred.resolve).fail(deferred.reject);
            });
            return deferred.promise();
        };
        DataServiceClient.prototype.get = function (id, type, dataService) {
            if (dataService === void 0) { dataService = null; }
            var deferred = $.Deferred();
            metadataService.getDataServiceSafely(this.getDataService(dataService)).done(function (ds) {
                return $.ajax({
                    url: ds.hostUrl + "/api/" + ds.name + "/" + type + "/" + id,
                    xhrFields: { withCredentials: true }
                }).done(deferred.resolve).fail(deferred.reject);
            });
            return deferred.promise();
        };
        DataServiceClient.prototype.query = function (query, dataService) {
            if (dataService === void 0) { dataService = null; }
            var deferred = $.Deferred();
            metadataService.getDataServiceSafely(this.getDataService(dataService)).done(function (ds) {
                return $.ajax({
                    url: ds.hostUrl + "/api/" + ds.name + "/query/" + query,
                    xhrFields: { withCredentials: true }
                }).done(deferred.resolve).fail(deferred.reject);
            });
            return deferred.promise();
        };
        DataServiceClient.prototype.getMetadataForType = function (type, dataService) {
            if (dataService === void 0) { dataService = null; }
            return metadataService.getType(this.getDataService(dataService), type);
        };
        DataServiceClient.prototype.lock = function (id, context, reason, itemType) {
            var deferred = $.Deferred();
            var item = {
                Id: 0,
                Context: context,
                EntityType: itemType,
                EntityId: id,
                Reason: reason,
                Username: options.username || '',
                TimeStamp: "0001-01-01T00:00:00",
            };
            this.create(item, this.recordLockTypeString, "explorer", false).done(function (result) {
                result.WasSuccessful ? deferred.resolve() : deferred.reject();
            }).fail(deferred.reject);
            return deferred.promise();
        };
        DataServiceClient.prototype.getUnlockQueryString = function (id, itemType, context) {
            var qb = new queryBuilder.QueryBuilder(this.recordLockTypeString);
            var query = qb
                .Top(1)
                .Filter(function (f) {
                return f.Property("EntityType").EqualTo(itemType)
                    .And()
                    .Property("EntityId").EqualTo(id)
                    .And()
                    .Property("Context").EqualTo(context);
            }).GetQuery();
            return query;
        };
        DataServiceClient.prototype.unlock = function (id, itemType, context) {
            var _this = this;
            var deferred = $.Deferred();
            var query = this.getUnlockQueryString(id, itemType, context);
            this.query(query, "explorer").done(function (queryResult) {
                if (queryResult.Records && queryResult.TotalCount === 1) {
                    var record = queryResult.Records[0];
                    _this.delete(record.Id, _this.recordLockTypeString, "explorer").done(function () {
                        deferred.resolve();
                    }).fail(deferred.reject);
                }
                else {
                    deferred.resolve();
                }
            }).fail(deferred.reject);
            return deferred.promise();
        };
        DataServiceClient.prototype.getTypesForDataService = function (dataServiceName) {
            var deferred = $.Deferred();
            metadataService.getDataServiceSafely(dataServiceName).done(function (ds) {
                $.ajax({
                    url: ds.url + "/availableTypes",
                    xhrFields: { withCredentials: true }
                }).done(deferred.resolve).fail(deferred.reject);
            });
            return deferred.promise();
        };
        return DataServiceClient;
    }());
    return DataServiceClient;
});
