"use strict";
define(["require", "exports", 'knockout'], function (require, exports, ko) {
    "use strict";
    var AggregatedComponents = (function () {
        function AggregatedComponents() {
            var _this = this;
            this.addGrid = function (gridElement) {
                var grid = ko.dataFor(document.getElementById(gridElement));
                _this.components[gridElement] = grid.footer;
                _this.grids.push(gridElement);
                if (_this.grids().length == 1)
                    _this.setActiveGrid(gridElement);
                ko.postbox.publish(gridElement + "-use-combined-footer");
            };
            this.removeGrid = function (gridElement) {
                var current = _this.activeGrid();
                _this.grids.remove(gridElement);
                if (_this.activeGrid() == gridElement) {
                    if (_this.grids().length > 0)
                        _this.activeGrid(_this.grids()[0]);
                    else
                        _this.activeGrid('');
                }
                delete _this.components[gridElement];
            };
            this.setActiveGrid = function (gridElement) {
                var current = _this.activeGrid();
                if (gridElement == null || gridElement == '') {
                    _this.activeGrid('');
                }
                if (current == gridElement)
                    return;
                var config = _this.components[gridElement];
                if (config != null && config.enabled())
                    _this.activeGrid(gridElement);
            };
            this.activeGrid = ko.observable('');
            this.grids = ko.observableArray([]);
            this.components = {};
            this.subscriptions = [];
            this.subscriptions.push(ko.postbox.subscribe('bwf-grid-disposing', this.removeGrid));
            this.subscriptions.push(ko.postbox.subscribe("bwf-grid-addPageEvents", this.addGrid));
            this.subscriptions.push(ko.postbox.subscribe("set-active-grid", this.setActiveGrid));
        }
        AggregatedComponents.prototype.dispose = function () {
            this.subscriptions.forEach(function (sub) { return sub.dispose(); });
        };
        return AggregatedComponents;
    }());
});
