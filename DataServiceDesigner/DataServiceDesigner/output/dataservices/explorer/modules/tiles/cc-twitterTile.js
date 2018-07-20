define(["require", "exports", "knockout", "scripts/twitter"], function (require, exports, ko, twitter) {
    "use strict";
    var TwitterTile = /** @class */ (function () {
        function TwitterTile(params) {
            var _this = this;
            var tileObj = params.tile.tileObject();
            this.account = JSON.parse(tileObj.Content).Account;
            if (this.account != null && this.account[0] == '@')
                this.account = this.account.substring(1);
            this.uniqueId = "tile-" + tileObj.Id + "-timeline";
            this.height = params.contentAreaHeight;
            this.width = params.contentAreaWidth;
            this.heightString = ko.pureComputed(function () { return _this.height().toString() + "px"; });
            this.widthString = ko.pureComputed(function () { return _this.width().toString() + "px"; });
        }
        TwitterTile.prototype.rendered = function () {
            var twitterParameters = {
                sourceType: "profile",
                screenName: this.account,
            };
            var element = document.getElementById(this.uniqueId);
            var options = {
                chrome: 'noheader noborders transparent'
            };
            twitter.ready(function () { return twitter.widgets.createTimeline(twitterParameters, element, options); });
        };
        return TwitterTile;
    }());
    return TwitterTile;
});
