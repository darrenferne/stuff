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
define(["require", "exports", "knockout", "modules/grid/aggregatedComponents-base"], function (require, exports, ko, base) {
    "use strict";
    var CombinedHeader = /** @class */ (function (_super) {
        __extends(CombinedHeader, _super);
        function CombinedHeader() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        CombinedHeader.prototype.getOptionalComponent = function (grid) {
            return grid.header;
        };
        CombinedHeader.prototype.postAdd = function (gridElement) {
            ko.postbox.publish(gridElement + "-use-combined-header");
        };
        return CombinedHeader;
    }(base));
    return CombinedHeader;
});
