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
define(["require", "exports", "knockout", "modules/grid/parameters/parameter-base"], function (require, exports, ko, parameters) {
    "use strict";
    var DropDownParameterViewModel = /** @class */ (function (_super) {
        __extends(DropDownParameterViewModel, _super);
        function DropDownParameterViewModel(params) {
            var _this = _super.call(this, params, parameters.ParameterControlType.kendoDropDownList) || this;
            _this.selectedValue = ko.computed({
                read: function () { return _this.selectedValues()[0]; },
                write: function (value) {
                    if (value != null && value !== '') {
                        _this.selectedValues()[0] = value;
                        _this.selectedValues.valueHasMutated();
                    }
                    else {
                        _this.selectedValues([]);
                    }
                },
                owner: _this
            });
            return _this;
        }
        return DropDownParameterViewModel;
    }(parameters.ParameterViewModel));
    return DropDownParameterViewModel;
});
