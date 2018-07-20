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
    var DisplayImageCell = /** @class */ (function (_super) {
        __extends(DisplayImageCell, _super);
        function DisplayImageCell(params) {
            var _this = _super.call(this, params) || this;
            _this.hasImageSet = ko.pureComputed(function () {
                return !(_this.value() == null || _this.value() == '');
            });
            _this.value = ko.pureComputed(function () {
                var image = _this.recordValue();
                return _this.column.formatter(image);
            });
            _this.valueString = ko.pureComputed(function () { return "data:image/png;base64," + _this.value(); });
            return _this;
        }
        DisplayImageCell.prototype.getClipboardValue = function () {
            return '';
        };
        return DisplayImageCell;
    }(cell.BwfWrapperCell));
    return DisplayImageCell;
});
