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
    var GridTotalsDisplayRow = /** @class */ (function (_super) {
        __extends(GridTotalsDisplayRow, _super);
        function GridTotalsDisplayRow(params) {
            var _this = _super.call(this, params) || this;
            var record = _this.row.record;
            _this.showCells = record.aggregations.map(function (a) { return a.Key; });
            if (record.aggregationRowType.Value === "Total") {
                var firstCell = _this.columns()[0].path;
                _this.totalCell = firstCell;
                _this.showCells.push(firstCell);
            }
            else {
                _this.totalCell = record.groupedBy[record.groupedBy.length - 1].Key;
                for (var i = 0; i < _this.columns().length; ++i) {
                    var colPath = _this.columns()[i].path;
                    _this.showCells.push(colPath);
                    if (colPath === _this.totalCell)
                        break;
                }
            }
            return _this;
        }
        GridTotalsDisplayRow.prototype.cssClasses = function (column) {
            return (column.path === this.totalCell) ? "bwf-total-cell" : "";
        };
        GridTotalsDisplayRow.prototype.showCell = function (column) {
            return this.showCells.some(function (c) { return c === column.path; });
        };
        return GridTotalsDisplayRow;
    }(BaseGridDisplayRow));
    return GridTotalsDisplayRow;
});
