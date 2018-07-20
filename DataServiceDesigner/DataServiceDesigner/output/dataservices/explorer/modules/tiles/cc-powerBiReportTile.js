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
define(["require", "exports", "knockout", "options", "scripts/sprintf", "jquery", "modules/ds-explorer-tiles;cc-basePowerBiTile"], function (require, exports, ko, options, sprintfM, $, basePowerBiTile) {
    "use strict";
    var sprintf = sprintfM.sprintf;
    var PowerBiReportTile = /** @class */ (function (_super) {
        __extends(PowerBiReportTile, _super);
        function PowerBiReportTile(params) {
            var _this = _super.call(this, params) || this;
            _this.paddingRightForButton = ko.observable(_this.tileConfig.tileContentMarginSize + 32);
            _this.redirectToReportUrl = ko.observable("");
            var requestUrl = options.explorerHostUrl + "/powerbi/report";
            var tileObj = params.tile.tileObject();
            var serialized = JSON.parse(tileObj.Content);
            var reportId = serialized.SourceReportId;
            var getReportDataRequestPromise = $.ajax({
                url: requestUrl,
                contentType: 'application/json',
                type: 'POST',
                headers: _this.getAccessTokenHeader(),
                data: JSON.stringify({
                    reportId: reportId
                })
            });
            getReportDataRequestPromise.done(function (data) {
                _this.redirectToReportUrl(data.webUrl);
                _this.requestSuccess(data);
            });
            getReportDataRequestPromise.fail(function (failure) {
                _this.requestFailure(failure);
            });
            return _this;
        }
        return PowerBiReportTile;
    }(basePowerBiTile.BasePowerBiTile));
    return PowerBiReportTile;
});
