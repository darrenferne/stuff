define(["require", "exports", "loglevel", "modules/bwf-metadata", "options", "kendo.all.min", "modules/bwf-datetimeUtilities", "sprintf"], function (require, exports, log, metadataService, options, kendo, datetimeUtils, sprintf) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Array;
    (function (Array) {
        function findIndex(source, predicate) {
            if (source == null)
                throw new TypeError('Array.findIndex called on null or undefined');
            if (typeof predicate !== 'function')
                throw new TypeError('predicate must be a function');
            var length = source.length;
            for (var i = 0; i < length; i++) {
                if (predicate(source[i]))
                    return i;
            }
            return -1;
        }
        Array.findIndex = findIndex;
        ;
    })(Array = exports.Array || (exports.Array = {}));
    function parseValueWithUnit(input) {
        // a string consisting of a number optionally with a decimal fragment followed by a non-number unit specifyer
        var fragments = input.match(/^(\d+[\.,\d*]*)(\D*)$/);
        var parsed = fragments === null ? null : kendo.parseFloat(fragments[1]);
        if (parsed !== null) {
            return {
                Unit: fragments[2],
                Value: parsed
            };
        }
        else {
            return null;
        }
    }
    exports.parseValueWithUnit = parseValueWithUnit;
    function formatValue(value, type, defaultFormat) {
        var format = defaultFormat;
        var culture = options.formattingCulture;
        if (value === null)
            return '';
        switch (type) {
            case 'date':
                var f = format || options.dateDisplayFormat;
                var parsed = Date.parse(value);
                if (isNaN(parsed)) {
                    return '';
                }
                else {
                    var parsedValue = kendo.toString(new Date(parsed), 'yyyy-MM-ddTHH:mm:sszzz', culture);
                    var convertedValue = datetimeUtils.convertToTZ(parsedValue, 'UTC', 'YYYY-MM-DDTHH:mm:ss');
                    return kendo.toString(kendo.parseDate(convertedValue), f, culture);
                }
            case 'time':
                var f = format || options.dateTimeDisplayFormat;
                var parsed = Date.parse(value);
                if (isNaN(parsed)) {
                    return '';
                }
                else {
                    var parsedValue = kendo.toString(new Date(parsed), 'yyyy-MM-ddTHH:mm:sszzz', culture);
                    var convertedValue = datetimeUtils.convertToTZ(parsedValue, options.derivedTimezone, 'YYYY-MM-DDTHH:mm:ss');
                    return kendo.toString(kendo.parseDate(convertedValue), f, culture);
                }
            case 'enum':
                return typeof value === 'object' ? value.Text : value;
            case 'measure':
                return typeof value === 'object'
                    ? kendo.toString(value.Value, format, culture) + value.Unit
                    : kendo.toString(value.Value, format, culture);
            case 'integer':
            case 'numeric':
                return kendo.toString(value, format, culture);
            case 'collection':
                if (value.length === 1)
                    return "1 item";
                else if (value.length > 1)
                    return sprintf.sprintf('%i items', value.length);
                return '';
            default:
                return value;
        }
    }
    exports.formatValue = formatValue;
    function getLevenshteinDistance(left, right) {
        var a = left, b = right;
        if (a.length === 0)
            return b.length;
        if (b.length === 0)
            return a.length;
        var matrix = [];
        // increment along the first column of each row
        var i;
        for (i = 0; i <= b.length; i++) {
            matrix[i] = [i];
        }
        // increment each column in the first row
        var j;
        for (j = 0; j <= a.length; j++) {
            matrix[0][j] = j;
        }
        // Fill in the rest of the matrix
        for (i = 1; i <= b.length; i++) {
            for (j = 1; j <= a.length; j++) {
                if (b.charAt(i - 1) == a.charAt(j - 1)) {
                    matrix[i][j] = matrix[i - 1][j - 1];
                }
                else {
                    matrix[i][j] = Math.min(matrix[i - 1][j - 1] + 1, // substitution
                    Math.min(matrix[i][j - 1] + 1, // insertion
                    matrix[i - 1][j] + 1)); // deletion
                }
            }
        }
        return matrix[b.length][a.length];
    }
    exports.getLevenshteinDistance = getLevenshteinDistance;
    function constructChoiceUrl(typeMetadata, propertyMetadata, refreshOnChangesTo, property, options) {
        var refinedUrl = '';
        var scope = {};
        scope.options = options;
        scope.refreshOnChangesTo = {};
        if (refreshOnChangesTo != null && typeof refreshOnChangesTo === 'object') {
            // copy the properties into our scope object in case the eval'd url needs them
            Object.keys(refreshOnChangesTo)
                .forEach(function (key) {
                scope[key] = refreshOnChangesTo[key];
            });
        }
        var func = new Function("self", "body", "return eval(body)");
        if (propertyMetadata.populateChoiceUrl && propertyMetadata.populateChoiceUrl.length > 0) {
            scope.dataService = metadataService.getDataService(typeMetadata.dataService);
            try {
                refinedUrl = func.apply(scope, [scope, propertyMetadata.populateChoiceUrl]);
            }
            catch (e) {
                log.warn("Error attempting to generate choice url:", e);
                refinedUrl = '';
            }
        }
        else if (propertyMetadata.populateChoiceQuery && propertyMetadata.populateChoiceQuery.length > 0) {
            scope.dataService = typeMetadata.dataService;
            var url = '';
            try {
                url = func.apply(scope, [scope, propertyMetadata.populateChoiceQuery]);
            }
            catch (e) {
                url = '';
                log.warn("Error attempting to generate choice url:", e);
            }
            // a string consisting of a string followed by a slash then any sequence of characters
            var fragments = url.match(/(\w+)(\/.+)(['"])?$/);
            var dataservice = metadataService.getDataService(fragments[1]);
            refinedUrl = dataservice.url + fragments[2];
        }
        else {
            var dataService = metadataService.getDataService(propertyMetadata.dataService);
            if (propertyMetadata.isEnum) {
                refinedUrl = dataService.url + '/enumerations/' + propertyMetadata._clrType;
            }
            else {
                var metadata = typeMetadata.properties[property.split('/').splice(-2)[0]];
                refinedUrl = sprintf.sprintf("%s/Query/%ss?$orderby=%s", dataService.url, metadata._clrType, metadata.displayFieldInEditorChoice);
            }
        }
        return refinedUrl;
    }
    exports.constructChoiceUrl = constructChoiceUrl;
});
