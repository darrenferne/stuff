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
define(["require", "exports", "knockout", "modules/bwf-metadata", "loglevel", "options", "modules/bwf-utilities", "modules/bwf-datetimeUtilities", "modules/bwf-basePanelEditor", "sprintf", "modules/bwf-knockout-validators"], function (require, exports, knockout, metadataService, log, options, bwf, datetimeUtils, basePanelEditor, sprintfModule) {
    "use strict";
    var sprintf = sprintfModule.sprintf;
    var PanelEditor = /** @class */ (function (_super) {
        __extends(PanelEditor, _super);
        function PanelEditor(entity) {
            var _this = _super.call(this, entity) || this;
            _this.isCreate = knockout.pureComputed(function () { return _this.current.state.isCreate; });
            _this.isBeingViewed = knockout.pureComputed(function () { return _this.current.state.action == 'view'; });
            _this.isLockable = knockout.pureComputed(function () {
                var permissions = _this.typePermissions();
                return permissions ? permissions.lockable : false;
            });
            _this.title = knockout.pureComputed(function () {
                var metadata = _this.metadata();
                var resource = '';
                if (_this.isCreate())
                    resource = _this.resources['bwf_create'];
                else if (_this.isBeingViewed())
                    resource = _this.resources['bwf_view'];
                else
                    resource = _this.resources['bwf_edit'];
                return metadata ? sprintf("%s %s", resource, metadata.displayName) : '';
            });
            _this.formDisabled = knockout.pureComputed(function () {
                var m = _this.metadata();
                if (!m || _this.isBeingViewed())
                    return true;
            });
            _this.saveVisible = knockout.pureComputed(function () {
                var rendered = _this.rendered;
                var isBeingViewed = _this.isBeingViewed();
                return rendered && !isBeingViewed;
            });
            _this.saveDisabled = knockout.pureComputed(function () {
                var c = _this.current;
                if (c.observables['customControlDisableSave']())
                    return true;
                var lockable = _this.isLockable();
                var hasErrors = _this.errorMessages().length > 0;
                var disabledByLock = false;
                var knockoutValidators = Object.keys(c.observables)
                    .map(function (key) { return c.observables[key]; })
                    .filter(function (obs) { return !!obs.isValid; })
                    .map(function (obs) { return obs.isValid(); });
                if (lockable) {
                    var locked = c.observables['isLocked']();
                    var ignoreLock = c.observables['hasIgnoreLock']();
                    disabledByLock = locked && !ignoreLock;
                }
                return disabledByLock || knockoutValidators.some(function (v) { return !v; });
            });
            _this.current = null;
            _this.properties = knockout.pureComputed(function () {
                var metadata = _this.metadata();
                var permissions = _this.typePermissions();
                var loaded = _this.recordLoaded();
                if (loaded && metadata && permissions) {
                    return Object.keys(metadata.properties)
                        .map(function (key) { return metadata.properties[key]; })
                        .filter(function (property) { return _super.prototype.isPropertyVisible.call(_this, property, _this.isCreate()); })
                        .sort(function (a, b) { return a.positionInEditor - b.positionInEditor; });
                }
                else {
                    return [];
                }
            });
            _this.close = function () {
                _this.current.observables['customControlDisableSave'](false);
                knockout.postbox.publish(_this.grid + '-pop-panel');
            };
            _this.save = function (current, metadata, errorMessages, closePanelCallback) {
                var c = current;
                var dataService = metadataService.getDataService(c.state.dataService);
                var record = c.record;
                if (c.state.isCreate && (metadata.hasEditabilityToRoles || metadata.hasVisibilityToRoles)) {
                    record = {
                        EditableByRoles: metadata.hasEditabilityToRoles ? c.observables['editableByRoles']() : [],
                        VisibleToRoles: metadata.hasVisibilityToRoles ? c.observables['visibilityToRoles']() : [],
                        Record: c.record
                    };
                }
                Object.keys(c.validations.messages)
                    .forEach(function (key) { return c.validations.messages[key](''); });
                var id = metadata.properties['Id'].type == 'string'
                    ? bwf.filterEncode(record.Id)
                    : record.Id;
                var saveUrl = sprintf("%s/%s/%s/%s", metadataService.resourceUrl(dataService, metadata), c.state.typeName, (c.state.isCreate ? '' : id), (c.state.parentIsSource ? 'withoutpersistance' : ''));
                var query = $.ajax({
                    url: saveUrl,
                    xhrFields: { withCredentials: true },
                    type: c.state.isCreate ? 'POST' : 'PUT',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify(record)
                });
                query.fail(function (message) {
                    var result = JSON.parse(message.responseText);
                    if (bwf.Results.isMessageSet(result))
                        errorMessages(result.Messages);
                    else if (bwf.Results.isModelValidation(result)) {
                        var validations_1 = result;
                        Object.keys(result.PropertyValidations)
                            .filter(function (key) { return key[0] !== '$' && key.indexOf('[') == -1; })
                            .forEach(function (key) {
                            c.validations.messages[key](validations_1.PropertyValidations[key]);
                            if (c.observables[key].isValid)
                                c.observables[key].isValid(false);
                        });
                        errorMessages(result.ModelValidations);
                    }
                    else if (bwf.Results.isException(result))
                        errorMessages([result.fullException]);
                    else
                        errorMessages([message.responseText]);
                });
                query.done(function (savedRecord) {
                    var completionParams = {
                        record: savedRecord,
                        state: c.state
                    };
                    c.state.onCompletion(completionParams);
                    var message = c.state.parentIsSource
                        ? _this.resources['bwf_successfully_added']
                        : _this.resources['bwf_successfully_saved'];
                    var notification = sprintf("%s %s", _this.metadata().displayName, message);
                    knockout.postbox.publish("bwf-transient-notification", notification);
                    closePanelCallback();
                });
            };
            _this.current = entity;
            _this.grid = entity.state.gridId;
            _this.metadata = knockout.observable(null);
            _this.typePermissions = knockout.observable(null);
            _this.resources = options.resources;
            _this.hostUrl = options.explorerHostUrl;
            _this.persistentWarningMessages = knockout.observableArray([]);
            _this.recordLoaded = knockout.observable(false);
            knockout.postbox.subscribe(_this.grid + "-persistent-warning-message", function (x) { return _this.persistentWarningMessages.push(x); });
            _this.subscriptions = [];
            if (typeof entity.state.customConfirm === 'function')
                _this.save = entity.state.customConfirm;
            _this.fetchRenderData(entity);
            return _this;
        }
        PanelEditor.prototype.dispose = function () {
            _super.prototype.dispose.call(this);
            this.writeCurrentValuesToRecord();
            this.current = null;
            this.metadata(null);
            if (this.recordQuery)
                this.recordQuery.abort();
            if (this.typePermissionsQuery)
                this.typePermissionsQuery.abort();
            this.clearSubscriptions();
        };
        PanelEditor.prototype.fetchRenderData = function (entity) {
            this.updateMetadata(entity);
            this.updateTypePermissions(entity);
        };
        PanelEditor.prototype.updateMetadata = function (c) {
            var _this = this;
            // this check should hopefully stop us making duplicate calls
            if (this.metadataPromise && this.metadataPromise.state() === "pending")
                return;
            this.metadataPromise = metadataService.getType(c.state.dataService, c.state.typeName);
            this.metadataPromise.fail(function () { return _this.metadata(null); });
            this.metadataPromise.done(function (metadata) {
                // call this here and passin the metadata instead of relying on the
                // observable so that our observables are setup before the components
                // for the sub-controls are rendered
                _this.createSubscriptions(metadata, c);
                _this.metadata(metadata);
                _this.metadataPromise = null;
                _this.updateRecordValues(c, metadata);
            });
        };
        PanelEditor.prototype.updateTypePermissions = function (c) {
            var _this = this;
            if (this.typePermissionsQuery) {
                this.typePermissionsQuery.abort();
            }
            var dataService = metadataService.getDataService(c.state.dataService);
            var url = sprintf("%s/rendermodel/%s", dataService.url, c.state.typeName);
            this.typePermissionsQuery = $.ajax({
                url: url,
                xhrFields: { withCredentials: true },
            });
            this.typePermissionsQuery.done(function (response) { return _this.typePermissions(response); });
            this.typePermissionsQuery.fail(function (error) {
                log.error("Error occurred getting render model for type " + c.state.typeName, error);
            });
        };
        PanelEditor.prototype.updateRecordValues = function (panelItem, metadata) {
            var _this = this;
            if (this.recordQuery) {
                this.recordQuery.abort();
            }
            if (!panelItem) {
                return;
            }
            var record = panelItem.record;
            if (!panelItem.state.requiresUpdate) {
                this.setDefaultValues(metadata, record);
                this.populateObservables(this.getPropertiesFromMetadata(metadata, panelItem.state.isCreate, true), this.current, record);
                this.recordLoaded(true);
                return;
            }
            var toExpand = metadata.expandsRequiredForEdit.join(',');
            if (toExpand.length === 0) {
                toExpand = Object.keys(metadata.properties)
                    .map(function (key) { return metadata.properties[key]; })
                    .filter(function (property) { return property._isType || property._isCollection; })
                    .map(function (property) { return property.name; })
                    .join(',');
            }
            var dataService = metadataService.getDataService(panelItem.state.dataService);
            var id = metadata.properties['Id'].type == 'string'
                ? sprintf("Id='%s'", bwf.filterEncode(record.Id))
                : "Id=" + record.Id;
            var url = sprintf("%s/Query/%ss?$top=1&$filter=%s&$expand=%s", dataService.url, panelItem.state.typeName, id, toExpand);
            this.recordQuery = $.ajax({
                url: url,
                xhrFields: { withCredentials: true },
            });
            this.recordQuery.done(function (response) {
                var record = response.Records[0];
                panelItem.state.requiresUpdate = false;
                // we need to fetch the record to make sure we have the latest values
                // for the copy, but after that we don't want to accidentally save
                // any changed values back into the copied object. We also want to 
                // apply the appropriate action to any type or collection properties 
                if (panelItem.state.action.toLowerCase() === 'copy') {
                    _this.applyCopyBehaviour(record, metadata);
                }
                _this.populateObservables(_this.getPropertiesFromMetadata(metadata, panelItem.state.isCreate, true), _this.current, record);
                _this.recordLoaded(true);
            });
        };
        PanelEditor.prototype.applyCopyBehaviour = function (record, metadata) {
            record.Id = 0;
            var properties = Object.keys(metadata.properties)
                .map(function (key) { return metadata.properties[key]; });
            var internalApply = function (items, ignoreAction, resetAction) {
                items.forEach(function (property) {
                    switch (property.copyBehaviour.toLowerCase()) {
                        case "ignore":
                            ignoreAction(property.name);
                            break;
                        case "resetids":
                            resetAction(property.name);
                            break;
                        case "custom":
                            var customCopy = new Function("record", "name", "bwf", property.customCopyScript);
                            customCopy(record, property.name, bwf);
                            break;
                    }
                });
            };
            internalApply(properties.filter(function (p) { return p._isCollection; }), function (name) { return record[name] = []; }, function (name) { return record[name].forEach(function (c) { return c.Id = 0; }); });
            internalApply(properties.filter(function (p) { return p._isType; }), function (name) { return record[name] = null; }, function (name) { return record[name].Id = 0; });
        };
        PanelEditor.prototype.setDefaultValues = function (metadata, record) {
            Object.keys(metadata.properties)
                .map(function (key) { return metadata.properties[key]; })
                .forEach(function (property) {
                if (record[property.name] === undefined) {
                    record[property.name] = property._isType
                        ? null
                        : bwf.clone(property.defaultValue);
                }
            });
        };
        PanelEditor.prototype.writeCurrentValuesToRecord = function () {
            var _this = this;
            var metadata = this.metadata();
            Object.keys(metadata.properties)
                .map(function (key) { return metadata.properties[key]; })
                .forEach(function (property) {
                var key = (property._isType || property.isEnum)
                    ? 'selected' + property.name
                    : property.name;
                if (!_this.current.observables[key])
                    return;
                var value = _this.current.observables[key]();
                if ((value != null) && property.type === 'time') {
                    value = datetimeUtils.reapplyUserTZForSaving(value, options.derivedTimezone);
                }
                if ((value != null) && property.type === 'date') {
                    value = datetimeUtils.applyUtcTZToDate(value, options.derivedTimezone, 'YYYY-MM-DDT00:00:00Z');
                }
                if ((value == null) && !property.isNullable)
                    value = bwf.clone(property.defaultValue);
                if ((value === '' || value == null) && (property._isType || property.isNullable))
                    value = null;
                _this.current.record[property.name] = value;
            });
        };
        PanelEditor.prototype.saveClick = function () {
            this.writeCurrentValuesToRecord();
            this.save(this.current, this.metadata(), this.errorMessages, this.close);
        };
        PanelEditor.prototype.clearSubscriptions = function () {
            this.subscriptions.forEach(function (sub) { return sub.dispose(); });
            this.subscriptions = [];
        };
        PanelEditor.prototype.createSubscriptions = function (m, c) {
            var _this = this;
            this.clearSubscriptions();
            var properties = this.getPropertiesFromMetadata(m, c.state.isCreate);
            this.createObservables(m, c, this.formDisabled, this.createObservablesForChoice, function () { return _this.setupValidation(properties, _this.current); });
            properties.forEach(function (property) {
                var subscription = _this.createSubscription(property, c);
                // if the property requires a custom control there will be no subscription
                if (subscription)
                    _this.subscriptions.push(subscription);
            });
        };
        PanelEditor.prototype.getComponentName = function (property) {
            if (property.useCustomControl) {
                this.subscriptions.push(this.listenForChangesTo(property.name, this.current));
                return property.customControl;
            }
            return this.getComponentNameFromProperty(property);
        };
        return PanelEditor;
    }(basePanelEditor.BasePanelEditor));
    return PanelEditor;
});
