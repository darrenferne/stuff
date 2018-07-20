define(["require", "exports", "loglevel", "modules/bwf-metadata"], function (require, exports, log, metadataService) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var AvailablePermissions = /** @class */ (function () {
        function AvailablePermissions(data) {
            var _this = this;
            this.breadCrumbClicked = function (item) {
                log.debug('Bread crumb clicked', item);
                if (item.depth == 1) {
                    _this.resetAvailableProperties();
                }
                else if (item.depth == 2) {
                    _this.updateAvailablePermissions({
                        displayName: item.displayName,
                        name: item.name,
                        displayNameWithIndicator: item.displayName + '...',
                        children: _this.availablePermissions
                    });
                }
                else if (item.depth == 3) {
                    var dataService = _this.breadCrumbs().filter(function (x) { return x.depth == 2; })[0].name;
                    _this.updateAvailablePermissions({
                        displayName: item.displayName,
                        name: item.name,
                        displayNameWithIndicator: item.displayName + '...',
                        children: _this.availablePermissions[dataService]
                    });
                }
                _this.breadCrumbs.remove(function (i) { return i.depth > item.depth; });
            };
            this.propertyClicked = function (item) {
                log.debug('Property clicked', item);
                _this.updateAvailablePermissions(item);
            };
            this.subscriptions = ko.observableArray([]);
            this.instanceName = data.instanceName;
            this.breadCrumbs = ko.observableArray([]);
            this.propertySet = ko.observableArray([]);
            this.isCreate = data.model.state.isCreate;
            this.rendered = ko.observable(false);
            this.loaded = ko.observable(false);
            this.ready = ko.pureComputed(function () {
                var rendered = _this.rendered();
                var loaded = _this.loaded();
                return rendered && loaded;
            });
            this.requestInProgress = ko.observable(false);
            this.isDataLevelPermission = data["permissionType"] === 'DataLevelPermission';
            this.formDisabled = data.model.observables['formDisabled'];
            data.model.observables['__renderedState'].push(this.ready);
            this.availablePermissions = data["availablePermissions"];
            this.currentLevelAvailablePermissions = ko.observableArray([]);
            this.resetAvailableProperties();
            this.loaded(true);
        }
        AvailablePermissions.prototype.dispose = function () {
            this.subscriptions().forEach(function (s) { return s.dispose(); });
        };
        AvailablePermissions.prototype.resetAvailableProperties = function () {
            var _this = this;
            this.breadCrumbs([{
                    depth: 1,
                    displayName: "Permissions",
                    name: '',
                }]);
            this.currentLevelAvailablePermissions(Object.keys(this.availablePermissions).map(function (k) {
                var dataServiceObject = {
                    displayName: k,
                    displayNameWithIndicator: k + '...',
                    name: k,
                    children: _this.availablePermissions
                };
                return dataServiceObject;
            }));
        };
        AvailablePermissions.prototype.sortPropertiesByDisplayName = function (a, b) {
            if (a.displayName && b.displayName)
                return a.displayName.localeCompare(b.displayName);
            return 0;
        };
        AvailablePermissions.prototype.updateAvailablePermissions = function (item) {
            var _this = this;
            if (!!item.children && Object.keys(item.children).length > 0) {
                this.breadCrumbs.push({
                    depth: this.breadCrumbs().length + 1,
                    displayName: item.displayName,
                    name: item.name,
                });
                if (Array.isArray(item.children[item.name])) {
                    var array = item.children[item.name];
                    this.currentLevelAvailablePermissions(array.map(function (p) {
                        var permissionObject = {
                            displayName: p,
                            displayNameWithIndicator: _this.isDataLevelPermission ? p + '...' : p,
                            name: p,
                            children: null
                        };
                        return permissionObject;
                    }).sort(this.sortPropertiesByDisplayName));
                }
                else {
                    this.currentLevelAvailablePermissions(Object.keys(item.children[item.name]).map(function (k) {
                        var dataServiceObject = {
                            displayName: k,
                            displayNameWithIndicator: k + '...',
                            name: k,
                            children: item.children[item.name]
                        };
                        return dataServiceObject;
                    }).sort(this.sortPropertiesByDisplayName));
                }
            }
            else {
                var breadCrumbNames = this.breadCrumbs().map(function (x) { return x.name; }).filter(function (x) { return !!x; });
                if (this.breadCrumbs().length === 3 && this.isDataLevelPermission) {
                    this.breadCrumbs.push({
                        depth: this.breadCrumbs().length + 1,
                        displayName: item.displayName,
                        name: item.name,
                    });
                    this.requestInProgress(true);
                    var typeToGet = breadCrumbNames[1];
                    var queryUrl = metadataService.getDataService(breadCrumbNames[0]).url;
                    var getRecordsRequest = $.ajax({
                        type: 'GET',
                        url: queryUrl + "/query/" + typeToGet + "s",
                        xhrFields: { withCredentials: true },
                    });
                    var getTypePromise = metadataService.getType(breadCrumbNames[0], breadCrumbNames[1]);
                    $.when(getRecordsRequest, getTypePromise)
                        .done(function (recordsRequestResult, typeMetadata) {
                        var dataRecords = recordsRequestResult[0].Records;
                        _this.currentLevelAvailablePermissions(dataRecords.map(function (d) {
                            var possibleDisplayNameProperties = typeMetadata.identificationSummaryFields;
                            var possibleDisplayNamePropertiesWithoutId = possibleDisplayNameProperties.filter(function (x) { return x != 'Id'; });
                            var objectProperties = Object.keys(d).filter(function (x) { return x[0] !== '$'; });
                            var displayProperty = "";
                            if (possibleDisplayNamePropertiesWithoutId.length > 0)
                                displayProperty = possibleDisplayNamePropertiesWithoutId[0];
                            else if (objectProperties.indexOf("Name") > -1)
                                displayProperty = 'Name';
                            else if (possibleDisplayNameProperties.length > 0)
                                displayProperty = possibleDisplayNameProperties[0];
                            else
                                displayProperty = objectProperties[0];
                            var displayName = d[displayProperty];
                            var recordObject = {
                                displayName: displayName,
                                children: null,
                                displayNameWithIndicator: displayName,
                                name: d.Id
                            };
                            return recordObject;
                        }).sort(_this.sortPropertiesByDisplayName));
                    }).fail(function (failure) {
                        _this.currentLevelAvailablePermissions([]);
                        log.error("Error occurred when performing requests: ", failure);
                    }).always(function () { return _this.requestInProgress(false); });
                }
                else {
                    var publish;
                    if (this.isDataLevelPermission) {
                        publish = {
                            dataService: breadCrumbNames[0],
                            type: breadCrumbNames[1],
                            permissionName: breadCrumbNames[2],
                            entityId: item.name
                        };
                    }
                    else {
                        publish = {
                            dataService: breadCrumbNames[0],
                            type: breadCrumbNames[1],
                            permissionName: item.name
                        };
                    }
                    ko.postbox.publish(this.instanceName + '-cc-available-permission-selected', publish);
                }
            }
        };
        return AvailablePermissions;
    }());
    exports.default = AvailablePermissions;
});
