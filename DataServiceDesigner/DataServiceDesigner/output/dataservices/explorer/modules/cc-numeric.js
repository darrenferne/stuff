define(["require", "exports", "knockout", "options"], function (require, exports, knockout, options) {
    "use strict";
    var NumericControl = /** @class */ (function () {
        function NumericControl(params) {
            var _this = this;
            this.rendered = knockout.observable(false);
            this.name = params.metadata.name;
            this.propertyMetadata = params.metadata;
            this.label = params.metadata.editingName;
            this.property = params.model.observables[this.propertyMetadata.name];
            this.formDisabled = ko.pureComputed(function () {
                return params.model.observables['formDisabled']()
                    || (params.model.state.isCreate && params.metadata.isDisabledInCreateMode)
                    || (!params.model.state.isCreate && params.metadata.isDisabledInEditMode);
            });
            this.value = knockout.pureComputed({
                read: function () {
                    var v = _this.property();
                    if (_this.formDisabled()) {
                        var format = _this.propertyMetadata.format;
                        if (format == null || format === '')
                            format = _this.propertyMetadata.defaultFormat;
                        return kendo.toString(v, format, options.formattingCulture);
                    }
                    return v;
                },
                write: function (newValue) { return _this.property(newValue); }
            });
            params.model.observables['__renderedState'].push(this.rendered);
        }
        return NumericControl;
    }());
    return NumericControl;
});
