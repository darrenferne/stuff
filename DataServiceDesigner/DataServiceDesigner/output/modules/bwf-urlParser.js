define(["require", "exports"], function (require, exports) {
    "use strict";
    var urlParser = {
        parseUrl: function (locationHash) {
            var urlParams = [], urlFilterby = [];
            var toParse = decodeURIComponent(locationHash);
            var paramsPosition = toParse.indexOf('?parameters=');
            var params = paramsPosition > -1
                ? toParse.substring(paramsPosition + 12)
                : '';
            var presetParamsPosition = toParse.indexOf('?filteredby=');
            var presetParams = presetParamsPosition > -1
                ? toParse.substring(presetParamsPosition + 12)
                : '';
            var furtherSplitPattern = /([\w|/]+)\s*(>=|<=|<|>|=|!=|\slike|\snotLike)\s*(.+)/i;
            var isNull = /^([\w|/]+)\s+(isnull)/i;
            if (params !== '') {
                var splitByAnd = params.split(' and ');
                urlParams = splitByAnd.map(function (item) {
                    var trimmed = item.trim();
                    if (trimmed.search(isNull) === 0) {
                        var name = trimmed.split(isNull);
                        return {
                            Property: name[1],
                            Operator: null,
                            Value: null,
                            AllowNull: true
                        };
                    }
                    else {
                        var furtherSplit = trimmed.split(furtherSplitPattern);
                        var operator = furtherSplit[2].trim();
                        return {
                            Property: furtherSplit[1].trim(),
                            Operator: operator,
                            Value: furtherSplit[3].trim().replace(/^'+|\'$/g, "")
                        };
                    }
                });
            }
            ;
            if (presetParams !== '') {
                if (presetParams[0] === '(')
                    presetParams = presetParams.substring(1, presetParams.length - 1);
                var splitByAnd = presetParams.split(/ and /);
                splitByAnd.forEach(function (item) {
                    var trimmed = item.trim();
                    if (trimmed[0] === '(')
                        trimmed = trimmed.substring(1, trimmed.length - 1);
                    var splitByOr = trimmed.split(/ or /);
                    splitByOr.forEach(function (item) {
                        var trimmed = item.trim();
                        if (trimmed[0] === '(')
                            trimmed = trimmed.substring(1, trimmed.length - 1);
                        if (trimmed.search(isNull) === 0) {
                            var name = trimmed.split(isNull);
                            urlFilterby.push({
                                Property: name[1],
                                Operator: null,
                                Value: null,
                                AllowNull: true
                            });
                        }
                        else {
                            var furtherSplit = trimmed.split(furtherSplitPattern);
                            urlFilterby.push({
                                Property: furtherSplit[1].trim(),
                                Operator: furtherSplit[2].trim(),
                                Value: furtherSplit[3].trim().replace(/^'+|\'$/g, "")
                            });
                        }
                    });
                });
            }
            ;
            return {
                urlParameters: urlParams,
                urlFilteredby: urlFilterby
            };
        }
    };
    return urlParser;
});
