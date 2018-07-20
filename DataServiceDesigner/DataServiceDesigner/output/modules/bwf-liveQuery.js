define(["require", "exports", "jquery", "modules/bwf-utilities", "loglevel"], function (require, exports, $, utils, log) {
    "use strict";
    var LiveQuerier = /** @class */ (function () {
        function LiveQuerier() {
            this.numberOfConnectionRetries = 5;
            this.pollingIntervalMillisecondsLowerLimit = 1000;
            this.pollingIntervalMilliseconds = 1000;
            this.connectionId = null;
            this.watchChangesTimeoutsPerHostUrl = {};
            this.dataServiceHostUrls = [];
            this.Queries = {};
        }
        LiveQuerier.prototype.watchingChangesDone = function (response, hostUrl) {
            var _this = this;
            log.debug('Changes response received');
            if (response) {
                Object.keys(response).forEach(function (queryId) {
                    if (response[queryId] && _this.Queries[queryId].watchChanges)
                        _this.Queries[queryId].changesQueryDone(response[queryId]);
                });
            }
            clearTimeout(this.watchChangesTimeoutsPerHostUrl[hostUrl]);
            this.watchChangesTimeoutsPerHostUrl[hostUrl] = setTimeout($.proxy(this.checkForChanges, this, hostUrl), this.pollingIntervalMilliseconds);
        };
        ;
        LiveQuerier.prototype.watchingChangesFailed = function (failure, liveQueryEntries, hostUrl) {
            log.error("Watch changes error for host " + liveQueryEntries[0].dataServiceHostUrl + " " +
                ("(trying " + liveQueryEntries[0].connectionRetriesRemaining + " more times)"));
            liveQueryEntries.forEach(function (x) { return x.connectionRetriesRemaining -= 1; });
            clearTimeout(this.watchChangesTimeoutsPerHostUrl[hostUrl]);
            this.watchChangesTimeoutsPerHostUrl[hostUrl] = setTimeout($.proxy(this.checkForChanges, this, hostUrl), this.pollingIntervalMilliseconds);
        };
        ;
        LiveQuerier.prototype.checkForChanges = function (dataServiceHostUrl) {
            var _this = this;
            var changesUrl = dataServiceHostUrl + "/api/live/changes/";
            var querysForDataServiceHost = Object.keys(this.Queries).map(function (k) { return _this.Queries[k]; })
                .filter(function (x) { return x.dataServiceHostUrl === dataServiceHostUrl; });
            var activeQueries = querysForDataServiceHost.filter(function (x) { return x.watchChanges && x.connectionRetriesRemaining > 0; });
            if (activeQueries.length > 0) {
                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json',
                    xhrFields: { withCredentials: true },
                    url: changesUrl,
                    data: JSON.stringify(querysForDataServiceHost.map(function (x) { return x.Id; }))
                }).done(function (result) {
                    _this.watchingChangesDone(result, dataServiceHostUrl);
                }).fail(function (failure) {
                    _this.watchingChangesFailed(failure, activeQueries, dataServiceHostUrl);
                });
            }
            else {
                setTimeout($.proxy(this.checkForChanges, this, dataServiceHostUrl), this.pollingIntervalMilliseconds);
            }
        };
        ;
        LiveQuerier.prototype.setPollingInterval = function (interval) {
            if (typeof interval !== 'number') {
                log.error("Polling interval must be a number");
                return;
            }
            // We are going to take the lowest value
            if (interval >= this.pollingIntervalMillisecondsLowerLimit &&
                interval < this.pollingIntervalMilliseconds)
                this.pollingIntervalMilliseconds = interval;
        };
        LiveQuerier.prototype.setWatchChanges = function (queryId, watch) {
            if (watch === void 0) { watch = true; }
            if (this.Queries[queryId])
                this.Queries[queryId].watchChanges = watch;
        };
        LiveQuerier.prototype.add = function (dataServiceHostUrl, dataService, watchChanges, queryApi, query, initialQueryDone, changesQueryDone, initialQueryFail) {
            var _this = this;
            if (!this.connectionId)
                this.connectionId = utils.generateGuid();
            var newQueryId = utils.generateGuid();
            $.ajax({
                url: dataServiceHostUrl + "/api/live/" + queryApi + "/" + this.connectionId + "/" + newQueryId + "/" + dataService + "/" + query,
                xhrFields: { withCredentials: true },
            }).done(function (result) {
                if (_this.dataServiceHostUrls.indexOf(dataServiceHostUrl.trim()) === -1)
                    _this.dataServiceHostUrls.push(dataServiceHostUrl.trim());
                initialQueryDone(result);
                _this.Queries[newQueryId] = {
                    dataService: dataService,
                    dataServiceHostUrl: dataServiceHostUrl,
                    query: query.trim(),
                    changesQueryDone: changesQueryDone,
                    watchChanges: watchChanges,
                    Id: newQueryId,
                    connectionRetriesRemaining: _this.numberOfConnectionRetries
                };
                if (!_this.watchChangesTimeoutsPerHostUrl[dataServiceHostUrl]) {
                    _this.watchChangesTimeoutsPerHostUrl[dataServiceHostUrl] = setTimeout($.proxy(_this.checkForChanges, _this, dataServiceHostUrl), _this.pollingIntervalMilliseconds);
                }
            }).fail(function (failure) {
                if (initialQueryFail)
                    initialQueryFail(failure.responseJSON);
                log.error("Error occurred performing initial query for live query", failure);
            });
            return newQueryId;
        };
        LiveQuerier.prototype.remove = function () {
            var _this = this;
            var queryIds = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                queryIds[_i] = arguments[_i];
            }
            var querys = queryIds.map(function (x) { return _this.Queries[x]; }).filter(function (x, index) { return !!x; });
            var distinctDataServiceHostUrls = querys.map(function (x) { return x.dataServiceHostUrl; }).filter(function (v, i, a) { return a.indexOf(v) === i; });
            log.debug("Removing queries", queryIds);
            distinctDataServiceHostUrls.forEach(function (ds) {
                var thisDataServiceQueries = querys.filter(function (x) { return x.dataServiceHostUrl == ds; });
                var queriesToCancel = thisDataServiceQueries.filter(function (x) { return !x.cancelQueryRequestInProgress; });
                queriesToCancel.forEach(function (q) { return q.cancelQueryRequestInProgress = true; });
                var cancelIds = queriesToCancel.map(function (x) { return x.Id; });
                $.ajax({
                    url: ds + "/api/live/cancelquery",
                    type: 'DELETE',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify(cancelIds),
                    xhrFields: { withCredentials: true },
                }).done(function (result) {
                    cancelIds.forEach(function (id) { return delete _this.Queries[id]; });
                    log.debug("Removed query ids from live querier for data service '" + thisDataServiceQueries[0].dataService + "'", cancelIds);
                }).fail(function (failure) {
                    log.warn("Failed to remove query Ids from live querier", queryIds, failure);
                }).always(function () {
                    queriesToCancel.forEach(function (q) { return q.cancelQueryRequestInProgress = false; });
                });
            });
        };
        LiveQuerier.prototype.getCancelQueryUrl = function (queryId) {
            var liveQueryInfo = this.Queries[queryId];
            return liveQueryInfo.dataServiceHostUrl + "/api/live/cancelquery";
        };
        return LiveQuerier;
    }());
    var liveQuerierInstance = new LiveQuerier();
    var LiveQuerier_Shim = /** @class */ (function () {
        function LiveQuerier_Shim(dataServiceHostUrl, dataService, watchChanges, queryapi, query, pollingIntervalMilliseconds, initialQueryDone, changesQueryDone, initialQueryFail) {
            var _this = this;
            this.watchChangesInternal = true;
            this.stop = function () {
                liveQuerierInstance.remove(_this.queryId);
            };
            this.watchChangesInternal = watchChanges;
            this.queryId = liveQuerierInstance.add(dataServiceHostUrl, dataService, watchChanges, queryapi, query, initialQueryDone, changesQueryDone, initialQueryFail);
            liveQuerierInstance.setPollingInterval(pollingIntervalMilliseconds);
        }
        Object.defineProperty(LiveQuerier_Shim.prototype, "watchChanges", {
            get: function () {
                return this.watchChangesInternal;
            },
            set: function (watch) {
                if (this.watchChangesInternal != watch) {
                    this.watchChangesInternal = watch;
                    liveQuerierInstance.setWatchChanges(this.queryId, watch);
                }
            },
            enumerable: true,
            configurable: true
        });
        return LiveQuerier_Shim;
    }());
    return LiveQuerier_Shim;
});
