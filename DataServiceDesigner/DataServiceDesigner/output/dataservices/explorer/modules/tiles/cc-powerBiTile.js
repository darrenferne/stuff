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
define(["require", "exports", "knockout", "options", "jquery", "modules/ds-explorer-tiles;cc-basePowerBiTile"], function (require, exports, ko, options, $, baseTile) {
    "use strict";
    var PowerBiTile = /** @class */ (function (_super) {
        __extends(PowerBiTile, _super);
        function PowerBiTile(params) {
            var _this = _super.call(this, params) || this;
            var tileObj = params.tile.tileObject();
            var serialized = JSON.parse(tileObj.Content);
            var tileId = serialized.SourceTileId;
            var dashboardId = serialized.SourceDashboardId;
            var autoRefreshTime = serialized.AutoRefreshTime;
            if (!autoRefreshTime || autoRefreshTime <= 0)
                autoRefreshTime = 10;
            _this.autoRefreshTime = ko.observable(autoRefreshTime);
            var requestUrl = options.explorerHostUrl + "/powerbi/tile";
            var getTileDataRequestPromise = $.ajax({
                url: requestUrl,
                contentType: 'application/json',
                type: 'POST',
                headers: _this.getAccessTokenHeader(),
                data: JSON.stringify({
                    tileId: tileId,
                    dashboardId: dashboardId
                })
            });
            getTileDataRequestPromise.done(function (data) {
                _this.requestSuccess(data);
            });
            getTileDataRequestPromise.fail(function (failure) {
                _this.requestFailure(failure);
            });
            return _this;
        }
        return PowerBiTile;
    }(baseTile.BasePowerBiTile));
    return PowerBiTile;
});
