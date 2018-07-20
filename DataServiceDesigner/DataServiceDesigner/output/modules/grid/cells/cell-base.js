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
define(["require", "exports", "loglevel", "sprintf", "knockout"], function (require, exports, log, sprintfM, ko) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var sprintf = sprintfM.sprintf;
    var BwfCell = /** @class */ (function () {
        function BwfCell(params, isEditable) {
            if (isEditable === void 0) { isEditable = false; }
            var _this = this;
            this.pasteValue = function (value) {
                _this.value(value);
            };
            this.gridId = params.gridId;
            this.row = params.row;
            this.column = params.column;
            this.gridDisabled = params.gridDisabled;
            this.isEditMode = isEditable;
            this.customClasses = params.customClasses || "";
            this.value = params.row.values[this.column.path];
            var rowInCreate = params.row.isNewRecord() && !params.column.metadata.isDisabledInCreateMode;
            var rowInEdit = !params.row.isNewRecord() && !params.column.metadata.isDisabledInEditMode;
            this.isEditableCell = ko.observable(isEditable &&
                !params.column.metadata.isNotEditableInGrid &&
                (rowInCreate || rowInEdit));
            this.isEditable = ko.pureComputed({
                read: function () { return _this.isEditableCell() && !_this.value.isReadonly(); },
                write: function (value) { return _this.isEditableCell(value); }
            });
            this.isChangeTrackingDisabled = this.row.isChangeTrackingDisabled;
            this.typeMetadata = params.typeMetadata;
            this.callback = params.onRender;
            this.highlight = ko.observable(false);
            if (this.isEditable()) {
                this.updateCellValue = this.subscribeToValue(this.gridId, this.row.position(), this.column.position());
                this.updateRowPosition = params.row.position.subscribe(function (pos) {
                    if (_this.updateCellValue)
                        _this.updateCellValue.dispose();
                    _this.updateCellValue = _this.subscribeToValue(_this.gridId, pos, _this.column.position());
                });
            }
            this.colSpan = this.value.colSpan || 1;
            this.isSelected = this.value.isSelected;
            this.isDirty = this.value.isDirty;
            this.isInCopyOrPasteGroup = this.value.isInCopyOrPasteGroup;
            this.isValid = this.value.isValid;
            // this is a seemingly useless computed because we don't want this 
            // particular observable to be bi-directional. If it is then the 
            // selected cell colouring disappears while CTRL is pressed, and
            // sometimes fails to reappear (depending on how long the key was held)
            //
            // it has a write method so errors aren't thrown if knockout tries to
            // write to it on focus, which will happen after releasing CTRL
            this.isFocused = ko.pureComputed({
                read: function () { return _this.isSelected(); },
                write: function () { return; }
            });
            if (!this.row.isValidateOnChangeDisabled) {
                // validate on value change
                this.validationSubscription = this.value.subscribe(function () {
                    if (_this.isSelected() && _this.isEditable())
                        ko.postbox.publish(_this.gridId + "-validate-row", _this.row.bwfId);
                });
            }
            this.cssClasses = ko.computed(function () {
                var classes = ['bwf-cell'];
                if (_this.isSelected())
                    classes.push('selected-origin-cell');
                else if (_this.isInCopyOrPasteGroup())
                    classes.push('selected-copy-paste-cell');
                if (!_this.isValid())
                    classes.push('has-error');
                if (_this.highlight())
                    classes.push('highlight');
                if (_this.isEditable()) {
                    classes.push('editable-cell');
                    if (!_this.isChangeTrackingDisabled && _this.isDirty())
                        classes.push('bwf-unsaved-change');
                }
                if (!_this.isEditable()) {
                    classes.push('not-editable');
                    if (_this.column.metadata.type === 'integer' || _this.column.metadata.type === 'numeric')
                        classes.push('numeric-cell');
                }
                classes.push(_this.customClasses);
                return classes.join(' ');
            });
            this.attributes = ko.computed(function () {
                return {
                    colSpan: _this.colSpan
                };
            });
        }
        BwfCell.prototype.dispose = function () {
            if (this.isEditable()) {
                if (this.updateCellValue)
                    this.updateCellValue.dispose();
                if (this.updateRowPosition)
                    this.updateRowPosition.dispose();
            }
            if (this.validationSubscription)
                this.validationSubscription.dispose();
            clearTimeout(this.highlightId);
            this.cssClasses.dispose();
            this.highlight(false);
        };
        BwfCell.prototype.onRendered = function () {
            if (this.callback)
                this.callback(this.column.position());
        };
        BwfCell.prototype.subscribeToValue = function (gridId, rowPos, colPos) {
            var _this = this;
            var topic = sprintf("%s-update-cell-%d-%d", gridId, rowPos, colPos);
            var callback = function (newValue) {
                log.debug(sprintf("Updated cell %s,%s value = %s", rowPos, colPos, newValue));
                _this.pasteValue(newValue);
            };
            return ko.postbox.subscribe(topic, callback);
        };
        BwfCell.prototype.doHighlight = function () {
            var _this = this;
            if (this.highlightId)
                clearTimeout(this.highlightId);
            this.highlight(true);
            this.highlightId = setTimeout(function () {
                _this.highlight(false);
                _this.highlightId = null;
            }, 1600);
        };
        BwfCell.prototype.getClipboardValue = function () {
            var v = this.value();
            return v ? v.toString() : '';
        };
        return BwfCell;
    }());
    exports.BwfCell = BwfCell;
    var BwfWrapperCell = /** @class */ (function (_super) {
        __extends(BwfWrapperCell, _super);
        function BwfWrapperCell(params, isEditable) {
            if (isEditable === void 0) { isEditable = false; }
            var _this = _super.call(this, params, isEditable) || this;
            _this.recordValue = params.row.values[_this.column.path];
            _this.isSelected = _this.recordValue.isSelected;
            _this.isDirty = _this.recordValue.isDirty;
            _this.isInCopyOrPasteGroup = _this.recordValue.isInCopyOrPasteGroup;
            _this.isValid = _this.recordValue.isValid;
            return _this;
        }
        BwfWrapperCell.prototype.dispose = function () {
            if (ko.isComputed(this.value))
                this.value.dispose();
            _super.prototype.dispose.call(this);
        };
        return BwfWrapperCell;
    }(BwfCell));
    exports.BwfWrapperCell = BwfWrapperCell;
    var BwfWrapperCell2 = /** @class */ (function (_super) {
        __extends(BwfWrapperCell2, _super);
        function BwfWrapperCell2(params, isEditable) {
            if (isEditable === void 0) { isEditable = false; }
            return _super.call(this, params, isEditable) || this;
        }
        return BwfWrapperCell2;
    }(BwfWrapperCell));
    exports.BwfWrapperCell2 = BwfWrapperCell2;
});
