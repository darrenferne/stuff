define(["require", "exports", "knockout", "modules/bwf-metadata", "options", "scripts/sprintf", "modules/bwf-explorer", "modules/bwf-knockout-validators"], function (require, exports, ko, metadataService, options, sprintf, explorer) {
    "use strict";
    var LockViewModel = /** @class */ (function () {
        function LockViewModel(panelEntity) {
            var _this = this;
            this.lockReason = ko.observable('');
            this.lockReasonLabel = options.resources['bwf_lock_reason'];
            this.resources = options.resources;
            this.locking = ko.observable(false);
            this.ready = ko.observable(false);
            this.canRemove = ko.pureComputed(function () { return _this.ready() && _this.gridConfiguration.selectedRecords().length > 0; });
            this.typeLoaded = ko.observable(false);
            this.gridVisible = ko.pureComputed(function () { return _this.typeLoaded(); });
            this.disableSave = ko.pureComputed(function () { return _this.locking(); });
            this.removeSelected = function () {
                var toRemove = _this.gridConfiguration.selectedRecords();
                toRemove.forEach(function (d) {
                    _this.entities().forEach(function (e) {
                        if (e.record == d.record)
                            _this.entities.remove(e);
                    });
                    _this.gridConfiguration.records.remove(d);
                });
                if (_this.entities().length == 0)
                    _this.cancel();
            };
            this.baseType = panelEntity.state.typeName;
            this.dataService = panelEntity.state.dataService;
            this.displayName = ko.observable('');
            this.entities = ko.observableArray([]);
            this.errorMessages = ko.observableArray([]);
            this.grid = panelEntity.state.gridId;
            this.title = ko.pureComputed(function () { return 'Lock ' + _this.displayName(); });
            this.lockReason.extend({
                nonEmptyString: {
                    message: "The lock reason must not be empty"
                }
            });
            this.lockQuestion = ko.pureComputed(function () {
                return sprintf.sprintf("%s %s?", options.resources['bwf_lock_question'], _this.displayName());
            });
            metadataService.getType(this.dataService, this.baseType)
                .done(function (metadata) {
                _this.displayName(metadata.pluralisedDisplayName);
                var records = panelEntity.record;
                var entitiesWithSummary = records.map(function (r) {
                    var summary = metadata.identificationSummaryFields.length > 1
                        ? metadata.identificationSummaryFields.reduce(function (a, c) { return sprintf.sprintf("%s, %s", a, r[c]); })
                        : r[metadata.identificationSummaryFields[0]];
                    return {
                        record: r, identificationSummary: summary
                    };
                });
                _this.entities(entitiesWithSummary);
                _this.configureGrid(metadata);
                _this.typeLoaded(true);
            });
        }
        LockViewModel.prototype.configureGrid = function (metadata) {
            var grid = explorer.generateIdentificationSummaryGridConfiguration(this.grid + "-lockedObjects", metadata, this.disableSave);
            this.gridConfiguration = grid.configuration;
            this.gridConfiguration.disabled = ko.observable(false);
            this.gridConfiguration.showValidationInDisplayMode = true;
            var records = this.entities().map(function (r) { return r.record; });
            grid.setRecords(records);
            this.ready(true);
        };
        LockViewModel.prototype.confirmLock = function () {
            var _this = this;
            if (this.locking()) {
                return;
            }
            this.locking(true);
            this.errorMessages([]);
            var ds = metadataService.getDataService(this.dataService);
            var url = sprintf.sprintf("%s/changeset/BwfRecordLock", ds.url);
            var reason = this.lockReason();
            var locks = {};
            this.gridConfiguration.records().map(function (item) {
                return {
                    Username: options.username,
                    Reason: reason,
                    EntityId: item.record.Id,
                    EntityType: _this.baseType,
                    Context: "Manual"
                };
            }).forEach(function (lock) { return locks[lock.EntityId] = lock; });
            var request = $.ajax({
                url: url,
                xhrFields: { withCredentials: true },
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({ Create: locks })
            });
            request.done(function (result) {
                var failedIds = Object.keys(result.FailedCreates).filter(function (fc) { return fc !== '$type'; });
                var failedCreates = failedIds.map(function (key) { return result.FailedCreates[key]; });
                if (failedIds.length === 0) {
                    var notification = _this.resources['bwf_successfully_locked'];
                    ko.postbox.publish("bwf-transient-notification", notification);
                    ko.postbox.publish(_this.grid + '-pop-panel');
                }
                else {
                    var entities_1 = _this.entities();
                    var failedEntities = failedIds.map(function (f) { return entities_1.filter(function (e) { return e.record.Id.toString() === f.toString(); })[0]; });
                    _this.entities(failedEntities);
                    failedEntities.forEach(function (e) {
                        var failedCreate = result.FailedCreates[e.record.Id];
                        if (failedCreate == null)
                            return;
                        var modelValidationMsgs;
                        var propertyValidationsMsgs = {};
                        if (failedCreate.Type === "MessageSet") {
                            modelValidationMsgs = failedCreate.Messages;
                        }
                        else {
                            var modelValidation_1 = failedCreate;
                            // we cannot use Summary here as it includes validation message for Reason
                            // which for display purposes should be shown only once, below the Reason input
                            if (modelValidation_1.PropertyValidations) {
                                Object.keys(modelValidation_1.PropertyValidations)
                                    .filter(function (key) { return key !== "Reason"; })
                                    .forEach(function (key) { return propertyValidationsMsgs[key] = modelValidation_1.PropertyValidations[key]; });
                            }
                            modelValidationMsgs = modelValidation_1.ModelValidations;
                        }
                        var modelValidationToApply = {
                            ModelValidations: modelValidationMsgs,
                            PropertyValidations: propertyValidationsMsgs,
                            Summary: '',
                            Type: 'this'
                        };
                        _this.gridConfiguration.records()
                            .filter(function (r) { return r.record === e.record; })
                            .forEach(function (g) { return g.applyValidation(modelValidationToApply); });
                    });
                    var reasonMsgs = failedCreates
                        .filter(function (fc) { return fc.Type === "ModelValidation"; })
                        .map(function (fc) { return fc; })
                        .filter(function (mv) { return mv.PropertyValidations["Reason"] != null; })
                        .map(function (mv) { return mv.PropertyValidations["Reason"]; })
                        .filter(function (value, index, arr) { return arr.indexOf(value) == index; }); // filter out duplicates
                    _this.errorMessages(reasonMsgs);
                }
            }).always(function () { return _this.locking(false); });
        };
        LockViewModel.prototype.cancel = function () {
            ko.postbox.publish(this.grid + '-pop-panel');
        };
        return LockViewModel;
    }());
    return LockViewModel;
});
