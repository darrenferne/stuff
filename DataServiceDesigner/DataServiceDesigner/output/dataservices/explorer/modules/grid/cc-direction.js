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
define(["require", "exports", "knockout", "modules/grid/cells/cell-base"], function (require, exports, ko, cell) {
    "use strict";
    var DirectionCell = /** @class */ (function (_super) {
        __extends(DirectionCell, _super);
        function DirectionCell(params) {
            var _this = _super.call(this, params, true) || this;
            _this.pasteValue = function (newValue) {
                if (typeof newValue === 'string') {
                    if (newValue.toUpperCase() === 'ASC')
                        _this.value('');
                    else if (newValue.toUpperCase() === 'DESC')
                        _this.value('desc');
                }
            };
            _this.directionOptions = [
                { DisplayName: "Asc", Value: "" },
                { DisplayName: "Desc", Value: "desc" }
            ];
            _this.recordValue(params.row.record.Direction);
            // this will fix any older orderbys that have 'asc' instead of a blank value
            if (_this.recordValue().toLowerCase() === 'asc')
                _this.recordValue('');
            _this.selectedText = ko.pureComputed(function () {
                var op = _this.recordValue();
                var matching = _this.directionOptions.filter(function (x) { return x.Value === op; });
                // two characters worth of space to be clear of the dropdown arrow
                return matching.length > 0 ? matching[0].DisplayName + '__' : '';
            });
            _this.value = ko.computed({
                read: function () { return _this.recordValue(); },
                write: function (newValue) { return _this.recordValue(newValue); }
            });
            return _this;
        }
        DirectionCell.prototype.dispose = function () {
            this.value.dispose();
            _super.prototype.dispose.call(this);
        };
        DirectionCell.prototype.getClipboardValue = function () {
            return this.value() === 'desc' ? 'Desc' : 'Asc';
        };
        return DirectionCell;
    }(cell.BwfWrapperCell));
    return DirectionCell;
});
