define(["require", "exports", "options", "jquery", "loglevel", "modules/bwf-liveQuery"], function (require, exports, options, $, log, liveQuery) {
    "use strict";
    var AutoLogOutViewModel = /** @class */ (function () {
        function AutoLogOutViewModel() {
            this.liveQuery = null;
            this.observeSessionTerminations();
        }
        AutoLogOutViewModel.prototype.dispose = function () {
            if (this.liveQuery != null)
                this.liveQuery.stop();
        };
        AutoLogOutViewModel.prototype.getCookie = function (c_name) {
            var c_value = document.cookie;
            var c_start = c_value.indexOf(" " + c_name + "=");
            if (c_start == -1) {
                c_start = c_value.indexOf(c_name + "=");
            }
            if (c_start == -1) {
                c_value = null;
            }
            else {
                c_start = c_value.indexOf("=", c_start) + 1;
                var c_end = c_value.indexOf(";", c_start);
                if (c_end == -1) {
                    c_end = c_value.length;
                }
                c_value = decodeURI(c_value.substring(c_start, c_end));
            }
            return c_value;
        };
        AutoLogOutViewModel.prototype.observeSessionTerminations = function () {
            var _this = this;
            var initialQueryDone = function (queryResults) {
                if (queryResults.Records.length !== 0) {
                    _this.redirect(queryResults.Records[0].Record.Data.Reason);
                }
                ;
            };
            var changesQueryDone = function (queryResults) {
                var cacheItem = queryResults.Fields[0];
                var data = cacheItem.Fields[0];
                $.each(data.RecordChanges, function (index, item) {
                    switch (item.ResultType) {
                        case 'Added':
                            _this.redirect(item.Record.Data.Reason);
                            break;
                        case 'Removed':
                            log.warn('Removed SessionTerminated should never occur');
                            break;
                        case 'Updated':
                            log.warn('Updated SessionTerminated should never occur');
                            break;
                    }
                    ;
                });
            };
            var sessionId = this.getCookie('bwftoken');
            var query = "SessionTerminateds?$filter=SessionId='" + sessionId + "'&$top=1";
            this.liveQuery = new liveQuery(options.explorerHostUrl, 'membership', true, "query", query, 10000, initialQueryDone, changesQueryDone);
        };
        ;
        AutoLogOutViewModel.prototype.redirect = function (reason) {
            this.liveQuery.stop();
            var redirectUrl = options.explorerHostUrl + '/authentication/logout?reason=' + reason + '&returnUrl=' + encodeURIComponent(window.location.href);
            window.location.assign(redirectUrl);
        };
        ;
        return AutoLogOutViewModel;
    }());
    return AutoLogOutViewModel;
});
