define(["require", "exports", "knockout", "options", "modules/bwf-help", "modules/bwf-utilities", "modules/bwf-bindingHandlers"], function (require, exports, ko, options, bwfHelp, utils) {
    "use strict";
    var GridHeader = /** @class */ (function () {
        function GridHeader(config) {
            var _this = this;
            this.r = options.resources;
            this.help = bwfHelp;
            this.insertEnabled = ko.pureComputed(function () { return _this.config.recordsCount() < 150; });
            this.disableSave = ko.pureComputed(function () { return !_this.config.canApplyChanges(); });
            this.showHelpAndNotInEditMode = ko.pureComputed(function () {
                var showHelp = bwfHelp.showHelp();
                var notInEditMode = _this.notInEditMode();
                return showHelp && notInEditMode;
            });
            this.showHelpAndSupportsEditMode = ko.pureComputed(function () {
                var showHelp = bwfHelp.showHelp();
                var supportsEditMode = _this.config.supportsEditMode();
                return showHelp && supportsEditMode;
            });
            this.config = config;
            this.cellSelect = this.config.canSelectIndividualCells;
            this.rowSelect = ko.pureComputed(function () { return !_this.cellSelect(); });
            this.notInEditMode = ko.pureComputed(function () { return !_this.config.inEditMode(); });
            this.showActionButtons = ko.pureComputed(function () { return _this.config.showActionButtons() && _this.notInEditMode(); });
            this.isTouchModeEnabled = utils.isTouchModeEnabled;
            this.isTouchModeDisabled = ko.pureComputed(function () { return !_this.isTouchModeEnabled(); });
        }
        GridHeader.prototype.dispose = function () {
            this.notInEditMode.dispose();
            this.showHelpAndNotInEditMode.dispose();
            this.showHelpAndSupportsEditMode.dispose();
        };
        GridHeader.prototype.setCellSelectMode = function () {
            this.cellSelect(true);
        };
        GridHeader.prototype.setRowSelectMode = function () {
            this.cellSelect(false);
        };
        GridHeader.prototype.selectAll = function () {
            ko.postbox.publish(this.config.viewGridId + '-select-all');
        };
        return GridHeader;
    }());
    return GridHeader;
});
