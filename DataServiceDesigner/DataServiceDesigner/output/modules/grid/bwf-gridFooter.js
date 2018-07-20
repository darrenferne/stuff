define(["require", "exports", "knockout", "options", "modules/bwf-utilities", "modules/bwf-help"], function (require, exports, ko, options, utils, help) {
    "use strict";
    var ExplorerGridFooter = /** @class */ (function () {
        function ExplorerGridFooter(config) {
            var _this = this;
            this.help = help;
            this.r = options.resources;
            this.toggleUpdatesText = ko.pureComputed(function () {
                return _this.config.autoUpdatesEnabled()
                    ? _this.r['bwf_pause_updates']
                    : _this.r['bwf_resume_updates'];
            });
            this.config = config;
            this.paging = config.paging;
            this.inEditMode = config.inEditMode;
            this.notInEditMode = ko.pureComputed(function () { return !_this.inEditMode(); });
            this.isTouchModeEnabled = utils.isTouchModeEnabled;
            this.isTouchModeDisabled = ko.pureComputed(function () { return !_this.isTouchModeEnabled(); });
            this.disableGridSorting = config.disableGridSorting;
            this.showClearOrderButton = ko.pureComputed(function () { return _this.enableGridSorting(); });
            this.disableClearOrderButton = ko.pureComputed(function () { return _this.inEditMode(); });
            this.enableGridSorting = ko.pureComputed(function () { return !_this.disableGridSorting(); });
        }
        ExplorerGridFooter.prototype.clearOrderBy = function () {
            if (!this.disableGridSorting())
                this.config.orderedBy([]);
        };
        ExplorerGridFooter.prototype.editCurrentView = function () {
            var args = {
                action: 'edit',
                baseType: 'View',
                component: 'bwf-panel-editor',
                data: { Id: this.config.viewId() },
                dataService: this.config.explorerDataService().name,
                dataServiceUrl: this.config.explorerDataService().hostUrl
            };
            ko.postbox.publish(this.config.viewGridId + '-doAction', args);
        };
        ExplorerGridFooter.prototype.openPowerBiPane = function () {
            var args = {
                action: 'powerbi',
                baseType: '',
                component: 'bwf-powerbi',
                data: { Id: this.config.viewId() },
                dataService: this.config.explorerDataService().name,
                dataServiceUrl: this.config.explorerDataService().hostUrl
            };
            ko.postbox.publish(this.config.viewGridId + '-doAction', args);
        };
        ExplorerGridFooter.prototype.toggleAutomaticUpdates = function () {
            this.config.autoUpdatesEnabled(!this.config.autoUpdatesEnabled());
        };
        return ExplorerGridFooter;
    }());
    return ExplorerGridFooter;
});
