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
define(["require", "exports", "modules/grid/aggregatedComponents-base"], function (require, exports, base) {
    "use strict";
    var CombinedFooter = /** @class */ (function (_super) {
        __extends(CombinedFooter, _super);
        function CombinedFooter() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        CombinedFooter.prototype.getOptionalComponent = function (grid) {
            return grid.footer;
        };
        CombinedFooter.prototype.postAdd = function (gridElement) {
            ko.postbox.publish(gridElement + "-use-combined-footer");
        };
        return CombinedFooter;
    }(base));
    return CombinedFooter;
});
