define(["require", "exports", "knockout"], function (require, exports, knockout) {
    "use strict";
    var BooleanProperty = /** @class */ (function () {
        function BooleanProperty(params) {
            this.rendered = knockout.observable(false);
            this.id = window.performance.now();
            this.value = params.model.observables[params.metadata.name];
            this.label = params.metadata.editingName;
            this.formDisabled = ko.pureComputed(function () {
                return params.model.observables['formDisabled']()
                    || (params.model.state.isCreate && params.metadata.isDisabledInCreateMode)
                    || (!params.model.state.isCreate && params.metadata.isDisabledInEditMode);
            });
            params.model.observables['__renderedState'].push(this.rendered);
        }
        return BooleanProperty;
    }());
    return BooleanProperty;
});
