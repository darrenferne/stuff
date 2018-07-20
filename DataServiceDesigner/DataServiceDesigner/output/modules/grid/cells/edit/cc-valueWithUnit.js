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
define(["require", "exports", "knockout", "modules/bwf-gridUtilities", "modules/grid/cells/cell-base", "sprintf"], function (require, exports, ko, utilities, cell, sprintf) {
    "use strict";
    var EditableValueWithUnitCell = /** @class */ (function (_super) {
        __extends(EditableValueWithUnitCell, _super);
        function EditableValueWithUnitCell(params) {
            var _this = _super.call(this, params, true) || this;
            _this.invalidValueWithUnitMessage =
                sprintf.sprintf("Value should consist of a number, optionally%s" +
                    "followed by a unit that does not contain numbers", String.fromCharCode(10));
            _this.inputValue = ko.observable(_this.recordValue() ? _this.recordValue().Value + _this.recordValue().Unit : '');
            // we dispose of the subscription so that we don't validate on change unless the 
            // value is a valid value with unit this is so we can have our own error message
            _this.validationSubscription.dispose();
            // this subscription deals with when the value is updated from another source
            // for example, if the value is a computed and has been updated
            _this.recordValueSubscription = _this.recordValue.subscribe(function (x) {
                if (!_this.isSelected()) {
                    var updatedRecordValue = x ? x.Value + x.Unit : "";
                    if (_this.inputValue() !== updatedRecordValue)
                        _this.value(updatedRecordValue);
                }
            });
            _this.value = ko.computed({
                read: function () {
                    return _this.inputValue();
                },
                write: function (input) {
                    _this.inputValue(input);
                    if (input == "") {
                        // if we have an empty input the value is null
                        _this.recordValue(null);
                        // we need to validate with the server to see if null is valid
                        if (!_this.row.isValidateOnChangeDisabled && _this.isSelected() && _this.isEditable())
                            ko.postbox.publish(_this.gridId + "-validate-row", _this.row.bwfId);
                        return;
                    }
                    var parsed = utilities.parseValueWithUnit(input);
                    _this.recordValue(parsed);
                    if (parsed) {
                        _this.isValid(true);
                        _this.recordValue.validationMessages([]);
                        if (!_this.row.isValidateOnChangeDisabled && _this.isSelected() && _this.isEditable())
                            ko.postbox.publish(_this.gridId + "-validate-row", _this.row.bwfId);
                    }
                    else {
                        _this.isValid(false);
                        _this.recordValue.validationMessages([_this.invalidValueWithUnitMessage]);
                    }
                }
            });
            return _this;
        }
        EditableValueWithUnitCell.prototype.dispose = function () {
            this.value.dispose();
            this.recordValueSubscription.dispose();
            _super.prototype.dispose.call(this);
        };
        return EditableValueWithUnitCell;
    }(cell.BwfWrapperCell2));
    return EditableValueWithUnitCell;
});
