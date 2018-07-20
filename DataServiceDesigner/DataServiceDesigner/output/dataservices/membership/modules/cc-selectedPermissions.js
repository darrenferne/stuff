define(["require", "exports", "knockout", "loglevel", "modules/bwf-metadata", "scripts/sprintf", "options", "modules/bwf-utilities", "modules/bwf-explorer"], function (require, exports, knockout, log, metadataService, sprintf, options, bwf, explorer) {
    "use strict";
    var SelectedPermissionsViewModel = /** @class */ (function () {
        function SelectedPermissionsViewModel(params) {
            var _this = this;
            this.r = options.resources;
            this.permissions = {};
            this.transformedPermissions = {};
            this.createGridItemsFromPermissionObjects = function (items, add, callback) {
                var gridItems = items.map(function (permission, index) {
                    var columnRecord = {
                        Id: permission.Id,
                        BaseTypeName: _this.instanceData.model.state.typeName,
                        Data: permission,
                        Position: index,
                        TypeName: _this.baseType()
                    };
                    return new explorer.ExplorerGridItem(columnRecord, _this.gridColumns(), _this.current.metadata.dataService, {
                        disableChangeTracking: true,
                        disableValidateOnChange: true
                    });
                });
                if (add)
                    gridItems.forEach(function (g) { return _this.permissionGridItems.push(g); });
                else
                    _this.permissionGridItems(gridItems);
                _this.permissionGridItems().forEach(function (x) { return x.configureColumns(_this.gridColumns()); });
                if (typeof callback === "function") {
                    callback();
                }
            };
            this.gridConfiguration = function () {
                var gridConfig = {
                    createNewRecord: function () { return null; },
                    disabled: _this.formDisabled,
                    disableInsertRecords: true,
                    disableRemoveRecords: false,
                    disableSoftDelete: true,
                    header: {
                        enabled: knockout.observable(false),
                        name: null, config: null
                    },
                    footer: {
                        enabled: knockout.observable(false),
                        name: null, config: null
                    },
                    isView: false,
                    metadata: _this.metadata,
                    viewGridId: _this.gridId,
                    disableGridSorting: knockout.observable(true),
                    canSelectIndividualCells: knockout.observable(false),
                    records: _this.permissionGridItems,
                    recordsCount: knockout.pureComputed(function () { return _this.permissionGridItems().length; }),
                    selectedColumns: knockout.pureComputed(function () { return _this.gridColumns(); }),
                    selectedRecords: _this.selectedPermissionGridItems,
                    inEditMode: knockout.observable(false),
                    postRender: function () { return; },
                    validate: function () { return null; },
                    embedded: true
                };
                return gridConfig;
            };
            this.current = params;
            this.metadata = knockout.observable(null);
            this.gridColumns = knockout.observableArray([]);
            this.subscriptions = knockout.observableArray([]);
            this.loaded = knockout.observable(false);
            this.rendered = knockout.observable(false);
            this.isCreate = params.model.state.isCreate;
            this.isCreateFromRole = !!params.model.record["roleId"];
            if (this.isCreateFromRole)
                this.roleId = params.model.record["roleId"];
            this.showProcessingOverlay = knockout.observable(false);
            this.permissionType = params.metadata._clrType.indexOf("DataLevelPermission") > -1 ?
                "DataLevelPermission" : "ServiceLevelPermission";
            this.instanceData = {
                instanceName: 'cc-selectedPermissions-for' + this.permissionType,
                model: params.model,
                permissionType: this.permissionType,
                availablePermissions: {}
            };
            this.baseType = knockout.observable(params.metadata._clrType);
            params.model.observables['System'] = knockout.observable('membership');
            params.model.observables['BaseType'] = this.baseType;
            this.formDisabled = params.model.observables['formDisabled'];
            this.ready = knockout.pureComputed(function () { return _this.rendered() && _this.loaded(); });
            params.model.observables['__renderedState'].push(this.ready);
            this.enableClear = knockout.pureComputed(function () { return _this.permissionGridItems().length > 0; });
            this.enableSelectionRemove = knockout.pureComputed(function () { return _this.selectedPermissionGridItems().length > 0; });
            this.permissionGridItems = knockout.observableArray([]);
            this.selectedPermissionGridItems = knockout.pureComputed(function () { return _this.permissionGridItems().filter(function (x) { return x.selected(); }); });
            this.gridId = sprintf.sprintf("%s-%s", this.instanceData.instanceName, window.performance.now().toFixed(4).replace(".", ""));
            this.selectedPermissions = params.model.observables[params.metadata.fullName];
            var allDataServices = metadataService.getAllDataServices();
            var distinctHostUrls = allDataServices.map(function (x) { return x.hostUrl; }).filter(function (v, i, arr) { return arr.indexOf(v) === i; });
            var hostInfoPromises = distinctHostUrls.map(function (h) {
                return $.ajax({
                    type: 'GET',
                    url: h + '/info/dataservices',
                    xhrFields: { withCredentials: true }
                });
            });
            var allDataServicesInfo = {};
            $.when.apply($, hostInfoPromises).done(function () {
                var dataServicesPromiseResults;
                if (hostInfoPromises.length > 1)
                    dataServicesPromiseResults = Array.prototype.slice.call(arguments).map(function (x) { return x[0]; });
                else
                    dataServicesPromiseResults = arguments[0];
                var dataServicesInfo = Array.prototype.concat.apply([], dataServicesPromiseResults);
                dataServicesInfo.forEach(function (dsi) {
                    var dsName = dsi.Name.toLowerCase();
                    allDataServicesInfo[dsName] = dsi;
                });
            }).fail(function (failure) {
                log.error("Error occurred getting information on data service hosts", failure);
            }).then(function () {
                var numberOfResolvedAvailablePermissionRequests = ko.observable(0);
                var allDataServicesAvailablePermissionsDone = ko.pureComputed(function () { return numberOfResolvedAvailablePermissionRequests() >= allDataServices.length; });
                allDataServices.forEach(function (ds) {
                    var requestUrl;
                    if (_this.permissionType === "DataLevelPermission")
                        requestUrl = ds.url + "/availabledatalevelpermissions";
                    else
                        requestUrl = ds.url + "/availableservicelevelpermissions";
                    var promise = $.ajax({
                        url: requestUrl,
                        type: 'GET',
                        xhrFields: { withCredentials: true },
                    });
                    promise.done(function (p) {
                        _this.permissions[ds.name.toLowerCase()] = p;
                        numberOfResolvedAvailablePermissionRequests(numberOfResolvedAvailablePermissionRequests() + 1);
                    });
                    promise.fail(function (fail) {
                        log.warn("Error getting available permissions from data service '" + ds.name + "'");
                        numberOfResolvedAvailablePermissionRequests(numberOfResolvedAvailablePermissionRequests() + 1);
                    });
                });
                allDataServicesAvailablePermissionsDone.subscribe(function (done) {
                    if (done) {
                        var allPermissionPromises = [];
                        if (_this.isCreateFromRole) {
                            _this.showProcessingOverlay(true);
                            var numberOfPermissionsRequestsToDo_1 = ko.observable(0);
                            var numberOfLoopsCompleted_1 = ko.observable(0);
                            var numberOfResolvedPermissionsRequests_1 = ko.observable(0);
                            var allDataServicesLoopFinished_1 = ko.pureComputed(function () { return allDataServices.length === numberOfLoopsCompleted_1(); });
                            var resolved = ko.pureComputed(function () { return allDataServicesLoopFinished_1() && (numberOfPermissionsRequestsToDo_1() === numberOfResolvedPermissionsRequests_1()); });
                            allDataServices.forEach(function (ds) {
                                var requestUrl;
                                if (_this.permissionType === "DataLevelPermission" && allDataServicesInfo[ds.name.toLowerCase()].HasDataLevelPermissions)
                                    requestUrl = ds.url + '/query/BwfDataLevelPermissions?$filter=RoleId=' + _this.roleId;
                                else if (_this.permissionType === 'ServiceLevelPermission' && allDataServicesInfo[ds.name.toLowerCase()].HasServiceLevelPermissions)
                                    requestUrl = ds.url + '/query/BwfServiceLevelPermissions?$filter=RoleId=' + _this.roleId;
                                if (requestUrl) {
                                    numberOfPermissionsRequestsToDo_1(numberOfPermissionsRequestsToDo_1() + 1);
                                    var promise = $.ajax({
                                        url: requestUrl,
                                        type: 'GET',
                                        xhrFields: { withCredentials: true }
                                    });
                                    promise.done(function (p) {
                                        if (_this.permissionType === "DataLevelPermission") {
                                            p.Records.forEach(function (r) {
                                                var dlp = {
                                                    DataService: ds.name,
                                                    EntityDescription: r.EntityDescription,
                                                    EntityType: r.EntityType,
                                                    PermissionName: r.PermissionName
                                                };
                                                _this.selectedPermissions.push(dlp);
                                            });
                                        }
                                        else {
                                            p.Records.forEach(function (r) {
                                                var slp = {
                                                    DataService: ds.name,
                                                    Name: r.Name,
                                                    Type: r.Type
                                                };
                                                _this.selectedPermissions.push(slp);
                                            });
                                        }
                                        numberOfResolvedPermissionsRequests_1(numberOfResolvedPermissionsRequests_1() + 1);
                                    });
                                    promise.fail(function () {
                                        log.warn("Error getting permissions for role with Role Id = " + _this.roleId);
                                        numberOfResolvedPermissionsRequests_1(numberOfResolvedPermissionsRequests_1() + 1);
                                    });
                                }
                                numberOfLoopsCompleted_1(numberOfLoopsCompleted_1() + 1);
                            });
                            resolved.subscribe(function (done) {
                                if (done) {
                                    _this.createGridItemsFromPermissionObjects(_this.selectedPermissions(), false, function () {
                                        _this.showProcessingOverlay(false);
                                    });
                                }
                            });
                        }
                        _this.subscriptions.push(ko.postbox.subscribe(_this.instanceData.instanceName + "-cc-available-permission-selected", function (publish) { return _this.addSelectedPermission(publish); }));
                        metadataService.getType('membership', _this.baseType()).done(function (typeMetadata) {
                            _this.metadata(typeMetadata);
                            var metadataClone = bwf.clone(_this.metadata());
                            var transformedPermissions = Object.keys(_this.permissions).map(function (dsName) {
                                if (_this.permissions.hasOwnProperty(dsName)) {
                                    var permissionsForDataService = _this.permissions[dsName];
                                    var allTypes = permissionsForDataService.map(function (p) { return p.Type; });
                                    var distinctTypes = allTypes.filter(function (t, typeIndex, typeArray) { return typeArray.indexOf(t) === typeIndex; });
                                    var returnItem = {
                                        dataService: dsName,
                                        permissions: {}
                                    };
                                    distinctTypes.forEach(function (t) {
                                        var arrayOfPermissions = permissionsForDataService.map(function (x) { return x.Type === t ? x.Name : null; }).filter(function (x) { return !!x; });
                                        returnItem["permissions"][t] = arrayOfPermissions;
                                    });
                                    return returnItem;
                                }
                                return null;
                            }).filter(function (x) { return !!x; });
                            var columnProperties = Object.keys(metadataClone.properties).map(function (k) { return metadataClone.properties[k]; }).map(function (property) {
                                if (_this.metadata().identificationSummaryFields.filter(function (x) { return x.indexOf(property.fullName) > -1; }).length > 0) {
                                    property.isNotEditableInGrid = true;
                                    return property;
                                }
                                else
                                    return null;
                            }).filter(function (x) { return !!x; });
                            transformedPermissions.forEach(function (x) {
                                if (Object.keys(x.permissions).length > 0)
                                    _this.transformedPermissions[x.dataService] = x.permissions;
                            });
                            _this.gridColumns(columnProperties.map(function (cp, index) { return new explorer.ExplorerGridColumn(cp, cp.name, index + 1); }));
                            var items = _this.selectedPermissions();
                            if (_this.isCreateFromRole) {
                                _this.loaded(true);
                            }
                            else {
                                _this.createGridItemsFromPermissionObjects(items, false, function () { return _this.loaded(true); });
                            }
                        });
                    }
                });
            });
        }
        SelectedPermissionsViewModel.prototype.availablePermissionsParams = function () {
            var permissionParams = this.instanceData;
            this.instanceData["availablePermissions"] = this.transformedPermissions;
            return permissionParams;
        };
        SelectedPermissionsViewModel.prototype.removeSelectedPermissions = function () {
            var toRemoveGridItems = this.selectedPermissionGridItems();
            this.permissionGridItems.removeAll(toRemoveGridItems);
            this.selectedPermissions.removeAll(toRemoveGridItems.map(function (x) { return x.record; }));
        };
        SelectedPermissionsViewModel.prototype.clearSelectedPermissions = function () {
            this.permissionGridItems([]);
            this.selectedPermissions([]);
        };
        SelectedPermissionsViewModel.prototype.addSelectedPermission = function (permission) {
            var _this = this;
            this.showProcessingOverlay(true);
            if (this.permissionType === 'DataLevelPermission') {
                var url = metadataService.getDataService(permission.dataService).url;
                var getDataLevelPermissionsPromise = $.ajax({
                    type: 'GET',
                    url: url + "/query/BwfDataLevelPermissions?$filter=EntityType=" +
                        ("'" + permission.type + "' and EntityId=" + permission.entityId),
                    xhrFields: { withCredentials: true }
                });
                getDataLevelPermissionsPromise.done(function (dlp) {
                    var dataLevelPermission = dlp.Records[0];
                    var newPermission = {
                        EntityDescription: dataLevelPermission["EntityDescription"],
                        EntityType: permission.type,
                        PermissionName: permission.permissionName,
                        DataService: permission.dataService
                    };
                    var alreadyHasPermission = function (newPermission) {
                        return _this.selectedPermissions().filter(function (x) {
                            return x.EntityDescription == newPermission.EntityDescription &&
                                x.EntityType == newPermission.EntityType &&
                                x.PermissionName == newPermission.PermissionName &&
                                x.DataService == newPermission.DataService;
                        }).length > 0;
                    };
                    if (!alreadyHasPermission(newPermission)) {
                        _this.selectedPermissions.push(newPermission);
                        _this.createGridItemsFromPermissionObjects([newPermission], true, function () { return _this.showProcessingOverlay(false); });
                    }
                    else {
                        log.info("Already has Data Level Permission '" + newPermission.EntityType + "|" + newPermission.PermissionName + "|" + newPermission.EntityDescription + "' so not adding again.");
                        _this.showProcessingOverlay(false);
                    }
                });
            }
            else {
                var newPermission = {
                    Type: permission.type,
                    Name: permission.permissionName,
                    DataService: permission.dataService
                };
                var alreadyHasPermission = function (newPermission) {
                    return _this.selectedPermissions().filter(function (x) {
                        return x.Type == newPermission.Type &&
                            x.Name == newPermission.Name &&
                            x.DataService == newPermission.DataService;
                    }).length > 0;
                };
                if (!alreadyHasPermission(newPermission)) {
                    this.selectedPermissions.push(newPermission);
                    this.createGridItemsFromPermissionObjects([newPermission], true, function () { return _this.showProcessingOverlay(false); });
                }
                else {
                    log.info("Already has Service Level Permission '" + newPermission.Type + "|" + newPermission.Name + "' so not adding again.");
                    this.showProcessingOverlay(false);
                }
            }
        };
        return SelectedPermissionsViewModel;
    }());
    return SelectedPermissionsViewModel;
});
