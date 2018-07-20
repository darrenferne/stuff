define(["require", "exports", "knockout", "sprintf", "modules/bwf-explorer", "modules/bwf-metadata"], function (require, exports, knockout, sprintf, explorer, metadataService) {
    "use strict";
    var ReadOnlyCollectionControl = /** @class */ (function () {
        function ReadOnlyCollectionControl(params) {
            var _this = this;
            this.rendered = knockout.observable(false);
            this.columns = knockout.observableArray([]);
            this.records = knockout.observableArray([]);
            this.selectedRecords = knockout.pureComputed(function () { return _this.records().filter(function (r) { return r.selected(); }); });
            this.enableViewItemButton = knockout.pureComputed(function () { return _this.selectedRecords().length === 1; });
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
            this.isCopyAction = knockout.pureComputed(function () { return params.model.state.action === "copy"; });
            this.property = params.model.observables[this.propertyMetadata.name];
            this.formDisabled = ko.pureComputed(function () {
                return params.model.observables['formDisabled']()
                    || (params.model.state.isCreate && params.metadata.isDisabledInCreateMode)
                    || (!params.model.state.isCreate && params.metadata.isDisabledInEditMode);
            });
            params.model.observables['__renderedState'].push(this.rendered);
            metadataService.getType(params.model.state.dataService, params.metadata._clrType)
                .done(function (metadata) {
                _this.metadata(metadata);
                _this.configureGrid(metadata);
            });
        }
        ReadOnlyCollectionControl.prototype.dispose = function () {
            if (this.propertySubscription)
                this.propertySubscription.dispose();
        };
        //#region grid configuration
        ReadOnlyCollectionControl.prototype.configureGrid = function (metadata) {
            var _this = this;
            var columns = (metadata.identificationSummaryFields ? metadata.identificationSummaryFields : []).map(function (field, index) {
                var property = metadataService.getPropertyWithPrefix(metadata.dataService, metadata, field);
                return new explorer.ExplorerGridColumn(property, field, index);
            });
            this.propertySubscription = this.property.subscribe(function (x) { return _this.mapPropertyToRecords(columns, metadata); });
            this.mapPropertyToRecords(columns, metadata);
            this.columns(columns);
        };
        ReadOnlyCollectionControl.prototype.mapPropertyToRecords = function (columns, metadata) {
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
            this.records(records);
        };
        ReadOnlyCollectionControl.prototype.gridConfiguration = function () {
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
        //#endregion
        ReadOnlyCollectionControl.prototype.viewCollectionItem = function () {
            var ds = metadataService.getDataService(this.propertyMetadata.dataService);
            var baseType = this.propertyMetadata._clrType;
            var payload = {
                action: 'view',
                component: 'bwf-panel-editor',
                baseType: baseType,
                dataService: this.propertyMetadata.dataService,
                dataServiceUrl: ds.hostUrl,
                preserveStack: true,
                data: { Id: this.selectedRecords()[0].id },
                onCompletion: function (params) { return; }
            };
            knockout.postbox.publish(this.gridId + '-doAction', payload);
        };
        return ReadOnlyCollectionControl;
    }());
    return ReadOnlyCollectionControl;
});
