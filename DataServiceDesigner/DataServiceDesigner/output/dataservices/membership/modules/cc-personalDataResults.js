define(["require", "exports", "knockout", "options", "sprintf", "modules/bwf-explorer", "modules/bwf-metadata"], function (require, exports, knockout, options, sprintf, explorer, metadataService) {
    "use strict";
    var PersonalDataResultsControl = /** @class */ (function () {
        function PersonalDataResultsControl(params) {
            var _this = this;
            this.rendered = knockout.observable(false);
            this.hasErasePermission = ko.observable(false);
            this.columns = knockout.observableArray([]);
            this.records = knockout.observableArray([]);
            this.selectedIds = knockout.observableArray([]);
            this.selectedRecords = knockout.pureComputed(function () { return _this.records().filter(function (r) { return r.selected(); }); });
            this.enableButtons = knockout.pureComputed(function () { return _this.selectedRecords().length >= 1; });
            this.eraseVisible = knockout.pureComputed(function () { return _this.taskStatus().toLowerCase() == "querysuccessful" && _this.hasErasePermission(); });
            this.retryVisible = knockout.pureComputed(function () { return _this.taskStatus().toLowerCase() == "erasurefailed" && _this.hasErasePermission(); });
            this.r = options.resources;
            this.name = params.metadata.name;
            this.propertyMetadata = params.metadata;
            this.showRecordCount = params["showRecordCount"];
            this.label = ko.pureComputed(function () {
                if (_this.showRecordCount && _this.property && Array.isArray(_this.property())) {
                    var length = _this.property().length;
                    return params.metadata.editingName + " (" + length + " records)";
                }
                return params.metadata.editingName;
            });
            this.metadata = knockout.observable(null);
            this.gridId = params.grid;
            this.checkHasErasePermission();
            this.isCopyAction = knockout.pureComputed(function () { return params.model.state.action === "copy"; });
            this.property = params.model.observables[this.propertyMetadata.name];
            if (params.model.observables[this.propertyMetadata.name + '-selectedIds'])
                this.selectedIds = params.model.observables[this.propertyMetadata.name + '-selectedIds'];
            else
                params.model.observables[this.propertyMetadata.name + '-selectedIds'] = this.selectedIds;
            this.taskStatus = params.model.observables["Status"];
            this.formDisabled = ko.pureComputed(function () {
                return params.model.observables['formDisabled']()
                    || (params.model.state.isCreate && params.metadata.isDisabledInCreateMode)
                    || (!params.model.state.isCreate && params.metadata.isDisabledInEditMode);
            });
            params.model.observables['__renderedState'].push(this.rendered);
            knockout.postbox.publish(params.grid + '-togglePanelWidth', true);
            metadataService.getType(params.model.state.dataService, params.metadata._clrType)
                .done(function (metadata) {
                _this.metadata(metadata);
                _this.configureGrid(metadata);
            });
        }
        PersonalDataResultsControl.prototype.dispose = function () {
            if (this.propertySubscription)
                this.propertySubscription.dispose();
        };
        PersonalDataResultsControl.prototype.checkHasErasePermission = function () {
            var _this = this;
            var dataService = metadataService.getDataService("membership");
            $.ajax({
                url: dataService.hostUrl + "/authorisation/haspermission/membership/PersonalData/Erase",
                xhrFields: {
                    withCredentials: true
                }
            }).done(function () { return _this.hasErasePermission(true); })
                .fail(function () { return _this.hasErasePermission(false); });
        };
        PersonalDataResultsControl.prototype.configureGrid = function (metadata) {
            var _this = this;
            var columns = (metadata.identificationSummaryFields ? metadata.identificationSummaryFields : []).map(function (field, index) {
                var property = metadataService.getPropertyWithPrefix(metadata.dataService, metadata, field);
                return new explorer.ExplorerGridColumn(property, field, index);
            });
            this.propertySubscription = this.property.subscribe(function (x) { return _this.mapPropertyToRecords(columns, metadata); });
            this.mapPropertyToRecords(columns, metadata);
            this.columns(columns);
        };
        PersonalDataResultsControl.prototype.mapPropertyToRecords = function (columns, metadata) {
            var records = (this.property() ? this.property() : []).map(function (record, index) {
                var gridItem = new explorer.ExplorerGridItem({
                    Id: record.Id,
                    Position: index,
                    Data: record,
                    BaseTypeName: metadata.type,
                    TypeName: metadata.type
                }, columns, metadata.dataService);
                return gridItem;
            });
            var selectedIds = this.selectedIds() ? this.selectedIds() : [];
            if (selectedIds.length > 0) {
                var recordsToSelect = records.filter(function (r) { return selectedIds.some(function (s) { return s == r.id; }); });
                recordsToSelect.forEach(function (r) { return r.selected(true); });
            }
            this.records(records);
        };
        PersonalDataResultsControl.prototype.gridConfiguration = function () {
            var _this = this;
            var gridId = sprintf.sprintf("cc-readonlycollection-%s-%s", this.propertyMetadata.fullName, window.performance.now().toFixed(4).replace(".", ""));
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
        PersonalDataResultsControl.prototype.downloadData = function () {
            var ds = metadataService.getDataService(this.propertyMetadata.dataService);
            var downloadData = [];
            this.selectedRecords().forEach(function (r) {
                downloadData.push({
                    Component: r.record.Component,
                    Type: r.record.Type,
                    Data: JSON.parse(r.record.Data),
                });
            });
            var a = document.createElement('a');
            var blob = new Blob([JSON.stringify(downloadData, null, '\t')], { 'type': 'application/octet-stream' });
            a.href = window.URL.createObjectURL(blob);
            a.download = 'personalData.json';
            a.click();
        };
        PersonalDataResultsControl.prototype.requestErasure = function () {
            var ds = metadataService.getDataService(this.propertyMetadata.dataService);
            var baseType = this.propertyMetadata._clrType;
            var selectedIds = this.selectedRecords().map(function (record, index) { return record.id; });
            this.selectedIds(selectedIds);
            var payload = {
                action: '',
                component: 'ds-membership-cc-erasureWarning',
                baseType: baseType,
                dataService: this.propertyMetadata.dataService,
                dataServiceUrl: ds.hostUrl,
                preserveStack: true,
                data: this.selectedRecords().map(function (record, index) { return record.record; }),
                metadata: this.metadata(),
                onCompletion: function (params) { return; }
            };
            knockout.postbox.publish(this.gridId + '-doAction', payload);
        };
        PersonalDataResultsControl.prototype.retryErasure = function () {
            var ds = metadataService.getDataService(this.propertyMetadata.dataService);
            var baseType = this.propertyMetadata._clrType;
            var payload = {
                action: '',
                component: 'ds-membership-cc-erasureWarning',
                baseType: baseType,
                dataService: this.propertyMetadata.dataService,
                dataServiceUrl: ds.hostUrl,
                preserveStack: true,
                data: this.records().map(function (record, index) { return record.record; }).filter(function (r) { return r.EraseRequested == true && r.Error != ""; }),
                metadata: this.metadata(),
                onCompletion: function (params) { return; }
            };
            knockout.postbox.publish(this.gridId + '-doAction', payload);
        };
        return PersonalDataResultsControl;
    }());
    return PersonalDataResultsControl;
});
