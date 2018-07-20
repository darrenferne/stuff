define(["require", "exports", "knockout", "sprintf", "options", "loglevel", "jquery", "modules/bwf-utilities"], function (require, exports, ko, sprintfM, options, log, $, utils) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var sprintf = sprintfM.sprintf;
    var ViewTileBase = /** @class */ (function () {
        function ViewTileBase(params) {
            var _this = this;
            this.options = options;
            this.subscriptions = ko.observableArray([]);
            this.tileViewGridRendered = function () {
                var linkedTileId = _this.tileContent.LinkedTileId;
                var enableQuerying = true;
                if (linkedTileId)
                    enableQuerying = false;
                ko.postbox.publish(_this.viewGridId() + '-loadView', {
                    viewName: _this.viewName(),
                    urlParameters: [],
                    urlFilteredby: [],
                    enableQuerying: enableQuerying
                });
                _this.onResize();
            };
            this.receivePublish = function (data) {
                var parameters = _this.getViewParameters(data.body);
                if (parameters) {
                    ko.postbox.publish(_this.viewGridId() + '-querying-enabled', true);
                    ko.postbox.publish(_this.viewGridId() + '-updateViewParameters', parameters);
                }
            };
            var tileViewModel = params.tile;
            this.tile = params.tile.tileObject();
            this.tileComponentParams = params;
            this.tileContent = JSON.parse(this.tile.Content);
            this.linkedTileId = ko.observable(this.tileContent.LinkedTileId);
            var hasLinkedTile = !!this.linkedTileId();
            this.viewId = ko.observable(this.tileContent.ViewId);
            this.viewGridId = ko.observable(sprintf("tile-%s-view", this.tile.Id));
            this.viewName = ko.observable('');
            this.viewQueryCompleted = ko.observable(false);
            this.canLoadView = ko.observable(false);
            this.enableParameterBar = hasLinkedTile || this.tileContent.ShowParameterBar;
            this.showParameterBar = this.tileContent.ShowParameterBar && !hasLinkedTile;
            this.inTouchMode = utils.isTouchModeEnabled;
            this.availableHeight = params.contentAreaHeight;
            this.availableWidth = params.contentAreaWidth;
            var queryUrl = sprintf('%s/api/explorer/query/Views?$filter=Id=%d', options.explorerHostUrl, this.viewId());
            $.get(queryUrl, function (result) {
                _this.viewQueryCompleted(true);
                if (result.TotalCount == 1) {
                    _this.viewName(result['Records'][0].Name);
                    _this.canLoadView(true);
                }
                else {
                    log.warn("View could not be loaded. This could be due to a permissions issue.");
                    _this.canLoadView(false);
                }
            });
            this.subscriptions.push(this.availableHeight.subscribe(function () { return _this.onResize(); }));
            this.subscriptions.push(this.availableWidth.subscribe(function () { return _this.onResize(); }));
            this.tileComponentParams.receivePublishMethodObservable(this.receivePublish);
            this.subscriptions.push(ko.postbox.subscribe(this.viewGridId() + "-recordsSelected", function (selectedRecords) {
                _this.tileComponentParams.publish({
                    publishingTile: _this.tile,
                    body: {
                        selectedRecords: selectedRecords
                    }
                });
            }));
        }
        ViewTileBase.prototype.dispose = function () {
            this.subscriptions().forEach(function (x) { return x.dispose(); });
        };
        ViewTileBase.prototype.onResize = function () {
            var params = {
                height: this.availableHeight(),
                width: this.availableWidth()
            };
            ko.postbox.publish(sprintf('%s-viewGridResized', this.viewGridId()), params);
        };
        ViewTileBase.prototype.getViewParameters = function (data) {
            return data["viewParameters"];
        };
        return ViewTileBase;
    }());
    exports.ViewTileBase = ViewTileBase;
});
