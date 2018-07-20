define(["require", "exports", "sprintf", "options", "kendo.all.min", "knockout"], function (require, exports, sprintfM, options, kendo, ko) {
    "use strict";
    var sprintf = sprintfM.sprintf;
    var self;
    var KnockoutValidators = /** @class */ (function () {
        function KnockoutValidators() {
            self = this;
        }
        KnockoutValidators.prototype.createValidator = function (validator, defaultMessage) {
            return function (target, validatorOptions) {
                target.isValid = ko.observable(true);
                target.validationMessage = ((!validatorOptions) || (!validatorOptions.message))
                    ? defaultMessage
                    : validatorOptions.message;
                if (target.validationMessages == null)
                    target.validationMessages = ko.observableArray([]);
                function validate(newValue) {
                    var isValid = validator(newValue, validatorOptions);
                    if (isValid) {
                        target.validationMessages.remove(target.validationMessage);
                    }
                    else if (target.isValid()) {
                        target.validationMessages.push(target.validationMessage);
                    }
                    target.isValid(isValid);
                }
                target.subscribe(validate);
                return target;
            };
        };
        KnockoutValidators.prototype.isValidNumeric = function (value, validatorOptions) {
            var pattern = /^-?(?:\d+[.,]?)*(?:e(?:\d+[.,]?)+)?$/;
            // should match any string consisting of numbers, the letter 'e', commas and periods as long as 
            // there are no consecutive letters, commas, or periods. There may optionally be a leading -
            if (validatorOptions && validatorOptions.allowNull && (value == null || value.toString().length == 0)) {
                return true;
            }
            var matches = pattern.exec(value);
            var parsed = kendo.parseFloat(value, options.formattingCulture);
            return parsed !== null && matches !== null && matches.length > 0;
        };
        KnockoutValidators.prototype.isValidInteger = function (value, validatorOptions) {
            var pattern = /^-?(?:\d+[.,]?)*(?:e(?:\d+[.,]?)+)?$/;
            // same as above
            if (validatorOptions && validatorOptions.allowNull && (value == null || value.toString().length == 0)) {
                return true;
            }
            var matches = pattern.exec(value);
            var asFloat = kendo.parseFloat(value, options.formattingCulture);
            var asInt = kendo.parseInt(value, options.formattingCulture);
            return asInt !== null && asInt === asFloat && matches !== null && matches.length > 0;
        };
        KnockoutValidators.prototype.isNotNullOrEmpty = function (value, validatorOptions) {
            return value != null && value.length > 0;
        };
        // date/time formats
        KnockoutValidators.prototype.isValidDateFormat = function (value, validatorOptions) {
            if (!self.isOverallFormatValid(value))
                return false;
            // maybe add some more checks here
            return true;
        };
        KnockoutValidators.prototype.isValidTimeFormat = function (value, validatorOptions) {
            if (!self.isOverallFormatValid(value))
                return false;
            // maybe add some more checks here
            return true;
        };
        KnockoutValidators.prototype.isValidNumericFormat = function (value, validatorOptions) {
            if (!self.isOverallFormatValid(value))
                return false;
            if (validatorOptions.allowNull && !value)
                return true;
            if (value.length == 1)
                return self.checkStandardNumericFormatWithOneCharacter(value);
            // maybe add some more checks here
            return true;
        };
        KnockoutValidators.prototype.isValidIntegerFormat = function (value, validatorOptions) {
            if (!self.isOverallFormatValid(value))
                return false;
            if (validatorOptions.allowNull && !value)
                return true;
            if (value.length == 1)
                return self.checkStandardNumericFormatWithOneCharacter(value);
            // maybe add some more checks here
            return true;
        };
        KnockoutValidators.prototype.isOverallFormatValid = function (value) {
            if (value == null)
                return true;
            return !(value.indexOf("#") > -1 && (value.indexOf("(") > -1 || value.indexOf(")") > -1));
        };
        KnockoutValidators.prototype.checkStandardNumericFormatWithOneCharacter = function (value) {
            return ["n", "c", "p", "e"].indexOf(value.toLowerCase()) >= 0;
        };
        KnockoutValidators.prototype.isValidAlias = function (value, validatorOptions) {
            return value == null || value.indexOf("\\") < 0;
        };
        return KnockoutValidators;
    }());
    var validators = new KnockoutValidators();
    ko.extenders.validNumeric = validators.createValidator(validators.isValidNumeric, "Value must be a valid number");
    ko.extenders.validInteger = validators.createValidator(validators.isValidInteger, "Value must be a valid integer");
    ko.extenders.nonEmptyString = validators.createValidator(validators.isNotNullOrEmpty, "Value must not be null or empty");
    ko.extenders.validAlias = validators.createValidator(validators.isValidAlias, "Value cannot contain a \\");
    ko.extenders.validDateFormat = validators.createValidator(validators.isValidDateFormat, "Value must not be null or empty");
    ko.extenders.validTimeFormat = validators.createValidator(validators.isValidDateFormat, "Value must not be null or empty");
    ko.extenders.validNumericFormat = validators.createValidator(validators.isValidNumericFormat, "Value must not be null or empty");
    ko.extenders.validIntegerFormat = validators.createValidator(validators.isValidIntegerFormat, "Value must not be null or empty");
    return validators;
});
