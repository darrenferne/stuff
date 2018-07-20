define(["require", "exports", "options", "sprintf", "knockout", "modules/bwf-help", "modules/bwf-utilities"], function (require, exports, options, sprintf_m, ko, help, utils) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var resources = options.resources;
    var sprintf = sprintf_m.sprintf;
    var Dimensions = /** @class */ (function () {
        function Dimensions() {
            this.height = ko.observable(0);
            this.width = ko.observable(0);
        }
        return Dimensions;
    }());
    var ErrorMessage = /** @class */ (function () {
        function ErrorMessage() {
            this.heading = ko.observable('');
            this.body = ko.observable('');
            this.isVisible = ko.observable(false);
        }
        ErrorMessage.prototype.showError = function (heading, body) {
            this.heading(heading);
            this.body(body);
            this.isVisible(true);
        };
        ErrorMessage.prototype.hideError = function () {
            this.isVisible(false);
            this.heading('');
            this.body('');
        };
        return ErrorMessage;
    }());
    exports.ErrorMessage = ErrorMessage;
    var Paging = /** @class */ (function () {
        function Paging(viewGrid) {
            var _this = this;
            this.canGoBackward = ko.pureComputed(function () { return _this.currentPage() > 1; });
            this.canGoForward = ko.pureComputed(function () { return _this.currentPage() < _this.pages(); });
            this.currentPage = ko.pureComputed(function () { return Math.floor(_this.skip() / _this.top()) + 1; });
            this.skip = ko.observable(0);
            this.top = ko.observable(50);
            this.firstPage = function () { return _this.skip(0); };
            this.lastPage = function () { return _this.skip((_this.pages() - 1) * _this.top()); };
            this.nextPage = function () {
                if (_this.canGoForward())
                    _this.skip(_this.skip() + _this.top());
            };
            this.previousPage = function () {
                if (_this.canGoBackward()) {
                    var newSkip = _this.skip() - _this.top();
                    _this.skip(newSkip < 0 ? 0 : newSkip);
                }
            };
            this.reset = function () {
                _this.skip(0);
                _this.top(50);
            };
            this.description = ko.pureComputed(function () {
                var total = viewGrid.totalCount();
                var skip = _this.skip();
                var top = _this.top();
                var inEditMode = viewGrid.flags.inEditMode();
                if (skip < 0) {
                    _this.skip(0);
                    skip = 0;
                }
                if (inEditMode)
                    return sprintf('%d %s', viewGrid.recordsCount(), resources['bwf_records']);
                if (skip >= total)
                    return 'No records for current page';
                if (top >= total)
                    return sprintf('%d %s', total, resources['bwf_records']);
                return sprintf("%d to %d of %d", skip + 1, (skip + top <= total ? skip + top : total), total);
            });
            this.hasManyPages = ko.pureComputed(function () { return viewGrid.totalCount() > _this.top(); });
            this.pages = ko.pureComputed(function () { return Math.ceil(viewGrid.totalCount() / _this.top()); });
        }
        return Paging;
    }());
    var Flags = /** @class */ (function () {
        function Flags(moduleData) {
            this.applyInProgress = ko.observable(false);
            this.autoUpdatesEnabled = ko.observable(true);
            this.disableGridSorting = ko.observable(false);
            this.enableQuerying = ko.observable(false);
            this.exportInProgress = ko.observable(false);
            this.pasteInProgress = ko.observable(false);
            this.inEditMode = ko.observable(false);
            this.loadingView = ko.observable(true);
            this.navigatingAway = false;
            this.panelVisible = ko.observable(false);
            this.parameterBarRendered = ko.observable(false);
            this.querying = ko.observable(false);
            this.rendered = ko.observable(false);
            this.supportsEditMode = ko.observable(false);
            this.widePanel = ko.observable(false);
            this.disableEditMode = ko.observable(!!moduleData.disableEditMode);
            this.disableParamsBar = ko.observable(!!moduleData.disableParamsBar);
            this.isEmbedded = moduleData.isEmbedded != null ? moduleData.isEmbedded : true;
            this.loadTransientNotificationsModule = moduleData.loadTransientNotificationsModule != null ? moduleData.loadTransientNotificationsModule : true;
            this.showParameterBar = ko.observable(moduleData.showParamsBar != null ? moduleData.showParamsBar : true);
            this.showViewTitle = ko.observable(moduleData.showViewTitle != null ? moduleData.showViewTitle : true);
        }
        return Flags;
    }());
    var ViewGridBase = /** @class */ (function () {
        function ViewGridBase(moduleConfig) {
            var _this = this;
            /*** Properties ***/
            this.canSelectIndividualCells = ko.observable(false);
            this.customButtons = ko.observableArray([]);
            this.dataService = ko.observable(null);
            this.dataServiceActions = ko.observableArray([]);
            this.dataServiceUrl = ko.observable('');
            this.errorMessage = new ErrorMessage();
            this.help = help;
            this.metadata = ko.observable(null);
            this.queryExecutor = ko.observable(null);
            this.records = ko.observableArray([]);
            this.recordTypeActions = ko.observableArray([]);
            this.subscriptions = [];
            this.totalCount = ko.observable(0);
            this.urlFilteredBy = ko.observableArray([]);
            this.urlParameters = ko.observableArray([]);
            this.userCanEditViews = ko.observable(options.userCanEditViews);
            this.viewGridDimensions = new Dimensions();
            this.viewId = ko.observable(0);
            this.viewName = ko.observable('');
            /*** Computed Properties ***/
            this.canEditCurrentView = ko.pureComputed(function () {
                var userCanEditViews = _this.userCanEditViews();
                var viewLoaded = !_this.flags.loadingView();
                return userCanEditViews && viewLoaded && !_this.flags.isEmbedded;
            });
            this.enableParameters = ko.pureComputed(function () { return !_this.flags.disableParamsBar(); });
            this.overlayMessage = ko.pureComputed(function () {
                var opening = _this.flags.loadingView();
                var applying = _this.flags.applyInProgress();
                var querying = _this.flags.querying();
                var exporting = _this.flags.exportInProgress();
                _this.flags.pasteInProgress();
                if (opening)
                    return resources['bwf_opening_view_message'];
                if (!opening && querying)
                    return resources['bwf_getting_records'];
                if (applying)
                    return resources['bwf_saving'];
                if (exporting)
                    return resources['bwf_generating_excel'];
                return resources['bwf_paste_in_progress'];
            });
            this.overlayVisible = ko.pureComputed(function () {
                return _this.flags.loadingView() ||
                    _this.flags.applyInProgress() ||
                    _this.flags.querying() ||
                    _this.flags.exportInProgress() ||
                    _this.flags.pasteInProgress();
            });
            this.ready = ko.pureComputed(function () {
                var finishedLoadingView = !_this.flags.loadingView();
                var rendered = _this.flags.rendered();
                return rendered && finishedLoadingView;
            });
            this.recordsCount = ko.pureComputed(function () { return _this.records().filter(function (r) { return r._destroy !== true; }).length; });
            this.selectedRecords = ko.pureComputed(function () {
                if (_this.canSelectIndividualCells()) {
                    return _this.records().filter(function (x) {
                        var filtered = Object.keys(x.values).filter(function (k) {
                            return x.values[k].isInCopyOrPasteGroup() || x.values[k].isSelected();
                        });
                        return filtered.length > 0;
                    });
                }
                else {
                    return _this.records().filter(function (r) { return r.selected(); });
                }
            });
            this.viewGridClass = ko.pureComputed(function () {
                var panelVisble = _this.flags.panelVisible();
                var widePanel = _this.flags.widePanel();
                if (!panelVisble)
                    return 'bwf-viewGrid-grid-wrapper max-height';
                return widePanel
                    ? 'bwf-viewGrid-grid-wrapper max-height wide-panel'
                    : 'bwf-viewGrid-grid-wrapper max-height';
            });
            this.viewGridHeight = ko.pureComputed(function () {
                var height = _this.viewGridDimensions.height();
                return height === 0 ? null : height + 'px';
            });
            this.viewGridWidth = ko.pureComputed(function () {
                var width = _this.viewGridDimensions.width();
                return width === 0 ? null : width + 'px';
            });
            /*** Methods ***/
            this.cancelEditMode = function () {
                _this.clearSelected();
                _this.flags.inEditMode(false);
                ko.postbox.publish(_this.viewGridId + '-resizeRequired');
            };
            this.goToFirstPage = function () {
                _this.paging.firstPage();
            };
            this.refresh = function () {
                _this.flags.enableQuerying(true);
                if (_this.queryExecutor)
                    return _this.queryExecutor().refresh();
            };
            this.reloadView = function (newName) {
                utils.clone(_this.loadViewData).viewName = newName;
                _this.loadView(_this.loadViewData);
            };
            this.showGridLoadError = function (messageText) {
                if (_this.flags.isEmbedded) {
                    _this.errorMessage.showError("Error Loading View", messageText);
                }
                else {
                    window.location.assign(options.explorerHostUrl + "/error/" + messageText);
                }
            };
            this.viewGridResized = function (dimensions) {
                _this.viewGridDimensions.height(dimensions.height);
                _this.viewGridDimensions.width(dimensions.width);
            };
            this.viewGridId = moduleConfig.viewGridId;
            this.flags = new Flags(moduleConfig);
            this.paging = new Paging(this);
        }
        ViewGridBase.prototype.dispose = function () {
            this.subscriptions.forEach(function (s) { return s.dispose(); });
            if (this.queryExecutor()) {
                this.queryExecutor().dispose();
                this.queryExecutor(null);
            }
        };
        ViewGridBase.prototype.clearSelectedCells = function () {
            this.records().forEach(function (r) { return Object.keys(r.values).forEach(function (key, index) {
                var cell = r.values[key];
                cell.isSelected(false);
                cell.isInCopyOrPasteGroup(false);
            }); });
        };
        ViewGridBase.prototype.clearSelectedRows = function () {
            this.selectedRecords().forEach(function (r) { return r.selected(false); });
        };
        ViewGridBase.prototype.clearSelected = function () {
            this.clearSelectedCells();
            this.clearSelectedRows();
        };
        ViewGridBase.prototype.deriveFormat = function (column) {
            if (column.metadata.format != null)
                return column.metadata.format;
            if (column.metadata.type === 'date' && options.dateDisplayFormat) {
                return options.dateDisplayFormat;
            }
            else if (column.metadata.type === 'time' && options.dateTimeDisplayFormat) {
                return options.dateTimeDisplayFormat;
            }
            return column.metadata.defaultFormat;
        };
        ;
        ViewGridBase.prototype.resetView = function (data) {
            this.loadViewData = data;
            this.cancelEditMode();
            this.errorMessage.hideError();
            // Reset State
            this.customButtons([]);
            this.dataService(null);
            this.dataServiceActions([]);
            this.dataServiceUrl(null);
            this.flags.loadingView(true);
            this.flags.querying(true);
            this.flags.panelVisible(false);
            this.flags.parameterBarRendered(false);
            this.metadata(null);
            this.paging.reset();
            this.records([]);
            this.recordTypeActions([]);
            this.userCanEditViews(options.userCanEditViews);
        };
        ViewGridBase.prototype.subscribePostbox = function (topic, action) {
            this.subscriptions.push(ko.postbox.subscribe(topic, action));
        };
        ViewGridBase.prototype.subscribeKnockout = function (observable, action) {
            this.subscriptions.push(observable.subscribe(action));
        };
        ViewGridBase.prototype.setupSubscriptions = function (moduleConfig) {
            var _this = this;
            // Knockout Subscriptions
            this.subscribeKnockout(this.flags.enableQuerying, function (enableQuerying) {
                ko.postbox.publish(_this.viewGridId + "-querying-enabled", enableQuerying);
            });
            this.subscribeKnockout(this.flags.rendered, function (rendered) {
                if (rendered && moduleConfig.gridRendered) {
                    moduleConfig.gridRendered({ records: _this.records, totalCount: _this.totalCount });
                }
                if (_this.flags.disableParamsBar()) {
                    _this.flags.parameterBarRendered(true);
                    if (!_this.flags.enableQuerying())
                        _this.flags.querying(false);
                }
            });
            this.subscribeKnockout(this.flags.parameterBarRendered, function (rendered) {
                if (rendered)
                    ko.postbox.publish(_this.viewGridId + '-resizeRequired');
            });
            this.subscribeKnockout(this.ready, function () {
                ko.postbox.publish(_this.viewGridId + '-resizeRequired');
            });
            this.subscribeKnockout(this.selectedRecords, function (records) {
                ko.postbox.publish(_this.viewGridId + '-recordsSelected', records.map(function (x) { return x.bwfId; }));
            });
            this.subscribeKnockout(this.showParameters, function () {
                ko.postbox.publish(_this.viewGridId + '-resizeRequired');
            });
            this.subscribeKnockout(this.totalCount, function (totalCount) {
                if (_this.paging.skip() >= totalCount) {
                    _this.paging.lastPage();
                }
            });
            // Postbox Subscriptions
            this.subscribePostbox('onbeforeunload', function () { return _this.flags.navigatingAway = true; });
            this.subscribePostbox(this.viewGridId + '-enable-edit-mode', function () { return _this.flags.disableEditMode(false); });
            this.subscribePostbox(this.viewGridId + '-disable-edit-mode', function () { return _this.flags.disableEditMode(true); });
            this.subscribePostbox(this.viewGridId + '-goto-first-page', function () { return _this.goToFirstPage(); });
            this.subscribePostbox(this.viewGridId + '-hidePane', function () { return _this.flags.panelVisible(false); });
            this.subscribePostbox(this.viewGridId + '-hubDisconnected', function () { return _this.showHubDisconnected(); });
            this.subscribePostbox(this.viewGridId + '-hubError', function () { return _this.showHubError(); });
            this.subscribePostbox(this.viewGridId + '-incorrectQueryUrlParams', function () { return _this.showIncorrectQueryUrlParams(); });
            this.subscribePostbox(this.viewGridId + '-isWidePane', function (isWide) { return _this.flags.widePanel(isWide); });
            this.subscribePostbox(this.viewGridId + '-loadView', function (loadViewData) { return _this.loadView(loadViewData); });
            this.subscribePostbox(this.viewGridId + '-removeErrorMessages', function () { return _this.errorMessage.hideError(); });
            this.subscribePostbox(this.viewGridId + '-showPane', function () { return _this.flags.panelVisible(true); });
            this.subscribePostbox(this.viewGridId + '-viewGridResized', function (dimensions) { return _this.viewGridResized(dimensions); });
        };
        ViewGridBase.prototype.showHubDisconnected = function () {
            if (this.flags.navigatingAway)
                return;
            this.errorMessage.showError(resources['bwf_hub_disconnected'], resources['bwf_hub_disconnected_message']);
        };
        ViewGridBase.prototype.showHubError = function () {
            if (this.flags.navigatingAway)
                return;
            this.errorMessage.showError(resources['bwf_hub_error'], resources['bwf_hub_error_text']);
        };
        ViewGridBase.prototype.showIncorrectQueryUrlParams = function () {
            this.errorMessage.showError(resources['bwf_incorrect_query_url_params'], resources['bwf_incorrect_query_url_params_text']);
        };
        return ViewGridBase;
    }());
    exports.ViewGridBase = ViewGridBase;
});
