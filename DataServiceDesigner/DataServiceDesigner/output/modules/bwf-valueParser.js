define(["require", "exports", "kendo.all.min", "options"], function (require, exports, kendo, options) {
    "use strict";
    var ValueParser;
    (function (ValueParser) {
        function parseValue(itemType, toParse, defaultFormat, parseCulture, toStringCulture, parseDateTimeFormats, parseDateFormats, toStringDateTimeFormat, toStringDateFormat) {
            switch (itemType) {
                case 'date':
                    parseDateFormats != null ? parseDateFormats.push.apply(parseDateFormats, ["yyyy-MM-ddT00:00:00", "yyyy-MM-dd"]) : parseDateFormats = ["yyyy-MM-ddT00:00:00", "yyyy-MM-dd"];
                    var parsedDate = kendo.parseDate(toParse, parseDateFormats, parseCulture);
                    if (parsedDate === null)
                        return null;
                    var parsed = kendo.toString(parsedDate, toStringDateFormat, toStringCulture);
                    return parsed;
                case 'integer':
                    var parsedInt = kendo.parseInt(toParse, parseCulture);
                    if (parsedInt === null)
                        return null;
                    var format = defaultFormat === null || defaultFormat === undefined ? 'n0' : defaultFormat;
                    var parsed = kendo.toString(parsedInt, format, toStringCulture);
                    return parsed;
                case 'numeric':
                    var parsedNumeric = kendo.parseFloat(toParse, parseCulture);
                    if (parsedNumeric === null)
                        return null;
                    var format = defaultFormat === null || defaultFormat === undefined ? 'n4' : defaultFormat;
                    var parsed = kendo.toString(parsedNumeric, format, toStringCulture);
                    return parsed;
                case 'time':
                    parseDateTimeFormats != null ? parseDateTimeFormats.push("yyyy-MM-ddTHH:mm:ss") : parseDateTimeFormats = ["yyyy-MM-ddTHH:mm:ss"];
                    var parsedTime = kendo.parseDate(toParse, parseDateTimeFormats.concat(parseDateFormats), parseCulture);
                    if (parsedTime === null)
                        return null;
                    var parsed = kendo.toString(parsedTime, toStringDateTimeFormat, toStringCulture);
                    return parsed;
                default:
                    return toParse;
            }
        }
        ValueParser.parseValue = parseValue;
        function getRawValue(itemType, value) {
            var rawValue;
            switch (itemType) {
                case 'download':
                case 'link':
                    rawValue = value.Text;
                    break;
                case 'enum':
                    rawValue = value.Value;
                    break;
                default:
                    rawValue = value;
            }
            return rawValue;
        }
        ValueParser.getRawValue = getRawValue;
        function getDisplayValue(itemType, value) {
            var displayValue;
            switch (itemType) {
                case 'download':
                case 'link':
                    displayValue = value.Text;
                    break;
                case 'enum':
                    displayValue = value.Text;
                    break;
                case 'measure':
                    displayValue = value.Value + value.Unit;
                    break;
                default:
                    displayValue = value;
            }
            return displayValue === null ? options.resources["bwf_empty"] : displayValue;
        }
        ValueParser.getDisplayValue = getDisplayValue;
    })(ValueParser || (ValueParser = {}));
    return ValueParser;
});
