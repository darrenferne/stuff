define(["require", "exports", "knockout", "loglevel", "modules/bwf-metadata"], function (require, exports, ko, log, metadataService) {
    "use strict";
    var ParameterBarTile = /** @class */ (function () {
        function ParameterBarTile(params) {
            var _this = this;
            this.parameterBarComponentName = 'grid/parameters/bwf-standAloneParameterBar';
            this.tile = params.tile.tileObject();
            this.disable = ko.observable(false);
            this.rendered = ko.observable(false);
            this.typeMetadata = ko.observable(null);
            this.tileContent = JSON.parse(this.tile.Content);
            this.selectedParameters = ko.observableArray(this.tileContent.Parameters);
            this.selectedParameterValues = ko.observableArray(this.tileContent.ParameterValues);
            var getMetadataPromise = metadataService.getType(this.tileContent.DataService, this.tileContent.BaseType);
            getMetadataPromise.done(function (metadata) { return _this.typeMetadata(metadata); });
            getMetadataPromise.fail(function (error) { return log.warn("Getting metadata failed -", error); });
            this.selectedParameterValues.subscribe(function (pvs) {
                var publishBody = {
                    "viewParameters": pvs.map(function (p) {
                        return {
                            Operator: p.operator,
                            Property: p.field,
                            Value: p.values.join(",")
                        };
                    })
                };
                var publishData = {
                    publishingTile: _this.tile,
                    body: publishBody
                };
                params.publish(publishData);
            });
        }
        ParameterBarTile.prototype.parameterBarConfiguration = function () {
            return {
                disableParamsBar: this.disable,
                metadata: this.typeMetadata,
                parameterBarRendered: this.rendered,
                forceQueryRefresh: function () { return; },
                enableQuerying: ko.observable(true),
                selectedParameters: this.selectedParameters,
                selectedParameterValues: this.selectedParameterValues,
                urlParameters: [],
                urlFilteredBys: [],
                viewGridId: "parameter-bar-tile-" + this.tile.Id,
                viewId: null
            };
        };
        return ParameterBarTile;
    }());
    return ParameterBarTile;
});
