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
define(["require", "exports", "modules/grid/parameters/parameter-base"], function (require, exports, parameters) {
    "use strict";
    var MultiParameterViewModel = /** @class */ (function (_super) {
        __extends(MultiParameterViewModel, _super);
        function MultiParameterViewModel(params) {
            var _this = _super.call(this, params, parameters.ParameterControlType.kendoMultiSelect) || this;
            _this.capacity = _this.isSingle ? 1 : null;
            return _this;
        }
        return MultiParameterViewModel;
    }(parameters.ParameterViewModel));
    return MultiParameterViewModel;
});
