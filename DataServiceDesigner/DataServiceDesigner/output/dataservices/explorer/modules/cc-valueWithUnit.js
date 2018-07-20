define(["require", "exports", "knockout", "modules/bwf-gridUtilities", "sprintf"], function (require, exports, knockout, gridUtilities, sprintfM) {
    "use strict";
    var sprintf = sprintfM.sprintf;
    var ValueWithUnitModel = /** @class */ (function () {
        function ValueWithUnitModel(params) {
            var _this = this;
            this.rendered = knockout.observable(false);
            this.name = params.metadata.name;
            this.propertyMetadata = params.metadata;
            this.label = params.metadata.editingName;
            // we setup up the isValid observable on the
            // property in bwf-panel-editor component
            this.property = params.model.observables[this.propertyMetadata.name];
            this.formDisabled = ko.pureComputed(function () {
                return params.model.observables['formDisabled']()
                    || (params.model.state.isCreate && params.metadata.isDisabledInCreateMode)
                    || (!params.model.state.isCreate && params.metadata.isDisabledInEditMode);
            });
            this.validationMessage = params.model.validations.messages[this.name];
            this.inputValue = ko.observable(this.property() ?
                this.property().Value + this.property().Unit : "");
            // validaton message with a line break in the middle
            var validationMessage = sprintf("'%s' needs to consist of a number, optionally%s" +
                "followed by a unit that does not contain numbers.", this.label, String.fromCharCode(10));
            this.value = knockout.computed({
                read: function () {
                    return _this.inputValue();
                },
                write: function (newValue) {
                    // we don't want to change the input as it would be quite jarring
                    // and it can remove leading 0s as it is parsing the number etc.
                    _this.inputValue(newValue);
                    if (newValue == "") {
                        _this.property(null);
                        _this.validationMessage('');
                        _this.property.isValid(_this.propertyMetadata.isNullable);
                        if (!_this.property.isValid())
                            _this.validationMessage(sprintf("'%s' requires a value", _this.label));
                        return;
                    }
                    var value = gridUtilities.parseValueWithUnit(newValue);
                    if (value) {
                        _this.property.isValid(true);
                        _this.property(value);
                        _this.validationMessage("");
                    }
                    else {
                        _this.property.isValid(false);
                        _this.validationMessage(validationMessage);
                    }
                }
            });
            params.model.observables['__renderedState'].push(this.rendered);
        }
        ValueWithUnitModel.prototype.dispose = function () {
            this.value.dispose();
        };
        return ValueWithUnitModel;
    }());
    return ValueWithUnitModel;
});
