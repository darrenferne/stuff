define(["require", "exports", "knockout", "options", "loglevel", "sprintf", "modules/bwf-explorer", "modules/bwf-metadata"], function (require, exports, ko, options, log, sprintf, explorer, metadataService) {
    "use strict";
    var MassAssign = /** @class */ (function () {
        function MassAssign(data) {
            var _this = this;
            this.metadata = ko.observable(null);
            this.loaded = ko.observable(false);
            this.resources = options.resources;
            this.roles = ko.observableArray([]);
            this.rolesLoaded = ko.observable(false);
            this.title = ko.observable('Mass Assign');
            this.performingAction = ko.observable(false);
            this.columns = ko.observableArray([]);
            this.records = ko.observableArray([]);
            this.selectedRecords = ko.pureComputed(function () { return _this.records().filter(function (r) { return r.selected(); }); });
            this.data = data;
            this.grid = data.state.gridId;
            metadataService.getType(data.state.dataService, data.state.typeName)
                .done(function (metadata) {
                _this.metadata(metadata);
                _this.configureGrid(metadata);
                _this.loaded(true);
            });
        }
        MassAssign.prototype.configureGrid = function (metadata) {
            var columns = metadata.identificationSummaryFields.map(function (field, index) {
                var property = metadataService.getPropertyWithPrefix(metadata.dataService, metadata, field);
                return new explorer.ExplorerGridColumn(property, field, index);
            });
            var records = this.data.record.map(function (record, index) {
                var gridItem = new explorer.ExplorerGridItem({
                    Id: record.Id,
                    Position: index,
                    Data: record,
                    BaseTypeName: metadata.type,
                    TypeName: metadata.type
                }, columns, metadata.dataService);
                return gridItem;
            });
            this.columns(columns);
            this.records(records);
        };
        MassAssign.prototype.cancel = function () {
            ko.postbox.publish(this.grid + '-pop-panel');
        };
        MassAssign.prototype.confirmAssign = function () {
            var _this = this;
            this.performingAction(true);
            var available = this.data.observables['availableRoles']();
            var roles = available.reduce(function (acc, role) {
                return acc[role.Name] = role.Id, acc;
            }, {});
            var createPermission = function (permissionId, permissionName, roleId, entityId) {
                var permission = {
                    EntityType: _this.metadata().type,
                    PermissionId: permissionId,
                    PermissionName: permissionName,
                    RoleId: roleId,
                    EntityId: entityId
                };
                return permission;
            };
            var visibleTo = this.data.observables['visibilityToRoles']();
            var editableBy = this.data.observables['editableByRoles']();
            var permissionsByEntity = this.records().map(function (entity) {
                var read = visibleTo.map(function (v) { return createPermission(1, "Read", roles[v], entity.record.Id); });
                var edit = editableBy.map(function (e) { return createPermission(2, "Edit/Delete", roles[e], entity.record.Id); });
                return read.concat(edit);
            });
            var i = 0;
            var allPermissions = permissionsByEntity
                .reduce(function (acc, byEntity) { return acc.concat(byEntity); }, [])
                .reduce(function (acc, permission) { return acc[i++] = permission, acc; }, {});
            var changeSet = {
                Create: allPermissions,
                Update: {},
                Delete: []
            };
            var dataService = metadataService.getDataService(this.metadata().dataService);
            var query = $.ajax({
                url: sprintf.sprintf("%s/changeset/BwfDataLevelPermission", dataService.url),
                xhrFields: {
                    withCredentials: true
                },
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(changeSet)
            });
            query.done(function (result) {
                _this.performingAction(false);
                _this.processResponse(changeSet.Create, result);
            });
            query.fail(function (result) {
                log.error(result);
                _this.performingAction(false);
            });
        };
        MassAssign.prototype.processResponse = function (request, result) {
            var failed = Object.keys(result.FailedCreates)
                .filter(function (key) { return key[0] !== '$'; })
                .map(function (key) {
                return {
                    key: key,
                    entityId: request[key].EntityId,
                    value: result.FailedCreates[key]
                };
            })
                .filter(function (create) { return create.value.Summary !== 'The chosen permission for the entity has already been assigned to the chosen role'; });
            if (failed.length === 0) {
                ko.postbox.publish("bwf-transient-notification", "Permissions granted");
                ko.postbox.publish(this.grid + '-pop-panel');
                return;
            }
            var failedIds = failed.map(function (f) { return f.entityId; });
            this.records.remove(function (r) { return failedIds.indexOf(r.id) === -1; });
            this.records().forEach(function (r) {
                var fails = failed.filter(function (f) { return f.entityId === r.id; });
                fails.forEach(function (fail) { return r.applyValidation(fail.value); });
            });
        };
        MassAssign.prototype.gridConfiguration = function () {
            var _this = this;
            var config = {
                createNewRecord: function () { return null; },
                disableGridSorting: ko.observable(false),
                header: {
                    enabled: ko.observable(false),
                    name: null, config: null
                },
                footer: {
                    enabled: ko.observable(false),
                    name: null, config: null
                },
                isView: false,
                metadata: this.metadata,
                records: this.records,
                recordsCount: ko.pureComputed(function () { return _this.records().length; }),
                selectedColumns: ko.pureComputed(function () { return _this.columns(); }),
                selectedRecords: this.selectedRecords,
                showValidationInDisplayMode: true,
                embedded: true
            };
            return config;
        };
        return MassAssign;
    }());
    return MassAssign;
});
