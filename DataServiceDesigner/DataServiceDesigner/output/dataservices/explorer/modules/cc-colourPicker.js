define(["require", "exports", "options", "sprintf", "knockout", "knockout-kendo", "kendo.all.min"], function (require, exports, options, sprintf, knockout, koKendo, kendo) {
    "use strict";
    var k = kendo;
    var kkendo = koKendo;
    var ColourPicker = /** @class */ (function () {
        function ColourPicker(params) {
            this.r = options.resources;
            this.rendered = knockout.observable(false);
            params.model.observables['__renderedState'].push(this.rendered);
            this.value = params.model.observables[params.metadata.name];
            this.label = params.metadata.displayName;
            this.formDisabled = ko.pureComputed(function () {
                return params.model.observables['formDisabled']()
                    || (params.model.state.isCreate && params.metadata.isDisabledInCreateMode)
                    || (!params.model.state.isCreate && params.metadata.isDisabledInEditMode);
            });
            this.elementId = sprintf.sprintf("%s-%s-colourPicker", params.metadata.name, window.performance.now().toFixed(0));
        }
        ColourPicker.prototype.afterRender = function () {
            this.rendered(true);
        };
        ColourPicker.prototype.dispose = function () {
            var thisElement = $(document.getElementById(this.elementId));
            if (thisElement) {
                var colorPicker = thisElement.data('kendoColorPicker');
                if (colorPicker)
                    colorPicker.destroy();
            }
        };
        return ColourPicker;
    }());
    return ColourPicker;
});
