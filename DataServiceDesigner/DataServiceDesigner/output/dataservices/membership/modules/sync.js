define(["require", "exports", "options", "knockout", "modules/bwf-help", "modules/bwf-title", "loglevel", "modules/bwf-utilities", "knockout-kendo", "knockout-postbox"], function (require, exports, options, ko, help, title, log, utils) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var SyncViewModel = /** @class */ (function () {
        function SyncViewModel() {
            var _this = this;
            this.toggleHelpText = help.toggleHelpText;
            this.toggleHelp = help.toggleHelp;
            this.showHelp = help.showHelp;
            this.openHelp = help.openHelp;
            this.r = options.resources;
            this.confirmSync = ko.observable(false);
            this.errorMessages = ko.observableArray([]);
            this.doSync = function () {
                _this.errorMessages([]);
                var request = $.ajax({
                    url: options.explorerHostUrl + "/ext/membership/sync",
                    xhrFields: {
                        withCredentials: true
                    },
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json'
                });
                request.done(function (result) {
                    log.debug('sync result: ', result);
                    var message = "Synchronisation of user and role configuration successfully started.";
                    ko.postbox.publish('bwf-transient-notification', message);
                });
                request.fail(function (failure) {
                    log.error(failure);
                    if (failure.status === 403) {
                        _this.errorMessages(['You do not have permission to synchronise user and role configuration.']);
                    }
                    else {
                        var fail = JSON.parse(failure.responseText);
                        if (utils.Results.isMessageSet(fail)) {
                            _this.errorMessages(fail.Messages);
                        }
                        else if (utils.Results.isModelValidation(fail)) {
                            var validations = $.makeArray(fail.PropertyValidations)
                                .filter(function (val) { return val[0] !== '$'; })
                                .concat(fail.ModelValidations);
                            _this.errorMessages(validations);
                        }
                    }
                });
            };
            this.canSync = ko.pureComputed(function () { return _this.confirmSync(); });
            title.setTitle(options.resources['bwf_synchronise_configuration'] + ' | ' + options.resources['bwf_explorer']);
        }
        return SyncViewModel;
    }());
    ko.applyBindings(new SyncViewModel(), document.getElementById('content'));
});
