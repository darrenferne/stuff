define(["require", "exports", "options", "knockout", "modules/bwf-help", "loglevel", "modules/bwf-metadata", "modules/bwf-title", "text", "bootstrap", "knockout-kendo", "knockout-postbox"], function (require, exports, options, ko, help, log, metadata, title) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var aboutViewModel = /** @class */ (function () {
        function aboutViewModel() {
            var _this = this;
            this.toggleHelpText = help.toggleHelpText;
            this.toggleHelp = help.toggleHelp;
            this.showHelp = help.showHelp;
            this.openHelp = help.openHelp;
            this.resources = options.resources;
            this.pageTitle = options.resources['bwf_about'] + ' | ' + options.resources['bwf_explorer'];
            this.showSpinner = ko.observable(true);
            this.checkingText = options.resources['bwf_checking'] + '...';
            this.activeText = options.resources['bwf_active'];
            this.noResponseText = options.resources['bwf_no_response'];
            title.setTitle(this.pageTitle);
            this.availableDataservices = ko.observableArray([]);
            var allDataServices = metadata.getAllDataServicesSafely();
            allDataServices.done(function (x) {
                var sortedDataServices = x.sort(function (a, b) {
                    var left = a.name;
                    var right = b.name;
                    return left > right ? 1
                        : right > left ? -1
                            : 0;
                });
                _this.showSpinner(false);
                sortedDataServices.forEach(function (x) {
                    var vds = { Name: x.name, Status: _this.checkingText, Version: '' };
                    _this.availableDataservices.push(vds);
                    $.ajax({
                        url: x.url + '/version',
                        xhrFields: {
                            withCredentials: true
                        },
                        dataType: 'json',
                        contentType: 'application/json'
                    }).done(function (result) {
                        var versionText = [result.Major, result.Minor, result.Build, result.Revision].join('.');
                        var vdsNew = { Name: x.name, Status: _this.activeText, Version: versionText };
                        _this.availableDataservices.replace(vds, vdsNew);
                    }).fail(function (error) {
                        var vdsNew = { Name: x.name, Status: _this.noResponseText, Version: '' };
                        _this.availableDataservices.replace(vds, vdsNew);
                        log.error("Error retrieving data service version for " + x.name, error);
                        ko.postbox.publish("bwf-transient-notification", {
                            message: 'Error retrieving data service version for ' + x.name,
                            styleClass: 'alert-danger'
                        });
                    });
                });
            });
        }
        return aboutViewModel;
    }());
    var viewModel = new aboutViewModel();
    ko.applyBindings(viewModel, document.getElementById("content"));
});
