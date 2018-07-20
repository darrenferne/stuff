define(["require", "exports", "knockout", "loglevel", "modules/bwf-metadata", "scripts/sprintf", "options", "modules/bwf-explorer"], function (require, exports, knockout, log, metadataService, sprintf, options, explorer) {
    "use strict";
    var ErasureWarningPanel = /** @class */ (function () {
        function ErasureWarningPanel(params) {
            var _this = this;
            this.rendered = knockout.observable(false);
            this.columns = knockout.observableArray([]);
            this.records = knockout.observableArray([]);
            this.selectedRecords = knockout.pureComputed(function () { return _this.records().filter(function (r) { return r.selected(); }); });
            this.confirmDelete = ko.observable(false);
            this.requestInProgress = knockout.observable(false);
            this.errorMessage = ko.observable("");
            this.enableAccept = knockout.pureComputed(function () { return _this.confirmDelete() && !_this.requestInProgress(); });
            this.r = options.resources;
            this.data = params;
            this.metadata = knockout.observable(null);
            this.rendered(true);
            knockout.postbox.publish(params.state.gridId + '-togglePanelWidth', true);
            this.metadata(params.state['metadata']);
            this.configureGrid(this.metadata());
        }
        ErasureWarningPanel.prototype.configureGrid = function (metadata) {
            var columns = (metadata.identificationSummaryFields ? metadata.identificationSummaryFields : []).map(function (field, index) {
                var property = metadataService.getPropertyWithPrefix(metadata.dataService, metadata, field);
                return new explorer.ExplorerGridColumn(property, field, index);
            });
            this.mapPropertyToRecords(columns, metadata);
            this.columns(columns);
        };
        ErasureWarningPanel.prototype.mapPropertyToRecords = function (columns, metadata) {
            var records = (this.data.record ? this.data.record : []).map(function (record, index) {
                var gridItem = new explorer.ExplorerGridItem({
                    Id: record.Id,
                    Position: index,
                    Data: record,
                    BaseTypeName: metadata.type,
                    TypeName: metadata.type
                }, columns, metadata.dataService);
                return gridItem;
            });
            this.records(records);
        };
        ErasureWarningPanel.prototype.gridConfiguration = function () {
            var _this = this;
            var gridId = sprintf.sprintf("cc-erasuredata-recordstoerase-%s", window.performance.now().toFixed(4).replace(".", ""));
            var config = {
                createNewRecord: function () { return null; },
                disableGridSorting: knockout.observable(true),
                header: {
                    enabled: knockout.observable(false),
                    name: null, config: null
                },
                footer: {
                    enabled: knockout.observable(false),
                    name: null, config: null
                },
                isView: false,
                viewGridId: gridId,
                inEditMode: knockout.observable(false),
                metadata: this.metadata,
                records: this.records,
                selectedColumns: knockout.pureComputed(function () { return _this.columns(); }),
                selectedRecords: this.selectedRecords,
                recordsCount: knockout.pureComputed(function () { return _this.selectedRecords().length; }),
                showValidationInDisplayMode: false,
                embedded: true
            };
            return config;
        };
        ErasureWarningPanel.prototype.confirm = function () {
            var _this = this;
            var requestUrl = options.explorerHostUrl + "/ext/membership/personaldata/erase";
            var recordsToErase = (this.data.record ? this.data.record : []).map(function (record, index) { return record.Id; });
            if (recordsToErase.length > 0) {
                this.requestInProgress(true);
                var requestErasure = $.ajax({
                    url: requestUrl,
                    type: 'POST',
                    contentType: 'application/json',
                    xhrFields: { withCredentials: true },
                    data: JSON.stringify({
                        Id: this.data.record[0].PersonalDataTaskId,
                        Data: recordsToErase
                    })
                });
                requestErasure.done(function (result) {
                    _this.requestInProgress(false);
                    _this.errorMessage("");
                    ko.postbox.publish("bwf-transient-notification", result.message);
                    ko.postbox.publish(_this.data.state.gridId + '-hidePane');
                });
                requestErasure.fail(function (error) {
                    _this.requestInProgress(false);
                    _this.errorMessage(error.responseJSON.message);
                    log.error(error.responseJSON.message);
                });
            }
            ko.postbox.publish(this.data.state.gridId + '-pop-panel');
        };
        ErasureWarningPanel.prototype.cancel = function () {
            ko.postbox.publish(this.data.state.gridId + '-pop-panel');
        };
        return ErasureWarningPanel;
    }());
    return ErasureWarningPanel;
});
