define(["require", "exports", "knockout"], function (require, exports, knockout) {
    "use strict";
    var StringControl = /** @class */ (function () {
        function StringControl(params) {
            this.rendered = knockout.observable(false);
            this.height = 1;
            this.name = params.metadata.name;
            this.propertyMetadata = params.metadata;
            this.label = params.metadata.editingName;
            this.property = params.model.observables[this.propertyMetadata.name];
            this.formDisabled = ko.pureComputed(function () {
                return params.model.observables['formDisabled']()
                    || (params.model.state.isCreate && params.metadata.isDisabledInCreateMode)
                    || (!params.model.state.isCreate && params.metadata.isDisabledInEditMode);
            });
            this.height = this.propertyMetadata.heightInLines || 1;
            params.model.observables['__renderedState'].push(this.rendered);
        }
        return StringControl;
    }());
    return StringControl;
});
