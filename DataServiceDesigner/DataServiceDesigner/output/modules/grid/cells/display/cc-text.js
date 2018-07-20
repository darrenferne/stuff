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
    var DisplayTextCell = /** @class */ (function (_super) {
        __extends(DisplayTextCell, _super);
        function DisplayTextCell(params) {
            var _this = _super.call(this, params) || this;
            _this.value = ko.computed(function () {
                var value = _this.recordValue();
                return _this.column.formatter(value);
            });
            return _this;
        }
        return DisplayTextCell;
    }(cell.BwfWrapperCell));
    return DisplayTextCell;
});
