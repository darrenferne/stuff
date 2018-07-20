define(["require", "exports"], function (require, exports) {
    "use strict";
    var DummyParameterViewModel = /** @class */ (function () {
        function DummyParameterViewModel(params) {
            var _this = this;
            this.includeEmpty = function () { return true; };
            this.resetToPrevious = function () { return void 0; };
            this.savePreviousValues = function () { return void 0; };
            this.selectedValues = function () { return []; };
            this.postRender = function () { return params.ready(_this); };
        }
        return DummyParameterViewModel;
    }());
    return DummyParameterViewModel;
});
