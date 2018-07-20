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
define(["require", "exports", "knockout", "options", "sprintf", "modules/grid/cells/cell-base", "modules/bwf-datetimeUtilities"], function (require, exports, ko, options, sprintfM, cell, datetimeUtils) {
    "use strict";
    var sprintf = sprintfM.sprintf;
    var EditableTimeCell = /** @class */ (function (_super) {
        __extends(EditableTimeCell, _super);
        function EditableTimeCell(params) {
            var _this = _super.call(this, params, true) || this;
            _this.validationSubscription.dispose();
            _this.isDate = params.column.metadata.type == 'date';
            if (params.column.metadata.format != null)
                _this.format = params.column.metadata.format;
            else if (_this.isDate && options.dateDisplayFormat)
                _this.format = options.dateDisplayFormat;
            else if (!_this.isDate && options.dateTimeDisplayFormat)
                _this.format = options.dateTimeDisplayFormat;
            else
                _this.format = params.column.metadata.defaultFormat;
            _this.isNullable = params.column.metadata.isNullable;
            if (params.column.metadata.type == 'time') {
                var parsedValue = kendo.toString(kendo.parseDate(_this.recordValue()), 'yyyy-MM-ddTHH:mm:sszzz', options.formattingCulture);
                var convertedValue = datetimeUtils.convertToTZ(parsedValue, options.derivedTimezone, 'YYYY-MM-DDTHH:mm:ss');
                _this.intermediaryValue = ko.observable(kendo.toString(kendo.parseDate(convertedValue), _this.format, options.formattingCulture));
            }
            else {
                _this.intermediaryValue = ko.observable(kendo.toString(kendo.parseDate(_this.recordValue()), _this.format, options.formattingCulture));
            }
            _this.invisibleSpacerValue = ko.pureComputed(function () { return _this.intermediaryValue() + "__"; });
            _this.message = sprintf("Input doesn't match expected format '%s'", _this.format);
            // this subscription deals with when the value is updated from another source
            // for example, if the value is a computed and has been updated
            _this.recordValueSubscription = _this.recordValue.subscribe(function (x) {
                if (!_this.isSelected() || !_this.isEditable()) {
                    var updatedRecordValue = '';
                    if (x) {
                        if (_this.column.metadata.type == 'time') {
                            var convertedValue = datetimeUtils.convertToTZ(x, options.derivedTimezone, 'YYYY-MM-DDTHH:mm:ss');
                            updatedRecordValue = kendo.toString(kendo.parseDate(convertedValue), _this.format, options.formattingCulture);
                        }
                        else {
                            var convertedValue = datetimeUtils.convertToTZ(x, 'UTC', 'YYYY-MM-DDTHH:mm:ss');
                            updatedRecordValue = kendo.toString(kendo.parseDate(convertedValue), _this.format, options.formattingCulture);
                        }
                    }
                    if (_this.value() !== updatedRecordValue)
                        _this.value(updatedRecordValue);
                }
            });
            _this.value = ko.computed({
                read: function () { return _this.intermediaryValue(); },
                write: function (newValue) {
                    _this.intermediaryValue(newValue);
                    var parsed = kendo.parseDate(newValue, [_this.format], options.formattingCulture);
                    if ((!newValue && _this.isNullable) || parsed != null) {
                        if (_this.column.metadata.type == 'time') {
                            var parsedValue = kendo.toString(parsed, 'yyyy-MM-ddTHH:mm:ss', options.formattingCulture);
                            _this.recordValue(datetimeUtils.convertFromUserTZForSavingInUtc(parsedValue, options.derivedTimezone, 'YYYY-MM-DDTHH:mm:ssZ'));
                        }
                        else {
                            _this.recordValue(kendo.toString(parsed, 'yyyy-MM-ddT00:00:00Z', options.formattingCulture));
                        }
                        _this.recordValue.validationMessages.remove(_this.message);
                        _this.isValid(true);
                        // this is a trick so that we do not post back strings which do not
                        // deserialize as dates, and we can actually get a decent error 
                        // message based on the expected format.
                        if (!_this.row.isValidateOnChangeDisabled && _this.isSelected() && _this.isEditable())
                            ko.postbox.publish(_this.gridId + "-validate-row", _this.row.bwfId);
                    }
                    else {
                        _this.isValid(false);
                        _this.recordValue(newValue);
                        if (!_this.recordValue.validationMessages().some(function (m) { return m == _this.message; }))
                            _this.recordValue.validationMessages.push(_this.message);
                    }
                }
            });
            return _this;
        }
        EditableTimeCell.prototype.dispose = function () {
            this.value.dispose();
            this.recordValueSubscription.dispose();
            _super.prototype.dispose.call(this);
        };
        return EditableTimeCell;
    }(cell.BwfWrapperCell));
    return EditableTimeCell;
});
