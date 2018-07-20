define(["require", "exports", "options", "modules/bwf-utilities", "sprintf", "modules/bwf-bindingHandlers", "bootstrapSelect", "scripts/bootstrap-select-binding"], function (require, exports, options, utils, sprintfM, bindingHandlers, bootstrapSelect, bootstrapSelectBinding) {
    "use strict";
    var sprintf = sprintfM.sprintf;
    var bh = bindingHandlers;
    var bss = bootstrapSelect;
    var bsb = bootstrapSelectBinding;
    var SingleTileLinker = /** @class */ (function () {
        function SingleTileLinker(params) {
            this.r = options.resources;
            this.isMobile = utils.isTouchModeEnabled;
            this.rendered = ko.observable(false);
            params.model.observables['__renderedState'].push(this.rendered);
            this.label = params.metadata.displayName;
            this.valueObservable = params.model.observables[params.metadata.name];
            if (!this.valueObservable())
                this.valueObservable(null);
            var dashboard = ko.dataFor(document.getElementById(params.grid)).dashboard();
            var choiceArray = [];
            dashboard.Tiles.forEach(function (x) {
                if (!(params.model.state.action.toLowerCase() === 'edit' && x.Id == params.model.record["Id"]))
                    choiceArray.push(x);
            });
            this.tileChoices = ko.observableArray(choiceArray);
        }
        SingleTileLinker.prototype.onRenderDone = function () {
            this.rendered(true);
        };
        return SingleTileLinker;
    }());
    return SingleTileLinker;
});
