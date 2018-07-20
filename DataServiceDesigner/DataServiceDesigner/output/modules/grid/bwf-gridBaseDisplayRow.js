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
define(["require", "exports", "modules/grid/bwf-gridRow"], function (require, exports, GridRow) {
    "use strict";
    var BaseGridDisplayRow = /** @class */ (function (_super) {
        __extends(BaseGridDisplayRow, _super);
        function BaseGridDisplayRow(params) {
            var _this = _super.call(this, params) || this;
            _this.highlight = params.highlight;
            _this.validationInDisplayMode = params.validationInDisplayMode || false;
            return _this;
        }
        BaseGridDisplayRow.prototype.componentForCell = function (column) {
            if (column.metadata.useCustomDisplayCell)
                return column.metadata.customDisplayCell;
            switch (column.metadata.type) {
                case 'download':
                    return 'grid/cells/display/cc-download';
                case 'link':
                    return 'grid/cells/display/cc-link';
                case 'boolean':
                    return 'grid/cells/display/cc-boolean';
                case 'image':
                    return 'grid/cells/display/cc-image';
                default:
                    return 'grid/cells/display/cc-text';
            }
        };
        return BaseGridDisplayRow;
    }(GridRow));
    return BaseGridDisplayRow;
});
