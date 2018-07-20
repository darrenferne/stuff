define(["require", "exports", "knockout", "loglevel", "sprintf", "modules/bwf-metadata", "modules/bwf-utilities", "modules/grid/parameters/parameter-base", "modules/bwf-datetimeUtilities"], function (require, exports, ko, log, sprintf, metadataService, utils, parametersTypes, datetimeUtils) {
    "use strict";
    var ParameterBarBase = /** @class */ (function () {
        function ParameterBarBase(config, assignRandomId) {
            if (assignRandomId === void 0) { assignRandomId = false; }
            var _this = this;
            this.assignRandomId = false;
            this.inEditMode = ko.observable(false);
            this.loaded = ko.observable(false);
            this.loadedParameters = ko.observable(false);
            this.rendered = ko.observable(false);
            this.renderedChildren = 0;
            this.selectedParameters = ko.observableArray([]);
            this.selectedParameterValues = ko.observableArray([]);
            this.urlParameters = ko.observableArray([]);
            this.urlFilteredBys = ko.observableArray([]);
            this.savingEnabled = ko.observable(false);
            this.parameters = ko.observableArray([]);
            this.visibleParameters = ko.observableArray([]);
            this.hiddenParameters = ko.observableArray([]);
            this.loadedAndReady = ko.pureComputed(function () {
                var loaded = _this.loaded();
                var rendered = _this.rendered();
                return loaded && rendered;
            });
            this.edit = function () {
                if (_this.inEditMode())
                    return;
                _this.visibleParameters().forEach(function (parameter) { return parameter.savePreviousValues(); });
                _this.inEditMode(true);
            };
            this.modelReadyFunc = function (vm) {
                _this.visibleParameters.push(vm);
                log.debug(sprintf.sprintf('parameter model ready for %s %s', vm.field, vm.operator));
                _this.renderedChildren++;
                if (_this.renderedChildren === _this.parameters().length) {
                    _this.onRendered();
                }
            };
            this.generateParameterModelConfig = function (parameter) {
                var typeMetadata = _this.metadata();
                var propertyMetadata = null;
                if (typeMetadata != null) {
                    propertyMetadata = metadataService.getPropertyWithPrefix(typeMetadata.dataService, typeMetadata, parameter.Parameter);
                }
                if (!propertyMetadata) {
                    log.warn("Parameter property '" + parameter.Parameter + "' could not be found in current metadata.", typeMetadata);
                    return {
                        component: 'grid/parameters/cc-dummyParameter',
                        inEditMode: _this.inEditMode,
                        metadata: null,
                        parameter: parameter,
                        viewGridId: _this.viewGridId,
                        ready: _this.modelReadyFunc,
                        startEditing: _this.edit
                    };
                }
                var component = parameter.Component;
                var isSingle = parameter.Operator.indexOf('<') > -1
                    || parameter.Operator.indexOf('>') > -1;
                var isFreeFormat = parameter.Operator === 'like'
                    || parameter.Operator === 'notLike'
                    || propertyMetadata.isFreeFormat;
                if (component == null) {
                    switch (propertyMetadata.type) {
                        case 'date':
                        case 'time':
                            component = 'grid/parameters/cc-dateTimeParameter';
                            break;
                        default:
                            if (isSingle && !isFreeFormat)
                                component = 'grid/parameters/cc-dropDownParameter';
                            component = isSingle
                                ? 'grid/parameters/cc-singleParameter'
                                : 'grid/parameters/cc-multiParameter';
                            break;
                    }
                }
                if (_this.assignRandomId)
                    parameter.Id = utils.getNextBwfId();
                return {
                    component: component,
                    inEditMode: _this.inEditMode,
                    metadata: propertyMetadata,
                    parameter: parameter,
                    viewGridId: _this.viewGridId,
                    ready: _this.modelReadyFunc,
                    startEditing: _this.edit
                };
            };
            this.generateParameterModelConfigs = ko.pureComputed(function () {
                if (_this.metadata() == null)
                    return [];
                log.debug('generating parameter model configs for ' + _this.parameters().length + ' parameters');
                return _this.parameters().map(_this.generateParameterModelConfig);
            });
            this.assignRandomId = assignRandomId;
            this.refresh = config.forceQueryRefresh;
            this.metadata = config.metadata;
            this.viewGridId = config.viewGridId;
            this.viewId = config.viewId;
            this.enableQuerying = config.enableQuerying;
            this.loadedAndReady.subscribe(function (r) { return config.parameterBarRendered(r); });
            this.selectedParameters = config.selectedParameters;
            this.selectedParameterValues = config.selectedParameterValues;
            this.urlParameters(config.urlParameters || []);
            this.urlFilteredBys(config.urlFilteredBys || []);
            this.rendered.subscribe(function (newValue) {
                ko.postbox.publish(_this.viewGridId + '-resizeRequired');
            });
            this.updateViewParamsSubscription = ko.postbox.subscribe(this.viewGridId + '-updateViewParameters', function (urlParameters) {
                _this.updateUrlParameters(urlParameters);
            });
            this.updateViewParamsSubscription = ko.postbox.subscribe(this.viewGridId + '-updateViewFilteredBy', function (urlFilteredBys) {
                _this.updateUrlFilteredBys(urlFilteredBys);
            });
        }
        ParameterBarBase.prototype.dispose = function () {
            this.loadedAndReady.dispose();
            this.updateViewParamsSubscription.dispose();
            this.generateParameterModelConfigs.dispose();
        };
        ParameterBarBase.prototype.onRendered = function () {
            log.debug('parameter bar and all parameter components rendered');
            this.rendered(true);
            this.postOnRender();
        };
        // button actions
        ParameterBarBase.prototype.apply = function () {
            var selectedValues = this.visibleParameters().concat(this.hiddenParameters())
                .map(function (item, index) {
                return {
                    id: item.id,
                    field: item.field,
                    includeEmpty: item.includeEmpty(),
                    position: (index + 1) * 100,
                    operator: item.operator,
                    type: item.type,
                    values: item.selectedValues()
                };
            }).filter(function (item) { return item.values.length > 0 || item.includeEmpty === false; });
            this.selectedParameterValues(selectedValues);
            this.saveSelectedParameterValues();
            this.inEditMode(false);
            ko.postbox.publish(this.viewGridId + '-goto-first-page');
            this.loaded(true);
        };
        ParameterBarBase.prototype.applyButton = function () {
            this.apply();
            this.enableQuerying(true);
        };
        ParameterBarBase.prototype.cancel = function () {
            this.inEditMode(false);
            this.visibleParameters().forEach(function (parameter) { return parameter.resetToPrevious(); });
        };
        ParameterBarBase.prototype.loadParameters = function () {
            log.debug('loading parameters');
            var urlParameters = this.urlParameters();
            var selectedParameters = this.selectedParameters();
            var parameters = [];
            if (urlParameters.length === 0) {
                parameters = selectedParameters
                    .sort(function (l, r) { return l.Position - r.Position; });
            }
            else {
                var index = 0;
                parameters = urlParameters.filter(function (p) { return p.Operator != null; }).map(function (urlP) {
                    var parameter = {
                        Parameter: urlP.Property,
                        Operator: urlP.Operator,
                        Position: index,
                        Id: ++index,
                        Component: urlP.Component,
                        AllowNullOrEmpty: urlP.AllowNull,
                        Alias: urlP.Alias
                    };
                    return parameter;
                });
            }
            this.renderedChildren = 0;
            this.visibleParameters([]);
            this.parameters(parameters);
            this.loadedParameters(true);
            if (parameters.length === 0)
                this.onRendered();
        };
        ParameterBarBase.prototype.updateUrlParameters = function (urlParams) {
            this.urlParameters(urlParams);
            this.savingEnabled(false);
            this.loadParameters();
        };
        ParameterBarBase.prototype.updateUrlFilteredBys = function (urlFilteredBys) {
            this.urlFilteredBys(urlFilteredBys);
            this.savingEnabled(false);
            this.loadFilteredByValues();
        };
        ParameterBarBase.prototype.loadFilteredByValues = function () {
            var _this = this;
            var parameters = this.visibleParameters();
            var filteredBys = this.urlFilteredBys();
            var indexOffset = this.visibleParameters().length + 1;
            filteredBys.filter(function (p) { return p.Operator != null; }).forEach(function (filter, i) {
                var index = indexOffset + i;
                var parameter = parameters.filter(function (p) { return p.field === filter.Property
                    && p.operator === filter.Operator; })[0];
                if (parameter) {
                    if (parameter.type === "time")
                        filter.Value.split(";").forEach(function (v) { return parameter.addValue(datetimeUtils.convertToTZ(decodeURIComponent(v), 'UTC', 'YYYY-MM-DDTHH:mm:ss')); });
                    else
                        filter.Value.split(";").forEach(function (v) { return parameter.addValue(decodeURIComponent(v)); });
                    var includeEmpty = filteredBys.some(function (f) { return f.Property === parameter.field && f.AllowNull; });
                    parameter.includeEmpty(includeEmpty);
                }
                else {
                    // load it into a hidden parameter
                    var hidden = _this.hiddenParameters()
                        .filter(function (h) { return h.field === filter.Property && h.operator === filter.Operator; })[0];
                    if (!hidden) {
                        var selected = {
                            Parameter: filter.Property,
                            Operator: filter.Operator,
                            Position: index,
                            Id: index + 1
                        };
                        hidden = new parametersTypes.HiddenParameterViewModel(_this.generateParameterModelConfig(selected));
                        _this.hiddenParameters.push(hidden);
                    }
                    filter.Value.split(";").forEach(function (v) { return hidden.addValue(decodeURIComponent(v)); });
                    var includeEmpty = filteredBys.some(function (f) { return f.Property === hidden.field && f.AllowNull; });
                    hidden.includeEmpty(includeEmpty);
                }
            });
        };
        ParameterBarBase.prototype.resetParameters = function () {
            this.hiddenParameters([]);
            this.selectedParameters([]);
            this.selectedParameterValues([]);
            this.urlParameters([]);
        };
        return ParameterBarBase;
    }());
    return ParameterBarBase;
});
