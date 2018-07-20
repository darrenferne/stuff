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
define(["require", "exports", "knockout", "modules/bwf-metadata", "modules/grid/cells/cell-base"], function (require, exports, ko, metadataService, cell) {
    "use strict";
    var FormatCell = /** @class */ (function (_super) {
        __extends(FormatCell, _super);
        function FormatCell(params) {
            var _this = _super.call(this, params, true) || this;
            _this.canBeEdited = ko.observable(true);
            var thisProp = _this.row.metadata.properties[params.row.record.Name];
            var type;
            if (!thisProp) {
                var typeMetadata = metadataService.getPropertyWithPrefix(params.row.metadata.dataService, params.row.metadata, params.row.record.Name);
                type = typeMetadata.type;
            }
            else
                type = thisProp.type;
            switch (type) {
                case "date":
                    _this.value.extend({
                        validDateFormat: {
                            message: "Invalid date format",
                            allowNull: true
                        },
                        notify: 'always'
                    });
                    break;
                case "time":
                    _this.value.extend({
                        validTimeFormat: {
                            message: "Invalid date/time format",
                            allowNull: true
                        },
                        notify: 'always'
                    });
                    break;
                case "numeric":
                    _this.value.extend({
                        validNumericFormat: {
                            message: "Invalid numeric format",
                            allowNull: true
                        },
                        notify: 'always'
                    });
                    break;
                case "integer":
                    _this.value.extend({
                        validIntegerFormat: {
                            message: "Invalid integer format",
                            allowNull: true
                        },
                        notify: 'always'
                    });
                    break;
                default:
                    _this.canBeEdited(false);
                    break;
            }
            _this.cannotBeEdited = ko.pureComputed(function () { return !_this.canBeEdited() || _this.gridDisabled(); });
            _this.cssClasses_Format = ko.pureComputed(function () {
                var classes = [];
                if (!_this.value.isValid())
                    classes.push('has-error');
                if (_this.cannotBeEdited() && !_this.gridDisabled())
                    classes.push("disallow-edit");
                classes.push(_this.cssClasses());
                return classes.join(' ');
            });
            // prevent pastes from putting data into the value if it can't be edited
            if (_this.cannotBeEdited())
                _this.updateCellValue.dispose();
            return _this;
        }
        return FormatCell;
    }(cell.BwfCell));
    return FormatCell;
});
