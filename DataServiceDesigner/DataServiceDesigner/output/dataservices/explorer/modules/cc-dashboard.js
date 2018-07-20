define(["require", "exports", "options", "scripts/sprintf", "jquery", "knockout", "loglevel", "modules/bwf-utilities", "sammy", "modules/bwf-bindingHandlers", "modules/bwf-metadata"], function (require, exports, optionsImport, sprintfM, $, ko, log, utils, Sammy, bindingHandlers, metadataService) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var bh = bindingHandlers;
    var fullScreen = utils.FullscreenAPI;
    var sprintf = sprintfM.sprintf;
    var options = optionsImport;
    var Dashboard = /** @class */ (function () {
        function Dashboard(params) {
            var _this = this;
            this.resources = options.resources;
            this.addTileText = "Add Tile";
            this.actionsButtons = ko.observableArray([]);
            this.browserSupportsFullscreen = false; //fullScreen.isFullScreenAvailable();
            // disable full screen - see BWF-1447
            this.browserSupportsTouch = false;
            this.isAzureMembershipActive = options.isAzureMembershipActive || false;
            this.areAzureADSettingsPresentInConfig = options.areAzureADSettingsPresentInConfig || false;
            this.subscriptions = ko.observableArray([]);
            this.tileConfig = {
                gridItemWidth: 160,
                gridItemHeight: 120,
                gutterWidth: 5,
                tileContentMarginSize: 6
            };
            this.storage = new utils.LocalStorageWithExpiry(2147483647);
            this.powerBiTokenStorageKey = 'powerbitoken';
            metadataService.getDataServiceSafely('explorer').done(function (x) {
                _this.dataService = ko.observable(x);
            });
            this.isTouchModeEnabled = utils.isTouchModeEnabled;
            this.isTouchModeDisabled = ko.pureComputed(function () { return !_this.isTouchModeEnabled(); });
            this.isLoading = ko.observable(true);
            this.inEditMode = ko.observable(false);
            this.name = params.dashboardName || ko.observable("");
            this.dashboardId = ko.observable(-1);
            this.dashboard = ko.observable(null);
            this.areAnyEditModeChanges = ko.observable(false);
            this.isRefreshing = ko.observable(false);
            this.isLocked = ko.observable(false);
            this.isSaving = ko.observable(false);
            this.hasIgnoreLockPermission = ko.observable(false);
            this.elementId = params.elementId;
            this.panelVisible = ko.observable(false);
            this.isFullscreen = ko.observable(false);
            this.isPanelWide = ko.observable(false);
            this.tiles = ko.observableArray([]);
            this.overlayMessage = ko.pureComputed(function () {
                if (_this.isRefreshing())
                    return _this.resources["bwf_refreshing"] + "...";
                if (_this.isLoading())
                    return _this.resources["bwf_loading"] + "...";
                if (_this.isSaving())
                    return _this.resources["bwf_saving"] + "...";
                return _this.resources["bwf_working"] + "...";
            });
            this.showOverlay = ko.pureComputed(function () { return _this.isRefreshing() || _this.isLoading() || _this.isSaving(); });
            $(document).on('webkitfullscreenchange mozfullscreenchange fullscreenchange MSFullscreenChange', function (event) {
                log.debug(event.type);
                _this.isFullscreen(fullScreen.isAnyElementIsFullScreen());
            });
            this.azureLoggedIn = ko.observable(false);
            this.showAzureADLoginButton = ko.pureComputed(function () {
                return _this.areAzureADSettingsPresentInConfig && !_this.isAzureMembershipActive
                    && !_this.azureLoggedIn() && !_this.isTouchModeEnabled();
            });
            this.showAzureADLogoutButton = ko.pureComputed(function () {
                return _this.areAzureADSettingsPresentInConfig && !_this.isAzureMembershipActive
                    && _this.azureLoggedIn() && !_this.isTouchModeEnabled();
            });
            // when azure membership is active we use keys on the server
            // so we can remove the client azure token
            if (this.isAzureMembershipActive || !this.areAzureADSettingsPresentInConfig) {
                this.storage.removeItem('azuretoken');
            }
            this.subscriptions.push(ko.postbox.subscribe(this.elementId + "-isWidePane", function (val) { return _this.isPanelWide(val); }));
            this.subscriptions.push(ko.postbox.subscribe(this.elementId + "-close-dashboard-panel", function () { return _this.panelVisible(false); }));
            this.subscriptions.push(ko.postbox.subscribe(this.elementId + "-editTile", function (tile) { return _this.editTile(tile); }));
            this.subscriptions.push(ko.postbox.subscribe(this.elementId + "-deleteTile", function (tile) { return _this.deleteTile(tile); }));
            this.subscriptions.push(ko.postbox.subscribe(this.elementId + "-createDashboard", function () { return _this.createDashboard(); }));
            this.subscriptions.push(ko.postbox.subscribe(this.elementId + "-copyDashboard", function () { return _this.copyDashboard(); }));
            this.subscriptions.push(ko.postbox.subscribe(this.elementId + "-refreshDashboard", function () { return _this.refreshDashboard(); }));
            this.subscriptions.push(ko.postbox.subscribe(this.elementId + '-hidePane', function () { return _this.panelVisible(false); }));
            this.subscriptions.push(ko.postbox.subscribe(this.elementId + '-showPane', function () { return _this.panelVisible(true); }));
            this.subscriptions.push(ko.postbox.subscribe(this.elementId + '-dashboard-modified', function () { return _this.areAnyEditModeChanges(true); }));
            this.subscriptions.push(ko.postbox.subscribe(this.elementId + '-login-to-azure', function () { return _this.loginToAzure(); }));
            this.backgroundStyles = ko.computed(function () {
                if (_this.inEditMode()) {
                    var tileConfig = _this.tileConfig;
                    var backgroundImageCss = sprintf("repeating-linear-gradient(180deg,rgba(0,0,0,0.1),rgba(0,0,0,0.1) 1px, transparent 1px, transparent %dpx)," +
                        "repeating-linear-gradient(-90deg, rgba(0,0,0,0.1), rgba(0,0,0,0.1) 1px, transparent 1px, transparent %dpx)", tileConfig.gridItemHeight, tileConfig.gridItemWidth);
                    return {
                        'backgroundImage': backgroundImageCss,
                        'backgroundSize': sprintf("%dpx %dpx", tileConfig.gridItemWidth + tileConfig.gutterWidth, tileConfig.gridItemHeight + tileConfig.gutterWidth),
                        'backgroundPositionY': tileConfig.gutterWidth - 1 + "px"
                    };
                }
                else {
                    return {
                        'backgroundImage': 'none'
                    };
                }
            });
            this.showCombinedViewHeaders = ko.observable(false);
            this.subscriptions.push(ko.postbox.subscribe("bwf-grid-addPageEvents", function () { return _this.showCombinedViewHeaders(true); }));
            this.subscriptions.push(this.isTouchModeEnabled.subscribe(function (t) {
                ko["tasks"].schedule(function () { return _this.getTileViewModels().forEach(function (x) { return x.onResize(); }); });
                if (_this.inEditMode())
                    _this.areAnyEditModeChanges(true);
            }));
            this.getTileTypes(function (tileTypes) { return _this.loadDashboard(tileTypes); });
            this.canCreate = ko.pureComputed(function () { return !_this.inEditMode() && options.userCanCreateDashboards && !_this.isTouchModeEnabled(); });
            this.canCopy = ko.pureComputed(function () { return !_this.inEditMode() && options.userCanCreateDashboards && !_this.isTouchModeEnabled(); });
            this.canToggleEditMode = ko.pureComputed(function () { return !_this.inEditMode() && options.userCanEditDashboards && !_this.isTouchModeEnabled(); });
            this.editButtonText = ko.pureComputed(function () { return _this.inEditMode() ? _this.resources["bwf_exit_edit_mode"] : _this.resources["bwf_edit"]; });
            this.fullscreenButtonText = ko.pureComputed(function () { return _this.isFullscreen() ? _this.resources["bwf_exit_fullscreen"] : _this.resources["bwf_fullscreen"]; });
            this.actionsButtons.push({
                Icon: "fa fa-plus",
                DisplayName: this.resources["bwf_create"],
                Explanation: this.resources["bwf_create"],
                OnClick: function () { return _this.createDashboard(); },
                Visible: this.canCreate
            });
            this.actionsButtons.push({
                Icon: "fa fa-pencil-square-o",
                DisplayName: this.editButtonText,
                Explanation: this.editButtonText,
                OnClick: function () { return _this.toggleEditMode(); },
                Visible: this.canToggleEditMode
            });
            this.actionsButtons.push({
                Icon: "fa fa-copy",
                DisplayName: this.resources["bwf_copy"],
                Explanation: this.resources["bwf_copy"],
                OnClick: function () { return _this.copyDashboard(); },
                Visible: this.canCopy
            });
            this.actionsButtons.push({
                Icon: "fa fa-refresh",
                DisplayName: this.resources["bwf_refresh"],
                Explanation: this.resources["bwf_refresh"],
                OnClick: function () { return _this.refreshDashboard(); },
                Visible: this.isTouchModeDisabled
            });
            this.actionsButtons.push({
                Icon: "fa fa-sign-in",
                DisplayName: this.resources["bwf_login_to_power_bi"],
                Explanation: this.resources["bwf_login_to_power_bi"],
                OnClick: function () { return _this.loginToAzure(); },
                Visible: this.showAzureADLoginButton
            });
            this.actionsButtons.push({
                Icon: "fa fa-sign-out",
                DisplayName: this.resources["bwf_logout_of_power_bi"],
                Explanation: this.resources["bwf_logout_of_power_bi"],
                OnClick: function () { return _this.logoutFromAzure(); },
                Visible: this.showAzureADLogoutButton
            });
            this.actionsButtons.push({
                Icon: "fa fa-arrows-alt",
                DisplayName: this.fullscreenButtonText,
                Explanation: this.fullscreenButtonText,
                OnClick: function () { return _this.toggleFullscreen(); },
                Visible: this.browserSupportsFullscreen
            });
        }
        Dashboard.prototype.dispose = function () {
            this.subscriptions().forEach(function (sub) { return sub.dispose(); });
            this.backgroundStyles.dispose();
        };
        Dashboard.prototype.getTileTypes = function (callback) {
            var _this = this;
            $.ajax({
                url: sprintf("%s/tile/types", options.explorerHostUrl),
                xhrFields: { withCredentials: true },
                type: 'GET'
            }).done(function (types) {
                _this.tileTypes = ko.observableArray(types);
                if (typeof callback === 'function') {
                    callback(_this.tileTypes);
                }
            }).fail(function (error) {
                _this.tileTypes = null;
                log.error("Error getting tile types", error);
                ko.postbox.publish("bwf-transient-notification", {
                    message: "Error occurred getting tile types from server.",
                    styleClass: "alert-danger"
                });
            });
        };
        Dashboard.prototype.getTileTypeFromTile = function (tile) {
            var typeName = tile.TileType;
            var type = this.tileTypes().filter(function (x) { return x.typeName == typeName; })[0];
            return type;
        };
        Dashboard.prototype.loginToAzure = function () {
            window.location.assign(options.explorerHostUrl + "/powerbi/login");
        };
        Dashboard.prototype.logoutFromAzure = function () {
            window.location.assign(options.explorerHostUrl + "/powerbi/logout");
        };
        Dashboard.prototype.redirectToCurrentDashboardFromLocalStorage = function () {
            var currentDashboardId = this.storage.getItem('currentDashboardId');
            if (currentDashboardId)
                window.location.hash = "#open/" + currentDashboardId;
        };
        Dashboard.prototype.clearAzureLoginAndOpenDashboard = function () {
            this.storage.removeItem(this.powerBiTokenStorageKey);
            this.azureLoggedIn(false);
            this.redirectToCurrentDashboardFromLocalStorage();
        };
        Dashboard.prototype.loadDashboard = function (tileTypes) {
            var _this = this;
            this.sammyApp = Sammy();
            this.sammyApp.get('#azureloggedout', function (context) {
                _this.clearAzureLoginAndOpenDashboard();
            });
            this.sammyApp.get('#nopowerbionaccount', function (context) {
                _this.clearAzureLoginAndOpenDashboard();
                ko.postbox.publish("bwf-transient-notification", {
                    message: _this.resources["bwf_power_bi_not_on_account"],
                    styleClass: "alert-danger",
                    requireDismissal: true,
                });
            });
            this.sammyApp.get('#error/:errorMessage', function (context) {
                var errorMessage = context.params['errorMessage'];
                _this.clearAzureLoginAndOpenDashboard();
                ko.postbox.publish("bwf-transient-notification", {
                    message: errorMessage,
                    styleClass: "alert-danger",
                    requireDismissal: true,
                });
            });
            this.sammyApp.get('#azuretoken/:azureToken/:expiresOn', function (context) {
                var azureToken = context.params['azureToken'];
                var expiresOn = parseInt(context.params['expiresOn'], 10);
                _this.storage.setItem(_this.powerBiTokenStorageKey, azureToken, expiresOn);
                _this.azureLoggedIn(true);
                _this.redirectToCurrentDashboardFromLocalStorage();
            });
            this.sammyApp.get('#open/:dashboardId', function (context) {
                var azureToken = _this.storage.getItem(_this.powerBiTokenStorageKey);
                if (azureToken)
                    _this.azureLoggedIn(true);
                var dashboardId = parseInt(context.params['dashboardId']);
                _this.dashboardId(dashboardId);
                _this.storage.setItem('currentDashboardId', dashboardId.toString());
                var queryUrl = sprintf('%s/api/explorer/query/Dashboards?$filter=Id=%s&$expand=Tiles', options.explorerHostUrl, dashboardId);
                $.ajax({
                    url: queryUrl,
                    xhrFields: { withCredentials: true },
                    type: 'GET'
                }).done(function (data) {
                    if (data.TotalCount === 1) {
                        var dashboard = data['Records'][0];
                        _this.dashboard(dashboard);
                        _this.name(dashboard.Name);
                        _this.tiles(dashboard.Tiles);
                        _this.isLoading(false);
                        // we have to make sure we can edit the tiles after a refresh
                        var tileCount = _this.tiles().length;
                        if (tileCount > 0) {
                            var renderedTileCount = 0;
                            _this.tiles().forEach(function (x) { return _this.subscriptions.push(ko.postbox.subscribe(sprintf('%s-tile-%s-finished-render', _this.elementId, x.Id), function () {
                                renderedTileCount++;
                                if (renderedTileCount >= tileCount) {
                                    _this.isRefreshing(false);
                                }
                            })); });
                        }
                        else {
                            _this.isRefreshing(false);
                        }
                    }
                    else if (data.TotalCount === 0) {
                        var messageText = sprintf("Dashboard with Id of %s could not be found. " +
                            "This may be because it doesn't exist, or you do not have the required permissions to open it.", dashboardId);
                        var redirectUrl = sprintf("%s/error/%s", options.explorerHostUrl, messageText);
                        window.location.assign(redirectUrl);
                    }
                });
            });
            this.sammyApp.run();
        };
        Dashboard.prototype.toggleFullscreen = function () {
            if (this.isFullscreen())
                fullScreen.exitFullScreen();
            else
                fullScreen.requestFullScreen(document.getElementById(this.elementId).parentElement);
        };
        Dashboard.prototype.refreshDashboard = function () {
            this.isRefreshing(true);
            this.isLoading(true);
            this.isLoading.valueHasMutated();
            if (this.areAnyEditModeChanges()) {
                this.tiles([]);
                this.tiles.valueHasMutated();
                this.dashboard(null);
                this.dashboard.valueHasMutated();
            }
            this.sammyApp.refresh();
            this.areAnyEditModeChanges(false);
        };
        Dashboard.prototype.viewDashboard = function (dashboardId) {
            var url = sprintf("%s/dashboard/open/%s", options.explorerHostUrl, dashboardId);
            window.location.assign(url);
        };
        Dashboard.prototype.createDashboard = function () {
            var _this = this;
            this.panelVisible(true);
            var ds = this.dataService();
            var dashboardMetadataPromise = metadataService.getType(ds.name, "Dashboard");
            dashboardMetadataPromise.done(function (metadata) {
                var payload = {
                    action: 'create',
                    component: 'bwf-panel-editor',
                    baseType: 'Dashboard',
                    dataService: ds.name,
                    dataServiceUrl: ds.hostUrl,
                    preserveStack: true,
                    metadata: metadata,
                    onCompletion: function (newDashboard) { return _this.viewDashboard(newDashboard.record.Id); }
                };
                ko.postbox.publish(_this.elementId + '-doAction', payload);
            });
        };
        Dashboard.prototype.copyDashboard = function () {
            var _this = this;
            var dashboard = JSON.parse(JSON.stringify(this.dashboard()));
            this.panelVisible(true);
            var ds = this.dataService();
            var dashboardMetadataPromise = metadataService.getType(ds.name, "Dashboard");
            dashboardMetadataPromise.done(function (metadata) {
                var payload = {
                    action: 'copy',
                    component: 'bwf-panel-editor',
                    baseType: 'Dashboard',
                    dataService: ds.name,
                    dataServiceUrl: ds.hostUrl,
                    preserveStack: true,
                    metadata: metadata,
                    data: dashboard,
                    onCompletion: function (newDashboard) { return _this.viewDashboard(newDashboard.record.Id); }
                };
                ko.postbox.publish(_this.elementId + '-doAction', payload);
            });
        };
        Dashboard.prototype.editModeCancel = function () {
            this.exitEditMode();
            if (this.areAnyEditModeChanges())
                this.refreshDashboard();
            else
                this.getTileViewModels().forEach(function (x) { return x.onResize(); });
        };
        Dashboard.prototype.editModeSave = function () {
            var _this = this;
            this.isSaving(true);
            utils.removeAllSelectedRanges();
            this.saveTileState(function () {
                _this.exitEditMode();
                if (_this.areAnyEditModeChanges())
                    _this.refreshDashboard();
                else
                    _this.getTileViewModels().forEach(function (x) { return x.onResize(); });
            });
        };
        Dashboard.prototype.toggleEditMode = function () {
            if (this.inEditMode()) {
                this.exitEditMode();
            }
            else {
                if (!this.isLocked() || (this.isLocked() && this.hasIgnoreLockPermission())) {
                    this.enterEditMode();
                }
                else {
                    ko.postbox.publish("bwf-transient-notification", {
                        message: "This dashboard is locked.",
                        styleClass: "alert-info"
                    });
                }
            }
        };
        Dashboard.prototype.getTileViewModels = function () {
            var tiles = document.getElementsByClassName('tile');
            var viewModels = [];
            for (var tile = 0; tile < tiles.length; tile++) {
                viewModels.push(ko.dataFor(tiles[tile]));
            }
            return viewModels;
        };
        Dashboard.prototype.enterEditMode = function () {
            this.inEditMode(true);
        };
        Dashboard.prototype.exitEditMode = function () {
            this.inEditMode(false);
            ko.postbox.publish(this.elementId + '-clear-panel');
            this.panelVisible(false);
        };
        Dashboard.prototype.saveTileState = function (callback) {
            var _this = this;
            var tileViewModels = this.getTileViewModels();
            var tiles = tileViewModels.map(function (vm) { return vm.tileObject(); });
            var updatedDashboard = this.dashboard();
            updatedDashboard.Tiles = tiles;
            log.debug("Saved dashboard state", updatedDashboard);
            var dataService = metadataService.getDataServiceSafely('explorer').done(function (ds) {
                var saveUrl = sprintf("%s/%s/%s", ds.url, "Dashboard", updatedDashboard.Id);
                var saveQuery = $.ajax({
                    url: saveUrl,
                    xhrFields: { withCredentials: true },
                    type: 'PUT',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify(updatedDashboard)
                });
                saveQuery.fail(function (x) {
                    var message = "Error occurred saving tile state.";
                    if (x.responseJSON && x.responseJSON.Summary)
                        message = x.responseJSON.Summary;
                    console.warn(x);
                    ko.postbox.publish("bwf-transient-notification", {
                        message: message,
                        styleClass: 'alert-warning'
                    });
                });
                saveQuery.done(function (x) {
                    if (typeof callback === 'function')
                        callback();
                });
                saveQuery.always(function (x) { return _this.isSaving(false); });
            });
        };
        Dashboard.prototype.addTile = function () {
            var _this = this;
            this.panelVisible(true);
            var ds = this.dataService();
            var dashboardMetadataPromise = metadataService.getType(ds.name, "Dashboard");
            dashboardMetadataPromise.done(function (metadata) {
                var payload = {
                    action: 'create',
                    component: 'ds-explorer-cc-tilePanel',
                    baseType: 'Dashboard',
                    dataService: ds.name,
                    dataServiceUrl: ds.hostUrl,
                    preserveStack: true,
                    metadata: metadata,
                    data: _this.dashboard()
                };
                payload['azureLoggedIn'] = _this.azureLoggedIn() || _this.isAzureMembershipActive || false;
                payload['areAzureSettingsPresentInConfig'] = _this.areAzureADSettingsPresentInConfig;
                ko.postbox.publish(_this.elementId + '-doAction', payload);
            });
        };
        Dashboard.prototype.editTile = function (tileToEdit) {
            var _this = this;
            this.panelVisible(true);
            var ds = this.dataService();
            var dashboardMetadataPromise = metadataService.getType(ds.name, "Dashboard");
            dashboardMetadataPromise.done(function (metadata) {
                var payload = {
                    action: 'edit',
                    component: 'ds-explorer-cc-tilePanel',
                    baseType: tileToEdit.TileType,
                    dataService: ds.name,
                    dataServiceUrl: ds.hostUrl,
                    preserveStack: true,
                    metadata: metadata,
                    data: tileToEdit
                };
                payload['azureLoggedIn'] = _this.azureLoggedIn() || _this.isAzureMembershipActive || false;
                payload['areAzureSettingsPresentInConfig'] = _this.areAzureADSettingsPresentInConfig;
                ko.postbox.publish(_this.elementId + '-doAction', payload);
            });
        };
        Dashboard.prototype.deleteTile = function (tileToDelete) {
            var _this = this;
            this.panelVisible(true);
            var ds = this.dataService();
            var tileElement = document.querySelectorAll(sprintf('div[data-tileid="%s"]', tileToDelete.Id))[0];
            var dashboardMetadataPromise = metadataService.getType(ds.name, "Dashboard");
            dashboardMetadataPromise.done(function (metadata) {
                var payload = {
                    action: 'delete',
                    component: 'ds-explorer-cc-tilePanel',
                    baseType: tileToDelete.TileType,
                    dataService: ds.name,
                    dataServiceUrl: ds.url,
                    preserveStack: false,
                    metadata: metadata,
                    parentIsSource: false,
                    customConfirm: function () {
                        _this.tiles.remove(tileToDelete);
                        _this.areAnyEditModeChanges(true);
                        return true;
                    },
                    data: tileToDelete
                };
                ko.postbox.publish(_this.elementId + '-doAction', payload);
            });
        };
        Dashboard.prototype.getTileMetadataPromise = function (type) {
            var url = options.explorerHostUrl + "/tile/metadata";
            if (type)
                url += "/" + type;
            var tileMetadataPromise = $.ajax({
                type: 'GET',
                xhrFields: { withCredentials: true },
                url: url
            });
            return tileMetadataPromise;
        };
        Dashboard.prototype.getIsLockedParams = function () {
            var params = {
                typeMetadata: {
                    displayName: this.resources["bwf_dashboard"],
                },
                model: {
                    observables: {
                        isLocked: this.isLocked,
                        hasIgnoreLock: this.hasIgnoreLockPermission,
                        __renderedState: []
                    },
                    state: {
                        typeName: "Dashboard",
                        dataService: "explorer"
                    },
                    record: this.dashboard()
                },
                permissions: {}
            };
            return params;
        };
        return Dashboard;
    }());
    exports.default = Dashboard;
});
