define(["require", "exports", "knockout", "options", "modules/bwf-metadata", "modules/bwf-explorer"], function (require, exports, ko, options, metadataService, explorer) {
    "use strict";
    var SelectedRecipients = /** @class */ (function () {
        function SelectedRecipients(params) {
            var _this = this;
            this.availableRecipients = ko.pureComputed(function () {
                var all = _this.allRecipients();
                if (_this.selectedRecipients == null)
                    return [];
                var selected = _this.selectedRecipients();
                var available = all.filter(function (r) {
                    return !selected.some(function (x) { return x.Recipient.Source == r.Recipient.Source && x.Recipient.SourceId == r.Recipient.SourceId; });
                });
                return available;
            });
            this.allRecipients = ko.observableArray([]);
            this.ready = ko.observable(false);
            this.rendered = ko.observable(false);
            this.subscriptions = [];
            this.canClear = ko.pureComputed(function () { return _this.ready() && _this.selectedRecipients().length > 0; });
            this.canDelete = ko.pureComputed(function () { return _this.ready() && _this.selectedGrid.selectedRecords().length > 0; });
            this.canSelect = ko.pureComputed(function () { return _this.ready() && _this.availableGrid.selectedRecords().length > 0; });
            this.deleteSelected = function () {
                var toDelete = _this.selectedGrid.selectedRecords().map(function (r) { return r.record; });
                var toKeep = _this.selectedRecipients().filter(function (c) { return !toDelete.some(function (d) { return d.Recipient.Source == c.Recipient.Source && d.Recipient.SourceId == c.Recipient.SourceId; }); });
                _this.selectedRecipients(toKeep);
            };
            this.clear = function () { return _this.selectedRecipients([]); };
            this.r = options.resources;
            this.formDisabled = params.model.observables['formDisabled'];
            this.gridId = params.grid;
            this.selectedRecipients = params.model.observables[params.metadata.name];
            var getMetadata = metadataService.getType('explorer', 'NotificationRecipient');
            getMetadata.done(function (md) {
                _this.setupGrids(md);
            });
            var query = $.ajax({
                url: options.explorerHostUrl + '/api/explorer/query/Recipients',
                xhrFields: {
                    withCredentials: true
                }
            });
            query.done(function (result) {
                var allRecipients = result.Records.map(function (x) {
                    return { Recipient: x };
                });
                _this.allRecipients(allRecipients);
            });
            this.subscriptions.push({ dispose: query.abort });
            params.model.observables['__renderedState'].push(this.ready);
        }
        SelectedRecipients.prototype.setupGrids = function (metadata) {
            var _this = this;
            var columns = ["Recipient/FullName", "Recipient/Source", "Recipient/SourceId"];
            var displayNames = { "Recipient/FullName": options.resources["{{bwf_fullname}}"], "Recipient/Source": options.resources["{{bwf_source}}"], "Recipient/SourceId": options.resources["{{bwf_sourceid}}"] };
            var availableGrid = explorer.generateReadOnlyGridConfiguration(columns, displayNames, this.gridId + '-availableRecipients', metadata, this.formDisabled);
            var selectedGrid = explorer.generateReadOnlyGridConfiguration(columns, displayNames, this.gridId + '-selectedRecipients', metadata, this.formDisabled);
            this.availableGrid = availableGrid.configuration;
            this.selectedGrid = selectedGrid.configuration;
            this.subscriptions.push(this.availableRecipients.subscribe(function (av) { return availableGrid.setRecords(av); }));
            this.subscriptions.push(this.selectedRecipients.subscribe(function (sr) { return selectedGrid.setRecords(sr); }));
            selectedGrid.setRecords(this.selectedRecipients());
            this.select = function () {
                var selected = availableGrid.configuration.selectedRecords().map(function (r) { return r.record; });
                ko.utils.arrayPushAll(_this.selectedRecipients, selected);
            };
            this.ready(true);
        };
        SelectedRecipients.prototype.dispose = function () {
            this.subscriptions.forEach(function (x) {
                if (x != null && typeof x.dispose == 'function')
                    x.dispose();
            });
        };
        return SelectedRecipients;
    }());
    return SelectedRecipients;
});
