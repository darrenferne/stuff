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
define(["require", "exports", "modules/grid/parameters/parameterBar-base", "knockout-kendo", "loglevel"], function (require, exports, parameterBarBase, knockoutKendo, log) {
    "use strict";
    var koKendo = knockoutKendo;
    var StandAloneParameterBar = /** @class */ (function (_super) {
        __extends(StandAloneParameterBar, _super);
        function StandAloneParameterBar(config) {
            var _this = _super.call(this, config, true) || this;
            _this.loadParameters();
            return _this;
        }
        StandAloneParameterBar.prototype.postOnRender = function () {
            this.loadParameterValues();
        };
        StandAloneParameterBar.prototype.loadParameterValues = function () {
            this.apply();
        };
        StandAloneParameterBar.prototype.saveSelectedParameterValues = function () {
            log.debug("Saving parameters on the server is not yet implemented");
        };
        StandAloneParameterBar.prototype.dispose = function () {
            _super.prototype.dispose.call(this);
        };
        return StandAloneParameterBar;
    }(parameterBarBase));
    return StandAloneParameterBar;
});
