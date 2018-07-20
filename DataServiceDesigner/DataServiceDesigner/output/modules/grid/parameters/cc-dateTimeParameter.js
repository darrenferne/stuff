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
define(["require", "exports", "options", "modules/bwf-valueParser", "modules/grid/parameters/parameter-base", "knockout", "knockout-kendo", "kendo.all.min", "modules/bwf-datetimeUtilities"], function (require, exports, options, valueParser, parameters, ko, knockoutKendo, kendo, datetimeUtils) {
    "use strict";
    var kk = knockoutKendo;
    var DateTimeParameterViewModel = /** @class */ (function (_super) {
        __extends(DateTimeParameterViewModel, _super);
        function DateTimeParameterViewModel(params) {
            var _this = _super.call(this, params, parameters.ParameterControlType.kendoMultiSelect) || this;
            _this.dateTimeValue = ko.observable(null);
            _this.isValid = ko.pureComputed(function () { return _this.dateTimeValue() != null; });
            _this.pickerIsVisible = ko.pureComputed(function () { return _this.isSingle || _this.inEditMode(); });
            _this.isTime = params.metadata.type == 'time';
            if (params.metadata.format)
                _this.format = params.metadata.format;
            else if (options.dateTimeDisplayFormat)
                _this.format = options.dateTimeDisplayFormat;
            else
                _this.format = params.metadata.defaultFormat;
            _this.pickerControlId = _this.controlId + '-picker';
            if (_this.isSingle)
                _this.subscriptions.push(_this.dateTimeValue.subscribe(function (dt) { return _this.addDateTime(); }));
            return _this;
        }
        DateTimeParameterViewModel.prototype.dispose = function () {
            _super.prototype.dispose.call(this);
            var dateTimeControlType = this.isTime ? 'kendoDateTimePicker' : 'kendoDatePicker';
            var control = $(document.getElementById(this.controlId)).data(dateTimeControlType);
            if (control)
                control.destroy();
        };
        DateTimeParameterViewModel.prototype.addDateTime = function () {
            var date = this.dateTimeValue();
            if (date == null) {
                if (this.isSingle)
                    this.selectedValues([]);
                return;
            }
            var displayText = kendo.toString(date, this.format, options.formattingCulture);
            if (this.isTime) {
                var parsedValue = kendo.toString(date, 'yyyy-MM-ddTHH:mm:ss', 'en-GB');
                var filterValue = datetimeUtils.convertFromUserTZForSavingInUtc(parsedValue, options.derivedTimezone, 'YYYY-MM-DDTHH:mm:ssZ');
            }
            else {
                var filterValue = kendo.toString(date, "yyyy-MM-ddT00:00:00", 'en-GB');
            }
            this.addSelection(displayText, filterValue);
        };
        DateTimeParameterViewModel.prototype.addSelection = function (displayText, filterValue) {
            var toAdd = {
                text: displayText,
                value: filterValue,
                used: true
            };
            if (this.isSingle) {
                this.selectedValues([filterValue]);
                return;
            }
            var matching = this.values().filter(function (i) { return i.value === filterValue; })[0];
            if (!matching) {
                this.values.push(toAdd);
                this.selectedValues(this.selectedValues().concat([filterValue]));
            }
            else if (this.selectedValues().every(function (s) { return s != filterValue; })) {
                this.selectedValues(this.selectedValues().concat([filterValue]));
            }
        };
        DateTimeParameterViewModel.prototype.getDateOrNull = function (text) {
            return valueParser.parseValue(this.isTime ? 'time' : 'date', text, null, options.formattingCulture, "en-GB", this.dateTimePatterns, this.datePatterns, "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddT00:00:00");
        };
        // this is used for the options shown when a user types directly into 
        // the multiselect box
        DateTimeParameterViewModel.prototype.parseValueIntoOption = function (text) {
            var reformatted = this.getDateOrNull(text);
            if (reformatted == null)
                return null;
            return { text: reformatted, value: reformatted, used: false };
        };
        DateTimeParameterViewModel.prototype.addValue = function (value) {
            var reformatted = this.getDateOrNull(value);
            var parsed = kendo.parseDate(reformatted, [this.isTime ? "yyyy-MM-ddTHH:mm:ss" : "yyyy-MM-ddT00:00:00"], 'en-GB');
            if (this.isTime) {
                var convertedDisplay = datetimeUtils.convertToTZ(reformatted, options.derivedTimezone, 'YYYY-MM-DDTHH:mm:ss');
                var convertedDate = kendo.parseDate(convertedDisplay);
                var displayText = kendo.toString(convertedDate, this.format, options.formattingCulture);
                var filterValue = kendo.toString(parsed, 'yyyy-MM-ddTHH:mm:ssZ', 'en-GB');
                if (this.isSingle)
                    this.dateTimeValue(convertedDate);
            }
            else {
                var displayText = kendo.toString(parsed, this.format, options.formattingCulture);
                var filterValue = kendo.toString(parsed, "yyyy-MM-ddT00:00:00", 'en-GB');
                if (this.isSingle)
                    this.dateTimeValue(parsed);
            }
            this.addSelection(displayText, filterValue);
        };
        DateTimeParameterViewModel.prototype.focus = function () {
            if (this.inEditMode())
                return true;
            this.startEditing();
            var pickerContainer = $(document.getElementById(this.pickerControlId));
            var picker = this.isTime
                ? pickerContainer.data('kendoDateTimePicker')
                : pickerContainer.data('kendoDatePicker');
            picker.element.focus();
        };
        return DateTimeParameterViewModel;
    }(parameters.ParameterViewModel));
    return DateTimeParameterViewModel;
});
