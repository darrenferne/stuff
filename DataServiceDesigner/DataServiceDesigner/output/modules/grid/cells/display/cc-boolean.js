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
define(["require", "exports", "modules/grid/cells/cell-base"], function (require, exports, base) {
    "use strict";
    var DisplayBooleanCell = /** @class */ (function (_super) {
        __extends(DisplayBooleanCell, _super);
        function DisplayBooleanCell(params) {
            var _this = _super.call(this, params) || this;
            _this.pasteValue = function (value) {
                if (typeof value == 'boolean')
                    _this.value(value);
                if (typeof value == 'string')
                    _this.value(value.toUpperCase() === 'TRUE');
                if (typeof value == 'number')
                    _this.value(value !== 0);
                _this.value(false);
            };
            return _this;
        }
        DisplayBooleanCell.prototype.getClipboardValue = function () {
            return this.value() ? 'TRUE' : 'FALSE';
        };
        return DisplayBooleanCell;
    }(base.BwfCell));
    return DisplayBooleanCell;
});
