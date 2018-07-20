define(["require", "exports", "knockout", "options", "sprintf"], function (require, exports, knockout, options, sprintf) {
    "use strict";
    var TimeComponent = /** @class */ (function () {
        function TimeComponent(params) {
            var _this = this;
            this.format = options.dateTimeDisplayFormat;
            this.rendered = knockout.observable(false);
            this.elementId = sprintf.sprintf("%s-%s-dateTime-picker", params.metadata.name, window.performance.now().toFixed(0));
            this.name = params.metadata.name;
            this.propertyMetadata = params.metadata;
            this.label = params.metadata.editingName;
            this.property = params.model.observables[this.propertyMetadata.name];
            this.formDisabled = ko.pureComputed(function () {
                return params.model.observables['formDisabled']()
                    || (params.model.state.isCreate && params.metadata.isDisabledInCreateMode)
                    || (!params.model.state.isCreate && params.metadata.isDisabledInEditMode);
            });
            if (params.metadata.format)
                this.format = params.metadata.format;
            else if (options.dateTimeDisplayFormat)
                this.format = options.dateTimeDisplayFormat;
            else
                this.format = params.metadata.defaultFormat;
            this.value = knockout.pureComputed({
                read: function () {
                    var v = _this.property();
                    if (!v)
                        return null;
                    return kendo.parseDate(v);
                },
                write: function (newValue) {
                    _this.property(kendo.toString(newValue, 'u', options.formattingCulture));
                }
            });
            params.model.observables['__renderedState'].push(this.rendered);
        }
        TimeComponent.prototype.dispose = function () {
            this.value.dispose();
            $(document.getElementById(this.elementId)).data('kendoDateTimePicker').destroy();
        };
        return TimeComponent;
    }());
    return TimeComponent;
});
