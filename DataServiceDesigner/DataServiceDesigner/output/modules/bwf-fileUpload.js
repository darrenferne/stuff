define(["require", "exports", "knockout", "loglevel", "options", "modules/bwf-utilities"], function (require, exports, ko, log, options, bwf) {
    "use strict";
    var FileUpload = /** @class */ (function () {
        function FileUpload(params) {
            var _this = this;
            this.intMaxValue = ((Math.pow(2, 32)) / 2) - 1;
            this.inputId = "file-upload-input-" + window.performance.now().toFixed(4).replace(".", "");
            this.resources = options.resources;
            this.showImage = ko.pureComputed(function () {
                var file = _this.uploadedFile();
                if (!file)
                    return false;
                return file.uploadDone() && file.uploadSuccessful();
            });
            this.noImageToShow = ko.pureComputed(function () {
                var file = _this.uploadedFile();
                if (!file)
                    return true;
                return file.uploadDone() && !file.uploadSuccessful();
            });
            this.imageError = ko.pureComputed(function () {
                var file = _this.uploadedFile();
                if (!file)
                    return false;
                return file.errorMessage() || "";
            });
            this.uploadedFileSize = ko.pureComputed(function () {
                if (_this.uploadedFile() && _this.uploadedFile().fileSize)
                    return _this.getSizeStringFromSize(_this.uploadedFile().fileSize);
                return "";
            });
            this.fileUpload = function (data, e) {
                var file = e.target.files[0];
                if (!file) {
                    _this.inputValue(null);
                    return;
                }
                var uploadedFileObject = {
                    id: "uploaded-file-" + window.performance.now().toFixed(4).replace(".", ""),
                    errorMessage: ko.observable(null),
                    uploadDone: ko.observable(false),
                    result: ko.observable(null),
                    uploadSuccessful: ko.pureComputed(function () { return !!uploadedFileObject.result() && uploadedFileObject.uploadDone(); }),
                    renderedImage: ko.pureComputed(function () { return uploadedFileObject.result() && _this.isImageUpload ?
                        _this.imageToRender(uploadedFileObject.result()) : ""; }),
                    fileSize: file.size,
                    name: file.name
                };
                _this.uploadedFile(uploadedFileObject);
                var isNotImage = function () {
                    uploadedFileObject.errorMessage("The uploaded file is not an image.");
                    uploadedFileObject.uploadDone(true);
                    _this.applyResult(null);
                    _this.inputValue(null);
                };
                if (_this.isImageUpload && !file.type.match(/image\//i)) {
                    isNotImage();
                    return;
                }
                if (file.size > _this.sizeLimit) {
                    log.warn("Uploaded file is too large. File size is " + _this.getSizeStringFromSize(file.size));
                    uploadedFileObject.errorMessage("File is too large (limit is " + _this.getSizeStringFromSize(_this.sizeLimit) + ") " +
                        ("but file is " + _this.getSizeStringFromSize(file.size)));
                    uploadedFileObject.uploadDone(true);
                    _this.applyResult(null);
                    _this.inputValue(null);
                    return;
                }
                var reader = new FileReader();
                reader.onloadstart = function (onloadstart_e) {
                    uploadedFileObject.uploadDone(false);
                };
                reader.onloadend = function (onloadend_e) {
                    var result = reader.result;
                    if (_this.isImageUpload && !bwf.isValidImage(result)) {
                        isNotImage();
                        return;
                    }
                    _this.property(result);
                    uploadedFileObject.result(result);
                    uploadedFileObject.uploadDone(true);
                    _this.applyResult(result);
                };
                var errorFunc = function (onerror_e) {
                    log.warn("File upload abort/error: ", onerror_e.message);
                    uploadedFileObject.errorMessage(onerror_e.message);
                    uploadedFileObject.uploadDone(true);
                    _this.applyResult(null);
                };
                reader.onabort = errorFunc;
                reader.onerror = errorFunc;
                reader.readAsDataURL(file);
            };
            this.uploadedFile = ko.observable(null);
            this.inputValue = ko.observable([]);
            this.property = params.propertyObservable;
            this.isImageUpload = params.isImageUpload;
            this.showBorder = params.showBorder || false;
            this.acceptFileTypes = ko.observable(null);
            // Size limit is in bytes - we have to limit to int max value for SQL server
            this.sizeLimit = params.sizeLimit || this.intMaxValue;
            if (!params.acceptFileTypes && this.isImageUpload)
                this.acceptFileTypes("image/*");
            else if (params.acceptFileTypes)
                this.acceptFileTypes(params.acceptFileTypes);
            this.disable = ko.pureComputed(function () {
                if (params.disable)
                    return params.disable();
                return false;
            });
            if (this.property()) {
                this.uploadedFile(this.createUploadedFileObjectFromPreviouslyUploadedItem(this.property()));
            }
        }
        FileUpload.prototype.removeItem = function () {
            this.inputValue(null);
            this.uploadedFile(null);
            this.applyResult(null);
        };
        FileUpload.prototype.createUploadedFileObjectFromPreviouslyUploadedItem = function (f) {
            var item = {
                id: "uploaded-file-" + window.performance.now().toFixed(4).replace(".", ""),
                errorMessage: ko.observable(null),
                fileSize: f.length,
                name: null,
                result: ko.observable(f),
                uploadSuccessful: ko.observable(true),
                renderedImage: this.imageToRender(f),
                uploadDone: ko.observable(true)
            };
            return item;
        };
        FileUpload.prototype.imageToRender = function (image) {
            if (this.isImageUpload) {
                var imgString = image;
                if (imgString.indexOf("data:image") > -1) {
                    return imgString;
                }
                else {
                    return "data:image/png;base64," + imgString;
                }
            }
            return "";
        };
        ;
        FileUpload.prototype.getSizeStringFromSize = function (size) {
            var numberInMegabytes = size / 1024 / 1024;
            var numberString = numberInMegabytes.toFixed(3);
            return numberString + "MB";
        };
        FileUpload.prototype.applyResult = function (result) {
            if (!result)
                this.property(null);
            else
                this.property(result);
        };
        return FileUpload;
    }());
    return FileUpload;
});
