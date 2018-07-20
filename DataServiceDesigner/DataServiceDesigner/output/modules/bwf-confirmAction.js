define(["require", "exports", "knockout", "options"], function (require, exports, ko, options) {
    "use strict";
    var ConfirmActionViewModel = /** @class */ (function () {
        function ConfirmActionViewModel(panelEntity) {
            var _this = this;
            this.running = ko.observable(false);
            this.errorMessages = ko.observableArray();
            this.close = function () { return ko.postbox.publish(_this.gridId + '-pop-panel'); };
            this.yesClick = function () {
                var request = $.ajax({
                    url: _this.actionUrl,
                    xhrFields: { withCredentials: true },
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json'
                });
                _this.running(true);
                request.done(function () {
                    _this.close();
                    ko.postbox.publish("bwf-transient-notification", _this.success);
                });
                request.fail(function (message) {
                    _this.running(false);
                    var failed = JSON.parse(message.responseText);
                    switch ((failed.Type || '').toLowerCase()) {
                        case "messageset":
                            _this.errorMessages(failed.Messages);
                            break;
                        case "modelvalidation":
                            var errors = [];
                            Object.keys(failed.PropertyValidations)
                                .filter(function (key) { return key[0] !== '$'; })
                                .forEach(function (key) {
                                errors.push(failed.PropertyValidations[key]);
                            });
                            errors.push(failed.ModelValidations);
                            _this.errorMessages(errors);
                            break;
                        default:
                            if (failed.message)
                                _this.errorMessages([failed.message, failed.fullException]);
                            else
                                _this.errorMessages([message.responseText]);
                            break;
                    }
                });
            };
            this.noClick = this.close;
            this.gridId = panelEntity.state.gridId;
            var data = panelEntity.record;
            this.actionUrl = data.actionUrl;
            var translate = function (value) { return value ? value.replace(/{{(.*?)}}/g, function (_, key) { return options.resources[key] || key; }) : ""; };
            this.title = translate(data.title);
            this.question = translate(data.question);
            this.warning = translate(data.warning);
            this.success = translate(data.success);
            this.yesCaption = translate(data.yesCaption || "{{yes}}");
            this.noCaption = translate(data.noCaption || "{{no}}");
        }
        return ConfirmActionViewModel;
    }());
    return ConfirmActionViewModel;
});
