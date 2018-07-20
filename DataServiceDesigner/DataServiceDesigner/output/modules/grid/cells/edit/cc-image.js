var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "knockout", "modules/grid/cells/cell-base", "modules/bwf-utilities"], function (require, exports, ko, cell, utils) {
    "use strict";
    var DisplayImageCell = /** @class */ (function (_super) {
        __extends(DisplayImageCell, _super);
        function DisplayImageCell(params) {
            var _this = _super.call(this, params, true) || this;
            _this.removeImage = function () {
                _this.recordValue(null);
                _this.inputValue(null);
            };
            _this.fileUpload = function (data, e) {
                var sizeLimit = 1 * 1024 * 1024;
                var file = e.target.files[0];
                if (!file)
                    return;
                if (file.size > sizeLimit) {
                    ko.postbox.publish("bwf-transient-notification", {
                        message: "Image is too large. Size limit is 1MB.",
                        styleClass: "alert-warning"
                    });
                    _this.recordValue(null);
                    _this.inputValue(null);
                    return;
                }
                var reader = new FileReader();
                reader.onloadstart = function (onloadstart) {
                    _this.isUploading(true);
                };
                reader.onloadend = function (onloadend_e) {
                    var result = reader.result;
                    var isValidImage = utils.isValidImage(result);
                    if (result && !isValidImage) {
                        ko.postbox.publish("bwf-transient-notification", {
                            message: "Uploaded file was not an image.",
                            styleClass: "alert-danger",
                            requireDismissal: true
                        });
                        _this.inputValue(null);
                        _this.recordValue(null);
                    }
                    else if (result && isValidImage) {
                        if (result.indexOf("data:image/") > -1) {
                            var item = result.split(",")[1];
                            _this.recordValue(item);
                        }
                        else {
                            _this.recordValue(result);
                        }
                    }
                    else {
                        _this.inputValue(null);
                        _this.recordValue(null);
                    }
                    _this.isUploading(false);
                };
                reader.onerror = function (onerror_e) {
                    _this.isUploading(false);
                    ko.postbox.publish("bwf-transient-notification", {
                        message: "Error occurred uploading image: " + onerror_e.message,
                        styleClass: "alert-danger",
                        requireDismissal: true
                    });
                };
                reader.onabort = reader.onerror;
                reader.readAsDataURL(file);
            };
            _this.isUploading = ko.observable(false);
            _this.inputValue = ko.observable(null);
            _this.inputUniqueId = "image-input-" + window.performance.now().toFixed(3).replace(".", "");
            _this.hasImageSet = ko.pureComputed(function () {
                return !(_this.value() == null || _this.value() == '');
            });
            _this.value = ko.pureComputed(function () {
                var image = _this.recordValue();
                if (!image)
                    return null;
                if (image.indexOf("data:image") > -1)
                    return image;
                return "data:image/png;base64," + image;
            });
            return _this;
        }
        // we can't put the image in the clipboard string as it is too large
        // and makes selection very slow. Also a base64 image hardly ever makes sense
        DisplayImageCell.prototype.getClipboardValue = function () {
            return '';
        };
        return DisplayImageCell;
    }(cell.BwfWrapperCell));
    return DisplayImageCell;
});
