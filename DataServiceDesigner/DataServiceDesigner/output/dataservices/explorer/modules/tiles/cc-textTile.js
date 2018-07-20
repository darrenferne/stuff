define(["require", "exports", "knockout", "markdown-it"], function (require, exports, ko, markdownIt) {
    "use strict";
    var TextTile = /** @class */ (function () {
        function TextTile(params) {
            var _this = this;
            var mdi = new markdownIt({
                html: true,
                linkify: true
            });
            this.height = params.contentAreaHeight;
            this.width = params.contentAreaWidth;
            this.heightString = ko.pureComputed(function () { return _this.height().toString() + "px"; });
            this.widthString = ko.pureComputed(function () { return _this.width().toString() + "px"; });
            this.content = ko.pureComputed(function () {
                var tileObj = params.tile.tileObject();
                var contentToRender = JSON.parse(tileObj.Content).Content;
                return mdi.render(contentToRender);
            });
        }
        return TextTile;
    }());
    return TextTile;
});
