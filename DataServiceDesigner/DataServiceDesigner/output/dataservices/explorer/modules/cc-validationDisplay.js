define(["require", "exports", "knockout"], function (require, exports, knockout) {
    "use strict";
    var ValidationDisplay = /** @class */ (function () {
        function ValidationDisplay(params) {
            var _this = this;
            this.name = params.metadata.name;
            this.property = params.model.observables[this.name];
            this.validation = params.model.validations.messages[this.name];
            this.message = knockout.pureComputed(function () {
                var text = _this.validation();
                if (_this.property.isValid && !text) {
                    text = _this.property.isValid() ? '' : _this.property.validationMessage;
                }
                return text;
            });
        }
        return ValidationDisplay;
    }());
    return ValidationDisplay;
});
