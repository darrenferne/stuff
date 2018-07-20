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
define(["require", "exports", "knockout", "scripts/sprintf", "modules/bwf-metadata", "modules/grid/cells/cell-base", "loglevel"], function (require, exports, ko, fmt, metadata, base, log) {
    "use strict";
    var DisplayDownloadCell = /** @class */ (function (_super) {
        __extends(DisplayDownloadCell, _super);
        function DisplayDownloadCell(params) {
            var _this = _super.call(this, params) || this;
            _this.value = ko.computed({
                read: function () {
                    var v = params.row.values[_this.column.path]();
                    if (v != null) {
                        var ds = metadata.getDataService(params.typeMetadata.dataService);
                        var url = fmt.sprintf('%s/Download/%s/%s/%s', ds.url, params.typeMetadata.type, params.column.path, v.Value);
                        return { Text: v.Text, Value: url };
                    }
                    return { Text: '', Value: '' };
                },
                write: function () {
                    log.warn('cannot write to a download cell');
                }
            });
            return _this;
        }
        DisplayDownloadCell.prototype.getClipboardValue = function () {
            var value = this.value();
            return value == null ? '' : value.Value;
        };
        DisplayDownloadCell.prototype.dispose = function () {
            if (this.value)
                this.value.dispose();
            _super.prototype.dispose.call(this);
        };
        return DisplayDownloadCell;
    }(base.BwfWrapperCell));
    return DisplayDownloadCell;
});
