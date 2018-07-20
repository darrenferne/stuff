define(["require", "exports", "knockout", "options", "sprintf", "modules/bwf-knockout-validators", "modules/bwf-clipboardButtonBindingHandler"], function (require, exports, ko, options, sprintf) {
    "use strict";
    var PowerBiViewModel = /** @class */ (function () {
        function PowerBiViewModel(panelEntity) {
            var _this = this;
            this.resources = options.resources;
            this.r = options.resources;
            this.baseType = panelEntity.state.typeName;
            this.errorMessages = ko.observableArray([]);
            this.title = this.r["bwf_power_bi"];
            this.grid = panelEntity.state.gridId;
            this.dynamicQuery = ko.observable('');
            this.staticQuery = ko.observable('');
            this.copyStaticQueryText = this.r["bwf_copy_powerbi_static"];
            this.copyDynamicQueryText = this.r["bwf_copy_powerbi_dynamic"];
            this.disableButtons = ko.observable(true);
            var url = sprintf.sprintf("%s/powerbi/queryforview/%s", options.explorerHostUrl, panelEntity.record.Id);
            var request = $.ajax({
                url: url,
                xhrFields: { withCredentials: true },
                type: 'GET',
                dataType: 'json',
                contentType: 'application/json'
            });
            request.done(function (result) {
                _this.dynamicQuery(result.DynamicQuery);
                _this.staticQuery(result.StaticQuery);
                _this.disableButtons(false);
            });
        }
        PowerBiViewModel.prototype.generateKey = function () {
            var payload = {
                action: 'create',
                component: 'ds-membership-bwf-generateKey',
                dataService: 'membership',
                preserveStack: true
            };
            ko.postbox.publish(this.grid + '-doAction', payload);
        };
        PowerBiViewModel.prototype.close = function () {
            ko.postbox.publish(this.grid + '-pop-panel');
        };
        return PowerBiViewModel;
    }());
    return PowerBiViewModel;
});
