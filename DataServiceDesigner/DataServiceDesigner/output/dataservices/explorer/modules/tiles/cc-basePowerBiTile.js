define(["require", "exports", "knockout", "options", "loglevel", "scripts/sprintf", "modules/bwf-utilities"], function (require, exports, ko, options, log, sprintfM, utilities) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var sprintf = sprintfM.sprintf;
    var storage = new utilities.LocalStorageWithExpiry();
    var BasePowerBiTile = /** @class */ (function () {
        function BasePowerBiTile(params) {
            var _this = this;
            this.height = params.contentAreaHeight;
            this.width = params.contentAreaWidth;
            this.heightString = ko.pureComputed(function () { return _this.height().toString() + "px"; });
            this.widthString = ko.pureComputed(function () { return _this.width().toString() + "px"; });
            this.tileConfig = params.tileConfig;
            this.showLinkButton = ko.observable(false);
            this.readyToRender = ko.observable(false);
            this.accessToken = ko.observable("");
            this.embedUrl = ko.observable("");
            this.hasErrored = ko.observable(false);
            this.errorMessage = ko.observable("Error occurred rendering tile.");
            var tileObj = params.tile.tileObject();
            var serialized = JSON.parse(tileObj.Content);
            var tileId = serialized.SourceTileId;
            var dashboardId = serialized.SourceDashboardId;
            this.redirectToDashboardUrl = ko.pureComputed(function () { return sprintf("https://app.powerbi.com/dashboards/%s", dashboardId); });
        }
        BasePowerBiTile.prototype.getAccessToken = function () {
            return storage.getItem('powerbitoken');
        };
        BasePowerBiTile.prototype.getAccessTokenHeader = function () {
            var header = {};
            header["X-PowerBi-Token"] = this.getAccessToken();
            return header;
        };
        BasePowerBiTile.prototype.requestSuccess = function (data) {
            this.accessToken(data.accessToken);
            this.embedUrl(data.embedUrl);
            this.readyToRender(true);
            this.showLinkButton(true);
        };
        BasePowerBiTile.prototype.requestFailure = function (failure) {
            this.hasErrored(true);
            this.errorMessage(failure.responseText);
            switch (failure.status) {
                case 404:// not found
                    if (!failure.responseText)
                        this.errorMessage(options.resources["bwf_power_bi_not_contactable"]);
                    break;
                case 406:// not acceptable
                    log.warn("The embed URL was not present in the tile from Power BI when requested.");
                    // the tile was found but we can't embed so show the button
                    this.showLinkButton(true);
                    break;
                case 401:// unauthorised
                    if (!failure.responseText)
                        this.errorMessage(options.resources["bwf_power_bi_not_on_account"]);
                    break;
            }
        };
        return BasePowerBiTile;
    }());
    exports.BasePowerBiTile = BasePowerBiTile;
});
