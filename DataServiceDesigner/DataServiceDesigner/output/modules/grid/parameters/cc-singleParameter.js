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
    var SingleParameterViewModel = /** @class */ (function (_super) {
        __extends(SingleParameterViewModel, _super);
        function SingleParameterViewModel(params) {
            var _this = _super.call(this, params, parameters.ParameterControlType.kendoMultiSelect) || this;
            _this.isValid = ko.observable(true);
            _this.selectedValue = ko.computed({
                read: function () { return _this.selectedValues()[0]; },
                write: function (value) {
                    var option = _super.prototype.parseValueIntoOption.call(_this, value);
                    if (value != null && value !== '' && option != null) {
                        _this.selectedValues()[0] = value;
                        _this.selectedValues.valueHasMutated();
                        _this.isValid(true);
                    }
                    else {
                        _this.selectedValues([]);
                        var invalid = option == null && value !== '';
                        _this.isValid(!invalid);
                    }
                },
                owner: _this
            });
            return _this;
        }
        SingleParameterViewModel.prototype.addValue = function (value) {
            this.selectedValues([]);
            _super.prototype.addValue.call(this, value);
        };
        return SingleParameterViewModel;
    }(parameters.ParameterViewModel));
    return SingleParameterViewModel;
});
