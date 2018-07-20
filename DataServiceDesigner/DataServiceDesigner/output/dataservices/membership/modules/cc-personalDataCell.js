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
    var PersonalDataCell = /** @class */ (function (_super) {
        __extends(PersonalDataCell, _super);
        function PersonalDataCell(params) {
            var _this = _super.call(this, params) || this;
            _this.value = ko.computed(function () {
                var value = JSON.parse(_this.recordValue());
                var props = [];
                _this.objectToString("", props, value);
                return _this.column.formatter(props.join(", "));
            });
            return _this;
        }
        PersonalDataCell.prototype.objectToString = function (prefix, props, obj) {
            for (var property in obj) {
                if (obj.hasOwnProperty(property)) {
                    if (typeof obj[property] == "object" && obj[property] !== null)
                        this.objectToString(prefix + property + " ", props, obj[property]);
                    else {
                        var value = obj[property] || "null";
                        var trimmedValue = value.toString().length > 256 ?
                            (value.toString().substring(0, 256) + "...") : value;
                        props.push(prefix + property + ": " + trimmedValue);
                    }
                }
            }
        };
        return PersonalDataCell;
    }(cell.BwfWrapperCell));
    return PersonalDataCell;
});
