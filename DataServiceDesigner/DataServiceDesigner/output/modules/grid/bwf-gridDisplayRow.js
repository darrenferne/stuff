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
define(["require", "exports", "modules/grid/bwf-gridBaseDisplayRow"], function (require, exports, BaseGridDisplayRow) {
    "use strict";
    var GridDisplayRow = /** @class */ (function (_super) {
        __extends(GridDisplayRow, _super);
        function GridDisplayRow(params) {
            var _this = _super.call(this, params) || this;
            // this makes sure that if the user enters edit mode, makes changes, but
            // does not save those changes then we go back to showing the proper 
            // state of the record, instead of whatever junk they wanted to forget
            params.columns().forEach(function (column) {
                var value = _this.row.values[column.path];
                value(column.accessor(_this.row));
            });
            return _this;
        }
        return GridDisplayRow;
    }(BaseGridDisplayRow));
    return GridDisplayRow;
});
