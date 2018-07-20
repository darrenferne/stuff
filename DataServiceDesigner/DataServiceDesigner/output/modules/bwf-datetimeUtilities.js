define(["require", "exports", "scripts/moment-timezone-with-data"], function (require, exports, momenttz) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    function applyUtcTZToDate(inputString, userTZ, f) {
        var dateTime = momenttz.tz(inputString, 'UTC');
        if (dateTime.isValid()) {
            return dateTime.format(f);
        }
        else {
            return inputString;
        }
    }
    exports.applyUtcTZToDate = applyUtcTZToDate;
    function convertFromUserTZForSavingInUtc(inputString, userTZ, f) {
        var utcTime = momenttz.tz(inputString, userTZ);
        if (utcTime.isValid()) {
            return utcTime.tz('UTC').format(f);
        }
        else {
            return '';
        }
    }
    exports.convertFromUserTZForSavingInUtc = convertFromUserTZForSavingInUtc;
    function reapplyUserTZForSaving(inputString, userTZ) {
        var dateTime = momenttz.utc(inputString); // momenttz.tz(d, 'UTC');
        if (dateTime.isValid()) {
            var moment = setTimezone(dateTime, userTZ);
            return moment.format();
        }
        else {
            return inputString;
        }
    }
    exports.reapplyUserTZForSaving = reapplyUserTZForSaving;
    function convertToTZ(inputString, toTZ, format) {
        var utcTime = momenttz.tz(inputString, 'UTC');
        if (utcTime.isValid()) {
            return utcTime.tz(toTZ).format(format);
        }
        else {
            return '';
        }
    }
    exports.convertToTZ = convertToTZ;
    function setTimezone(moment, timezone) {
        var a = moment.toArray(); // year,month,date,hours,minutes,seconds as an array
        moment = moment.tz(timezone);
        moment.year(a[0])
            .month(a[1])
            .date(a[2])
            .hours(a[3])
            .minutes(a[4])
            .seconds(a[5])
            .milliseconds(a[6]);
        return moment;
    }
    ;
});
