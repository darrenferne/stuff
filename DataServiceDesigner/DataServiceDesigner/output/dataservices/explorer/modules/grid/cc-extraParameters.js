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
    var EditableExtraParametersCell = /** @class */ (function (_super) {
        __extends(EditableExtraParametersCell, _super);
        function EditableExtraParametersCell(params) {
            var _this = _super.call(this, params, true) || this;
            _this.aggFunction = params.row.values["Function"];
            _this.cannotBeEdited = ko.pureComputed(function () { return !_this.aggFunction() || _this.aggFunction().Value !== "weightedaverage"; });
            _this.isExtraParametersEditable = ko.pureComputed(function () { return _this.isEditable() && !_this.cannotBeEdited(); });
            _this.extraParametersCssClasses = ko.pureComputed(function () {
                var classes = [];
                if (_this.cannotBeEdited() && !_this.gridDisabled())
                    classes.push("disallow-edit");
                classes.push(_this.cssClasses());
                return classes.join(' ');
            });
            return _this;
        }
        return EditableExtraParametersCell;
    }(cell.BwfCell));
    return EditableExtraParametersCell;
});
