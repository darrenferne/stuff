define(["require", "exports", "knockout", "modules/bwf-bindingHandlers", "modules/bwf-utilities"], function (require, exports, knockout, bindingHandlers, utils) {
    "use strict";
    var bh = bindingHandlers;
    var ImageControl = /** @class */ (function () {
        function ImageControl(params) {
            var _this = this;
            this.sizeLimit = 1 * 1024 * 1024; // 1mb;
            this.name = params.metadata.name;
            this.propertyMetadata = params.metadata;
            this.label = params.metadata.editingName;
            this.property = params.model.observables[this.propertyMetadata.name];
            this.fileUploadProperty = ko.observable(this.property());
            this.formDisabled = ko.pureComputed(function () {
                return params.model.observables['formDisabled']()
                    || (params.model.state.isCreate && params.metadata.isDisabledInCreateMode)
                    || (!params.model.state.isCreate && params.metadata.isDisabledInEditMode);
            });
            this.loaded = knockout.observable(false);
            this.rendered = knockout.observable(false);
            this.ready = knockout.pureComputed(function () { return _this.rendered() && _this.loaded(); });
            params.model.observables['__renderedState'].push(this.ready);
            this.disableSave = params.model.observables['customControlDisableSave'];
            this.loaded(true);
            this.fileUploadProperty.subscribe(function (x) {
                if (x && utils.isValidImage(x)) {
                    if (x.indexOf("data:image/") > -1) {
                        var item = x.split(",")[1];
                        _this.property(item);
                    }
                    else {
                        _this.property(x);
                    }
                }
                else {
                    _this.property(null);
                }
            });
        }
        ImageControl.prototype.fileUploadComponentParams = function () {
            return {
                isImageUpload: true,
                propertyObservable: this.fileUploadProperty,
                sizeLimit: this.sizeLimit,
                showBorder: true,
                disable: this.formDisabled
            };
        };
        return ImageControl;
    }());
    return ImageControl;
});
