define(["require", "exports", "knockout", "options", "modules/bwf-metadata", "modules/bwf-explorer", "loglevel"], function (require, exports, ko, options, metadataService, explorer, log) {
    "use strict";
    var SelectedRoles = /** @class */ (function () {
        function SelectedRoles(params) {
            var _this = this;
            this.allRoles = ko.observableArray([]);
            this.selectedRoles = ko.observableArray([]);
            this.ready = ko.observable(false);
            this.subscriptions = [];
            this.adaptorSupportsRoles = ko.observable(true);
            this.showEditor = ko.computed(function () { return _this.adaptorSupportsRoles(); });
            this.action = ko.observable("");
            this.username = ko.observable("");
            this.title = ko.computed(function () { return _this.action() + ' - ' + _this.username(); });
            this.assigningText = ko.observable("");
            this.assignmentError = ko.observable(false);
            this.errorMessage = ko.observable("");
            this.canClear = ko.pureComputed(function () { return _this.ready() && _this.selectedRoles().length > 0; });
            this.canDelete = ko.pureComputed(function () { return _this.ready() && _this.selectedGrid.selectedRecords().length > 0; });
            this.canSelect = ko.pureComputed(function () { return _this.ready() && _this.availableGrid.selectedRecords().length > 0; });
            this.availableRoles = ko.pureComputed(function () {
                var all = _this.allRoles();
                if (_this.selectedRoles == null)
                    return [];
                var selected = _this.selectedRoles();
                var available = all.filter(function (r) {
                    return !selected.some(function (x) { return x.Id == r.Id; });
                });
                ;
                return available;
            });
            this.deleteSelected = function () {
                var toDelete = _this.selectedGrid.selectedRecords().map(function (r) { return r.record; });
                var current = _this.selectedRoles();
                var toKeep = current.filter(function (c) { return !toDelete.some(function (d) { return d.Id == c.Id; }); });
                _this.selectedRoles(toKeep);
            };
            this.clear = function () { return _this.selectedRoles([]); };
            this.resources = options.resources;
            this.formDisabled = params.observables['formDisabled'];
            this.gridId = params.state.gridId;
            this.action(this.resources['bwf_assign_roles']);
            this.assigningText(this.resources['bwf_roles_assigning']);
            this.username(params.record[0].record.Username);
            this.applyInProgress = ko.observable(false);
            this.userId = params.record[0].record.Id;
            var getMetadata = metadataService.getType('membership', 'Role');
            getMetadata.done(function (md) { return _this.setupGrids(md); });
            var dataService = metadataService.getDataService("membership");
            var allRolesQuery = $.ajax({
                url: dataService.hostUrl + '/api/membership/query/Roles',
                xhrFields: {
                    withCredentials: true
                }
            });
            var assignedRolesQuery = $.ajax({
                url: dataService.hostUrl + ("/api/membership/query/Users?$filter=Id=" + this.userId + "&$expand=Roles"),
                xhrFields: {
                    withCredentials: true
                }
            });
            var canAssignRoles = $.ajax({
                url: dataService.hostUrl + "/ext/membership/assign/roles/supported",
                xhrFields: {
                    withCredentials: true
                }
            });
            allRolesQuery.done(function (result) { return _this.allRoles(result.Records); });
            assignedRolesQuery.done(function (result) { return _this.selectedRoles(result.Records[0].Roles); });
            canAssignRoles.done(function (result) { return _this.adaptorSupportsRoles(result); });
            canAssignRoles.always(function () { return _this.ready(true); });
            this.subscriptions.push({ dispose: allRolesQuery.abort });
            this.subscriptions.push({ dispose: assignedRolesQuery.abort });
            this.subscriptions.push({ dispose: canAssignRoles.abort });
            params.observables['__renderedState'].push(this.ready);
        }
        SelectedRoles.prototype.setupGrids = function (metadata) {
            var _this = this;
            var availableGrid = explorer.generateIdentificationSummaryGridConfiguration(this.gridId + '-availableRoles', metadata, this.formDisabled);
            var selectedGrid = explorer.generateIdentificationSummaryGridConfiguration(this.gridId + '-selectedRoles', metadata, this.formDisabled);
            this.availableGrid = availableGrid.configuration;
            this.selectedGrid = selectedGrid.configuration;
            selectedGrid.setRecords(this.selectedRoles());
            this.subscriptions.push(this.availableRoles.subscribe(function (av) { return availableGrid.setRecords(av); }));
            this.subscriptions.push(this.selectedRoles.subscribe(function (sr) { return selectedGrid.setRecords(sr); }));
            this.select = function () {
                var selected = availableGrid.configuration.selectedRecords().map(function (r) { return r.record; });
                ko.utils.arrayPushAll(_this.selectedRoles, selected);
            };
        };
        SelectedRoles.prototype.dispose = function () {
            this.subscriptions.forEach(function (x) {
                if (x != null && typeof x.dispose == 'function')
                    x.dispose();
            });
        };
        SelectedRoles.prototype.cancel = function () {
            this.assignmentError(false);
            this.errorMessage("");
            ko.postbox.publish(this.gridId + '-pop-panel');
        };
        SelectedRoles.prototype.assign = function () {
            var _this = this;
            var requestUrl = options.explorerHostUrl + "/ext/membership/assign/roles/" + this.userId;
            this.assignmentError(false);
            this.errorMessage("");
            this.applyInProgress(true);
            var assignRolesPromise = $.ajax({
                url: requestUrl,
                type: 'PUT',
                contentType: 'application/json',
                xhrFields: { withCredentials: true },
                data: JSON.stringify(this.selectedRoles().map(function (r) { return r.Name; }))
            });
            assignRolesPromise.done(function (changeResult) {
                if (changeResult === true) {
                    var message = _this.resources['bwf_roles_assigned_ok'];
                    ko.postbox.publish("bwf-transient-notification", message);
                    ko.postbox.publish(_this.gridId + '-pop-panel');
                }
                else {
                    log.error("An error occurred when assigning roles to user.");
                }
            });
            assignRolesPromise.fail(function (message) {
                if (message.status === 403) {
                    log.error('Assigning Roles forbidden.');
                    _this.assignmentError(true);
                    _this.errorMessage(_this.resources['bwf_roles_no_permission']);
                }
                else if (message.status !== 400) {
                    log.error(message);
                }
                else {
                    var failed = JSON.parse(message.responseText);
                    switch (failed.Type.toLowerCase()) {
                        case "messageset":
                            log.error('Message set', failed);
                            _this.assignmentError(true);
                            _this.errorMessage(failed.Messages);
                            break;
                        case "modelvalidation":
                            var propertyMessages = failed.PropertyValidations;
                            $.each(propertyMessages, function (key, value) {
                                if (key.charAt(0) !== '$') {
                                    this.errorMessage(this.errorMessage() + '\n' + value);
                                }
                                ;
                            });
                            var modelMessages = failed.ModelValidations;
                            $.each(modelMessages, function (key, value) {
                                this.errorMessages(this.errorMessage() + '\n' + value);
                            });
                            break;
                    }
                }
            });
            assignRolesPromise.always(function () { return _this.applyInProgress(false); });
        };
        return SelectedRoles;
    }());
    return SelectedRoles;
});
