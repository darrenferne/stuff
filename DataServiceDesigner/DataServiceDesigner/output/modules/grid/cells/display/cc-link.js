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
define(["require", "exports", "knockout", "modules/grid/cells/cell-base"], function (require, exports, ko, cell) {
    "use strict";
    var DisplayLinkCell = /** @class */ (function (_super) {
        __extends(DisplayLinkCell, _super);
        function DisplayLinkCell(params) {
            var _this = _super.call(this, params) || this;
            _this.isSelected = params.row.selected;
            _this.value = ko.computed(function () {
                var v = _this.recordValue();
                if (v != null)
                    return v;
                return { Text: '', Value: '' };
            });
            return _this;
        }
        DisplayLinkCell.prototype.getClipboardValue = function () {
            var value = this.value();
            return value == null ? '' : value.Value;
        };
        return DisplayLinkCell;
    }(cell.BwfWrapperCell));
    return DisplayLinkCell;
});
