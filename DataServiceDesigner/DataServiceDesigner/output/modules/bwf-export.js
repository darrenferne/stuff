define(["require", "exports", "options", "knockout", "modules/bwf-help", "modules/bwf-metadata", "scripts/loglevel", "scripts/sprintf"], function (require, exports, options, ko, help, metadata, log, sprintf) {
    "use strict";
    var ExportViewModel = /** @class */ (function () {
        function ExportViewModel(params) {
            var _this = this;
            this.title = ko.observable('Export');
            this.exportInProgress = ko.observable(false);
            this.errorMessages = ko.observableArray([]);
            this.availableTypes = ko.observableArray([]);
            this.typesToExport = ko.observableArray([]);
            this.resources = options.resources;
            this.help = help;
            this.exportData = function () {
                _this.exportInProgress(true);
                var url = _this.dataservice.url + "/export/" + _this.typesToExport().join();
                var fileNameToExport = "exported_data";
                $.ajax({
                    url: url,
                    xhrFields: {
                        withCredentials: true
                    },
                    type: 'POST'
                }).done(function (response) {
                    log.debug("Response:", response);
                    var url = sprintf.sprintf("%s/api/generated/%s/%s", _this.dataservice.hostUrl, response, fileNameToExport);
                    $("<iframe style='display: none' src='" + url + "'></iframe>").appendTo("body");
                    _this.clearAndHidePane();
                }).fail(function (error) {
                    _this.exportInProgress(false);
                    ko.postbox.publish("bwf-transient-notification", {
                        message: 'Error posting export to excel request:' + error.responseText,
                        styleClass: 'alert-danger',
                        requireDismissal: true
                    });
                });
            };
            this.enableExport = ko.computed(function () {
                return _this.typesToExport().length !== 0 && !_this.exportInProgress();
            });
            this.cancel = function () {
                _this.clearAndHidePane();
            };
            this.reset = function () {
                _this.exportInProgress(false);
                _this.errorMessages([]);
                _this.availableTypes([]);
                _this.typesToExport([]);
            };
            this.clearAndHidePane = function () {
                _this.reset();
                ko.postbox.publish(_this.hidePanel, []);
            };
            this.viewGridId = params.state.gridId;
            this.exportAction = this.viewGridId + '-action-export';
            this.hidePanel = this.viewGridId + '-hidePane';
            this.dataservice = metadata.getDataService(params.state.dataService);
            $.ajax({
                url: this.dataservice.url + '/AvailableTypes',
                xhrFields: {
                    withCredentials: true
                },
                dataType: 'json',
                contentType: 'application/json'
            }).done(function (result) {
                var exportableTypes = result.filter(function (item) { return item.AllowsExport; });
                var sortedExportableTypes = exportableTypes.sort(function (a, b) {
                    var left = a.PluralisedDisplayName;
                    var right = b.PluralisedDisplayName;
                    return left > right ? 1
                        : right > left ? -1
                            : 0;
                });
                _this.availableTypes(sortedExportableTypes);
            });
        }
        return ExportViewModel;
    }());
    return ExportViewModel;
});
