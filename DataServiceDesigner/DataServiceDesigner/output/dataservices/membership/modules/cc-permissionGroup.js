define(["require", "exports", "knockout", "loglevel", "modules/bwf-metadata", "options"], function (require, exports, knockout, log, metadataService, options) {
    "use strict";
    var PermissionGroupPanel = /** @class */ (function () {
        function PermissionGroupPanel(params) {
            var _this = this;
            this.roleSelectorButton = '<button type="button" class="multiselect dropdown-toggle panel-type-selector" data-toggle="dropdown"><span class="left-aligned-text flex-content multiselect-selected-text"></span> <i class="large-text flex-no-shrink fa fa-caret-down"></i></button>';
            this.r = options.resources;
            this.data = params;
            this.actionStartedFrom = params.state.typeName;
            this.isApply = params.state.action === 'apply';
            this.isRemove = params.state.action === 'remove';
            this.metadata = knockout.observable(null);
            this.permissionGroupDataLevelPermissionMetadata = knockout.observable(null);
            this.permissionGroupServiceLevelPermissionMetadata = knockout.observable(null);
            this.availableRoles = knockout.observableArray([]);
            this.availablePermissionGroups = knockout.observableArray([]);
            this.selectedPermissionGroups = knockout.observableArray([]);
            this.errorMessages = knockout.observableArray([]);
            this.applyInProgress = knockout.observable(false);
            if (this.data.observables["selectedRoles"])
                this.selectedRoles = this.data.observables["selectedRoles"];
            else
                this.selectedRoles = knockout.observableArray([]);
            if (this.data.observables["selectedPermissionGroups"])
                this.selectedPermissionGroups = this.data.observables["selectedPermissionGroups"];
            else
                this.selectedPermissionGroups = knockout.observableArray([]);
            this.setupStringComputeds();
            switch (this.actionStartedFrom) {
                case "PermissionGroup":
                    params.record.forEach(function (x) { return _this.selectedPermissionGroups.push(x.record.Name); });
                    break;
                case "Role":
                    params.record.forEach(function (x) {
                        if (x.record.IsAdministrator) {
                            var verb = _this.isApply ? "adding" : "removing";
                            var message = "Administrative roles cannot have their permissions modified by permission groups.";
                            ko.postbox.publish("bwf-transient-notification", {
                                message: message,
                                //requireDismissal: true,
                                styleClass: 'alert-warning'
                            });
                        }
                        else {
                            _this.selectedRoles.push(x.record.Name);
                        }
                    });
                    break;
            }
            this.dataLevelPermissionsComputed = ko.pureComputed(function () {
                var selectedGroups = _this.selectedPermissionGroups();
                var selectedPermissionGroups = _this.availablePermissionGroups().filter(function (x) { return selectedGroups.indexOf(x.Name) > -1; });
                var allPermissions = Array.prototype.concat.apply([], selectedPermissionGroups.map(function (x) { return x.DataLevelPermissions; }));
                return allPermissions;
            });
            this.serviceLevelPermissionsComputed = ko.pureComputed(function () {
                var selectedGroups = _this.selectedPermissionGroups();
                var selectedPermissionGroups = _this.availablePermissionGroups().filter(function (x) { return selectedGroups.indexOf(x.Name) > -1; });
                var allPermissions = Array.prototype.concat.apply([], selectedPermissionGroups.map(function (x) { return x.ServiceLevelPermissions; }));
                return allPermissions;
            });
            this.loaded = knockout.observable(false);
            this.rendered = knockout.observable(false);
            this.ready = knockout.pureComputed(function () {
                var rendered = _this.rendered();
                var loaded = _this.loaded();
                return rendered && loaded;
            });
            this.disableConfirm = knockout.pureComputed(function () {
                if (_this.selectedRoles && _this.selectedRoles().length === 0)
                    return true;
                if (_this.selectedPermissionGroups && _this.selectedPermissionGroups().length === 0)
                    return true;
                if (_this.applyInProgress())
                    return true;
                return !_this.ready();
            });
            var getRolesPromise = $.ajax({
                type: 'GET',
                url: options.explorerHostUrl + "/api/membership/query/Roles?$orderby=Name&$filter=IsAdministrator=false",
                xhrFields: { withCredentials: true }
            });
            var getPermissionGroupsPromise = $.ajax({
                type: 'GET',
                url: options.explorerHostUrl + "/api/membership/query/PermissionGroups?$orderby=Name&$expand=DataLevelPermissions,ServiceLevelPermissions",
                xhrFields: { withCredentials: true }
            });
            var getPermissionGroupMetadataPromise = metadataService.getType('membership', 'PermissionGroup');
            var getPermissionGroupDataLevelPermissionMetadataPromise = metadataService.getType('membership', 'PermissionGroupDataLevelPermission');
            var getPermissionGroupServiceLevelPermissionMetadataPromise = metadataService.getType('membership', 'PermissionGroupServiceLevelPermission');
            $.when(getRolesPromise, getPermissionGroupsPromise, getPermissionGroupMetadataPromise, getPermissionGroupDataLevelPermissionMetadataPromise, getPermissionGroupServiceLevelPermissionMetadataPromise).done(function (rolesQueryResult, permissionGroupsQueryResult, permissionGroupMetadata, permissionGroupDlpMetadata, permissionGroupSlpMetadata) {
                _this.availableRoles(rolesQueryResult[0].Records);
                _this.availablePermissionGroups(permissionGroupsQueryResult[0].Records);
                _this.metadata(permissionGroupMetadata);
                _this.permissionGroupDataLevelPermissionMetadata(permissionGroupDlpMetadata);
                _this.permissionGroupServiceLevelPermissionMetadata(permissionGroupSlpMetadata);
            }).then(function () {
                _this.data.observables["selectedRoles"] = _this.selectedRoles;
                _this.data.observables["selectedPermissionGroups"] = _this.selectedPermissionGroups;
                _this.loaded(true);
            }).fail(function () {
                _this.errorMessages.push("Error occurred loading information required for applying or removing group.");
                _this.loaded(true);
            });
        }
        PermissionGroupPanel.prototype.dispose = function () {
            this.dataLevelPermissionsComputed.dispose();
            this.serviceLevelPermissionsComputed.dispose();
        };
        PermissionGroupPanel.prototype.setupStringComputeds = function () {
            var _this = this;
            this.title = ko.pureComputed(function () {
                var baseTitle = _this.isApply ? "Apply Permission Group" : "Remove Permission Group";
                var isPlural = _this.selectedPermissionGroups().length > 1;
                return "" + baseTitle + (isPlural ? "s" : "");
            });
            this.rolesExplainationText = ko.pureComputed(function () {
                return _this.isApply ? "Role(s) to apply permissions to" : "Role(s) to remove permissions from";
            });
            this.permissionGroupsSelectLabelText = ko.pureComputed(function () {
                return _this.isApply ? "Permission group(s) to apply" : "Permission group(s) to remove";
            });
            this.permissionsExplainationText = ko.pureComputed(function () {
                return _this.isApply ? "Permissions to assign" : "Permissions to remove";
            });
            this.confirmButtonText = ko.pureComputed(function () {
                return _this.isApply ? _this.r["bwf_apply"] : _this.r["bwf_remove"];
            });
            this.applyingText = ko.pureComputed(function () {
                return _this.isApply ? "Applying..." : "Removing...";
            });
        };
        PermissionGroupPanel.prototype.cancel = function () {
            ko.postbox.publish(this.data.state.gridId + '-pop-panel');
        };
        PermissionGroupPanel.prototype.confirm = function () {
            if (this.isApply)
                this.confirmAssignGroup();
            else
                this.confirmRemoveGroup();
        };
        PermissionGroupPanel.prototype.confirmAssignGroup = function () {
            var _this = this;
            var requestUrl = options.explorerHostUrl + "/ext/membership/permissiongroup/apply";
            if (this.selectedRoles().length > 0) {
                this.applyInProgress(true);
                var addPermissionsPromise = $.ajax({
                    url: requestUrl,
                    type: 'POST',
                    contentType: 'application/json',
                    xhrFields: { withCredentials: true },
                    data: JSON.stringify({
                        RoleNames: this.selectedRoles(),
                        PermissionGroupNames: this.selectedPermissionGroups()
                    })
                });
                addPermissionsPromise.done(function (changeResults) {
                    var rolesAppliedTo = _this.selectedRoles();
                    var permissionGroupsApplied = _this.selectedPermissionGroups();
                    var allSuccessful = rolesAppliedTo.every(function (r) { return permissionGroupsApplied.every(function (p) {
                        return changeResults[p + "|" + r].every(function (x) { return !x.length || x.length == 0; });
                    }); });
                    if (allSuccessful) {
                        var message = "Successfully applied permission group" + (_this.selectedPermissionGroups().length > 1 ? "s" : "") + " " +
                            ("'" + _this.selectedPermissionGroups().join("', '") + "' to role" + (_this.selectedRoles().length > 1 ? "s" : "") + " ") +
                            ("'" + _this.selectedRoles().join("', '") + "'");
                        ko.postbox.publish("bwf-transient-notification", message);
                        ko.postbox.publish(_this.data.state.gridId + '-pop-panel');
                    }
                    else {
                        _this.processErrors(changeResults);
                    }
                });
                addPermissionsPromise.fail(function (error) {
                    log.error("Error occurred when assigning permissions", error);
                });
                addPermissionsPromise.always(function () { return _this.applyInProgress(false); });
            }
        };
        PermissionGroupPanel.prototype.confirmRemoveGroup = function () {
            var _this = this;
            var requestUrl = options.explorerHostUrl + "/ext/membership/permissiongroup/remove";
            if (this.selectedRoles().length > 0) {
                this.applyInProgress(true);
                var removePermissionsPromise = $.ajax({
                    url: requestUrl,
                    type: 'POST',
                    contentType: 'application/json',
                    xhrFields: { withCredentials: true },
                    data: JSON.stringify({
                        RoleNames: this.selectedRoles(),
                        PermissionGroupNames: this.selectedPermissionGroups()
                    })
                });
                removePermissionsPromise.done(function (changeResults) {
                    var rolesRemovedFrom = _this.selectedRoles();
                    var permissionGroupsRemoved = _this.selectedPermissionGroups();
                    var allSuccessful = rolesRemovedFrom.every(function (r) { return permissionGroupsRemoved.every(function (p) {
                        return changeResults[p + "|" + r].every(function (x) { return !x.length || x.length == 0; });
                    }); });
                    if (allSuccessful) {
                        var message = "Successfully removed permission group" + (_this.selectedPermissionGroups().length > 1 ? "s" : "") + " " +
                            ("'" + _this.selectedPermissionGroups().join("', '") + "' to role" + (_this.selectedRoles().length > 1 ? "s" : "") + " ") +
                            ("'" + _this.selectedRoles().join("', '") + "'");
                        ko.postbox.publish("bwf-transient-notification", message);
                        ko.postbox.publish(_this.data.state.gridId + '-pop-panel');
                    }
                    else {
                        _this.processErrors(changeResults);
                    }
                });
                removePermissionsPromise.fail(function (error) {
                    log.error("Error(s) occurred when removing permissions", error);
                });
                removePermissionsPromise.always(function () { return _this.applyInProgress(false); });
            }
        };
        PermissionGroupPanel.prototype.processErrors = function (changeResults) {
            var _this = this;
            var verb = this.isApply ? "applying" : "removing";
            var message = "Error(s) have occurred when " + verb + " permissions for the role(s) requested. Please see the error messages at the bottom of the panel.";
            ko.postbox.publish("bwf-transient-notification", {
                message: message,
                requireDismissal: true,
                styleClass: 'alert-warning'
            });
            log.error("Error(s) occurred when " + verb + " permissions", changeResults);
            this.errorMessages([]);
            var resultsSplit = Object.keys(changeResults).map(function (x) { return x.split("|"); });
            resultsSplit.forEach(function (x) {
                var permissionGroupName = x[0];
                var roleName = x[1];
                var errorResult = changeResults[permissionGroupName + "|" + roleName];
                errorResult.forEach(function (e) {
                    return _this.errorMessages.push("When " + verb + " group '" + permissionGroupName + "' to role '" + roleName + "', an error occurred: '" + e + "'");
                });
            });
        };
        return PermissionGroupPanel;
    }());
    return PermissionGroupPanel;
});
