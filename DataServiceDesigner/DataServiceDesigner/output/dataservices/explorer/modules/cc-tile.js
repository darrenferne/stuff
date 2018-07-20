define(["require", "exports", "options", "sprintf", "knockout", "loglevel", "modules/bwf-utilities", "modules/bwf-bindingHandlers"], function (require, exports, options, sprintfM, ko, log, utils, bindingHandlers) {
    "use strict";
    var sprintf = sprintfM.sprintf;
    var bh = bindingHandlers;
    var Tile = /** @class */ (function () {
        function Tile(params) {
            var _this = this;
            this.hasLookedForComponent = ko.observable(false);
            this.componentFound = ko.observable(false);
            this.r = options.resources;
            this.getXTransformPixelsFromPosition = function (x) {
                var gridItemWidthX = _this.gridItemWidth * x;
                var gutterOffset = _this.gutterWidth * x + _this.gutterWidth;
                return gridItemWidthX + gutterOffset;
            };
            this.getYTransformPixelsFromPosition = function (y) {
                var gridItemWidthY = (_this.gridItemHeight * y);
                var gutterOffset = _this.gutterWidth * y + _this.gutterWidth;
                return gridItemWidthY + gutterOffset;
            };
            this.getNumberFromPixelString = function (str) {
                return parseInt(str.substr(0, str.length - 2));
            };
            this.roundToClosestValidWidth = function (widthInPixels) {
                if (widthInPixels <= _this.gridItemWidth + _this.gutterWidth)
                    return 1;
                var approxWidth = widthInPixels / (_this.gridItemWidth + _this.gutterWidth);
                if (approxWidth < 1)
                    return 1;
                var newWidth = Math.round(approxWidth);
                return newWidth;
            };
            this.roundToClosestValidHeight = function (heightInPixels) {
                if (heightInPixels <= _this.gridItemHeight + _this.gutterWidth)
                    return 1;
                var approxHeight = heightInPixels / (_this.gridItemHeight + _this.gutterWidth);
                if (approxHeight < 1)
                    return 1;
                var newHeight = Math.round(approxHeight);
                return newHeight;
            };
            this.getPixelWidthFromWidth = function (width) {
                return (_this.gridItemWidth * width) + (_this.gutterWidth * (width - 1));
            };
            this.getPixelHeightFromHeight = function (height) {
                return (_this.gridItemHeight * height) + (_this.gutterWidth * (height - 1));
            };
            this.publishAction = function (data) {
                var publishTopic = _this.dashboardElementId + "-tile-" + data.publishingTile.Id + "-publish";
                ko.postbox.publish(publishTopic, data);
            };
            this.receivePublish = function (data) {
                var method = _this.receivePublishMethod();
                if (typeof method === "function") {
                    method(data);
                }
            };
            this.onResize = function () {
                var tileContentElement = _this.tileElement.getElementsByClassName('tile-content')[0];
                var boundingBox = tileContentElement.getBoundingClientRect();
                _this.contentAreaHeight(boundingBox.height - (_this.tileContentMarginSize * 2));
                _this.contentAreaWidth(boundingBox.width - (_this.tileContentMarginSize * 2));
            };
            this.setupDragging = function () {
                _this.tileElement.addEventListener('mousedown', _this.onDragTile_Start);
                _this.tileElement.addEventListener('touchstart', _this.onDragTile_Start);
                if (_this.resizeEnabled) {
                    _this.resizeHandle.addEventListener('mousedown', _this.onDragResize_Start);
                    _this.resizeHandle.addEventListener('touchstart', _this.onDragResize_Start);
                }
            };
            this.showContent = function () {
                var content = _this.tileElement.getElementsByClassName('tile-content')[0];
                content.style.display = 'flex';
            };
            this.teardownDragging = function () {
                _this.tileElement.removeEventListener('mousedown', _this.onDragTile_Start);
                _this.tileElement.removeEventListener('touchstart', _this.onDragTile_Start);
                if (_this.resizeEnabled) {
                    _this.resizeHandle.removeEventListener('mousedown', _this.onDragResize_Start);
                    _this.resizeHandle.removeEventListener('touchstart', _this.onDragResize_Start);
                }
            };
            this.getTileDragHelper = function () {
                var helperElement = document.createElement('div');
                helperElement.classList.add('tile-drag-helper');
                helperElement.draggable = false;
                helperElement.ondragstart = function () { return false; };
                helperElement.style.transform = sprintf('translate(%dpx, %dpx)', _this.xPositionInPixels(), _this.yPositionInPixels());
                helperElement.style.width = sprintf('%dpx', _this.widthInPixels());
                helperElement.style.height = sprintf('%dpx', _this.heightInPixels());
                return helperElement;
            };
            this.tapAndHoldTimeout = null;
            this.addDragHoldHelper = false;
            this.touchHoldDelayMs = 750;
            this.onDragTile_Start = function (event) {
                var isMouseEvent = false;
                if (event.button !== void 0)
                    isMouseEvent = true;
                if (!isMouseEvent) {
                    _this.tapAndHoldTimeout = setTimeout(function () {
                        log.info("Tap and hold timeout done");
                        event.preventDefault();
                        _this.moveDragStartInfo.moveHelper = _this.getTileDragHelper();
                        _this.moveDragStartInfo.moveHelper.style.cursor = 'move';
                        _this.moveDragStartInfo.moveHelper.style.webkitUserSelect = 'none';
                        _this.tileElement.parentElement.appendChild(_this.moveDragStartInfo.moveHelper);
                        _this.tapAndHoldTimeout = null;
                    }, _this.touchHoldDelayMs);
                }
                // cancel event if not using left click
                if (isMouseEvent && event.button !== 0)
                    return true;
                if (isMouseEvent)
                    var moveHelper = _this.getTileDragHelper();
                if (moveHelper)
                    moveHelper.style.cursor = 'move';
                var dashboardBody = _this.tileElement.parentElement.parentElement;
                var startX = Math.floor(isMouseEvent ? event.pageX : event.touches.item(0).pageX);
                var startY = Math.floor(isMouseEvent ? event.pageY : event.touches.item(0).pageY);
                log.debug("onDragTile_Start, isMouseEvent = " + isMouseEvent + ", startX = " + startX + ", startY = " + startY);
                _this.moveDragStartInfo = {
                    isMouseEvent: isMouseEvent,
                    startEvent: event,
                    startPointerX: startX,
                    startPointerY: startY,
                    startScrollLeft: dashboardBody.scrollLeft,
                    startScrollTop: dashboardBody.scrollTop,
                    startX: _this.positionX(),
                    startY: _this.positionY(),
                    currentX: _this.positionX(),
                    currentY: _this.positionY(),
                    moveHelper: moveHelper
                };
                if (moveHelper)
                    _this.tileElement.parentElement.appendChild(moveHelper);
                if (isMouseEvent) {
                    document.body.style.cursor = 'move';
                    document.addEventListener('mousemove', _this.onDragTile_Move);
                    document.addEventListener('mouseup', _this.onDragTile_End);
                }
                else {
                    document.addEventListener('touchmove', _this.onDragTile_Move);
                    document.addEventListener('touchend', _this.onDragTile_End);
                    document.addEventListener('touchcancel', _this.onDragTile_Cancel);
                }
            };
            this.isScrolling = false;
            this.onDragTile_Move = function (event) {
                var isMouseEvent = _this.moveDragStartInfo.isMouseEvent;
                var pageX = Math.floor(isMouseEvent ? event.pageX : event.touches.item(0).pageX);
                var pageY = Math.floor(isMouseEvent ? event.pageY : event.touches.item(0).pageY);
                log.debug("onDragTile_Move, isMouseEvent = " + isMouseEvent + ", pageX = " + pageX + ", pageY = " + pageY);
                var dashboardBody = _this.tileElement.parentElement.parentElement;
                var leftDifference = (_this.moveDragStartInfo.startPointerX + _this.moveDragStartInfo.startScrollLeft) - (pageX + dashboardBody.scrollLeft);
                var topDifference = (_this.moveDragStartInfo.startPointerY + _this.moveDragStartInfo.startScrollTop) - (pageY + dashboardBody.scrollTop);
                if (leftDifference > 10 || topDifference > 10) {
                    _this.isScrolling = true;
                    clearTimeout(_this.tapAndHoldTimeout);
                    _this.tapAndHoldTimeout = null;
                }
                var moveHelper = _this.moveDragStartInfo.moveHelper;
                if (!moveHelper)
                    return true;
                else
                    event.preventDefault();
                var oldX = _this.moveDragStartInfo.currentX;
                var oldY = _this.moveDragStartInfo.currentY;
                var leftDifferenceInPositionUnits = Math.round(leftDifference / (_this.gridItemWidth + _this.gutterWidth));
                var topDifferenceInPositionUnits = Math.round(topDifference / (_this.gridItemHeight + _this.gutterWidth));
                var newX = _this.positionX() - leftDifferenceInPositionUnits;
                var newY = _this.positionY() - topDifferenceInPositionUnits;
                // we check the old values as setting the width/height is expensive
                if (oldX != newX || oldY != newY) {
                    _this.moveDragStartInfo.currentX = newX;
                    _this.moveDragStartInfo.currentY = newY;
                    moveHelper.style.transform = sprintf('translate(%dpx, %dpx)', _this.getXTransformPixelsFromPosition(newX), _this.getYTransformPixelsFromPosition(newY));
                }
                var valid = true;
                if (newX < 0 || newY < 0)
                    valid = false;
                if (valid)
                    moveHelper.classList.remove('invalid');
                else
                    moveHelper.classList.add('invalid');
                // required so events are not cancelled
                return true;
            };
            this.onDragTile_End = function (event) {
                clearTimeout(_this.tapAndHoldTimeout);
                _this.tapAndHoldTimeout = null;
                _this.isScrolling = false;
                document.removeEventListener('mousemove', _this.onDragTile_Move);
                document.removeEventListener('mouseup', _this.onDragTile_End);
                document.removeEventListener('touchmove', _this.onDragTile_Move);
                document.removeEventListener('touchend', _this.onDragTile_End);
                document.removeEventListener('touchcancel', _this.onDragTile_Cancel);
                log.debug("onDragTile_End");
                if (_this.moveDragStartInfo.moveHelper)
                    _this.tileElement.parentElement.removeChild(_this.moveDragStartInfo.moveHelper);
                if (_this.moveDragStartInfo.currentX >= 0 && _this.moveDragStartInfo.currentY >= 0) {
                    _this.positionX(_this.moveDragStartInfo.currentX);
                    _this.positionY(_this.moveDragStartInfo.currentY);
                    ko.postbox.publish(_this.dashboardElementId + '-dashboard-modified');
                }
                // IE
                document.body.style.cursor = 'auto';
                // Chrome
                document.body.style.cursor = null;
                _this.moveDragStartInfo = null;
            };
            this.onDragTile_Cancel = function (event) {
                clearTimeout(_this.tapAndHoldTimeout);
                _this.tapAndHoldTimeout = null;
                document.removeEventListener('mousemove', _this.onDragTile_Move);
                document.removeEventListener('mouseup', _this.onDragTile_End);
                document.removeEventListener('touchmove', _this.onDragTile_Move);
                document.removeEventListener('touchend', _this.onDragTile_End);
                document.removeEventListener('touchcancel', _this.onDragTile_Cancel);
                log.debug("onDragTile_Cancel");
                if (_this.moveDragStartInfo.moveHelper)
                    _this.tileElement.parentElement.removeChild(_this.moveDragStartInfo.moveHelper);
                // IE
                document.body.style.cursor = 'auto';
                // Chrome
                document.body.style.cursor = null;
                _this.moveDragStartInfo = null;
            };
            this.onDragResize_Start = function (event) {
                var isMouseEvent = false;
                if (event.button !== void 0)
                    isMouseEvent = true;
                if (!isMouseEvent)
                    event.preventDefault();
                if ((isMouseEvent && event.button === 0) || !isMouseEvent) {
                    event.stopPropagation();
                    var resizeHelper = _this.getTileDragHelper();
                    var pageX = Math.floor(isMouseEvent ? event.pageX : event.touches.item(0).pageX);
                    var pageY = Math.floor(isMouseEvent ? event.pageY : event.touches.item(0).pageY);
                    log.debug("onDragResize_Start, isMouseEvent = " + isMouseEvent + ", pageX = " + pageX + ", pageY = " + pageY);
                    _this.resizeDragStartInfo = {
                        isMouseEvent: isMouseEvent,
                        startEvent: event,
                        startHeightPixels: _this.heightInPixels(),
                        startWidthPixels: _this.widthInPixels(),
                        startHeight: _this.height(),
                        startWidth: _this.width(),
                        startPointerX: pageX,
                        startPointerY: pageY,
                        currentHeight: _this.height(),
                        currentWidth: _this.width(),
                        resizeHelper: resizeHelper
                    };
                    var content = _this.tileElement.getElementsByClassName('tile-content')[0];
                    content.style.display = 'none';
                    if (isMouseEvent) {
                        document.body.style.cursor = 'se-resize';
                        document.addEventListener('mousemove', _this.onDragResize_Move);
                        document.addEventListener('mouseup', _this.onDragResize_End);
                    }
                    else {
                        document.addEventListener('touchmove', _this.onDragResize_Move);
                        document.addEventListener('touchend', _this.onDragResize_End);
                        document.addEventListener('touchcancel', _this.onDragResize_Cancel);
                    }
                    _this.tileElement.parentElement.appendChild(resizeHelper);
                    return true;
                }
            };
            this.onDragResize_Move = function (event) {
                event.stopPropagation();
                var resizeHelper = _this.resizeDragStartInfo.resizeHelper;
                var oldWidth = _this.resizeDragStartInfo.currentWidth;
                var oldHeight = _this.resizeDragStartInfo.currentHeight;
                var isMouseEvent = _this.resizeDragStartInfo.isMouseEvent;
                var pageX = Math.floor(isMouseEvent ? event.pageX : event.touches.item(0).pageX);
                var pageY = Math.floor(isMouseEvent ? event.pageY : event.touches.item(0).pageY);
                log.debug("onDragResize_Move, isMouseEvent = " + isMouseEvent + ", pageX = " + pageX + ", pageY = " + pageY);
                var newPixelWidth = _this.resizeDragStartInfo.startWidthPixels -
                    (_this.resizeDragStartInfo.startPointerX - pageX);
                var newPixelHeight = _this.resizeDragStartInfo.startHeightPixels -
                    (_this.resizeDragStartInfo.startPointerY - pageY);
                var newWidth = _this.roundToClosestValidWidth(newPixelWidth);
                var newHeight = _this.roundToClosestValidHeight(newPixelHeight);
                // we check the old values as setting the width/height is expensive
                if (oldWidth != newWidth) {
                    resizeHelper.style.width = sprintf('%dpx', _this.getPixelWidthFromWidth(newWidth));
                    _this.resizeDragStartInfo.currentWidth = newWidth;
                }
                if (oldHeight != newHeight) {
                    resizeHelper.style.height = sprintf('%dpx', _this.getPixelHeightFromHeight(newHeight));
                    _this.resizeDragStartInfo.currentHeight = newHeight;
                }
                if (newHeight >= _this.minHeight && newHeight <= _this.maxHeight &&
                    newWidth >= _this.minWidth && newWidth <= _this.maxWidth) {
                    resizeHelper.classList.remove('invalid');
                }
                else {
                    resizeHelper.classList.add('invalid');
                }
                return true;
            };
            this.onDragResize_End = function (event) {
                event.stopPropagation();
                var resizeHelper = _this.resizeDragStartInfo.resizeHelper;
                var newHeight = _this.roundToClosestValidHeight(_this.getNumberFromPixelString(resizeHelper.style.height));
                var newWidth = _this.roundToClosestValidWidth(_this.getNumberFromPixelString(resizeHelper.style.width));
                _this.tileElement.parentElement.removeChild(_this.resizeDragStartInfo.resizeHelper);
                log.debug("onDragResize_End");
                // if the new size is the same there won't be a transition
                if (newHeight == _this.height() && newWidth == _this.width()) {
                    _this.showContent();
                }
                else if (newHeight >= _this.minHeight && newHeight <= _this.maxHeight &&
                    newWidth >= _this.minWidth && newWidth <= _this.maxWidth) {
                    _this.height(newHeight);
                    _this.width(newWidth);
                    ko.postbox.publish(_this.dashboardElementId + '-dashboard-modified');
                }
                else {
                    _this.showContent();
                }
                document.removeEventListener('mousemove', _this.onDragResize_Move);
                document.removeEventListener('mouseup', _this.onDragResize_End);
                document.removeEventListener('touchmove', _this.onDragResize_Move);
                document.removeEventListener('touchend', _this.onDragResize_End);
                document.removeEventListener('touchcancel', _this.onDragResize_Cancel);
                // IE
                document.body.style.cursor = 'auto';
                // Chrome
                document.body.style.cursor = null;
                _this.resizeDragStartInfo = null;
                return true;
            };
            this.onDragResize_Cancel = function (event) {
                log.debug("onDragResize_Cancel");
                event.stopPropagation();
                _this.tileElement.parentElement.removeChild(_this.resizeDragStartInfo.resizeHelper);
                _this.showContent();
                document.removeEventListener('mousemove', _this.onDragResize_Move);
                document.removeEventListener('mouseup', _this.onDragResize_End);
                document.removeEventListener('touchmove', _this.onDragResize_Move);
                document.removeEventListener('touchend', _this.onDragResize_End);
                document.removeEventListener('touchcancel', _this.onDragResize_Cancel);
                // IE
                document.body.style.cursor = 'auto';
                // Chrome
                document.body.style.cursor = null;
                _this.resizeDragStartInfo = null;
                return true;
            };
            //#endregion
            this.cancelMouseDownEvent = function (data, event) {
                // This is used in the tile template for buttons to 
                // stop the drag/resize methods firing inadvertently
                event.stopPropagation();
                return false;
            };
            this.tileConfig = params.config;
            var tile = params.tile;
            this.isTouchModeEnabled = utils.isTouchModeEnabled;
            this.isTouchModeDisabled = ko.pureComputed(function () { return !_this.isTouchModeEnabled(); });
            this.gridItemWidth = this.tileConfig.gridItemWidth;
            this.gridItemHeight = this.tileConfig.gridItemHeight;
            this.gutterWidth = this.tileConfig.gutterWidth;
            this.tileContentMarginSize = this.tileConfig.tileContentMarginSize;
            this.dashboardElementId = params.dashboardElementId;
            this.dashboardInEditMode = params.dashboardInEditMode;
            this.tileType = params.tileType;
            this.tileDefinition = params.tileType.tileDefinition;
            this.tileContentMarginSizeString = ko.pureComputed(function () { return sprintf('%dpx', _this.tileConfig.tileContentMarginSize); });
            this.receivePublishMethod = ko.observable(null);
            this.subscriptions = ko.observableArray([]);
            this.id = ko.observable(tile.Id);
            this.rendered = ko.observable(false);
            this.positionX = ko.observable(tile.PositionX);
            this.positionY = ko.observable(tile.PositionY);
            this.width = ko.observable(tile.SizeX).extend({ notify: 'always' });
            this.height = ko.observable(tile.SizeY).extend({ notify: 'always' });
            this.showTitle = ko.observable(tile.DisplayTitle);
            this.title = ko.observable(tile.Title);
            this.showSubtitle = ko.observable(tile.DisplaySubtitle);
            this.subTitle = ko.observable(tile.Subtitle);
            this.startTileObject = ko.observable(tile);
            this.maxWidth = this.tileDefinition.MaxSizeX;
            this.maxHeight = this.tileDefinition.MaxSizeY;
            this.minWidth = this.tileDefinition.MinSizeX;
            this.minHeight = this.tileDefinition.MinSizeY;
            this.resizeDisabled = this.maxWidth == this.minWidth && this.maxHeight == this.minHeight;
            this.tileContentComponent = this.tileDefinition.Component;
            this.showHeader = ko.pureComputed(function () { return _this.showTitle() || _this.showSubtitle(); });
            this.isSmall = ko.pureComputed(function () { return _this.height() == 1 || _this.width() == 1; });
            this.isVerySmall = ko.pureComputed(function () { return _this.height() == 1 && _this.width() == 1; });
            this.tileObject = ko.computed(function () {
                var currentTile = _this.startTileObject();
                currentTile.PositionX = _this.positionX();
                currentTile.PositionY = _this.positionY();
                currentTile.SizeX = _this.width();
                currentTile.SizeY = _this.height();
                return currentTile;
            });
            this.contentAreaHeight = ko.observable(0);
            this.contentAreaWidth = ko.observable(0);
            this.resizeEnabled = !this.resizeDisabled;
            this.showResizeHandle = ko.pureComputed(function () { return _this.resizeEnabled && _this.dashboardInEditMode(); });
            this.xPositionInPixels = ko.pureComputed(function () { return _this.getXTransformPixelsFromPosition(_this.positionX()); });
            this.yPositionInPixels = ko.pureComputed(function () { return _this.getYTransformPixelsFromPosition(_this.positionY()); });
            this.heightInPixels = ko.pureComputed(function () { return _this.getPixelHeightFromHeight(_this.height()); });
            this.widthInPixels = ko.pureComputed(function () { return _this.getPixelWidthFromWidth(_this.width()); });
            this.tileStyles = ko.computed(function () {
                return {
                    'width': sprintf('%dpx', _this.widthInPixels()),
                    'height': sprintf('%dpx', _this.heightInPixels()),
                    'transform': sprintf('translate(%dpx, %dpx)', _this.xPositionInPixels(), _this.yPositionInPixels())
                };
            });
            this.loadErrorMessage = "The tile could not be loaded.";
            this.loadContentComponent = ko.pureComputed(function () { return _this.rendered() && _this.hasLookedForComponent() && _this.componentFound(); });
            this.loadFailure = ko.pureComputed(function () { return _this.rendered() && _this.hasLookedForComponent() && !_this.componentFound(); });
            this.subscriptions.push(this.dashboardInEditMode.subscribe(function (inEditMode) {
                if (inEditMode) {
                    _this.setupDragging();
                }
                else {
                    _this.teardownDragging();
                }
            }));
            this.isContentVisible = ko.pureComputed(function () { return !(_this.dashboardInEditMode() && _this.isTouchModeEnabled()); });
            if (this.tileType.hasLinkedTile) {
                var parsedJson = JSON.parse(tile.Content);
                var linkedTileId = parsedJson.LinkedTileId;
                var publishTopic = this.dashboardElementId + "-tile-" + linkedTileId + "-publish";
                this.subscriptions.push(ko.postbox.subscribe(publishTopic, function (data) { return _this.receivePublish(data); }));
            }
        }
        Tile.prototype.dispose = function () {
            this.tileObject.dispose();
            this.tileStyles.dispose();
            this.subscriptions().forEach(function (s) { return s.dispose(); });
            this.tileElement.removeEventListener('transitionEnd', this.onResize);
            this.tileElement.removeEventListener('mousedown', this.onDragTile_Start);
            this.resizeHandle.removeEventListener('mousedown', this.onDragResize_Start);
        };
        Tile.prototype.tileContentParams = function () {
            return {
                tile: this,
                contentAreaHeight: this.contentAreaHeight,
                contentAreaWidth: this.contentAreaWidth,
                tileConfig: this.tileConfig,
                publish: this.publishAction,
                receivePublishMethodObservable: this.receivePublishMethod
            };
        };
        Tile.prototype.editTile = function () {
            ko.postbox.publish(this.dashboardElementId + "-editTile", this.tileObject());
        };
        Tile.prototype.deleteTile = function () {
            ko.postbox.publish(this.dashboardElementId + "-deleteTile", this.tileObject());
        };
        Tile.prototype.finishedRender = function () {
            var _this = this;
            ko.postbox.publish(sprintf('%s-tile-%s-finished-render', this.dashboardElementId, this.tileObject().Id));
            this.tileElement = document.querySelector(sprintf('div[data-tileid="%s"]', this.tileObject().Id));
            this.resizeHandle = this.tileElement.getElementsByClassName('resize-handle')[0];
            // if the dashboard is refreshed or a tile is added the drag needs to be setup
            if (this.dashboardInEditMode()) {
                this.setupDragging();
            }
            this.tileElement.addEventListener('transitionend', function () {
                _this.showContent();
                _this.onResize();
            });
            ko.components.get(this.tileDefinition.Component, function (x) {
                if (x) {
                    _this.componentFound(true);
                    _this.onResize();
                }
                else {
                    log.warn("Component '" + _this.tileDefinition.Component + "' could not be found");
                    _this.componentFound(false);
                }
                _this.hasLookedForComponent(true);
                _this.rendered(true);
            });
        };
        return Tile;
    }());
    return Tile;
});
