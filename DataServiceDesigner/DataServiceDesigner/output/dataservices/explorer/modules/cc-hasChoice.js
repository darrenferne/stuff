define(["require", "exports", "knockout", "options", "sprintf", "loglevel", "modules/bwf-gridUtilities", "modules/bwf-metadata", "modules/bwf-utilities", "bootstrapSelect"], function (require, exports, ko, options, sprintf, log, gridUtilities, metadataService, utilities, bootstrapSelect) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var bss = bootstrapSelect;
    var storage = new utilities.LocalStorageWithExpiry();
    var ChoiceControl = /** @class */ (function () {
        function ChoiceControl(params) {
            var _this = this;
            this.rendered = ko.observable(false);
            this.loaded = ko.observable(false);
            this.ready = ko.pureComputed(function () {
                var rendered = _this.rendered();
                var loaded = _this.loaded();
                return rendered && loaded;
            });
            this.hasError = ko.pureComputed(function () { return _this.property.isValid ? !_this.property.isValid() : false; });
            this.isMobile = utilities.isTouchModeEnabled;
            this.displayCreateAsMenu = false;
            this.subTypes = [];
            this.refreshOnChangesValue = {};
            this.resources = options.resources;
            this.subscriptions = [];
            this.createNew = function (subtype) {
                var ds = metadataService.getDataService(_this.propertyMetadata.dataService);
                var baseType = subtype == null || typeof subtype != 'string'
                    ? _this.propertyMetadata._clrType
                    : subtype;
                var payload = {
                    action: 'create',
                    component: 'bwf-panel-editor',
                    baseType: baseType,
                    dataService: _this.propertyMetadata.dataService,
                    dataServiceUrl: ds.hostUrl,
                    preserveStack: true,
                    onCompletion: function (params) {
                        _this.record[_this.propertyMetadata.name] = params.record;
                    }
                };
                ko.postbox.publish(_this.grid + '-doAction', payload);
            };
            this.editSelected = function () {
                _this.publishDoAction('edit');
            };
            this.viewSelected = function () {
                _this.publishDoAction('view');
            };
            this.publishDoAction = function (typeOfAction) {
                var type = _this.propertyMetadata._clrType;
                var selected = _this.available()
                    .filter(function (a) { return a[_this.valueProperty] == _this.property(); })[0];
                // We need this in case this.property() does not give us the id
                // as we pass through the id property to the payload for the publish below
                var idToUse = _this.valueProperty === "Id" ? _this.property() : selected.Id;
                // turn 'BWF.Example.DataService.Models.MainCourse, BWF.Example.DataService'
                // into 'MainCourse'
                if (selected.$type)
                    type = selected.$type.match(/\.([A-Z])\w+,/g)[0].slice(1, -1);
                var ds = metadataService.getDataService(_this.propertyMetadata.dataService);
                var payload = {
                    action: typeOfAction,
                    component: 'bwf-panel-editor',
                    baseType: type,
                    dataService: _this.propertyMetadata.dataService,
                    dataServiceUrl: ds.hostUrl,
                    toEdit: [{ Id: idToUse }],
                    preserveStack: true
                };
                ko.postbox.publish(_this.grid + '-doAction', payload);
            };
            this.name = params.metadata.name;
            this.elementId = sprintf.sprintf("%s-%s-hasChoice", params.metadata.name, window.performance.now().toFixed(0));
            this.propertyMetadata = params.metadata;
            this.label = params.metadata.editingName;
            this.property = params.model.observables[this.propertyMetadata.name];
            this.sendPowerBiTokenWithRequests = params.sendPowerBiTokenWithRequests || false;
            this.typeMetadata = params.typeMetadata;
            this.grid = params.grid;
            // undefined if this isn't a nullable type stops 'none selected' from 
            // being a selectable value in the dropdown
            this.optionsCaption = params.metadata.isNullable
                ? this.resources['bwf_none_selected']
                : undefined;
            var create = params.permissions.permissionsToCreate[this.propertyMetadata._clrType];
            var edit = params.permissions.permissionsToEdit[this.propertyMetadata._clrType];
            this.permissionToCreate = !this.propertyMetadata.isNotCreatableInPanel && (create === undefined || create);
            this.permissionToEdit = !this.propertyMetadata.isNotEditableInPanel && (edit === undefined || edit);
            var subTypes = params.permissions.subTypes[this.propertyMetadata._clrType];
            var displayNames = params.permissions.subTypeDisplayNames[this.propertyMetadata._clrType];
            if (Array.isArray(params.model.record)) {
                throw "Unexpected array";
            }
            this.record = params.model.record;
            if (Array.isArray(subTypes) && subTypes.length > 0) {
                this.displayCreateAsMenu = true;
                this.subTypes = displayNames.map(function (displayName, index) {
                    return {
                        displayName: displayName,
                        value: subTypes[index]
                    };
                });
            }
            this.canEdit = ko.pureComputed(function () {
                var p = _this.property();
                return _this.permissionToEdit && (p !== null && p !== undefined && p != false);
            });
            this.canView = ko.pureComputed(function () { return !!_this.property(); });
            this.viewButtonVisible = ko.pureComputed(function () {
                if (_this.propertyMetadata.isNotEditableInPanel)
                    return false;
                if (_this.formDisabled())
                    return true;
                return (!_this.permissionToCreate && !_this.permissionToEdit);
            });
            this.showEditButton = ko.pureComputed(function () { return !_this.formDisabled() && _this.permissionToEdit; });
            this.showAddButton = ko.pureComputed(function () { return !_this.formDisabled() && _this.permissionToCreate; });
            this.showButtonGroup = ko.pureComputed(function () { return _this.propertyMetadata._isType && (_this.showAddButton() || _this.showEditButton()); });
            this.formDisabled = ko.pureComputed(function () {
                return params.model.observables['formDisabled']()
                    || (params.model.state.isCreate && params.metadata.isDisabledInCreateMode)
                    || (!params.model.state.isCreate && params.metadata.isDisabledInEditMode);
            });
            this.enabled = ko.pureComputed(function () {
                var isDisabled = _this.formDisabled();
                var hasChoices = _this.available().length > 0;
                return !isDisabled && hasChoices;
            });
            var available = 'available' + this.name + 's';
            this.available = params.model.observables[available];
            this.valueProperty = params.metadata.valueFieldInEditorChoice;
            this.displayProperty = params.metadata.displayFieldInEditorChoice;
            if (params.metadata.refreshChoiceOnChangesTo) {
                this.refreshOnChangesTo = params.metadata.refreshChoiceOnChangesTo;
                this.refreshOnChangesTo.split(';').forEach(function (c) {
                    _this.refreshOnChangesValue['selected' + c] = params.model.observables['selected' + c];
                });
            }
            this.getAvailableChoices = this.generateQueryFunction(this.available, params.model.observables, params.model.state.isCreate);
            this.setupSubscriptions(params.model);
            params.model.observables['__renderedState'].push(this.ready);
            this.getAvailableChoices();
        }
        ChoiceControl.prototype.dispose = function () {
            if (this.query) {
                this.query.abort();
            }
            this.subscriptions.forEach(function (sub) { return sub.dispose(); });
        };
        ChoiceControl.prototype.currentChoiceIsValid = function (items) {
            var _this = this;
            var current = this.property();
            if (current == null || items == null)
                return true;
            var matching = items.filter(function (v) { return v[_this.propertyMetadata.valueFieldInEditorChoice] == current; });
            return !(items.length == 0 || matching.length == 0);
        };
        ChoiceControl.prototype.getObservablesFromFilteredOn = function (observables) {
            var _this = this;
            // array of arrays, sub array is as follows
            // [0] = property name, eg "System"
            // [1] = path of the property to compare to, "System/System"
            // [2] = observable for the property named in [0] so that the latest values
            //       are fetched each time the filter function is called
            return this.propertyMetadata.filteredOn.map(function (f) {
                var pieces = f.split(',');
                var dataService = _this.typeMetadata.dataService;
                var path = sprintf.sprintf('%s/%s', _this.propertyMetadata.name, pieces[1]);
                return {
                    property: pieces[0],
                    path: pieces[1],
                    value: observables[pieces[0]],
                    metadata: metadataService.getPropertyWithPrefix(dataService, _this.typeMetadata, path)
                };
            });
        };
        ChoiceControl.prototype.generateFilter = function (observables) {
            if (!this.propertyMetadata._isType || this.propertyMetadata.filteredOn.length === 0) {
                return function () { return ''; };
            }
            var dependencies = this.getObservablesFromFilteredOn(observables);
            return function () {
                var filters = dependencies.map(function (d) {
                    var template = '';
                    switch (d.metadata.type) {
                        case 'integer':
                        case 'numeric':
                            template = "%s=%s";
                            break;
                        case 'date':
                        case 'time':
                            template = "%s=datetime(%s)";
                            break;
                        default:
                            template = "%s='%s'";
                            break;
                    }
                    return sprintf.sprintf(template, d.path, d.value());
                });
                return "&$filter=" + filters.join(' and ');
            };
        };
        ChoiceControl.prototype.generateShouldQuery = function (observables) {
            if (this.propertyMetadata.filteredOn.length === 0) {
                return function () { return true; };
            }
            var dependencies = this.getObservablesFromFilteredOn(observables);
            return function () { return dependencies.every(function (d) { return !!(d.value()); }); };
        };
        ChoiceControl.prototype.generateQueryFunction = function (availableChoices, observables, isCreate) {
            var _this = this;
            var filter = this.generateFilter(observables);
            var shouldQuery = this.generateShouldQuery(observables);
            return function () {
                var selected = _this.refreshOnChangesValue ? _this.refreshOnChangesValue : {};
                if ((selected && Object.keys(selected).some(function (key) { return selected[key]() == null; })) || !shouldQuery()) {
                    availableChoices([]);
                    _this.loaded(true);
                    return;
                }
                var queryUrl = gridUtilities.constructChoiceUrl(_this.typeMetadata, _this.propertyMetadata, selected, _this.name, options);
                if (queryUrl.indexOf('$orderby') > -1 && queryUrl.indexOf('$filter') == -1) {
                    queryUrl += filter();
                }
                log.debug(sprintf.sprintf('getting available %s from %s', _this.name, queryUrl));
                var requestOptions = {
                    url: queryUrl,
                    headers: {},
                    xhrFields: {
                        withCredentials: true
                    }
                };
                if (_this.sendPowerBiTokenWithRequests && storage.getItem('powerbitoken')) {
                    requestOptions.headers["X-PowerBi-Token"] = storage.getItem('powerbitoken');
                }
                var request = $.ajax(requestOptions);
                // check if the currently selected value is still valid given
                // the new set of options and clear it if not
                var verifyValidChoice = function (newItems) {
                    if (!_this.currentChoiceIsValid(newItems)) {
                        _this.property(null);
                    }
                    // if this isn't a nullable enum yet the value is null or 
                    // empty this is probably someone creating a new object, and 
                    // they will want their non-nullable enum to default to a sensible
                    // value, and we have to do that here because we don't yet have a 
                    // way to do per-property default values, and this is the first 
                    // place where we will know what the actual values are. 
                    if (newItems.length > 0
                        && !_this.propertyMetadata.isNullable
                        && _this.propertyMetadata.type == 'enum') {
                        var value = _this.property();
                        if (value == null || value == '')
                            _this.property(newItems[0][_this.valueProperty]);
                    }
                };
                request.done(function (response) {
                    var records = response.Records
                        ? response.Records
                        : response;
                    verifyValidChoice(records);
                    records.sort(function (a, b) {
                        var nameA = a[_this.displayProperty].toUpperCase();
                        var nameB = b[_this.displayProperty].toUpperCase();
                        if (nameA < nameB)
                            return -1;
                        if (nameA > nameB)
                            return 1;
                        return 0;
                    });
                    availableChoices(records);
                })
                    .fail(function () {
                    availableChoices([]);
                    _this.property(null);
                })
                    .always(function () {
                    _this.loaded(true);
                });
                _this.query = request;
            };
        };
        ChoiceControl.prototype.setupSubscriptions = function (model) {
            var _this = this;
            this.subscriptions.push(this.enabled.subscribe(function (enabled) {
                var element = $(document.getElementById(_this.elementId));
                if (enabled)
                    element.multiselect('enable');
                else
                    element.multiselect('disable');
            }));
            var resetWhenChanged = this.propertyMetadata.filteredOn.map(function (f) { return f.split(',')[0]; });
            if (this.refreshOnChangesTo) {
                this.refreshOnChangesTo.split(';').forEach(function (f) {
                    resetWhenChanged.push(f);
                });
            }
            // set up subscriptions so that the value is reset when a property it is
            // filtered on changes. IE, changing BaseType clears Selection
            resetWhenChanged.forEach(function (property) {
                _this.subscriptions.push(model.observables['selected' + property].subscribe(function (v) {
                    _this.getAvailableChoices();
                }));
            });
        };
        return ChoiceControl;
    }());
    exports.default = ChoiceControl;
});
