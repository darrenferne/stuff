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
define(["require", "exports", "knockout", "options", "sprintf", "modules/bwf-metadata", "modules/bwf-valueParser", "modules/bwf-gridUtilities", "modules/bwf-globalisation"], function (require, exports, ko, options, sprintf, metadataService, valueParser, util, globalisation) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var ParameterControlType = /** @class */ (function () {
        function ParameterControlType(value) {
            this.value = value;
        }
        ParameterControlType.prototype.toString = function () {
            return this.value;
        };
        // permitted values - only controls that match the IParameterControl interface should be added here
        ParameterControlType.kendoMultiSelect = new ParameterControlType("kendoMultiSelect");
        ParameterControlType.kendoDropDownList = new ParameterControlType("kendoDropDownList");
        return ParameterControlType;
    }());
    exports.ParameterControlType = ParameterControlType;
    var ParameterViewModelBase = /** @class */ (function () {
        function ParameterViewModelBase(params) {
            var _this = this;
            this.dateTimePatterns = globalisation.getCultureDateTimeFormats(options.formattingCulture, options.formattingCultures);
            this.datePatterns = globalisation.getCultureDateFormats(options.formattingCulture, options.formattingCultures);
            this.isSingle = false;
            this.newItemText = '';
            this.newItemValue = '';
            this.operator = '=';
            this.subscriptions = [];
            this.previousValues = ko.observableArray([]);
            this.values = ko.observableArray([]);
            // The reason for extending 'selectedValues' is to fix an issue where parameters wouldn't appear
            // when first loading the parameter component. They were in the filter, but were not visible to the 
            // user. The reason this works is because the array only notifies it's subscribers after the timeout
            // which gives the page more time to load in the parameters before firing the first update
            // and the subscriber missing the next update.
            this.selectedValues = ko.observableArray([])
                .extend({ rateLimit: { timeout: 50, method: 'notifyWhenChangesStop' }, notify: 'always' });
            var parameter = params.parameter;
            this.metadata = params.metadata;
            this.includeEmptyText = options.resources["bwf_include_empty"] + "?";
            this.controlId = sprintf.sprintf("%s-selectedValuesFor-%s", params.viewGridId, parameter.Id);
            this.field = parameter.Parameter;
            this.id = parameter.Id;
            this.inEditMode = params.inEditMode;
            var lowerOperator = parameter.Operator.toLowerCase();
            var isSingleEquals = lowerOperator === "=" &&
                !(this.metadata.parameterAvailableOperators == null ||
                    this.metadata.parameterAvailableOperators.some(function (op) { return op.Value === "in"; }));
            var isSingleNotEquals = lowerOperator === "!=" &&
                !(this.metadata.parameterAvailableOperators == null ||
                    this.metadata.parameterAvailableOperators.some(function (op) { return op.Value === "notIn"; }));
            this.isSingle = lowerOperator.indexOf('<') > -1 || lowerOperator.indexOf('>') > -1 || isSingleEquals || isSingleNotEquals;
            this.isLike = lowerOperator === 'like' || lowerOperator === 'notlike';
            this.operator = parameter.Operator;
            switch (lowerOperator) {
                case 'notlike':
                    this.title = sprintf.sprintf('%s %s', parameter.Alias || this.metadata.abbreviatedName, 'not like');
                    break;
                default:
                    this.title = sprintf.sprintf('%s %s', parameter.Alias || this.metadata.abbreviatedName, lowerOperator);
                    break;
            }
            this.type = this.metadata.type;
            this.startEditing = params.startEditing;
            this.viewGridId = params.viewGridId;
            if (parameter.AllowNullOrEmpty == null) {
                this.allowNullOrEmpty = (this.metadata.parameterAllowNullOrEmpty == null) ? true : this.metadata.parameterAllowNullOrEmpty;
            }
            else {
                this.allowNullOrEmpty = parameter.AllowNullOrEmpty;
            }
            this.includeEmpty = ko.observable(this.allowNullOrEmpty);
            var topic = sprintf.sprintf('%s-parameter-%s-include-empty', this.viewGridId, this.field);
            this.subscriptions.push(this.includeEmpty.subscribe(function (include) { return ko.postbox.publish(topic, include); }));
            this.subscriptions.push(ko.postbox.subscribe(topic, function (include) { return _this.includeEmpty(include); }));
        }
        ParameterViewModelBase.prototype.dispose = function () {
            this.subscriptions.forEach(function (sub) { return sub.dispose(); });
        };
        ParameterViewModelBase.prototype.addValue = function (value) {
            var matching = this.values().filter(function (i) { return i.value === value; })[0];
            if (!matching) {
                this.values.push({ text: value, value: value, used: true });
                this.selectedValues(this.selectedValues().concat([value]));
            }
            else {
                this.selectedValues(this.selectedValues().concat([value]));
            }
        };
        ParameterViewModelBase.prototype.loadValuesFromUrl = function (parameters) {
            var _this = this;
            var shouldAllowNull = parameters.some(function (p) { return _this.field === p.Property && p.AllowNull === true; });
            this.includeEmpty(shouldAllowNull);
            parameters
                .filter(function (urlParam) { return _this.field === urlParam.Property && _this.operator === urlParam.Operator; })
                .forEach(function (urlParam) {
                var values = [], start = 0, escaped = false;
                var parameterValue = urlParam.Value.toString();
                // parses semi-colon seperated parameter values from a string 
                // into seperate values
                for (var c = 0; c <= parameterValue.length; c++) {
                    var char = parameterValue[c];
                    if (escaped) {
                        escaped = false;
                    }
                    else if (char === '\\') {
                        escaped = true;
                    }
                    else if (char === ';' || char === "'" || c === parameterValue.length) {
                        if (c - start === 0) {
                            start++;
                            continue;
                        }
                        var toAdd = _this.isLike
                            ? decodeURIComponent(parameterValue.substr(start, c - start))
                            : parameterValue.substr(start, c - start);
                        values.push(toAdd);
                        start = c + 1;
                    }
                }
                values.forEach(function (value) { return _this.addValue(decodeURIComponent(value)); });
            });
        };
        ParameterViewModelBase.prototype.resetToPrevious = function () {
            this.selectedValues(this.previousValues());
            this.includeEmpty(this.previousEmpty);
        };
        ParameterViewModelBase.prototype.savePreviousValues = function () {
            this.previousEmpty = this.includeEmpty();
            this.previousValues(this.selectedValues());
        };
        ParameterViewModelBase.prototype.toggleIncludeEmpty = function () {
            var current = this.includeEmpty();
            this.includeEmpty(!current);
        };
        return ParameterViewModelBase;
    }());
    exports.ParameterViewModelBase = ParameterViewModelBase;
    var ParameterViewModel = /** @class */ (function (_super) {
        __extends(ParameterViewModel, _super);
        function ParameterViewModel(params, controlType) {
            var _this = _super.call(this, params) || this;
            _this.controlType = controlType;
            _this.postRender = function () {
                _this.setupEventHandler();
                params.ready(_this);
            };
            if (!_this.isLike)
                _this.populateValues();
            return _this;
        }
        ParameterViewModel.prototype.getControl = function () {
            var control = $(document.getElementById(this.controlId)).data(this.controlType.toString());
            return control;
        };
        ParameterViewModel.prototype.focus = function () {
            if (this.inEditMode())
                return true;
            this.startEditing();
            var control = this.getControl();
            if (control) {
                control.focus();
                control.open();
            }
        };
        ParameterViewModel.prototype.dispose = function () {
            var list = $(document.getElementById(this.controlId));
            var control = this.getControl();
            if (control)
                control.destroy();
            if (list)
                list.remove();
            _super.prototype.dispose.call(this);
        };
        ParameterViewModel.prototype.populateValues = function () {
            var _this = this;
            if (this.metadata.type === 'boolean') {
                this.values.push({
                    text: options.resources['no'],
                    value: 'false',
                    used: true
                });
                this.values.push({
                    text: options.resources['yes'],
                    value: 'true',
                    used: true
                });
            }
            if (this.metadata.type === 'enum' ||
                (!!this.metadata.parameterQuery && !!this.metadata.parameterDisplayProperty)) {
                var dataService = this.metadata.parameterQueryDataService || this.metadata.dataService;
                var url = metadataService.getDataService(dataService).url;
                var parameterQuery = this.metadata.type === 'enum'
                    ? sprintf.sprintf('%s/enumerations/%s', url, this.metadata._clrType)
                    : sprintf.sprintf('%s/Query/%s', url, this.metadata.parameterQuery);
                var query = $.ajax({
                    xhrFields: { withCredentials: true },
                    url: parameterQuery
                });
                query.done(function (queryResponse) {
                    var records = [];
                    if (queryResponse instanceof Array) {
                        records = queryResponse;
                    }
                    else {
                        records = queryResponse.Records;
                    }
                    var parameterValues = records.map(function (record) {
                        switch (_this.metadata.type) {
                            case 'download':
                            case 'link':
                                var linkValue = record[_this.metadata.parameterDisplayProperty];
                                return { text: linkValue.Text, value: linkValue.Text, used: true };
                            case 'enum':
                                return { text: record.Text, value: record.Value, used: true };
                            default:
                                var value = record[_this.metadata.parameterDisplayProperty];
                                return { text: value, value: value, used: true };
                        }
                    });
                    parameterValues = parameterValues
                        .filter(function (value, index, source) {
                        return util.Array.findIndex(source, function (r) { return r.text === value.text; }) === index;
                    })
                        .filter(function (r) { return !!r.text; });
                    _this.values.remove(function (v) { return !v.used; });
                    ko.utils.arrayPushAll(_this.values, parameterValues);
                });
            }
        };
        ParameterViewModel.prototype.parseValueIntoOption = function (text) {
            var type = this.type;
            var parsedText = valueParser.parseValue(type, text, this.metadata.format, options.formattingCulture, options.formattingCulture, this.dateTimePatterns, this.datePatterns, options.dateTimeDisplayFormat, options.dateDisplayFormat);
            if (parsedText === null || text === "")
                return null;
            return { text: parsedText, value: parsedText, used: false };
        };
        ParameterViewModel.prototype.setupEventHandler = function () {
            var type = this.metadata.type;
            var isLike = this.operator === 'like' || this.operator === 'notLike';
            if (type === 'boolean' || (!isLike && !this.metadata.isFreeFormat && type === 'string'))
                return;
            var control = this.getControl();
            var self = this;
            if (control) {
                self.selectedValues.subscribe(function (selected) {
                    self.values().forEach(function (v) {
                        if (selected.some(function (s) { return s === v.value; }))
                            v.used = true;
                    });
                });
                // `this` will be the kendo control inside this event handler
                control.bind('dataBound', function () {
                    var text = this._prev;
                    if (text === self.newItemText)
                        return;
                    self.newItemText = text;
                    self.newItemValue = text;
                    self.values.remove(function (i) { return !i.used; });
                    var option = self.parseValueIntoOption(text);
                    if (option == null)
                        return;
                    self.values.push(option);
                    this.refresh();
                    this.search(option.text);
                    this.open();
                });
            }
        };
        return ParameterViewModel;
    }(ParameterViewModelBase));
    exports.ParameterViewModel = ParameterViewModel;
    var HiddenParameterViewModel = /** @class */ (function (_super) {
        __extends(HiddenParameterViewModel, _super);
        function HiddenParameterViewModel() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        return HiddenParameterViewModel;
    }(ParameterViewModelBase));
    exports.HiddenParameterViewModel = HiddenParameterViewModel;
});
