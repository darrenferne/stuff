var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "modules/grid/cells/cell-base", "options", "modules/bwf-utilities", "modules/bwf-metadata"], function (require, exports, cell, options, utils, metadataService) {
    "use strict";
    var EditablePropertySelectorCell = /** @class */ (function (_super) {
        __extends(EditablePropertySelectorCell, _super);
        function EditablePropertySelectorCell(params) {
            var _this = _super.call(this, params, true) || this;
            _this.displayValue = ko.pureComputed(function () { var val = _this.value(); return val ? _this.getDisplayName(val) : "[" + options.resources["bwf_none"] + "]"; });
            _this.isPropertyEditable = ko.pureComputed(function () { return _this.isEditable() && _this.aggFunction() === "weightedaverage"; });
            _this.subscriptions = ko.observableArray([]);
            _this.propertyCellCssClasses = ko.pureComputed(function () {
                var classes = [];
                if (!_this.isPropertyEditable() && !_this.gridDisabled())
                    classes.push("disallow-edit");
                classes.push(_this.cssClasses());
                return classes.join(' ');
            });
            _this.onRenderedTemplate = function () {
                var popover = $("#" + _this.popoverId);
                popover.popover({
                    placement: 'right',
                    html: true,
                    content: "<div class=\"bwf-propertySelect-container\" id=\"" + _this.propertySelectorId + "\" data-bind=\"component: { name: 'ds-explorer-cc-availableProperties', params: availablePropertiesData }\"></div>",
                    container: '.bwf-panel-body'
                });
                popover.on('shown.bs.popover', function () {
                    var propertySelector = $("#" + _this.propertySelectorId);
                    ko.applyBindings(_this, propertySelector.get(0));
                });
                _this.onRendered();
            };
            _this.propertySelectorId = "bwf-propertySelect-" + utils.generateGuid();
            _this.popoverId = _this.propertySelectorId + "-popover";
            _this.aggFunction = params.row.values["Function"];
            _this.dataService = ko.observable(params.row.metadata.dataService);
            _this.metadata = ko.observable(params.row.metadata);
            _this.availablePropertiesData = {
                instanceName: _this.propertySelectorId,
                baseType: ko.observable(params.row.metadata.type),
                dataService: _this.dataService,
                formDisabled: params.gridDisabled,
                isCreate: true,
            };
            _this.subscriptions.push(_this.availablePropertySelectedPostboxSubscription());
            return _this;
        }
        EditablePropertySelectorCell.prototype.dispose = function () {
            this.subscriptions().forEach(function (sub) { return sub.dispose(); });
        };
        EditablePropertySelectorCell.prototype.availablePropertySelectedPostboxSubscription = function () {
            var _this = this;
            return ko.postbox.subscribe(this.availablePropertiesData.instanceName + '-cc-available-property-selected', function (item) {
                var popover = $("#" + _this.popoverId);
                _this.value(item.name);
                popover.popover('hide');
            });
        };
        EditablePropertySelectorCell.prototype.getDisplayName = function (propertyName) {
            var metadata = this.metadata();
            if (!metadata || !propertyName)
                return null;
            var isValue = metadataService.isPropertyPathValid(this.dataService(), metadata, propertyName);
            if (!isValue)
                return null;
            var p = metadataService.getPropertyWithPrefix(this.dataService(), metadata, propertyName);
            return p.displayName;
        };
        return EditablePropertySelectorCell;
    }(cell.BwfCell));
    return EditablePropertySelectorCell;
});
