define(["require", "exports", "knockout", "options", "sprintf", "modules/bwf-datetimeUtilities"], function (require, exports, knockout, options, sprintf, datetimeUtils) {
    "use strict";
    var DateComponent = /** @class */ (function () {
        function DateComponent(params) {
            var _this = this;
            this.format = options.dateDisplayFormat;
            this.rendered = knockout.observable(false);
            this.elementId = sprintf.sprintf("%s-%s-date-picker", params.metadata.name, window.performance.now().toFixed(0));
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
            else if (options.dateDisplayFormat)
                this.format = options.dateDisplayFormat;
            else
                this.format = params.metadata.defaultFormat;
            this.value = knockout.pureComputed({
                read: function () {
                    var v = _this.property();
                    if (!v)
                        return null;
                    var parsedValue = kendo.toString(kendo.parseDate(v), 'yyyy-MM-ddTHH:mm:sszzz', options.formattingCulture);
                    var convertedValue = datetimeUtils.convertToTZ(parsedValue, 'UTC', 'YYYY-MM-DDTHH:mm:ss');
                    return kendo.parseDate(convertedValue);
                },
                write: function (newValue) {
                    _this.property(kendo.toString(newValue, 'yyyy-MM-ddT00:00:00Z', options.formattingCulture));
                }
            });
            params.model.observables['__renderedState'].push(this.rendered);
        }
        DateComponent.prototype.dispose = function () {
            this.value.dispose();
            $(document.getElementById(this.elementId)).data('kendoDatePicker').destroy();
        };
        return DateComponent;
    }());
    return DateComponent;
});
