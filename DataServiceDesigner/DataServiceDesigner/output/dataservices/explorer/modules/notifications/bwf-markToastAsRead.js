define(["require", "exports", "options", "knockout", "loglevel"], function (require, exports, options, ko, log) {
    "use strict";
    var MarkToastAsRead = /** @class */ (function () {
        function MarkToastAsRead(args) {
            var _this = this;
            this.title = ko.observable('');
            this.error = ko.observable('');
            this.hasError = ko.observable(false);
            this.resources = options.resources;
            this.onError = function (response) {
                _this.hasError(true);
                _this.error(JSON.stringify(response));
            };
            this.close = function () {
                ko.postbox.publish(_this.gridId + '-pop-panel');
            };
            this.title(options.resources['bwf_marking_as_read']);
            this.gridId = args.state.gridId;
            var records = Array.isArray(args.record)
                ? args.record
                : [args.record];
            records.forEach(function (r) { return r.IsRead = true; });
            var url = options.explorerHostUrl + "/api/explorer/changeset/ToastMessage";
            var changeSet = {
                Update: records.reduce(function (acc, i) { return acc[i.Id] = i, acc; }, {})
            };
            var request = $.ajax({
                url: url,
                xhrFields: {
                    withCredentials: true
                },
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(changeSet)
            });
            if (args.state['viewAfterMarking']) {
                request.done(function () {
                    var payload = {
                        action: 'view',
                        component: 'bwf-panel-editor',
                        baseType: 'ToastMessage',
                        dataService: 'explorer',
                        data: records[0]
                    };
                    ko.postbox.publish(_this.gridId + '-doAction', payload);
                });
                request.fail(this.onError);
            }
            else {
                request.done(function () {
                    ko.postbox.publish('bwf-transient-notification', "Marked as read");
                });
                request.fail(function (fail) { return log.error(fail); });
            }
        }
        return MarkToastAsRead;
    }());
    return MarkToastAsRead;
});
