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
define(["require", "exports", "knockout", "modules/bwf-utilities", "sprintf", "modules/grid/bwf-gridRow"], function (require, exports, ko, bwf, sprintfM, GridRow) {
    "use strict";
    var sprintf = sprintfM.sprintf;
    var GridEditableRow = /** @class */ (function (_super) {
        __extends(GridEditableRow, _super);
        function GridEditableRow(params) {
            var _this = _super.call(this, params) || this;
            _this.renderedCells = [];
            _this.onCellRendered = function (position) {
                _this.renderedCells.push(position);
                if (_this.renderedCells.length === _this.columns().length)
                    ko.postbox.publish(sprintf("%s-rendered-row", _this.gridId), _this.row.bwfId);
            };
            _this.row.dirtyRecord = bwf.clone(_this.row.record);
            _this.highlightNewRecord = ko.pureComputed(function () {
                var isNewRecord = _this.row.isNewRecord();
                return isNewRecord && !_this.row.isChangeTrackingDisabled;
            });
            return _this;
        }
        GridEditableRow.prototype.dispose = function () {
            var _this = this;
            _super.prototype.dispose.call(this);
            this.columns().forEach(function (c) {
                var value = _this.row.values[c.path];
                value.isSelected(false);
                value.isValid(true);
            });
        };
        GridEditableRow.prototype.componentForCell = function (c) {
            if (c.metadata.useCustomEditingCell)
                return c.metadata.customEditingCell;
            switch (c.metadata.type) {
                case 'boolean':
                    return 'grid/cells/edit/cc-boolean';
                case 'measure':
                    return 'grid/cells/edit/cc-valueWithUnit';
                case 'link':
                    return 'grid/cells/edit/cc-link';
                case 'image':
                    return 'grid/cells/edit/cc-image';
                case 'date':
                case 'time':
                    return 'grid/cells/edit/cc-time';
                default:
                    if (c.metadata.hasChoice)
                        return 'grid/cells/edit/cc-hasChoice';
                    return 'grid/cells/edit/cc-text';
            }
        };
        return GridEditableRow;
    }(GridRow));
    return GridEditableRow;
});
