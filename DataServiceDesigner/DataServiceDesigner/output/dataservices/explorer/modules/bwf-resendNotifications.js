define(["require", "exports", "knockout", "options", "modules/bwf-metadata", "modules/bwf-explorer", "modules/bwf-dataServiceClient", "modules/bwf-queryBuilder", "linqjs"], function (require, exports, ko, options, metadataService, explorer, DataServiceClient, bwf_queryBuilder_1, Enumerable) {
    "use strict";
    var ResendNotificationsViewModel = /** @class */ (function () {
        function ResendNotificationsViewModel(panelEntity) {
            var _this = this;
            this.cancelButtonCaption = ko.observable(options.resources['no']);
            this.disableSave = ko.observable(false);
            this.errorMessages = ko.observableArray();
            this.errorsTitle = ko.observable("");
            this.recipients = ko.observableArray();
            this.resources = options.resources;
            this.saveButtonCaption = ko.observable(options.resources['yes']);
            this.showSaveButton = ko.observable(true);
            this.subscriptions = [];
            this.title = options.resources['bwf_resend_notification'];
            this.gridId = panelEntity.state.gridId;
            this.ids = panelEntity.record.Id;
            metadataService.getType("explorer", "Recipient").done(function (metadata) { return _this.setupGrid(metadata); });
            var query = new bwf_queryBuilder_1.QueryBuilder("NotificationRecipient")
                .Filter(function (f) {
                return (_a = f.Property("Id")).In.apply(_a, _this.ids);
                var _a;
            })
                .Expand("Recipient")
                .GetQuery();
            this.client = new DataServiceClient("explorer");
            this.client.query(query).done(function (result) {
                var recipients = Enumerable
                    .from(result.Records)
                    .distinct(function (x) { return x.Recipient.Id; })
                    .select(function (x) { return x.Recipient; })
                    .toArray();
                _this.recipients(recipients);
            });
        }
        ResendNotificationsViewModel.prototype.setupGrid = function (metadata) {
            var grid = explorer.generateIdentificationSummaryGridConfiguration(this.gridId + "-recipients", metadata, this.disableSave);
            this.subscriptions.push(this.recipients.subscribe(function (r) { return grid.setRecords(r); }));
            this.gridConfiguration = grid.configuration;
        };
        ResendNotificationsViewModel.prototype.confirmResend = function () {
            var _this = this;
            if (this.disableSave())
                return;
            this.disableSave(true);
            var ids = this.ids;
            var request = $.ajax({
                url: options.explorerHostUrl + "/notifications/resend",
                xhrFields: { withCredentials: true },
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(ids)
            });
            request.done(function (response) {
                if (response.Failures.length === 0) {
                    ko.postbox.publish("bwf-transient-notification", options.resources['bwf_resent_notifications']);
                    _this.cancel();
                    return;
                }
                _this.showSaveButton(!response.Success);
                _this.disableSave(false);
                _this.saveButtonCaption(options.resources['bwf_try_again']);
                _this.cancelButtonCaption(options.resources['bwf_cancel']);
                _this.errorsTitle(options.resources[response.Success ? 'bwf_resent_notifications_with_errors' : 'bwf_resend_notifications_failed']);
                _this.errorMessages(response.Failures);
            });
            request.fail(function (message) {
                _this.errorsTitle(options.resources['bwf_unexpected_error_encountered']);
                _this.errorMessages([JSON.parse(message.responseText).message]);
                _this.disableSave(false);
            });
        };
        ResendNotificationsViewModel.prototype.cancel = function () {
            ko.postbox.publish(this.gridId + "-pop-panel");
        };
        ResendNotificationsViewModel.prototype.dispose = function () {
            this.subscriptions.forEach(function (x) {
                if (x != null && typeof x.dispose == 'function')
                    x.dispose();
            });
            this.client.dispose();
        };
        return ResendNotificationsViewModel;
    }());
    return ResendNotificationsViewModel;
});
