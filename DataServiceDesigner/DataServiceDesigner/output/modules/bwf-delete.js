define(["require", "exports", "knockout", "options", "scripts/sprintf", "loglevel", "modules/bwf-explorer", "modules/bwf-metadata"], function (require, exports, ko, options, sprintf, log, explorer, metadataService) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var DeletionSummary = /** @class */ (function () {
        function DeletionSummary() {
        }
        return DeletionSummary;
    }());
    exports.DeletionSummary = DeletionSummary;
    var DeleteViewModel = /** @class */ (function () {
        function DeleteViewModel(panelEntity) {
            var _this = this;
            this.deletedRecords = [];
            this.entities = ko.observableArray([]);
            this.hasIgnoreLockPermission = ko.observable(false);
            this.parentIsSource = false;
            this.resources = options.resources;
            this.title = ko.observable(options.resources['bwf_delete']);
            this.deleting = ko.observable(false);
            this.subscriptions = [];
            this.checkRecordLocksCompleted = ko.observable(false);
            this.gridVisible = ko.pureComputed(function () {
                return _this.checkRecordLocksCompleted() && _this.typeLoaded();
            });
            this.typeLoaded = ko.observable(false);
            this.anyLocked = ko.pureComputed(function () {
                var e = _this.entities();
                var l = e.map(function (e) { return e.isLocked(); });
                return l.length > 0 && l.some(function (i) { return i; });
            });
            this.checkingLockStatus = ko.pureComputed(function () {
                var e = _this.entities();
                var l = e.map(function (e) { return e.isPendingLockCheck(); });
                return l.length > 0 && l.some(function (i) { return i; });
            });
            this.disableSave = ko.pureComputed(function () { return _this.deleting() ||
                _this.checkingLockStatus() ||
                (_this.anyLocked() && !_this.hasIgnoreLockPermission()) ||
                _this.entities().length == 0; });
            this.displayName = ko.pureComputed(function () {
                if (_this.metadata == null)
                    return '';
                if (_this.entities().length > 1)
                    return _this.metadata.pluralisedDisplayName;
                else
                    return _this.metadata.displayName;
            });
            this.deletedObjects = ko.pureComputed(function () {
                var records = _this.entities().map(function (record) { return record.record; });
                return records;
            });
            this.ready = ko.observable(false);
            this.canDelete = ko.pureComputed(function () { return _this.ready() && _this.gridConfiguration.selectedRecords().length > 0; });
            this.removeSelected = function () {
                var toRemove = _this.gridConfiguration.selectedRecords();
                toRemove.forEach(function (d) {
                    _this.entities().forEach(function (e) {
                        if (e.record == d.record)
                            _this.entities.remove(e);
                    });
                    _this.gridConfiguration.records.remove(d);
                });
            };
            this.data = panelEntity;
            this.baseType = panelEntity.state.typeName;
            this.dataService = panelEntity.state.dataService;
            this.deleteConfirmation = ko.pureComputed(function () {
                var textFormat;
                if (_this.entities().length > 1)
                    textFormat = options.resources['bwf_delete_confirmation_plural'];
                else
                    textFormat = options.resources['bwf_delete_confirmation_single'];
                return sprintf.sprintf(textFormat, _this.displayName());
            });
            this.errorMessages = ko.observableArray([]);
            this.grid = panelEntity.state.gridId;
            this.lockedPlural = options.resources['bwf_locked_plural'];
            this.lockChecking = options.resources['bwf_locked_checking'];
            this.lockDeletionWarning = options.resources['bwf_lock_delete_warning'];
            this.onCompletion = panelEntity.state.onCompletion;
            this.parentIsSource = panelEntity.state.parentIsSource;
            var ds = metadataService.getDataService(this.dataService);
            var url = sprintf.sprintf("%s/rendermodel/%s", ds.url, panelEntity.state.typeName);
            var typePermissionsQuery = $.ajax({
                url: url,
                xhrFields: { withCredentials: true },
            });
            typePermissionsQuery.done(function (response) {
                if (!response.lockable) {
                    _this.hasIgnoreLockPermission(true);
                    return;
                }
                var hasIgnoreLockQuery = sprintf.sprintf("%s/authorisation/haspermission/%s/%s/IgnoreLock", ds.hostUrl, ds.name, response.lockPermissionType || panelEntity.state.typeName);
                $.ajax({
                    url: hasIgnoreLockQuery,
                    xhrFields: { withCredentials: true },
                    type: "GET"
                }).done(function (result) { return _this.hasIgnoreLockPermission(result); })
                    .fail(function () { return _this.hasIgnoreLockPermission(false); });
            });
            metadataService.getType(this.dataService, this.baseType).done(function (metadata) {
                _this.metadata = metadata;
                var records = panelEntity.record;
                var withSummaries = records.map(function (record) {
                    var description = metadata.identificationSummaryFields.map(function (idSummaryField) {
                        if (idSummaryField.indexOf("/") !== -1) {
                            var descriptionArray = idSummaryField.split("/");
                            var currentItem = record[descriptionArray.splice(0, 1)[0]];
                            if (!currentItem) {
                                log.error("Error occurred - property does not exist");
                                return "";
                            }
                            descriptionArray.forEach(function (currentValue, index) {
                                currentItem = currentItem[currentValue];
                            });
                            return _this.isObject(currentItem) ? currentItem.Text : currentItem;
                        }
                        // check if we have object (an enum)
                        return _this.isObject(record[idSummaryField]) ? record[idSummaryField].Text : record[idSummaryField];
                    }).join(",");
                    var summary = {
                        record: record,
                        identificationSummary: description,
                        isLocked: ko.observable(false),
                        isPendingLockCheck: ko.observable(false)
                    };
                    if (!metadata.hasCompositeId)
                        _this.checkLockStatus(summary, ds);
                    return summary;
                });
                _this.entities(withSummaries);
                _this.configureGrid(metadata);
                _this.checkingLockStatus.subscribe(function (check) { return _this.checkRecordLocksCompleted(true); });
                _this.typeLoaded(true);
            });
        }
        DeleteViewModel.prototype.checkLockStatus = function (item, ds) {
            var _this = this;
            item.isPendingLockCheck(true);
            var url = sprintf.sprintf("%s/%s/%s/islocked", ds.url, this.baseType, item.record.Id);
            var lockQuery = $.ajax({
                url: url,
                xhrFields: { withCredentials: true },
                type: 'GET'
            });
            lockQuery.done(function (result) {
                item.isLocked(result);
                item.isPendingLockCheck(false);
                if (result) {
                    var modelValidation = {
                        ModelValidations: [options.resources['bwf_record_locked']],
                        PropertyValidations: {},
                        Summary: '',
                        Type: "this"
                    };
                    var gridItem = _this.gridConfiguration.records().filter(function (r) { return r.record == item.record; })[0];
                    gridItem.applyValidation(modelValidation);
                }
            });
            this.subscriptions.push({ dispose: lockQuery.abort });
        };
        DeleteViewModel.prototype.confirmDelete = function () {
            var _this = this;
            if (this.deleting()) {
                return;
            }
            this.deleting(true);
            this.deletedRecords = this.gridConfiguration.records()
                .filter(function (e) { return _this.parentIsSource || e.record.Id.toString() === '0' || e.record.Id === ''; })
                .map(function (e) { return e.record; });
            var idsToDelete = this.gridConfiguration.records()
                .map(function (e) { return e.record.Id; })
                .filter(function (id) { return id.toString() !== '0'; });
            if (idsToDelete.length === 0 || this.parentIsSource) {
                this.successfulDelete();
                return;
            }
            var ds = metadataService.getDataService(this.dataService);
            var url = sprintf.sprintf("%s/changeset/%s", ds.url, this.baseType);
            if (this.metadata.resourcefulRoute) {
                url = sprintf.sprintf("%s/%s/%s", metadataService.resourceUrl(ds, this.metadata), this.baseType, idsToDelete[0]);
            }
            var request = $.ajax({
                url: url,
                xhrFields: {
                    withCredentials: true
                },
                type: this.metadata.resourcefulRoute ? 'DELETE' : 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({ Delete: idsToDelete })
            });
            request.done(function (result) {
                var failed = [];
                _this.errorMessages([]);
                if (typeof result.FailedDeletions !== 'undefined')
                    failed = Object.keys(result.FailedDeletions).filter(function (fc) { return fc !== '$type'; });
                if (failed.length === 0) {
                    var deleted = _this.entities().map(function (d) { return d.record; });
                    _this.deletedRecords = _this.deletedRecords.concat(deleted);
                    _this.successfulDelete();
                }
                else {
                    var entities = _this.entities();
                    var failedEntities = failed.map(function (f) { return entities.filter(function (e) { return e.record.Id.toString() === f.toString(); })[0]; });
                    var deleted = entities.filter(function (e) { return failed.indexOf(e.record.Id.toString()) === -1; })
                        .map(function (e) { return e.record; });
                    _this.deletedRecords = _this.deletedRecords.concat(deleted);
                    _this.entities(failedEntities);
                    var failedModelValidationsEntities = failed.filter(function (f) { return result.FailedDeletions[f].Summary != ""; })
                        .map(function (f) { return entities.filter(function (e) { return e.record.Id.toString() === f.toString(); })[0]; });
                    failedModelValidationsEntities.forEach(function (e) {
                        var modelValidationMessage = failed
                            .filter(function (f) { return f.toString() == e.record.Id.toString(); })
                            .filter(function (f) { return result.FailedDeletions[f]["ModelValidations"] != ""; })
                            .map(function (f) { return result.FailedDeletions[f]["ModelValidations"]; })
                            .filter(function (v, i, t) { return t.indexOf(v) == i; });
                        var propertyValidationMessage = failed
                            .filter(function (f) { return f.toString() == e.record.Id.toString(); })
                            .filter(function (f) { return result.FailedDeletions[f]["PropertyValidations"]; })
                            .map(function (f) { return result.FailedDeletions[f]["PropertyValidations"]; })
                            .filter(function (v, i, t) { return t.indexOf(v) == i; });
                        var modelValidation = {
                            ModelValidations: modelValidationMessage,
                            PropertyValidations: {},
                            Summary: '',
                            Type: 'this'
                        };
                        if (e.isLocked() && !e.isPendingLockCheck())
                            modelValidation.ModelValidations.push(options.resources['bwf_record_locked']);
                        Object.keys(propertyValidationMessage).map(function (key) { return Object.keys(propertyValidationMessage[key])
                            .map(function (k) { return modelValidation.PropertyValidations[k] = propertyValidationMessage[key][k]; }); });
                        var gridRecord = _this.gridConfiguration.records().filter(function (r) { return r.record == e.record; });
                        gridRecord.forEach(function (g) { return g.applyValidation(modelValidation); });
                    });
                    var failMessages = failed
                        .filter(function (f) { return result.FailedDeletions[f]["Messages"]; })
                        .map(function (f) { return result.FailedDeletions[f]["Messages"]; })
                        .filter(function (v, i, t) { return t.indexOf(v) == i; });
                    _this.errorMessages(failMessages);
                }
            }).always(function () { return _this.deleting(false); });
            request.fail(function (message) {
                var failed = JSON.parse(message.responseText);
                switch ((failed.Type ? failed.Type : '').toLowerCase()) {
                    case "messageset":
                        _this.errorMessages(failed.Messages);
                        break;
                    case "modelvalidation":
                        var errors = [];
                        Object.keys(failed.PropertyValidations)
                            .filter(function (key) { return key[0] !== '$'; })
                            .forEach(function (key) {
                            errors.push(failed.PropertyValidations[key]);
                        });
                        errors.push(failed.ModelValidations);
                        _this.errorMessages(errors);
                        break;
                    default:
                        _this.errorMessages([failed.message, failed.fullException]);
                        break;
                }
            }).always(function () { return _this.deleting(false); });
        };
        DeleteViewModel.prototype.successfulDelete = function () {
            this.onCompletion({ record: this.deletedRecords, state: this.state });
            var notification = sprintf.sprintf("%s %s", this.displayName, options.resources['bwf_successfully_deleted']);
            ko.postbox.publish("bwf-transient-notification", notification);
            ko.postbox.publish(this.grid + '-pop-panel');
        };
        DeleteViewModel.prototype.cancel = function () {
            ko.postbox.publish(this.grid + '-pop-panel');
        };
        DeleteViewModel.prototype.dispose = function () {
            this.subscriptions.forEach(function (sub) { return sub.dispose(); });
            this.subscriptions = [];
        };
        DeleteViewModel.prototype.isObject = function (val) {
            if (val === null) {
                return false;
            }
            return ((typeof val === 'function') || (typeof val === 'object'));
        };
        DeleteViewModel.prototype.configureGrid = function (metadata) {
            var grid = explorer.generateIdentificationSummaryGridConfiguration(this.grid + "-deletedObjects", metadata, this.disableSave);
            this.gridConfiguration = grid.configuration;
            this.gridConfiguration.disabled = ko.observable(false);
            this.gridConfiguration.showValidationInDisplayMode = true;
            var records = this.entities().map(function (r) { return r.record; });
            grid.setRecords(records);
            this.ready(true);
        };
        return DeleteViewModel;
    }());
    exports.default = DeleteViewModel;
});
