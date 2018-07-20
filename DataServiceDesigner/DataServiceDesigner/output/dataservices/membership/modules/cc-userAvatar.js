define(["require", "exports", "knockout", "modules/bwf-utilities", "modules/bwf-bindingHandlers"], function (require, exports, knockout, utils) {
    "use strict";
    var AvatarControl = /** @class */ (function () {
        function AvatarControl(params) {
            var _this = this;
            this.avatarTypeProperty = params.model.observables["UserPictureType"];
            if (this.avatarTypeProperty() == null)
                this.avatarTypeProperty("none");
            this.imageProperty = params.model.observables["UserPicture"];
            this.fileUploadProperty = ko.observable(this.imageProperty());
            this.rendered = knockout.observable(false);
            this.formDisabled = ko.pureComputed(function () {
                return params.model.observables["formDisabled"]()
                    || (params.model.state.isCreate && params.metadata.isDisabledInCreateMode)
                    || (!params.model.state.isCreate && params.metadata.isDisabledInEditMode);
            });
            this.avatarTypeDisplayString = ko.pureComputed(function () {
                return _this.getAvatarTypeLabel(_this.avatarTypeProperty());
            });
            this.avatarTypeProperty.subscribe(function (newAvatarType) {
                if (newAvatarType !== "image") {
                    _this.imageProperty("");
                }
            });
            this.fileUploadProperty.subscribe(function (newImage) {
                if (newImage && utils.isValidImage(newImage)) {
                    if (newImage.indexOf("data:image/") > -1) {
                        var item = newImage.split(",")[1];
                        _this.imageProperty(item);
                    }
                    else {
                        _this.imageProperty(newImage);
                    }
                }
                else {
                    _this.imageProperty(null);
                }
            });
        }
        AvatarControl.prototype.getAvatarTypeLabel = function (avatarType) {
            switch (avatarType) {
                case "none":
                    return "No avatar";
                case "gravatar":
                    return "Gravatar";
                case "image":
                    return "Image";
            }
        };
        AvatarControl.prototype.fileUploadComponentParams = function () {
            return {
                isImageUpload: true,
                propertyObservable: this.fileUploadProperty,
                sizeLimit: 1 * 1024 * 1024,
                showBorder: true,
                disable: this.formDisabled
            };
        };
        return AvatarControl;
    }());
    return AvatarControl;
});
