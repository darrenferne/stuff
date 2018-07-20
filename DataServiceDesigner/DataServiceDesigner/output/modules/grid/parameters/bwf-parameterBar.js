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
define(["require", "exports", "knockout", "options", "sprintf", "modules/bwf-valueParser", "modules/bwf-utilities", "modules/grid/parameters/parameterBar-base", "knockout-kendo"], function (require, exports, ko, options, sprintf, valueParser, utilities, parameterBarBase, knockoutKendo) {
    "use strict";
    var koKendo = knockoutKendo;
    var ViewParameterBar = /** @class */ (function (_super) {
        __extends(ViewParameterBar, _super);
        function ViewParameterBar(config) {
            var _this = _super.call(this, config) || this;
            var usernameFilter = utilities.UrlUtilities.getEncodedValue("string", "Username", "=", options.username);
            _this.queryTemplate = config.explorerHostUrl + "/api/" + config.explorerDataService + "/Query/ViewParameterOptions?$expand=Values&$filter=" + usernameFilter;
            _this.changesetUrl = config.explorerHostUrl + "/api/" + config.explorerDataService + "/changeset/ViewParameterOption";
            _this.loadParametersFromRefreshSubscription = ko.postbox.subscribe(_this.viewGridId + '-refreshParameters', function () { return _this.loadParameters(); });
            _this.loadParameters();
            return _this;
        }
        ViewParameterBar.prototype.dispose = function () {
            this.loadParametersFromRefreshSubscription.dispose();
            _super.prototype.dispose.call(this);
        };
        ViewParameterBar.prototype.postOnRender = function () {
            var _this = this;
            if (this.urlParameters().length > 0) {
                this.visibleParameters().forEach(function (p) { return p.loadValuesFromUrl(_this.urlParameters()); });
                this.apply();
            }
            else {
                this.loadParameterValues();
                this.loadFilteredByValues();
            }
        };
        ViewParameterBar.prototype.loadParameterValues = function () {
            var _this = this;
            var queryUrl = this.queryTemplate + sprintf.sprintf(" and ViewId=%d", this.viewId);
            var query = $.ajax({
                xhrFields: { withCredentials: true },
                url: queryUrl
            });
            query.done(function (response) {
                var parameterOptions = response.Records;
                var parameters = _this.visibleParameters();
                parameters.forEach(function (parameter) {
                    var parameterOption = parameterOptions.filter(function (po) { return parameter.id === po.SelectedParameterId; })[0];
                    var parameterValues;
                    if (parameterOption == null) {
                        if (parameter.metadata.parameterDefaultValue == null)
                            return;
                        parameterValues = [parameter.metadata.parameterDefaultValue];
                    }
                    else {
                        parameter.includeEmpty(parameterOption.IncludeEmpty || false);
                        parameterValues = parameterOption.Values.map(function (v) { return v.SelectedValue; });
                    }
                    parameterValues.forEach(function (parameterValue) {
                        var parsedValue = valueParser.parseValue(parameter.type, parameterValue, parameter.metadata.format, "en-GB", options.formattingCulture, [options.dateTimeDisplayFormat], [options.dateDisplayFormat], "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddT00:00:00");
                        parameter.addValue(parsedValue);
                    });
                });
                if (_this.urlFilteredBys().length === 0 && _this.urlParameters().length === 0)
                    _this.savingEnabled(true);
            });
            query.always(function () { return _this.apply(); });
        };
        ViewParameterBar.prototype.saveSelectedParameterValues = function () {
            var _this = this;
            if (!this.savingEnabled())
                return;
            var parametersToSave = {};
            this.selectedParameterValues().forEach(function (spv) {
                var parsedValues = spv.values.map(function (value, index) {
                    var position = spv.position + index;
                    var parsedValue = valueParser.parseValue(spv.type, value, null, options.formattingCulture, "en-GB", [options.dateTimeDisplayFormat], [options.dateDisplayFormat], "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddT00:00:00");
                    return {
                        SelectedValue: parsedValue,
                        Position: position
                    };
                });
                parametersToSave[spv.position] = {
                    Username: options.username,
                    Values: parsedValues || [],
                    IncludeEmpty: spv.includeEmpty,
                    SelectedParameterId: spv.id,
                    ViewId: _this.viewId
                };
            });
            var query = $.ajax({
                url: this.changesetUrl,
                xhrFields: { withCredentials: true },
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({
                    Context: {
                        'ViewId': this.viewId,
                    },
                    Create: parametersToSave
                })
            });
        };
        return ViewParameterBar;
    }(parameterBarBase));
    return ViewParameterBar;
});
