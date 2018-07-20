define(["require", "exports", "knockout", "options"], function (require, exports, ko, options) {
    "use strict";
    var NotificationFromConfigurationViewModel = /** @class */ (function () {
        function NotificationFromConfigurationViewModel(panelEntity) {
            this.resources = options.resources;
            this.title = ko.observable(options.resources['bwf_generate_notification_from_configuration']);
            this.generating = ko.observable(false);
            this.data = panelEntity;
            this.gridId = panelEntity.state.gridId;
            this.errorMessages = ko.observableArray([]);
            this.replacements = ko.observableArray([]);
            this.replacementsParams = {
                model: {
                    observables: {
                        Replacements: this.replacements,
                        formDisabled: ko.observable(false),
                        __renderedState: ko.observableArray([])
                    }
                }
            };
        }
        NotificationFromConfigurationViewModel.prototype.generate = function () {
            var _this = this;
            if (this.generating())
                return;
            this.generating(true);
            var id = this.data.record[0].Id;
            $.ajax({
                url: options.explorerHostUrl + "/notifications/generatenotificationfromconfiguration/" + id,
                xhrFields: { withCredentials: true },
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(this.replacements())
            })
                .done(function (result) {
                _this.errorMessages(result.Failure ? [result.Failure.Summary] : []);
                if (result.WasSuccessful && !result.Failure)
                    _this.cancel();
            })
                .fail(function (message) {
                var failed = JSON.parse(message.responseText);
                _this.errorMessages([failed.message, failed.fullException]);
            })
                .always(function () { return _this.generating(false); });
        };
        NotificationFromConfigurationViewModel.prototype.cancel = function () {
            ko.postbox.publish(this.gridId + '-pop-panel');
        };
        return NotificationFromConfigurationViewModel;
    }());
    return NotificationFromConfigurationViewModel;
});
