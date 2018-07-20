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
define(["require", "exports", "modules/ds-explorer-tiles;cc-viewTileBase"], function (require, exports, base) {
    "use strict";
    var ViewTile = /** @class */ (function (_super) {
        __extends(ViewTile, _super);
        function ViewTile(params) {
            return _super.call(this, params) || this;
        }
        return ViewTile;
    }(base.ViewTileBase));
    return ViewTile;
});
