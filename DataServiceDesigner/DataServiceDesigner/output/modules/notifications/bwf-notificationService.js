define(["require", "exports", "modules/bwf-liveQuery", "options", "knockout", "kendo.all.min"], function (require, exports, liveQuery, options, ko, kendo) {
    "use strict";
    var NotificationService = /** @class */ (function () {
        function NotificationService() {
            var _this = this;
            this.disposables = [];
            this.newNotifications = ko.observable(0);
            var now = kendo.toString(new Date(), 'yyyy-MM-ddTHH:mm:ss', 'en-GB');
            var query = "ToastMessages?$filter=IsRead=false&$orderby=Received desc&$top=1";
            this.executor = new liveQuery(options.explorerHostUrl, 'explorer', true, "query", query, 1000, function (result) { return _this.newNotifications(result.TotalCount); }, function (result) {
                var changes = result.Fields[0].Fields[0];
                // for simplicity we only show the "latest" toast
                var latestToast = changes.RecordChanges
                    .filter(function (rc) { return rc.ResultType == "Added"; })
                    .map(function (rc) { return rc.Record.Data; })[0];
                _this.newNotifications(changes.TotalCount);
                if (latestToast != null)
                    _this.onNotification(latestToast);
            });
            this.disposables.push({ dispose: this.executor.stop });
        }
        NotificationService.prototype.onNotification = function (toast) {
            var notification = {
                title: toast.Title,
                message: toast.Message
            };
            ko.postbox.publish("bwf-transient-notification", notification);
        };
        NotificationService.prototype.Reset = function () {
            this.newNotifications(0);
        };
        return NotificationService;
    }());
    var ns = new NotificationService();
    return ns;
});
