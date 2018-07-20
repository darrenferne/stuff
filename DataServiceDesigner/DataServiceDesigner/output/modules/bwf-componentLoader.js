define(["require", "exports", "knockout", "loglevel", "sprintf", "knockout-amd-helpers"], function (require, exports, ko, logLevel, sprintf) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var untypedKo = ko;
    // We globally enable deferUpdates here
    untypedKo.options.deferUpdates = true;
    untypedKo.getComponentNameForNode = function (node) {
        var lowerName = node.nodeName.toLowerCase();
        var nameFragments = lowerName.toLowerCase().split('/');
        var pathlessName = nameFragments[nameFragments.length - 1];
        if (pathlessName.indexOf('cc-') === 0 || pathlessName.indexOf('ds-') === 0 || pathlessName.indexOf('bwf-') === 0) {
            return lowerName;
        }
        return null;
    };
    var bwfLoader = {};
    bwfLoader.getConfig = function (componentName, callback) {
        var lowerName = componentName.toLowerCase();
        var nameFragments = lowerName.split('/');
        var pathlessName = nameFragments[nameFragments.length - 1];
        if (pathlessName.indexOf('ds-') !== 0
            && pathlessName.indexOf('bwf-') !== 0
            && pathlessName.indexOf('to-') !== 0
            && pathlessName.indexOf('cc-') !== 0) {
            logLevel.warn(sprintf.sprintf("Not using BWF component loader for component '%s'.", lowerName));
            callback(null);
        }
        logLevel.debug(sprintf.sprintf("Using BWF component loader for component '%s'.", lowerName));
        if (pathlessName.indexOf('to-') === 0) {
            var config = {};
            config.template = { require: "text!templates/" + lowerName.substring(3) + ".tmpl.html" };
            config.synchronous = true;
            callback(config);
        }
        else {
            var notUsingMessage = sprintf.sprintf("Not using BWF component loader for component '%s' as a view model component could not be loaded", lowerName);
            require(["modules/" + lowerName], function (model) {
                var viewModel = undefined;
                if (typeof model === 'function')
                    viewModel = function (params) { return new model(params); };
                if (typeof model === 'object' && typeof model.default === 'function')
                    viewModel = function (params) { return new (model.default)(params); };
                else if (typeof model === 'object')
                    viewModel = function () { return model; };
                if (model === undefined) {
                    logLevel.warn(notUsingMessage);
                    callback(null);
                    return;
                }
                var config = {
                    template: { require: "text!templates/" + lowerName + ".tmpl.html" },
                    viewModel: viewModel,
                    synchronous: true
                };
                callback(config);
            }, function (error) {
                logLevel.warn(notUsingMessage);
                logLevel.debug(error);
                callback(null);
            });
        }
    };
    ko.components.loaders.unshift(bwfLoader);
});
