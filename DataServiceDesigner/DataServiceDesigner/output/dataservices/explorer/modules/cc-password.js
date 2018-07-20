define(["require", "exports", "knockout", "options"], function (require, exports, knockout, options) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var PasswordControl = /** @class */ (function () {
        function PasswordControl(params) {
            var _this = this;
            this.rendered = knockout.observable(false);
            this.name = params.metadata.name;
            this.propertyMetadata = params.metadata;
            this.passwordParams = JSON.parse(this.propertyMetadata.customControlParameter);
            this.isConfirmationRequired = this.passwordParams != null ? !!this.passwordParams.isConfirmationRequired : false;
            this.confirmationLabel = options.resources["bwf_confirm_new_password"];
            this.property = params.model.observables[this.propertyMetadata.name];
            this.property("");
            this.propertyConfirmation = ko.observable("");
            this.validationMessage = params.model.validations.messages[this.name];
            //If confirmation is required then run a check
            if (this.isConfirmationRequired)
                this.isValid = ko.computed(function () {
                    if (_this.property() !== _this.propertyConfirmation())
                        _this.validationMessage(options.resources["bwf_passwords_do_not_match"]);
                    else
                        _this.validationMessage("");
                });
            this.showConfirmation = ko.pureComputed(function () {
                return _this.isConfirmationRequired && !_this.formDisabled();
            });
            this.formDisabled = ko.pureComputed(function () {
                return params.model.observables["formDisabled"]()
                    || (params.model.state.isCreate && params.metadata.isDisabledInCreateMode)
                    || (!params.model.state.isCreate && params.metadata.isDisabledInEditMode);
            });
            params.model.observables["__renderedState"].push(this.rendered);
        }
        return PasswordControl;
    }());
    exports.default = PasswordControl;
});
