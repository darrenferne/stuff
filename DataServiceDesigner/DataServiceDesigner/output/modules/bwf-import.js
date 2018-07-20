define(["require", "exports", "knockout", "options", "modules/bwf-help", "loglevel", "sprintf"], function (require, exports, ko, options, help, log, sprintf) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var BaseTypesRequired;
    (function (BaseTypesRequired) {
        BaseTypesRequired[BaseTypesRequired["None"] = 0] = "None";
        BaseTypesRequired[BaseTypesRequired["One"] = 1] = "One";
        BaseTypesRequired[BaseTypesRequired["OneOrMore"] = 2] = "OneOrMore";
    })(BaseTypesRequired = exports.BaseTypesRequired || (exports.BaseTypesRequired = {}));
    var ImportViewModel = /** @class */ (function () {
        function ImportViewModel(params) {
            var _this = this;
            this.subscriptions = [];
            this.errorMessages = ko.observableArray([]);
            this.openHelp = help.openHelp;
            this.showHelp = help.showHelp;
            this.toggleHelp = help.toggleHelp;
            this.toggleHelpText = help.toggleHelpText;
            this.fileUploaded = ko.observable(false);
            this.selectedFile = ko.observable('');
            this.onFileTypeSelected = function (fileType) {
                if (fileType != null && fileType != '') {
                    var valid = _this.availableFileTypes().some(function (ft) { return ft.DisplayName == fileType; });
                    if (valid)
                        _this.getAvailableTypes();
                }
            };
            this.importInProgress = ko.observable(false);
            this.importNotInProgress = ko.pureComputed(function () { return !_this.importInProgress(); });
            this.selectedFileName = ko.pureComputed(function () {
                if (_this.selectedFile() == '')
                    return '';
                var splitValue = _this.selectedFile().split(/[\\/]/);
                return splitValue[splitValue.length - 1];
            });
            this.availableFileTypes = ko.observableArray([]);
            this.selectedFileTypeValue = ko.observable();
            this.selectedFileType = ko.pureComputed(function () {
                var value = _this.selectedFileTypeValue();
                var fileTypes = _this.availableFileTypes();
                if (value == '' || value == null || fileTypes == null)
                    return undefined;
                var index = -1;
                fileTypes.forEach(function (v, i) { return index = v.DisplayName == value ? i : index; });
                return index == -1
                    ? undefined
                    : fileTypes[index];
            });
            this.availableBaseTypes = ko.observableArray([]);
            this.selectedBaseTypes = ko.observableArray([]);
            this.selectedBaseType = ko.pureComputed(function () {
                if (_this.selectedBaseTypes().length == 1)
                    return _this.selectedBaseTypes()[0];
                return undefined;
            });
            this.getFile = function () {
                var element = $('#' + _this.uploadId)[0];
                if (element != null && element.files != null && element.files.length > 0)
                    return element.files[0];
                else
                    return null;
            };
            this.getAvailableTypes = function () {
                var request = $.ajax({
                    url: _this.dataServiceUrl + '/availabletypes',
                    xhrFields: { withCredentials: true },
                    type: "GET"
                });
                request.done(function (result) {
                    var availTypes = result.filter(function (x) {
                        return x.AllowsExport && x.SupportedImportFormats.indexOf(_this.selectedFileType().DisplayName) > -1;
                    });
                    _this.availableBaseTypes(availTypes);
                });
                _this.subscriptions.push({ dispose: request.abort });
            };
            this.endsWith = function (str, suffix) {
                return str.indexOf(suffix, str.length - suffix.length) !== -1;
            };
            this.onFileUpload = function () {
                var file = _this.getFile();
                if (file) {
                    _this.reset();
                    _this.fileUploaded(true);
                }
                else {
                    _this.fileUploaded(false);
                }
            };
            this.showFileBaseTypeSingle = ko.pureComputed(function () {
                var selFileType = _this.selectedFileType();
                if (selFileType != undefined && selFileType != null)
                    return selFileType.NumberOfBaseTypesRequired == BaseTypesRequired.One;
                return false;
            });
            this.canImport = ko.pureComputed(function () {
                var valid = false;
                try {
                    var typesRequired = _this.selectedFileType().NumberOfBaseTypesRequired;
                    var validNone = typesRequired == BaseTypesRequired.None;
                    var validSingle = typesRequired == BaseTypesRequired.One && _this.selectedBaseType() != undefined;
                    var validMany = typesRequired == BaseTypesRequired.OneOrMore && _this.selectedBaseTypes().length > 0;
                    valid = validNone || validSingle || validMany;
                }
                catch (ex) {
                    return false;
                }
                return _this.fileUploaded() && valid && !_this.importInProgress();
            });
            this.importData = function () {
                log.debug('Importing data');
                _this.importInProgress(true);
                var formData = new FormData();
                var fileType = _this.selectedFileType();
                formData.append('fileType', fileType.DisplayName);
                formData.append('file', _this.getFile());
                if (_this.selectedBaseType() != null && fileType.NumberOfBaseTypesRequired == BaseTypesRequired.One)
                    formData.append('baseType', _this.selectedBaseType().Type);
                else if (_this.selectedBaseTypes() != null && fileType.NumberOfBaseTypesRequired == BaseTypesRequired.OneOrMore) {
                    ko.postbox.publish('bwf-transient-notification', {
                        message: "Imports requiring multiple BaseTypes are not currently supported.",
                        requireDismissal: true,
                        styleClass: "alert-danger"
                    });
                    return;
                }
                var request = $.ajax({
                    url: _this.dataServiceUrl + '/import',
                    xhrFields: {
                        withCredentials: true
                    },
                    type: 'POST',
                    dataType: 'json',
                    data: formData,
                    cache: false,
                    contentType: false,
                    processData: false
                });
                request.done(function (result) {
                    log.debug('Import result', result);
                    _this.errorMessages([]);
                    _this.importInProgress(false);
                    if (result.ErrorMessages.length === 0) {
                        _this.clearAndHidePane();
                    }
                    else {
                        _this.errorMessages(result.ErrorMessages);
                    }
                    ;
                });
                request.fail(function (result) {
                    var responseObj = JSON.parse(result.responseText);
                    _this.importInProgress(false);
                    _this.errorMessages([responseObj.message]);
                });
                _this.subscriptions.push({ dispose: request.abort });
            };
            this.cancel = function () {
                log.debug('Cancel import');
                _this.clearAndHidePane();
            };
            this.reset = function () { return _this.errorMessages([]); };
            this.clearAndHidePane = function () {
                _this.reset();
                ko.postbox.publish(_this.hidePane, []);
            };
            this.dataServiceUrl = params.state.dataServiceUrl;
            this.viewGridId = params.state.gridId;
            this.hidePane = this.viewGridId + '-hidePane';
            this.resources = options.resources;
            this.uploadId = this.viewGridId + '-upload-form';
            this.title = ko.observable(sprintf.sprintf("%s into data service '%s'", this.resources['bwf_import'], params.state.dataService));
            this.subscriptions.push(this.selectedFileType.subscribe(this.onFileTypeSelected));
            var request = $.ajax({
                url: this.dataServiceUrl + '/availableimporttypes',
                xhrFields: { withCredentials: true },
                type: "GET"
            });
            request.done(this.availableFileTypes);
            this.subscriptions.push({ dispose: request.abort });
        }
        ImportViewModel.prototype.dispose = function () {
            this.subscriptions.forEach(function (s) { return s.dispose(); });
        };
        return ImportViewModel;
    }());
    exports.default = ImportViewModel;
});
