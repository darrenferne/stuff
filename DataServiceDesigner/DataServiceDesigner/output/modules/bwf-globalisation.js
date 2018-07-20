define(["require", "exports", "linqjs"], function (require, exports, Enumerable) {
    "use strict";
    var getCultureDateTimeFormats = function (cultureName, formattingCultures) {
        var singleCulture = formattingCultures.filter(function (c) { return c.Name == cultureName; })[0];
        var formats = singleCulture.DateTimeFormats.filter(function (f) { return f.indexOf('HH') > -1; });
        return formats;
    };
    var getCultureDateFormats = function (cultureName, formattingCultures) {
        var singleCulture = Enumerable.from(formattingCultures).single(function (x) { return x.Name == cultureName; });
        var formats = Enumerable.from(singleCulture.DateFormats).where(function (x) { return x.indexOf('HH') == -1; }).select(function (x) { return x; }).toArray();
        return formats;
    };
    var checkDateTimeFormatInFormatCulture = function (dateTimeFormat, formattingCulture, formattingCultures) {
        var formats = getCultureDateTimeFormats(formattingCulture, formattingCultures);
        var isIn = Enumerable.from(formats).any(function (x) { return x == dateTimeFormat; });
        return isIn;
    };
    var checkDateFormatInFormatCulture = function (dateFormat, formattingCulture, formattingCultures) {
        var formats = getCultureDateFormats(formattingCulture, formattingCultures);
        var isIn = Enumerable.from(formats).any(function (x) { return x == dateFormat; });
        return isIn;
    };
    var globalisationFunctions = {
        getCultureDateTimeFormats: getCultureDateTimeFormats,
        getCultureDateFormats: getCultureDateFormats,
        checkDateTimeFormatInFormatCulture: checkDateTimeFormatInFormatCulture,
        checkDateFormatInFormatCulture: checkDateFormatInFormatCulture
    };
    return globalisationFunctions;
});
