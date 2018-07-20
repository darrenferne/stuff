define(["require", "exports"], function (require, exports) {
    "use strict";
    var GridRow = /** @class */ (function () {
        function GridRow(params) {
            this.columns = params.columns;
            this.gridDisabled = params.gridDisabled;
            this.gridId = params.gridId;
            this.row = params.row;
            this.typeMetadata = params.typeMetadata;
        }
        GridRow.prototype.dispose = function () {
            this.row.selected(false);
            this.row.updateType('None');
        };
        GridRow.prototype.validationConfig = function () {
            var config = {
                columns: this.columns,
                row: this.row,
            };
            return config;
        };
        return GridRow;
    }());
    return GridRow;
});
