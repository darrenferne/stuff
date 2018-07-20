define(["require", "exports", "knockout"], function (require, exports, ko) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var RssTileBase = /** @class */ (function () {
        function RssTileBase(params) {
            this.items = ko.observableArray();
            var tileObj = params.tile.tileObject();
            var tileContent = JSON.parse(tileObj.Content);
            this.updateFeed({ url: tileContent.FeedUrl, keywords: tileContent.Keywords });
            this.pollingInterval = setInterval(this.updateFeedInterval, 60000, this);
        }
        RssTileBase.prototype.dispose = function () {
            clearInterval(this.pollingInterval);
        };
        RssTileBase.prototype.updateFeed = function (feed) {
            this.feed = feed;
            this.updateFeedInterval(this);
        };
        RssTileBase.prototype.updateFeedInterval = function (self) {
            if (!self.feed || !self.feed.url)
                return;
            var yql = "https://query.yahooapis.com/v1/public/yql?q=select title, description, pubDate, link from rss where url=\"" + self.feed.url + "\"&format=json&callback=?";
            $.getJSON(yql, function (data) {
                var txt = document.createElement("textarea");
                data.query.results.item.forEach(function (x) {
                    txt.innerHTML = x.description;
                    x.description = txt.value;
                });
                if (self.feed.keywords) {
                    var keywords = self.feed.keywords.toLowerCase().split(" ");
                    var filteredItems = data.query.results.item.filter(function (item) {
                        var title = item.title.toLowerCase();
                        var description = item.description.toLowerCase();
                        return keywords.some(function (x) { return title.includes(x) || description.includes(x); });
                    });
                    self.items(filteredItems);
                }
                else
                    self.items(data.query.results.item);
            });
        };
        return RssTileBase;
    }());
    exports.RssTileBase = RssTileBase;
});
