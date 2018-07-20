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
define(["require", "exports", "modules/grid/cells/cell-base"], function (require, exports, cell) {
    "use strict";
    var EditableBooleanCell = /** @class */ (function (_super) {
        __extends(EditableBooleanCell, _super);
        function EditableBooleanCell(params) {
            return _super.call(this, params, true) || this;
        }
        EditableBooleanCell.prototype.getClipboardValue = function () {
            return this.value() ? 'TRUE' : 'FALSE';
        };
        return EditableBooleanCell;
    }(cell.BwfCell));
    return EditableBooleanCell;
});
