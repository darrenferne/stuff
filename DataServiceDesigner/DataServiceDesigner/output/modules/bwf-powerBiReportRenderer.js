define(["require", "exports", "sprintf", "modules/bwf-globalisation", "knockout-postbox"], function (require, exports, sprintfM, globalisation) {
    "use strict";
    var sprintf = sprintfM.sprintf;
    // Here so that typescript doesn't remove import/required files
    var g = globalisation;
    var PowerBiReport = /** @class */ (function () {
        function PowerBiReport(params) {
            var _this = this;
            this.height = params.height;
            this.width = params.width;
            this.accessToken = params.accessToken;
            this.globalMethodsObject = "powerBiReportLoaders";
            this.hideFilterPanelInReport = params.hideFilterPanelInReport || false;
            this.embedUrl = sprintf("%s&width=%d&height=%d%s", params.embedUrl, this.width(), this.height(), this.hideFilterPanelInReport ? "&filterPaneEnabled=false" : "");
            this.uniqueId = window.performance.now().toFixed(0);
            this.iframeId = "powerBiReportFrame" + this.uniqueId;
            this.loadReportFunctionName = sprintf("loadReport%s", this.uniqueId);
            this.loadReportFunctionNameForBinding = sprintf("window.%s.%s()", this.globalMethodsObject, this.loadReportFunctionName);
            if (!window[this.globalMethodsObject])
                window[this.globalMethodsObject] = {};
            window[this.globalMethodsObject][this.loadReportFunctionName] = function () {
                var iframe = document.getElementById(_this.iframeId);
                var iframeArgs = {
                    action: "loadReport",
                    accessToken: _this.accessToken,
                    height: _this.height(),
                    width: _this.width()
                };
                var message = JSON.stringify(iframeArgs);
                iframe.contentWindow.postMessage(message, "*");
            };
        }
        PowerBiReport.prototype.dispose = function () {
            delete window[this.globalMethodsObject][this.loadReportFunctionName];
        };
        return PowerBiReport;
    }());
    return PowerBiReport;
});
