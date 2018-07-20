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
define(["require", "exports", "jquery", "options", "modules/bwf-metadata", "loglevel", "modules/bwf-utilities", "modules/bwf-basePanelEditor", "sprintf", "modules/bwf-bindingHandlers", "bootstrapSelect", "scripts/bootstrap-select-binding"], function (require, exports, $, options, metadataService, log, utils, basePanelEditor, sprintfM, bindingHandlers, bootstrapSelect, bootstrapSelectBinding) {
    "use strict";
    var sprintf = sprintfM.sprintf;
    var r = options.resources;
    var bh = bindingHandlers;
    var bss = bootstrapSelect;
    var bsb = bootstrapSelectBinding;
    var TilePanel = /** @class */ (function (_super) {
        __extends(TilePanel, _super);
        function TilePanel(params) {
            var _this = _super.call(this, params) || this;
            _this.r = options.resources;
            _this.currentAction = "";
            _this.resources = options.resources;
            _this.formDisabled = ko.observable(false);
            _this.ready = ko.observable(false);
            _this.editorSubscriptions = [];
            _this.subscriptions = [];
            _this.isMobile = utils.isTouchModeEnabled;
            _this.createObservablesCallback = function (m, c) {
                var properties = _this.properties();
                _this.setupValidation(properties, c);
                if (_this.isCreate()) {
                    var thisTileType = _this.tileTypes().filter(function (x) { return x.typeName == _this.selectedTileType(); })[0];
                    _this.populateObservables(properties, _this.panelEntity, thisTileType.defaultTile);
                    var title = _this.panelEntity.observables["Title"];
                    var existingTilesOfType = _this.dashboard.Tiles.filter(function (x) { return x.TileType == _this.selectedTileType(); });
                    var getDefaultTitle = function () {
                        var displayName = thisTileType.displayName;
                        if (existingTilesOfType.filter(function (y) { return y.Title == displayName; }).length == 0)
                            return displayName;
                        var defaultTitle = "";
                        var incrementer = 1;
                        while (defaultTitle === "") {
                            var newTitle = displayName + " " + incrementer;
                            if (existingTilesOfType.filter(function (t) { return t.Title == newTitle; }).length === 0)
                                defaultTitle = newTitle;
                            else
                                incrementer += 1;
                        }
                        return defaultTitle;
                    };
                    title(getDefaultTitle());
                }
                else if (_this.isEdit())
                    _this.deserialiseTile(_this.data).done(function (data) {
                        _this.populateObservables(properties, _this.panelEntity, data);
                    });
            };
            _this.panelEntity = params;
            _this.currentAction = params.state.action.toLowerCase();
            _this.tileTypes = ko.observableArray([]);
            _this.typePermissions = ko.observable(null);
            _this.updateTypePermissions(params);
            _this.dashboardElementId = params.state.gridId;
            _this.dataService = metadataService.getDataService(params.state.dataService);
            _this.dataServiceUrl = params.state.dataServiceUrl;
            _this.dashboardMetadata = params.state['metadata'];
            _this.selectedTileType = ko.observable(null);
            _this.properties = ko.observable([]);
            _this.dashboardViewModel = ko.dataFor(document.getElementById(_this.dashboardElementId));
            _this.dashboard = _this.dashboardViewModel.dashboard();
            _this.dashboardTiles = _this.dashboardViewModel.tiles;
            _this.loggedInToAzure = params.state['azureLoggedIn'];
            _this.areAzureADSettingsPresentInConfig = params.state['areAzureSettingsPresentInConfig'];
            _this.data = params.state['data'];
            _this.showTileTypePicker = ko.pureComputed(function () { return _this.isCreate() && (_this.tileTypes().length > 0); });
            _this.showMainPanelBody = ko.pureComputed(function () {
                if (_this.isCreate()) {
                    return !!_this.selectedTileType();
                }
                if (_this.ready() && !_this.isDelete()) {
                    return true;
                }
                return false;
            });
            _this.needsAzureLogin = ko.pureComputed(function () {
                if (_this.tileTypes().length > 0 && _this.selectedTileType()) {
                    var tileType = _this.tileTypes().filter(function (x) { return x.typeName == _this.selectedTileType(); })[0];
                    var requireLogin = tileType.requiresAzureLogin;
                    return !_this.loggedInToAzure && requireLogin;
                }
                return false;
            });
            _this.showEditingControls = ko.pureComputed(function () {
                var isSelected = _this.selectedTileType();
                var isDelete = _this.isDelete();
                var needsAzureLogin = _this.needsAzureLogin();
                return (isSelected && !isDelete) && !needsAzureLogin;
            });
            _this.editingControlsRendered = ko.pureComputed(function () { return _this.needsAzureLogin() ? true : _this.rendered(); });
            _this.enableSaveButton = ko.pureComputed(function () {
                var rendered = _this.editingControlsRendered();
                var needsAzure = _this.needsAzureLogin();
                var isDelete = _this.isDelete();
                return (rendered && !needsAzure) || isDelete;
            });
            // setup computeds
            _this.isCreate = ko.pureComputed(function () { return _this.currentAction == 'create'; });
            _this.isEdit = ko.pureComputed(function () { return _this.currentAction == 'edit'; });
            _this.isDelete = ko.pureComputed(function () { return _this.currentAction == 'delete'; });
            if (!(_this.isCreate() || _this.isEdit() || _this.isDelete()))
                log.error("The action has not been set correctly, so behaviour likely will not be as expected");
            _this.panelTitle = ko.pureComputed(function () {
                var type = '';
                if (_this.isCreate())
                    type = r["bwf_create"];
                else if (_this.isEdit())
                    type = r["bwf_edit"];
                else if (_this.isDelete())
                    type = r["bwf_delete"];
                return sprintf('%s %s', type, _this.getDisplayNameFromSelectedTileType() || r["bwf_tile"]);
            });
            _this.confirmButtonText = ko.pureComputed(function () {
                var type = '';
                if (_this.isCreate() || _this.isEdit())
                    type = r["bwf_save"];
                else if (_this.isDelete())
                    type = r["bwf_delete"];
                return type;
            });
            if (!_this.isEdit()) {
                // when editing we have components to load at startup
                // otherwise we can clear the timeout so we don't need it
                clearTimeout(_this.loadPanelTimeout);
            }
            // do the startup tasks, in the right order with callbacks
            _this.getTileTypes(function (tileTypes) {
                if (!_this.isCreate()) {
                    var selectedTileType = tileTypes.filter(function (x) { return params.state.typeName == x.typeName; });
                    if (selectedTileType.length == 1)
                        _this.selectedTileType(selectedTileType[0].typeName);
                    _this.getTileMetadataPromise().done(function (baseMetadata) {
                        _this.baseTileTypeMetadataProperties = _this.processTileMetadata(baseMetadata, true);
                        _this.getTileMetadataPromise(params.state.typeName).done(function (metadata) {
                            _this.afterGetTypePromiseCallback(metadata, true);
                            _this.tileTypeMetadataProperties = _this.processTileMetadata(metadata);
                            _this.afterInitialMetadataRequests();
                        });
                    });
                }
                else {
                    _this.getTileMetadataPromise().done(function (baseMetadata) {
                        _this.baseTileTypeMetadataProperties = _this.processTileMetadata(baseMetadata, true);
                        _this.afterInitialMetadataRequests();
                    });
                }
            });
            return _this;
        }
        TilePanel.prototype.loginToAzure = function () {
            ko.postbox.publish(this.dashboardElementId + '-login-to-azure');
        };
        TilePanel.prototype.afterInitialMetadataRequests = function () {
            var _this = this;
            var promise;
            if (this.isCreate())
                promise = $.Deferred().resolve(this.panelEntity.record).promise();
            else
                promise = this.deserialiseTile(this.data);
            promise.done(function (data) {
                _this.panelEntity.record = data;
                _this.ready(true);
                _this.subscriptions.push(_this.selectedTileType.subscribe(function (t) {
                    if (t) {
                        _this.ready(true);
                        _this.properties([]);
                        var tileDef = _this.tileTypes().filter(function (y) { return y.typeName == t; })[0];
                        var getTypePromise = _this.getTileMetadataPromise(t);
                        getTypePromise.done(function (metadata) { return _this.afterGetTypePromiseCallback(metadata, true); });
                        getTypePromise.fail(function (error) {
                            log.error("Error occurred getting type metadata for tile type " + t, error);
                        });
                    }
                    else {
                        _this.ready(false);
                        _this.properties([]);
                    }
                }));
            });
        };
        TilePanel.prototype.afterGetTypePromiseCallback = function (metadata, createSubscriptions) {
            var propertiesForEdit = this.processTileMetadata(metadata);
            this.properties(propertiesForEdit);
            if (createSubscriptions)
                this.createSubscriptions(this.properties(), this.panelEntity);
        };
        TilePanel.prototype.dispose = function () {
            _super.prototype.dispose.call(this);
            this.clearEditorSubscriptions();
            this.subscriptions.forEach(function (x) { return x.dispose(); });
        };
        TilePanel.prototype.clearEditorSubscriptions = function () {
            this.editorSubscriptions.forEach(function (sub) { return sub.dispose(); });
            this.editorSubscriptions = [];
        };
        TilePanel.prototype.processTileMetadata = function (metadata, includeHidden) {
            var _this = this;
            if (includeHidden === void 0) { includeHidden = false; }
            var properties = {};
            Object.keys(metadata.Properties).forEach(function (x) {
                var prop = metadata.Properties[x];
                var processed = _this.dataService.processMetadataItem(prop);
                processed.dataService = metadata.DataServiceName;
                properties[prop.Name] = processed;
            });
            var metadataProperties = Object.keys(properties).map(function (key) { return properties[key]; });
            if (!includeHidden)
                metadataProperties = metadataProperties.filter(function (x) { return !x.isHiddenInEditor; });
            return metadataProperties.sort(function (a, b) { return a.positionInEditor - b.positionInEditor; });
        };
        ;
        TilePanel.prototype.getDisplayNameFromSelectedTileType = function () {
            var _this = this;
            if (!this.ready() || !this.selectedTileType())
                return "";
            var type = this.tileTypes().filter(function (x) { return x.typeName == _this.selectedTileType(); })[0];
            if (!type)
                return "";
            var displayName = type.displayName;
            if (displayName == this.r["bwf_none_selected"])
                return "";
            return displayName;
        };
        TilePanel.prototype.confirmClick = function () {
            if (this.isCreate())
                this.createTile();
            else if (this.isEdit())
                this.validateAndSave(this.panelEntity, this.dashboardMetadata, this.errorMessages);
            else if (this.isDelete())
                this.confirmDeleteTile();
        };
        TilePanel.prototype.createTile = function () {
            this.validateAndSave(this.panelEntity, this.dashboardMetadata, this.errorMessages);
        };
        TilePanel.prototype.confirmDeleteTile = function () {
            this.validateAndSave(this.panelEntity, this.dashboardMetadata, this.errorMessages);
            this.closeEditor();
            ko.postbox.publish("bwf-transient-notification", "Successfully deleted tile");
        };
        TilePanel.prototype.getTileTypes = function (callback) {
            var _this = this;
            $.get(sprintf("%s/tile/types", options.explorerHostUrl), function (types) {
                _this.tileTypes(types);
                if (typeof callback === 'function') {
                    callback(_this.tileTypes());
                }
            });
        };
        TilePanel.prototype.getTileMetadataPromise = function (type) {
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
        TilePanel.prototype.closeEditor = function () {
            ko.postbox.publish(this.dashboardElementId + "-close-dashboard-panel");
        };
        TilePanel.prototype.updateTypePermissions = function (c) {
            var _this = this;
            if (this.typePermissionsQuery) {
                this.typePermissionsQuery.abort();
            }
            var dataService = metadataService.getDataService(c.state.dataService);
            var url = sprintf("%s/rendermodel/Dashboard", dataService.url);
            this.typePermissionsQuery = $.ajax({
                url: url,
                xhrFields: { withCredentials: true },
            });
            this.typePermissionsQuery.done(function (response) { return _this.typePermissions(response); });
            this.typePermissionsQuery.fail(function (error) {
                log.error("Error occurred getting render model for Dashboard", error);
            });
        };
        TilePanel.prototype.createSubscriptions = function (properties, c) {
            var _this = this;
            this.clearEditorSubscriptions();
            this.createObservables(properties, c, this.formDisabled, this.createObservablesForChoice, this.createObservablesCallback);
            properties.forEach(function (property) {
                var subscription = _this.createSubscription(property, c);
                // if the property requires a custom control there will be no subscription
                if (subscription)
                    _this.editorSubscriptions.push(subscription);
            });
        };
        TilePanel.prototype.deserialiseTile = function (serialised) {
            return $.ajax({
                url: options.explorerHostUrl + "/tile/deserialise",
                xhrFields: { withCredentials: true },
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(this.data)
            });
        };
        TilePanel.prototype.resetErrors = function () {
            var _this = this;
            Object.keys(this.panelEntity.validations.messages).forEach(function (key) { return _this.panelEntity.validations.messages[key](''); });
            Object.keys(this.panelEntity.observables)
                .map(function (x) { return _this.panelEntity.observables[x]; })
                .forEach(function (x) {
                if (x.isValid)
                    x.isValid(true);
            });
            this.errorMessages([]);
        };
        TilePanel.prototype.validateAndSave = function (current, metadata, errorMessages) {
            var _this = this;
            log.debug("Saving dashboard " + this.dashboard.Name);
            var dashboardToSave = utils.clone(this.dashboard);
            if (this.isDelete()) {
                this.panelEntity.state.customConfirm(this.panelEntity, this.dashboardMetadata, this.errorMessages, this.closeEditor);
            }
            else {
                var createdTileType = this.tileTypes().filter(function (x) { return x.typeName == _this.selectedTileType(); })[0];
                var tileObj;
                if (this.isCreate())
                    tileObj = {};
                else if (this.isEdit())
                    tileObj = utils.clone(this.panelEntity.record);
                tileObj["$type"] = createdTileType.tileTypeName;
                // now we copy the values of all the fields
                // we need to serialize the fields that are not on the base tile type
                var baseTileTypeNames = this.baseTileTypeMetadataProperties.map(function (x) { return x.name; }).filter(function (x) { return x.toLowerCase() != "content"; });
                var propertiesToStringifyIntoContent = {};
                this.properties().map(function (x) { return x.name; }).forEach(function (x) {
                    if (baseTileTypeNames.indexOf(x) > -1) {
                        tileObj[x] = current.observables[x]();
                    }
                    else {
                        propertiesToStringifyIntoContent[x] = current.observables[x]();
                    }
                });
                tileObj["Content"] = JSON.stringify(propertiesToStringifyIntoContent);
                tileObj["TileType"] = this.selectedTileType();
                var tile = tileObj;
                if (current.state.isCreate) {
                    tile.Id = utils.generateGuid();
                    tile.SizeX = createdTileType.tileDefinition.DefaultSizeX;
                    tile.SizeY = createdTileType.tileDefinition.DefaultSizeY;
                    tile.PositionX = 0;
                    tile.PositionY = 0;
                }
                this.resetErrors();
                var validateUrl = sprintf("%s/tile/validate", options.explorerHostUrl);
                var validationPromise = $.ajax({
                    url: validateUrl,
                    xhrFields: { withCredentials: true },
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify(tileObj)
                });
                validationPromise.done(function (x) {
                    _this.sendSave(current, dashboardToSave, tile);
                });
                validationPromise.fail(function (x) {
                    if (x.status === 400) {
                        var validationResult = x.responseJSON;
                        if (utils.Results.isModelValidation(validationResult)) {
                            var validation = validationResult;
                            // inform user of validation errors
                            Object.keys(validationResult.PropertyValidations)
                                .filter(function (key) { return key[0] !== '$'; })
                                .forEach(function (key) {
                                _this.panelEntity.validations.messages[key](validation.PropertyValidations[key]);
                                if (_this.panelEntity.observables[key].isValid) {
                                    _this.panelEntity.observables[key].isValid(false);
                                }
                            });
                            _this.errorMessages(validationResult.ModelValidations);
                            return false;
                        }
                    }
                    else {
                        log.error("Unexpected response - see error", x.responseJSON);
                    }
                });
            }
        };
        TilePanel.prototype.sendSave = function (current, dashboardToSave, tile) {
            if (!current.state.isCreate) {
                // remove the current tile so we don't save it twice
                var currentTile = this.dashboardTiles().filter(function (x) { return tile.Id === x.Id; })[0];
                if (currentTile)
                    this.dashboardTiles.remove(currentTile);
            }
            this.dashboardTiles.push(tile);
            ko.postbox.publish("bwf-transient-notification", current.state.isCreate ?
                this.r["bwf_create_tile_success"] : this.r["bwf_edit_tile_success"]);
            ko.postbox.publish(this.dashboardElementId + '-dashboard-modified');
            this.closeEditor();
        };
        TilePanel.prototype.getComponentName = function (property) {
            if (property.useCustomControl) {
                this.editorSubscriptions.push(this.listenForChangesTo(property.name, this.panelEntity));
                return property.customControl;
            }
            return this.getComponentNameFromProperty(property);
        };
        return TilePanel;
    }(basePanelEditor.BasePanelEditor));
    return TilePanel;
});
