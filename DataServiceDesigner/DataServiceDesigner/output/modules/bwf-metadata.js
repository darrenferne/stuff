define(["require", "exports", "jquery", "loglevel", "options", "modules/bwf-utilities", "modules/bwf-knockout-validators"], function (require, exports, $, log, options, bwf, koValidators) {
    "use strict";
    var DummyGridColumnMetadata = /** @class */ (function () {
        function DummyGridColumnMetadata() {
            this.abbreviatedName = '';
            this.additionalDescriptions = [];
            this.alignment = 'left';
            this.customDisplayCell = undefined;
            this.customEditingCell = undefined;
            this.useCustomDisplayCell = false;
            this.useCustomEditingCell = false;
            this.defaultFormat = '';
            this.hasChoice = false;
            this.isNotEditableInGrid = true;
            this.isMandatoryInEditMode = false;
            this.isNullable = true;
            this.description = '';
            this.isDisabledInEditMode = true;
            this.isDisabledInCreateMode = true;
        }
        return DummyGridColumnMetadata;
    }());
    var MetadataProperty = /** @class */ (function () {
        function MetadataProperty() {
        }
        MetadataProperty.processMetadataItem = function (item, dataServiceName, identityProperties, selectorFields) {
            var hasChoice = function (item) {
                return item.PropertyType.toLowerCase() === 'enum' || item.PropertyType.toLowerCase() === 'type'
                    || (item.PopulateChoiceQuery && item.PopulateChoiceQuery.trim().length > 0)
                    || (item.PopulateChoiceUrl && item.PopulateChoiceUrl.trim().length > 0);
            };
            var displayName = item.DisplayName;
            var isType = item.PropertyType.toLowerCase() === "type";
            return {
                abbreviatedName: (item.AbbreviatedName == null || item.AbbreviatedName === '') ? displayName : item.AbbreviatedName,
                additionalDescriptions: item.AdditionalDescriptions || [],
                alignment: MetadataProperty.getAlignment(item),
                copyBehaviour: item.CopyBehaviour || "Ignore",
                customControl: item.CustomControl || "",
                customControlHeight: item.CustomControlHeight || 0,
                isCustomControlHeightAuto: item.IsCustomControlHeightAuto || false,
                customControlParameter: item.CustomControlParameter || "",
                customCopyScript: item.CustomCopyScript || "",
                customDisplayCell: item.CustomDisplayCell || "",
                customEditingCell: item.CustomEditingCell || "",
                dataService: isType ? (item.FromDataService || dataServiceName) : dataServiceName,
                defaultFormat: MetadataProperty.getDefaultFormat(item),
                defaultValue: MetadataProperty.getDefaultValue(item.DefaultValue, item.IsNullable),
                description: item.Description || '',
                displayFieldInEditorChoice: item.DisplayFieldInEditorChoice || "Text",
                displayName: displayName,
                filteredOn: item.FilteredOn || [],
                format: item.Format,
                fullName: item.Name,
                editingName: item.EditingName,
                hasChoice: hasChoice(item),
                heightInLines: item.HeightInLines || 1,
                identityProperties: isType ? identityProperties : [],
                isEnum: item.PropertyType.toLowerCase() === 'enum',
                isFreeFormat: MetadataProperty.getIsFreeFormat(item) || false,
                isHidden: item.IsHidden || false,
                isHiddenInEditor: item.IsHiddenInEditor || false,
                isMandatoryInEditMode: item.IsMandatoryInEditMode || false,
                isNotEditableInGrid: item.IsNotEditableInGrid || false,
                isNotEditableInPanel: item.IsNotEditableInPanel || false,
                isNotCreatableInPanel: item.IsNotCreatableInPanel || false,
                isNullable: item.IsNullable,
                name: item.Name,
                parameterAvailableOperators: item.ParameterAvailableOperators,
                parameterDefaultValue: MetadataProperty.getDefaultValue(item.ParameterDefaultValue, item.ParameterDefaultValue == null),
                parameterAllowNullOrEmpty: (item.ParameterAllowNullOrEmpty === false) ? false : true,
                parameterDisplayProperty: MetadataProperty.getParameterDisplayProperty(item),
                parameterQuery: MetadataProperty.getParameterQuery(item),
                parameterQueryDataService: item.ParameterQueryDataService || '',
                populateChoiceUrl: item.PopulateChoiceUrl || '',
                populateChoiceQuery: item.PopulateChoiceQuery || '',
                positionInEditor: item.PositionInEditor || 0,
                refreshChoiceOnChangesTo: item.RefreshChoiceOnChangesTo || null,
                isSelectorField: selectorFields.filter(function (s) { return s === item.Name; }).length === 1,
                type: item.PropertyType.toLowerCase(),
                useCustomControl: item.UseCustomControl || false,
                useCustomDisplayCell: item.UseCustomDisplayCell || false,
                useCustomEditingCell: item.UseCustomEditingCell || false,
                valueFieldInEditorChoice: item.ValueFieldInEditorChoice || "Value",
                isDisabledInEditMode: item.IsDisabledInEditMode || false,
                isDisabledInCreateMode: item.IsDisabledInCreateMode || false,
                isHiddenInEditMode: item.IsHiddenInEditMode || false,
                isHiddenInCreateMode: item.IsHiddenInCreateMode || false,
                _abbreviatedWasEmpty: (item.AbbreviatedName == null || item.AbbreviatedName === ''),
                _clrType: item.TypeName,
                _isCollection: item.PropertyType.toLowerCase() === 'collection',
                _isType: isType
            };
        };
        MetadataProperty.getDefaultValue = function (defaultValue, isNullable) {
            if (isNullable === void 0) { isNullable = false; }
            if (isNullable)
                return null;
            var value = defaultValue.indexOf('{') > -1 ? JSON.parse(defaultValue) : eval(defaultValue);
            return value;
        };
        MetadataProperty.getDefaultFormat = function (item) {
            var vo = {
                message: "",
                allowNull: false
            };
            var format = item.DefaultFormat;
            switch (item.PropertyType.toLowerCase()) {
                case 'date':
                    return (format != null && koValidators.isValidDateFormat(format, vo)) ? format : '';
                case 'time':
                    return (format != null && koValidators.isValidTimeFormat(format, vo)) ? format : '';
                case 'integer':
                    return (format === '' || format == null) ? 'n0' : format;
                case 'numeric':
                case 'measure':
                    return (format === '' || format == null) ? 'n4' : format;
                default:
                    return '';
            }
        };
        MetadataProperty.getAlignment = function (item) {
            switch (item.PropertyType.toLowerCase()) {
                case 'boolean':
                    return 'center';
                case 'integer':
                case 'numeric':
                case 'measure':
                    return 'right';
                default:
                    return 'left';
            }
        };
        MetadataProperty.getIsFreeFormat = function (item) {
            switch (item.PropertyType.toLowerCase()) {
                case 'boolean':
                case 'date':
                case 'integer':
                case 'numeric':
                case 'measure':
                case 'time':
                    return false;
                default:
                    return item.IsFreeFormat;
            }
        };
        MetadataProperty.getParameterDisplayProperty = function (item) {
            switch (item.PropertyType.toLowerCase()) {
                case 'boolean':
                case 'date':
                case 'integer':
                case 'numeric':
                case 'time':
                    return '';
                default:
                    return item.ParameterDisplayProperty;
            }
        };
        MetadataProperty.getParameterQuery = function (item) {
            switch (item.PropertyType.toLowerCase()) {
                case 'boolean':
                case 'date':
                case 'integer':
                case 'numeric':
                case 'time':
                    return '';
                default:
                    return item.ParameterQuery;
            }
        };
        return MetadataProperty;
    }());
    var PropertyProxy = /** @class */ (function () {
        function PropertyProxy(proxiedProperty, overrides, prefix, displayPrefix, parentDescription, path, allowNullOrEmpty, isSelector, choiceQuery, choiceUrl, refreshChoiceOnChangesTo) {
            if (isSelector === void 0) { isSelector = false; }
            if (choiceQuery === void 0) { choiceQuery = ''; }
            if (choiceUrl === void 0) { choiceUrl = ''; }
            if (refreshChoiceOnChangesTo === void 0) { refreshChoiceOnChangesTo = ''; }
            var _this = this;
            this.isMandatoryInEditMode = false;
            this._isProxy = true;
            this.metadata = bwf.clone(proxiedProperty);
            Object.keys(overrides).forEach(function (key) {
                _this.metadata[key] = bwf.clone(overrides[key]);
            });
            this.isMandatoryInEditMode = this.metadata.isMandatoryInEditMode;
            this._prefix = prefix;
            this._displayPrefix = displayPrefix;
            this._isSelector = isSelector;
            this._isPropertyOnChild = path.split('/').length > 1;
            this._parentDescription = parentDescription;
            this._choiceQuery = choiceQuery;
            this._choiceUrl = choiceUrl;
            this._refreshChoiceOnChangesTo = refreshChoiceOnChangesTo;
            this.allowNullOrEmpty = allowNullOrEmpty;
        }
        Object.defineProperty(PropertyProxy.prototype, "abbreviatedName", {
            get: function () {
                return this._displayPrefix + this.metadata._abbreviatedWasEmpty
                    ? this.displayName
                    : this.metadata.abbreviatedName;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "displayName", {
            get: function () {
                if (this._displayPrefix !== '' && this.metadata.displayName.toLowerCase() === 'name') {
                    return this._displayPrefix;
                }
                else {
                    return this._displayPrefix + this.metadata.displayName;
                }
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "description", {
            get: function () {
                return this.metadata.description
                    ? this.metadata.description
                    : this._parentDescription;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isNotEditableInGrid", {
            get: function () {
                return this.metadata.isNotEditableInGrid || (this._isPropertyOnChild && !this._isSelector);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "alignment", {
            get: function () { return this.metadata.alignment; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "additionalDescriptions", {
            get: function () { return this.metadata.additionalDescriptions; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "copyBehaviour", {
            get: function () { return this.metadata.copyBehaviour; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "customControl", {
            get: function () { return this.metadata.customControl; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "customControlHeight", {
            get: function () { return this.metadata.customControlHeight; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isCustomControlHeightAuto", {
            get: function () { return this.metadata.isCustomControlHeightAuto; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "customControlParameter", {
            get: function () { return this.metadata.customControlParameter; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "customCopyScript", {
            get: function () { return this.metadata.customCopyScript; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "customDisplayCell", {
            get: function () { return this.metadata.customDisplayCell; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "customEditingCell", {
            get: function () { return this.metadata.customEditingCell; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "dataService", {
            get: function () { return this.metadata.dataService; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "defaultFormat", {
            get: function () { return this.metadata.defaultFormat; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "defaultValue", {
            get: function () { return this.metadata.defaultValue; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "displayFieldInEditorChoice", {
            get: function () { return this.metadata.displayFieldInEditorChoice; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "editingName", {
            get: function () { return this.metadata.editingName; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "filteredOn", {
            get: function () { return this.metadata.filteredOn; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "format", {
            get: function () { return this.metadata.format; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "fullName", {
            get: function () { return this._prefix + this.metadata.fullName; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "hasChoice", {
            get: function () { return this.metadata.hasChoice || this.isSelectorField; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "heightInLines", {
            get: function () { return this.metadata.heightInLines; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "identityProperties", {
            get: function () { return this.metadata.identityProperties; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isEnum", {
            get: function () { return this.metadata.isEnum; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isFreeFormat", {
            get: function () { return this.metadata.isFreeFormat; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isHidden", {
            get: function () { return this.metadata.isHidden; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isHiddenInEditor", {
            get: function () { return this.metadata.isHiddenInEditor; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isNotEditableInPanel", {
            get: function () { return this.metadata.isNotEditableInPanel; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isNotCreatableInPanel", {
            get: function () { return this.metadata.isNotCreatableInPanel; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isNullable", {
            get: function () { return this.metadata.isNullable; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "name", {
            get: function () { return this.metadata.name; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "parameterAllowNullOrEmpty", {
            get: function () { return this.allowNullOrEmpty; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "parameterAvailableOperators", {
            get: function () { return this.metadata.parameterAvailableOperators; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "parameterDefaultValue", {
            get: function () { return this.metadata.parameterDefaultValue; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "parameterDisplayProperty", {
            get: function () { return this.metadata.parameterDisplayProperty; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "parameterQuery", {
            get: function () { return this.metadata.parameterQuery; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "parameterQueryDataService", {
            get: function () { return this.metadata.parameterQueryDataService; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "populateChoiceUrl", {
            get: function () { return this.metadata.populateChoiceUrl || this._choiceUrl; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "populateChoiceQuery", {
            get: function () { return this.metadata.populateChoiceQuery || this._choiceQuery; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "positionInEditor", {
            get: function () { return this.metadata.positionInEditor; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "refreshChoiceOnChangesTo", {
            get: function () { return this.metadata.refreshChoiceOnChangesTo || this._refreshChoiceOnChangesTo; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isSelectorField", {
            get: function () { return this._isSelector || this.metadata.isSelectorField; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "type", {
            get: function () { return this.metadata.type; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "useCustomControl", {
            get: function () { return this.metadata.useCustomControl; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "useCustomDisplayCell", {
            get: function () { return this.metadata.useCustomDisplayCell; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "useCustomEditingCell", {
            get: function () { return this.metadata.useCustomEditingCell; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "valueFieldInEditorChoice", {
            get: function () { return this.metadata.valueFieldInEditorChoice; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isDisabledInEditMode", {
            get: function () { return this.metadata.isDisabledInEditMode; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isDisabledInCreateMode", {
            get: function () { return this.metadata.isDisabledInCreateMode; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isHiddenInEditMode", {
            get: function () { return this.metadata.isHiddenInEditMode; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "isHiddenInCreateMode", {
            get: function () { return this.metadata.isHiddenInCreateMode; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "_abbreviatedWasEmpty", {
            get: function () { return this.metadata._abbreviatedWasEmpty; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "_clrType", {
            get: function () { return this.metadata._clrType; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "_isCollection", {
            get: function () { return this.metadata._isCollection; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PropertyProxy.prototype, "_isType", {
            get: function () { return this.metadata._isType; },
            enumerable: true,
            configurable: true
        });
        return PropertyProxy;
    }());
    var TypeMetadata = /** @class */ (function () {
        function TypeMetadata(record, properties) {
            this.autoUpdatesByDefault = record.AutoUpdatesByDefault;
            this.dataService = record.DataServiceName;
            this.displayName = record.DisplayName;
            this.expandsRequiredForEdit = record.ExpandsRequiredForEdit == null
                ? []
                : record.ExpandsRequiredForEdit.split(',');
            this.hasCompositeId = record.HasCompositeId || false;
            this.hasEditabilityToRoles = record.HasEditabilityToRoles;
            this.hasVisibilityToRoles = record.HasVisibilityToRoles;
            this.identityProperties = record.IdentityProperties;
            this.identificationSummaryFields = record.IdentificationSummaryFields;
            this.properties = properties;
            this.pluralisedDisplayName = record.PluralisedDisplayName || record.DisplayName;
            this.selectorFields = record.SelectorFields;
            this.supportsEditMode = record.SupportsEditMode;
            this.type = record.Type;
            this.insertableInEditMode = !(record.InsertionDisabledInEditMode);
            this.deletableInEditMode = !(record.DeletionDisabledInEditMode);
            this.resourcefulRoute = record.ResourcefulRoute;
            this.usesTypesFromOtherDataServices = record.UsesTypesFromOtherDataServices;
            this.useCombinedQuerier = record.UseCombinedQuerier;
            this.supportsAggregations = record.SupportsAggregations;
        }
        return TypeMetadata;
    }());
    var DataService = /** @class */ (function () {
        function DataService(name, url, existingPromises) {
            var _this = this;
            this.name = name;
            this.url = url;
            this.hostUrl = "https://" + url.split('/')[2];
            this.types = {};
            this.promises = existingPromises || {};
            this.requests = {};
            Object.keys(this.promises).forEach(function (key) { return _this.getType(key); });
        }
        DataService.prototype.processMetadata = function (response) {
            var _this = this;
            var transform = function (record) {
                var properties = {};
                var dataServiceName = record ? record.DataServiceName : "";
                var identityProperties = record ? record.IdentityProperties || [] : [];
                var selectorFields = record ? record.SelectorFields || [] : [];
                record.Properties.forEach(function (item) {
                    var p = MetadataProperty.processMetadataItem(item, dataServiceName, identityProperties, selectorFields);
                    properties[p.fullName] = p;
                    // TODO: remove this when grid editable link types is sorted
                    if (p.type === 'link')
                        p.isNotEditableInGrid = true;
                });
                return new TypeMetadata(record, properties);
            };
            response.map(function (record) { return transform(record); })
                .forEach(function (tmd) { return _this.types[tmd.type.toLowerCase()] = tmd; });
        };
        DataService.prototype.fetchMetadata = function (type, deferred) {
            var _this = this;
            if (this.hasType(type)) {
                log.debug('found ' + type + ' in cache');
                deferred.resolve(this.types[type.toLowerCase()]);
                if (this.promises[type]) {
                    this.promises[type].forEach(function (promise) { return promise.resolve(_this.types[type.toLowerCase()]); });
                    this.promises[type] = [];
                }
                return;
            }
            if (this.promises[type] == null)
                this.promises[type] = [];
            this.promises[type].push(deferred);
            if (this.requests[type]) {
                log.debug('Already making a call for ' + type + ', storing promise.');
                return;
            }
            var metadataRequest = $.ajax({
                xhrFields: {
                    withCredentials: true
                },
                url: this.metadataQueryUrl(type)
            });
            this.requests[type] = metadataRequest;
            metadataRequest.done($.proxy(this.processMetadata, this))
                .done(function () {
                log.debug('query succesful, fulfilling ' + _this.promises[type].length + ' promise(s) for ' + type);
                var metadata = _this.types[type.toLowerCase()];
                _this.promises[type].forEach(function (promise) { return promise.resolve(metadata); });
                _this.promises[type] = [];
                _this.requests[type] = undefined;
            })
                .fail(function () {
                _this.requests[type] = undefined;
                deferred.reject();
            });
        };
        DataService.prototype.addType = function (tmd) {
            this.types[tmd.type.toLowerCase()] = tmd;
        };
        DataService.prototype.addOrUpdateTypes = function (records) {
            this.processMetadata(records);
        };
        DataService.prototype.addOrUpdateProperty = function (type, property) {
            var typeMetadata = this.types[type.toLowerCase()];
            if (typeMetadata != null) {
                var dataServiceName = typeMetadata.dataService;
                var identityProperties = typeMetadata.identityProperties || [];
                var selectorFields = typeMetadata.selectorFields || [];
                var p = MetadataProperty.processMetadataItem(property, dataServiceName, identityProperties, selectorFields);
                typeMetadata.properties[p.fullName] = p;
                // TODO: remove this when grid editable link types is sorted
                if (p.type === 'link')
                    p.isNotEditableInGrid = true;
            }
        };
        DataService.prototype.removeProperty = function (type, property) {
            var typeMetadata = this.types[type.toLowerCase()];
            if (typeMetadata) {
                delete typeMetadata.properties[property];
            }
        };
        DataService.prototype.setTypeProperties = function (type, properties) {
            var typeMetadata = this.types[type.toLowerCase()];
            if (typeMetadata) {
                var newProperties_1 = {};
                var dataServiceName_1 = typeMetadata.dataService;
                var identityProperties_1 = typeMetadata.identityProperties || [];
                var selectorFields_1 = typeMetadata.selectorFields || [];
                properties.forEach(function (item) {
                    var p = MetadataProperty.processMetadataItem(item, dataServiceName_1, identityProperties_1, selectorFields_1);
                    newProperties_1[p.fullName] = p;
                    // TODO: remove this when grid editable link types is sorted
                    if (p.type === 'link')
                        p.isNotEditableInGrid = true;
                });
                typeMetadata.properties = newProperties_1;
            }
        };
        DataService.prototype.hasType = function (name) {
            return this.types[name.toLowerCase()] != null;
        };
        DataService.prototype.metadataQueryUrl = function (type) {
            return this.url + '/TypeMetadata/' + type;
        };
        DataService.prototype.getType = function (name) {
            var deferred = $.Deferred();
            this.fetchMetadata(name, deferred);
            return deferred.promise();
        };
        DataService.prototype.processMetadataItem = function (property) {
            return MetadataProperty.processMetadataItem(property, this.name, [], []);
        };
        return DataService;
    }());
    var MetadataService = /** @class */ (function () {
        function MetadataService() {
            var _this = this;
            this.fetchDataServices = function (hostUrl) {
                var url = hostUrl + "/api/Explorer/Query/DataServices";
                var dsQuery = $.ajax({
                    xhrFields: {
                        withCredentials: true
                    },
                    url: url
                });
                dsQuery.fail(function (response) {
                    log.error("Couldn't get dataservices, don't expect anything to work:", response);
                });
                dsQuery.done(function (response) {
                    if (response.Records.length === 0)
                        return;
                    $.each(response.Records, function (i, record) {
                        var dataService = new DataService(record.System, record.Url, MetadataService.metadataPromises[record.System.toLowerCase()]);
                        MetadataService.dataservices[dataService.name.toLowerCase()] = dataService;
                    });
                    MetadataService.loadedDataServices = true;
                    if (MetadataService.allDataServicesPromise != null)
                        MetadataService.allDataServicesPromise.resolve(_this.getAllDataServices());
                    Object.keys(MetadataService.dataServicePromises).forEach(function (key) {
                        var promises = MetadataService.dataServicePromises[key];
                        var ds = MetadataService.dataservices[key];
                        if (ds != null) {
                            promises.forEach(function (p) { return p.resolve(ds); });
                        }
                        else {
                            log.error("Unknown dataservice: ", ds);
                            promises.forEach(function (p) { return p.reject(); });
                        }
                    });
                });
            };
            this.fetchDataServices(options.explorerHostUrl);
        }
        MetadataService.prototype.hasType = function (dataservice, typename) {
            var typeDictionary = MetadataService.dataservices[dataservice.toLowerCase()];
            if (typeDictionary != null) {
                var type = typeDictionary.types[typename.toLowerCase()];
                if (type != null) {
                    return true;
                }
            }
            log.debug('There is no metadata for type ' + dataservice + '/' + typename);
            return false;
        };
        MetadataService.prototype.hasDataService = function (dataservice) {
            var typeDictionary = MetadataService.dataservices[dataservice.toLowerCase()];
            if (typeDictionary != null) {
                return true;
            }
            return false;
        };
        MetadataService.prototype.generateNewRecord = function (metadata) {
            return this.generateNewRecordInternal(metadata, metadata.expandsRequiredForEdit);
        };
        MetadataService.prototype.generateNewRecordInternal = function (metadata, expands) {
            var _this = this;
            var record = {};
            var properties = Object.keys(metadata.properties)
                .map(function (key) { return metadata.properties[key]; });
            var currentExpands = expands.map(function (e) { return e.indexOf('/') === -1 ? e : e.substring(0, e.indexOf('/')); });
            var furtherExpands = expands.filter(function (e) { return e.indexOf('/') > -1; })
                .map(function (e) { return e.substring(e.indexOf('/') + 1); });
            properties.forEach(function (property) {
                if (property._isType) {
                    if (currentExpands.every(function (e) { return e.toLowerCase() !== property.name.toLowerCase(); })) {
                        record[property.name] = null;
                        return;
                    }
                    // if we have the metadata for the parent, we must have the metadata
                    // for the child, so this is totally OK for us to do.
                    var childMetadata = MetadataService.dataservices[metadata.dataService.toLowerCase()]
                        .types[property._clrType.toLowerCase()];
                    record[property.name] = _this.generateNewRecordInternal(childMetadata, furtherExpands);
                }
                else if (property._isCollection) {
                    record[property.name] = [];
                }
                else {
                    if (property.defaultValue != null) {
                        record[property.name] = bwf.clone(property.defaultValue);
                    }
                    else {
                        record[property.name] = null;
                    }
                }
            });
            return record;
        };
        MetadataService.prototype.getAllDataServices = function () {
            return Object.keys(MetadataService.dataservices)
                .map(function (key) { return MetadataService.dataservices[key]; });
        };
        MetadataService.prototype.getAllDataServicesSafely = function () {
            var deferred = $.Deferred();
            if (MetadataService.loadedDataServices) {
                deferred.resolve(Object.keys(MetadataService.dataservices).map(function (key) { return MetadataService.dataservices[key]; }));
            }
            else {
                if (MetadataService.allDataServicesPromise == null) {
                    MetadataService.allDataServicesPromise = deferred;
                }
                else {
                    deferred = MetadataService.allDataServicesPromise;
                }
            }
            return deferred.promise();
        };
        MetadataService.prototype.getDataService = function (dataService) {
            if (!dataService)
                return null;
            var ds = dataService.toLowerCase();
            if (this.hasDataService(ds)) {
                return MetadataService.dataservices[ds];
            }
            return null;
        };
        MetadataService.prototype.getDataServiceSafely = function (dataService) {
            var deferred = $.Deferred();
            var ds = dataService.toLowerCase();
            if (MetadataService.loadedDataServices && this.hasDataService(ds)) {
                deferred.resolve(MetadataService.dataservices[ds]);
            }
            else if (MetadataService.loadedDataServices) {
                deferred.reject();
            }
            else {
                if (!Array.isArray(MetadataService.dataServicePromises[ds]))
                    MetadataService.dataServicePromises[ds] = [];
                MetadataService.dataServicePromises[ds].push(deferred);
            }
            return deferred.promise();
        };
        MetadataService.prototype.getType = function (dataservice, typename) {
            log.debug("Getting " + dataservice + "/" + typename);
            if (!MetadataService.loadedDataServices) {
                // we don't want to reject potentially legitimate queries that happen
                // to be made before we have loaded the list of dataservices, so we 
                // need to catch any that are made here and log them so we can pass
                // them into the dataservice when we finally get it.
                var dsPromises = MetadataService.metadataPromises[dataservice.toLowerCase()];
                if (dsPromises == null)
                    dsPromises = MetadataService.metadataPromises[dataservice.toLowerCase()] = {};
                if (dsPromises[typename] == null)
                    dsPromises[typename] = [];
                log.debug('not loaded dataservices yet, storing a promise');
                var deferred = $.Deferred();
                dsPromises[typename].push(deferred);
                return deferred.promise();
            }
            var ds = null;
            if (dataservice)
                ds = MetadataService.dataservices[dataservice.toLowerCase()];
            if (!ds) {
                log.error('Unknown dataservice `%s`', dataservice);
                return $.Deferred().reject();
            }
            if (!typename) {
                log.error('Null or empty typename');
                return $.Deferred().reject();
            }
            return ds.getType(typename);
        };
        MetadataService.prototype.getProperty = function (rootObject, name, overrides) {
            var slashIndex = name.lastIndexOf('/');
            var dataservice = rootObject.dataService;
            overrides = overrides || [];
            if (slashIndex === -1) {
                var rootProperty = bwf.clone(rootObject.properties[name]);
                Object.keys(overrides).forEach(function (key) {
                    rootProperty[key] = bwf.clone(overrides[key]);
                });
                return rootProperty;
            }
            else {
                var allowNullOrEmpty = true;
                var fragments = name.split('/');
                var parent = rootObject;
                var property = rootObject.properties[fragments[0]];
                if (property.parameterAllowNullOrEmpty === false)
                    allowNullOrEmpty = false;
                var prefix = '';
                var displayPrefix = '';
                var description = property.description;
                var choiceUrl = property.populateChoiceUrl;
                var choiceQuery = property.populateChoiceQuery;
                var refreshChoiceOnChangesTo = property.refreshChoiceOnChangesTo;
                // start at 1 since we already have the root object
                for (var i = 1; i < fragments.length; i++) {
                    prefix = prefix + property.name + '/';
                    displayPrefix = displayPrefix + property.displayName + ' ';
                    parent = MetadataService.dataservices[dataservice.toLowerCase()].types[property._clrType.toLowerCase()];
                    property = parent.properties[fragments[i]];
                    if (property.parameterAllowNullOrEmpty === false)
                        allowNullOrEmpty = false;
                }
                var isSelector = parent.selectorFields.filter(function (item) { return (item === property.name); }).length === 1;
                var proxy = new PropertyProxy(property, overrides, prefix, displayPrefix, description, name, allowNullOrEmpty, isSelector, choiceQuery, choiceUrl, refreshChoiceOnChangesTo);
                return proxy;
            }
        };
        MetadataService.prototype.getAggregationProperty = function (aggMetadata, rootObject, name, overrides) {
            var dataservice = rootObject.dataService;
            overrides["isNotEditableInGrid"] = true;
            overrides["isDisabledInCreateMode"] = true;
            overrides["isNotCreatableInPanel"] = true;
            overrides["isDisabledInEditMode"] = true;
            overrides["isNotEditableInPanel"] = true;
            var propertyMetadata = this.getProperty(rootObject, name, overrides);
            var aggPropertyMetadata;
            var useResultPropertyType = aggMetadata.UseResultPropertyTypeForTypes.some(function (pt) { return pt.toLowerCase() === propertyMetadata.type; });
            if (!useResultPropertyType) {
                aggPropertyMetadata = propertyMetadata;
            }
            else {
                aggPropertyMetadata = MetadataProperty.processMetadataItem(aggMetadata.ResultPropertyType, dataservice, [], []);
                Object.keys(overrides).forEach(function (key) {
                    if (key !== "format" || useResultPropertyType) {
                        aggPropertyMetadata[key] = bwf.clone(overrides[key]);
                    }
                });
                aggPropertyMetadata.abbreviatedName = propertyMetadata.abbreviatedName;
                aggPropertyMetadata.displayName = propertyMetadata.displayName;
                aggPropertyMetadata.description = propertyMetadata.description;
                aggPropertyMetadata.fullName = propertyMetadata.fullName;
            }
            return aggPropertyMetadata;
        };
        // From a property path like 'Selection/System/Id' and a root object metadata (`View` in this 
        // example) a proxied metadata property that will have appropriate display and abbreviated 
        // names. The root type does not appear in the path, as the first fragment of the path should 
        // be a property name on the root object. 
        MetadataService.prototype.getPropertyWithPrefix = function (dataservice, rootObject, name) {
            return this.getProperty(rootObject, name, {});
        };
        MetadataService.prototype.isPropertyPathValid = function (dataService, typeMetadata, path) {
            var fragments = path.split('/');
            var last = fragments[fragments.length - 1];
            if (fragments.length > 1) {
                var parent = typeMetadata;
                var property = typeMetadata.properties[fragments[0]];
                if (!property)
                    return false;
                var prefix = '', displayPrefix = '';
                // start at 1 since we already have the root object
                for (var i = 1; i < fragments.length; i++) {
                    prefix = prefix + property.name + '/';
                    displayPrefix = displayPrefix + property.displayName + ' ';
                    parent = MetadataService.dataservices[dataService.toLowerCase()].types[property._clrType.toLowerCase()];
                    property = parent.properties[fragments[i]];
                    if (!property)
                        return false;
                }
            }
            else {
                return !!typeMetadata.properties[last];
            }
            return true;
        };
        MetadataService.prototype.isOrderByValid = function (metadata, orderBy) {
            if (!orderBy || orderBy.length === 0)
                return true;
            var splitOrderBy = orderBy.split(' ').filter(function (x) { return x != null && x.length > 0; });
            var propertyValid = this.isPropertyPathValid(metadata.dataService, metadata, splitOrderBy[0]);
            if (splitOrderBy.length === 1)
                return propertyValid;
            if (splitOrderBy.length === 2) {
                var direction = splitOrderBy[1].toLowerCase();
                var validDirection = (direction === 'asc' || direction === 'desc');
                return propertyValid && validDirection;
            }
            // if we don't have 1 or 2 items in the array we either have too many or an empty order by
            return false;
        };
        MetadataService.prototype.resourceUrl = function (metadataSource, typeMetadata) {
            return typeMetadata.resourcefulRoute ? metadataSource.hostUrl + '/' + typeMetadata.resourcefulRoute : metadataSource.url;
        };
        MetadataService.prototype.getEmptyGridColumnMetadata = function () {
            return new DummyGridColumnMetadata();
        };
        MetadataService.dataservices = {};
        MetadataService.loadedDataServices = false;
        MetadataService.allDataServicesPromise = null;
        MetadataService.metadataPromises = {};
        MetadataService.dataServicePromises = {};
        return MetadataService;
    }());
    var ms = new MetadataService();
    return ms;
});
