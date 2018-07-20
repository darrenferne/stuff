define(["require", "exports", "options", "sprintf", "modules/bwf-globalisation", "knockout-postbox"], function (require, exports, options, sprintfM, globalisation) {
    "use strict";
    var sprintf = sprintfM.sprintf;
    // Here so that typescript doesn't remove import/required files
    var g = globalisation;
    var PowerBiTile = /** @class */ (function () {
        function PowerBiTile(params) {
            var _this = this;
            this.title = options.resources['bwf_power_bi_tiles'];
            this.height = params.height;
            this.width = params.width;
            this.accessToken = params.accessToken;
            var autoRefreshTime = params.autoRefreshTime || 10;
            this.autoRefreshTimeInMilliseconds = autoRefreshTime * 1000 * 60;
            this.globalMethodsObject = "powerBiTileLoaders";
            this.autoRefreshTimeout = null;
            this.embedUrl = sprintf("%s&width=%d&height=%d", params.embedUrl, this.width(), this.height());
            this.uniqueId = window.performance.now().toFixed(0);
            this.iframeId = "powerBiTileFrame" + this.uniqueId;
            this.loadTileFunctionName = sprintf("loadTile%s", this.uniqueId);
            this.loadTileFunctionNameForBinding = sprintf("window.%s.%s()", this.globalMethodsObject, this.loadTileFunctionName);
            if (!window[this.globalMethodsObject])
                window[this.globalMethodsObject] = {};
            // On click of the tile, open the report associated with the tile
            window.addEventListener("message", this.receiveMessage, false);
            window[this.globalMethodsObject][this.loadTileFunctionName] = function () {
                var iframe = document.getElementById(_this.iframeId);
                var iframeArgs = {
                    action: "loadTile",
                    accessToken: _this.accessToken,
                    height: _this.height(),
                    width: _this.width()
                };
                var message = JSON.stringify(iframeArgs);
                iframe.contentWindow.postMessage(message, "*");
                // Set auto timeout
                if (_this.autoRefreshTimeInMilliseconds) {
                    if (_this.autoRefreshTimeout) {
                        clearTimeout(_this.autoRefreshTimeout);
                        _this.autoRefreshTimeout = null;
                    }
                    _this.autoRefreshTimeout = setTimeout(function () {
                        window[_this.globalMethodsObject][_this.loadTileFunctionName]();
                    }, _this.autoRefreshTimeInMilliseconds);
                }
                ;
            };
        }
        PowerBiTile.prototype.dispose = function () {
            clearTimeout(this.autoRefreshTimeout);
            this.autoRefreshTimeout = null;
            delete window[this.globalMethodsObject][this.loadTileFunctionName];
        };
        PowerBiTile.prototype.receiveMessage = function (event) {
            var messageData = JSON.parse(event.data);
            if (messageData.event === "tileClicked")
                window.open(messageData.navigationUrl);
        };
        return PowerBiTile;
    }());
    return PowerBiTile;
});
