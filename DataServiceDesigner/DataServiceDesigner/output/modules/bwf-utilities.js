define(["require", "exports", "sprintf", "modules/bwf-globalisation", "modules/bwf-valueParser", "modules/bwf-datetimeUtilities", "options", "loglevel", "knockout"], function (require, exports, sprintf, globalisation, valueParser, datetimeUtils, options, log, ko) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    function clone(toClone) {
        var serialised = JSON.stringify(toClone);
        return (serialised === undefined ? undefined : JSON.parse(serialised));
    }
    exports.clone = clone;
    function forEach(obj, cb) {
        Object.keys(obj).forEach(function (key, index) { return cb(obj[key], index); });
    }
    exports.forEach = forEach;
    var Results;
    (function (Results) {
        function isMessageSet(result) {
            return result.Messages !== undefined;
        }
        Results.isMessageSet = isMessageSet;
        function isModelValidation(result) {
            return result.ModelValidations !== undefined;
        }
        Results.isModelValidation = isModelValidation;
        function isException(result) {
            return result.fullException !== undefined;
        }
        Results.isException = isException;
    })(Results = exports.Results || (exports.Results = {}));
    function filterEncode(input) {
        if (typeof input !== 'string')
            return input;
        var escaped = false;
        var encoded = '';
        for (var i = 0; i < input.length; i++) {
            var c = input[i];
            switch (c) {
                case '\\':
                    encoded += '\\\\';
                    break;
                case '\'':
                    encoded += '\\\'';
                    break;
                default:
                    encoded += c;
            }
        }
        return encodeURIComponent(encoded);
    }
    exports.filterEncode = filterEncode;
    /**
     * @description Removes all selections from all locations, where a selection is user highlighted text on the page
     */
    function removeAllSelectedRanges() {
        if (isAnySelectionOnPage()) {
            document.getSelection().removeAllRanges();
            // needed for IE to clear the selection from textboxes
            document.getSelection().addRange(document.createRange());
        }
    }
    exports.removeAllSelectedRanges = removeAllSelectedRanges;
    /**
     * @description Return if anything is selected on page, where a selection is user highlighted text on the page
     * @returns True if there is anything selected on the page
     */
    function isAnySelectionOnPage() {
        if (window.getSelection().type) {
            return window.getSelection().type.toLowerCase() === "range";
        }
        else {
            if (document.activeElement.tagName.toLowerCase() === 'input') {
                var inputElement = document.activeElement;
                // check for types that will throw errors if we try
                // to get the start or end of the selection
                if (inputElement.type != 'checkbox' &&
                    inputElement.type != 'radio' &&
                    inputElement.type != 'button' &&
                    inputElement.type != 'file')
                    return inputElement.selectionEnd > inputElement.selectionStart;
            }
            return false;
        }
    }
    exports.isAnySelectionOnPage = isAnySelectionOnPage;
    /**
     * @description Sets the position of the caret in the input element
     * @param element The input element to insert the caret into
     * @param pos The position in which to insert the caret
     */
    function setCaretPosition(element, pos) {
        if (element.setSelectionRange) {
            element.focus();
            element.setSelectionRange(pos, pos);
        }
        else if (element.createTextRange) {
            var range = element.createTextRange();
            range.collapse(true);
            range.moveEnd('character', pos);
            range.moveStart('character', pos);
            range.select();
        }
    }
    exports.setCaretPosition = setCaretPosition;
    // adapted from https://gist.github.com/oslego/7265412
    // forked to https://gist.github.com/Rooster212/4549f9ab0acb2fc72fe3
    // then typescript adapted and commented
    function elementsFromPoint(x, y) {
        var elements = [], previousPointerEvents = [], current, i, d;
        // chrome supports this (and future unprefixed IE/Edge hopefully)
        if (typeof document.elementsFromPoint === "function")
            return Array.prototype.slice.call(document.elementsFromPoint(x, y));
        // IE11/10 should support this
        if (typeof document.msElementsFromPoint === "function")
            return Array.prototype.slice.call(document.msElementsFromPoint(x, y));
        // get all elements via elementFromPoint, and remove them from hit-testing in order
        while ((current = document.elementFromPoint(x, y)) && elements.indexOf(current) === -1 && current != null) {
            // push the element and its current style
            elements.push(current);
            previousPointerEvents.push({
                value: current.style.getPropertyValue('pointer-events'),
                priority: current.style.getPropertyPriority('pointer-events')
            });
            // add "pointer-events: none", to get to the underlying element
            current.style.setProperty('pointer-events', 'none', 'important');
        }
        // restore the previous pointer-events values
        for (i = previousPointerEvents.length; d = previousPointerEvents[--i];) {
            elements[i].style.setProperty('pointer-events', d.value ? d.value : '', d.priority);
        }
        // return our results
        return elements;
    }
    exports.elementsFromPoint = elementsFromPoint;
    var KEY_CODES;
    (function (KEY_CODES) {
        KEY_CODES[KEY_CODES["BACKSPACE"] = 8] = "BACKSPACE";
        KEY_CODES[KEY_CODES["TAB"] = 9] = "TAB";
        KEY_CODES[KEY_CODES["ENTER"] = 13] = "ENTER";
        KEY_CODES[KEY_CODES["SHIFT"] = 16] = "SHIFT";
        KEY_CODES[KEY_CODES["CTRL"] = 17] = "CTRL";
        KEY_CODES[KEY_CODES["ALT"] = 18] = "ALT";
        KEY_CODES[KEY_CODES["ESCAPE"] = 27] = "ESCAPE";
        KEY_CODES[KEY_CODES["SPACEBAR"] = 32] = "SPACEBAR";
        KEY_CODES[KEY_CODES["PAGE_UP"] = 33] = "PAGE_UP";
        KEY_CODES[KEY_CODES["PAGE_DOWN"] = 34] = "PAGE_DOWN";
        KEY_CODES[KEY_CODES["END"] = 35] = "END";
        KEY_CODES[KEY_CODES["HOME"] = 36] = "HOME";
        KEY_CODES[KEY_CODES["DELETE"] = 46] = "DELETE";
        KEY_CODES[KEY_CODES["ARROW_LEFT"] = 37] = "ARROW_LEFT";
        KEY_CODES[KEY_CODES["ARROW_UP"] = 38] = "ARROW_UP";
        KEY_CODES[KEY_CODES["ARROW_RIGHT"] = 39] = "ARROW_RIGHT";
        KEY_CODES[KEY_CODES["ARROW_DOWN"] = 40] = "ARROW_DOWN";
        KEY_CODES[KEY_CODES["A"] = 65] = "A";
        KEY_CODES[KEY_CODES["B"] = 66] = "B";
        KEY_CODES[KEY_CODES["C"] = 67] = "C";
        KEY_CODES[KEY_CODES["D"] = 68] = "D";
        KEY_CODES[KEY_CODES["E"] = 69] = "E";
        KEY_CODES[KEY_CODES["F"] = 70] = "F";
        KEY_CODES[KEY_CODES["G"] = 71] = "G";
        KEY_CODES[KEY_CODES["H"] = 72] = "H";
        KEY_CODES[KEY_CODES["I"] = 73] = "I";
        KEY_CODES[KEY_CODES["J"] = 74] = "J";
        KEY_CODES[KEY_CODES["K"] = 75] = "K";
        KEY_CODES[KEY_CODES["L"] = 76] = "L";
        KEY_CODES[KEY_CODES["M"] = 77] = "M";
        KEY_CODES[KEY_CODES["N"] = 78] = "N";
        KEY_CODES[KEY_CODES["O"] = 79] = "O";
        KEY_CODES[KEY_CODES["P"] = 80] = "P";
        KEY_CODES[KEY_CODES["Q"] = 81] = "Q";
        KEY_CODES[KEY_CODES["R"] = 82] = "R";
        KEY_CODES[KEY_CODES["S"] = 83] = "S";
        KEY_CODES[KEY_CODES["T"] = 84] = "T";
        KEY_CODES[KEY_CODES["U"] = 85] = "U";
        KEY_CODES[KEY_CODES["V"] = 86] = "V";
        KEY_CODES[KEY_CODES["W"] = 87] = "W";
        KEY_CODES[KEY_CODES["X"] = 88] = "X";
        KEY_CODES[KEY_CODES["Y"] = 89] = "Y";
        KEY_CODES[KEY_CODES["Z"] = 90] = "Z";
    })(KEY_CODES = exports.KEY_CODES || (exports.KEY_CODES = {}));
    ;
    var UrlUtilities;
    (function (UrlUtilities) {
        var QueryType;
        (function (QueryType) {
            QueryType[QueryType["Parameters"] = 0] = "Parameters";
            QueryType[QueryType["FilteredBy"] = 1] = "FilteredBy";
        })(QueryType = UrlUtilities.QueryType || (UrlUtilities.QueryType = {}));
        function buildUrl(url, parameterValues, queryType) {
            if (queryType === void 0) { queryType = QueryType.FilteredBy; }
            //Strip any existing url parameters to form the base
            var baseUrl = url.split('?')[0];
            var parameterString = '';
            if (parameterValues.length > 0) {
                if (queryType === QueryType.Parameters)
                    parameterString = '?parameters=(';
                else if (queryType === QueryType.FilteredBy)
                    parameterString = '?filteredby=(';
                else
                    return null;
                var dateTimeFormats = globalisation.getCultureDateTimeFormats(options.formattingCulture, options.formattingCultures);
                var dateFormats = globalisation.getCultureDateFormats(options.formattingCulture, options.formattingCultures);
                var parameterFilters = parameterValues
                    .map(function (spv) {
                    var fragments = spv.values.map(function (value) {
                        var parsedValue = valueParser.parseValue(spv.type, value, null, options.formattingCulture, "en-GB", dateTimeFormats, dateFormats, "dd-MMM-yyyy HH:mm:ss", "dd-MMM-yyyy");
                        var parameterFilter;
                        switch (spv.type) {
                            case 'string':
                            case 'enum':
                                parameterFilter = sprintf.sprintf("%s %s '%s'", spv.field, spv.operator, value.replace(/'/g, "\\'"));
                                break;
                            case 'download':
                            case 'link':
                                parameterFilter = sprintf.sprintf("%s.Text%s'%s'", spv.field, spv.operator, parsedValue);
                                break;
                            default:
                                parameterFilter = spv.field + spv.operator + parsedValue;
                                break;
                        }
                        return parameterFilter;
                    });
                    if (fragments.some(function (f) { return f == null; })) {
                        // hit a broken url parameter, return null
                        log.error(sprintf.sprintf("Failed to parse selected parameter %s", spv.field));
                        return null;
                    }
                    if (spv.includeEmpty)
                        fragments.push(spv.field + " isnull");
                    return fragments.length > 0
                        ? "(" + fragments.join(' or ') + ")"
                        : '';
                }).filter(function (pf) { return pf == null || pf.length > 0; });
                if (parameterFilters.some(function (pF) { return pF == null; })) {
                    // null to signify an list of parameters
                    log.error("Failed to parse selected parameters");
                    return null;
                }
                parameterString += parameterFilters.join(' and ');
                parameterString += ')';
            }
            return baseUrl + parameterString;
        }
        UrlUtilities.buildUrl = buildUrl;
        function getEncodedValue(type, field, operator, value) {
            var dateTimeFormats = globalisation.getCultureDateTimeFormats(options.formattingCulture, options.formattingCultures);
            var dateFormats = globalisation.getCultureDateFormats(options.formattingCulture, options.formattingCultures);
            var parsedValue = valueParser.parseValue(type, value, null, options.formattingCulture, "en-GB", dateTimeFormats, dateFormats, "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-dd");
            if (parsedValue === null)
                return null;
            var fragment;
            switch (type) {
                case 'date':
                    fragment = sprintf.sprintf("%s%sdate(%s)", field, operator, parsedValue);
                    break;
                case 'time':
                    var convertedValue = datetimeUtils.convertToTZ(value, 'UTC', 'YYYY-MM-DDTHH:mm:ssZ');
                    fragment = sprintf.sprintf("%s%sdatetime(%s)", field, operator, convertedValue);
                    break;
                case 'string':
                case 'enum':
                    fragment = sprintf.sprintf("%s %s'%s'", field, operator, value.replace(/'/g, "\\'"));
                    break;
                case 'download':
                case 'link':
                    fragment = sprintf.sprintf("%s.Text%s'%s'", field, operator, parsedValue);
                    break;
                case 'integer':
                    fragment = field + operator + kendo.parseInt(value);
                    break;
                case 'numeric':
                    fragment = field + operator + kendo.parseFloat(value);
                    break;
                default:
                    fragment = field + operator + parsedValue;
                    break;
            }
            return encodeURIComponent(fragment);
        }
        UrlUtilities.getEncodedValue = getEncodedValue;
        ;
    })(UrlUtilities = exports.UrlUtilities || (exports.UrlUtilities = {}));
    var FullscreenAPI;
    (function (FullscreenAPI) {
        function isAnyElementIsFullScreen() {
            if (document.fullscreenElement)
                return true;
            if (document.msFullscreenElement)
                return true;
            if (document.webkitCurrentFullScreenElement)
                return true;
            return false;
        }
        FullscreenAPI.isAnyElementIsFullScreen = isAnyElementIsFullScreen;
        ;
        function requestFullScreen(el) {
            var element = el;
            if (element.requestFullscreen)
                element.requestFullscreen();
            else if (element.msRequestFullscreen)
                element.msRequestFullscreen();
            else if (element.mozRequestFullScreen)
                element.mozRequestFullScreen();
            else if (element.webkitRequestFullscreen)
                element.webkitRequestFullscreen();
        }
        FullscreenAPI.requestFullScreen = requestFullScreen;
        function exitFullScreen() {
            var doc = document;
            if (doc.exitFullscreen)
                doc.exitFullscreen();
            else if (doc.msExitFullscreen)
                doc.msExitFullscreen();
            else if (doc.mozCancelFullScreen)
                doc.mozCancelFullScreen();
            else if (doc.webkitExitFullscreen)
                doc.webkitExitFullscreen();
        }
        FullscreenAPI.exitFullScreen = exitFullScreen;
        function isFullScreenAvailable() {
            return document.fullscreenEnabled ||
                document.webkitFullscreenEnabled ||
                document.mozFullScreenEnabled ||
                document.msFullscreenEnabled || false;
        }
        FullscreenAPI.isFullScreenAvailable = isFullScreenAvailable;
    })(FullscreenAPI = exports.FullscreenAPI || (exports.FullscreenAPI = {}));
    function isObservableArray(value) {
        return ko.isObservable(value) && 'push' in value;
    }
    exports.isObservableArray = isObservableArray;
    var LocalStorageWithExpiry = /** @class */ (function () {
        function LocalStorageWithExpiry(defaultExpiryInSeconds) {
            var _this = this;
            this.defaultExpiry = 3600;
            this.setItem = function (key, data, expiresOn) {
                var object = {
                    value: data,
                    expires: expiresOn || (Math.floor(Date.now() / 1000) + _this.defaultExpiry)
                };
                window.localStorage.setItem(key, JSON.stringify(object));
            };
            this.getItem = function (key) {
                var object = JSON.parse(window.localStorage.getItem(key));
                if (object == null)
                    return null;
                if (object.expires < (Math.floor(Date.now() / 1000))) {
                    localStorage.removeItem(key);
                    return null;
                }
                return object.value;
            };
            this.removeItem = function (key) { return localStorage.removeItem(key); };
            this.clear = function () { return window.localStorage.clear(); };
            if (typeof defaultExpiryInSeconds == 'number' && defaultExpiryInSeconds > 0) {
                this.defaultExpiry = defaultExpiryInSeconds;
            }
        }
        Object.defineProperty(LocalStorageWithExpiry.prototype, "length", {
            get: function () {
                return window.localStorage.length;
            },
            enumerable: true,
            configurable: true
        });
        return LocalStorageWithExpiry;
    }());
    exports.LocalStorageWithExpiry = LocalStorageWithExpiry;
    function isValidImage(value) {
        log.debug("Testing image string");
        if (value.indexOf("data:image/") > -1) {
            value = value.split(",").pop();
        }
        return isValidBase64(value);
    }
    exports.isValidImage = isValidImage;
    function isValidBase64(value) {
        try {
            window.atob(value);
            return true;
        }
        catch (err) {
            log.info("Error occurred while testing image string");
            return false;
        }
    }
    exports.isValidBase64 = isValidBase64;
    //http://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript/21963136#21963136
    function generateGuid() {
        var lut = [];
        for (var i = 0; i < 256; i++) {
            lut[i] = (i < 16 ? '0' : '') + (i).toString(16);
        }
        function getGuid() {
            var d0 = Math.random() * 0xffffffff | 0;
            var d1 = Math.random() * 0xffffffff | 0;
            var d2 = Math.random() * 0xffffffff | 0;
            var d3 = Math.random() * 0xffffffff | 0;
            return lut[d0 & 0xff] + lut[d0 >> 8 & 0xff] + lut[d0 >> 16 & 0xff] + lut[d0 >> 24 & 0xff] + '-' +
                lut[d1 & 0xff] + lut[d1 >> 8 & 0xff] + '-' +
                lut[d1 >> 16 & 0x0f | 0x40] + lut[d1 >> 24 & 0xff] + '-' +
                lut[d2 & 0x3f | 0x80] + lut[d2 >> 8 & 0xff] + '-' +
                lut[d2 >> 16 & 0xff] + lut[d2 >> 24 & 0xff] + lut[d3 & 0xff] +
                lut[d3 >> 8 & 0xff] + lut[d3 >> 16 & 0xff] + lut[d3 >> 24 & 0xff];
        }
        return getGuid();
    }
    exports.generateGuid = generateGuid;
    function getRecordFromGrid(bwfId) {
        return ko.dataFor(document.querySelector('tr[data-bwf-id="' + bwfId + '"]')).row.record;
    }
    exports.getRecordFromGrid = getRecordFromGrid;
    function getRecordsFromGrid(bwfIds) {
        return bwfIds.map(getRecordFromGrid);
    }
    exports.getRecordsFromGrid = getRecordsFromGrid;
    function supportsTouch() {
        return ('ontouchstart' in window) || (navigator.maxTouchPoints > 0) || (navigator.msMaxTouchPoints > 0);
    }
    exports.supportsTouch = supportsTouch;
    // Setup touch mode
    var isTouchModeEnabledStorageKey = 'bwf-inTouchMode';
    var isTouchMode = false;
    var localStorageItem = localStorage.getItem(isTouchModeEnabledStorageKey);
    if (!localStorageItem) {
        isTouchMode = supportsTouch();
        localStorage.setItem(isTouchModeEnabledStorageKey, isTouchMode.toString());
    }
    else {
        isTouchMode = localStorageItem.toLowerCase() === 'true';
    }
    var touchModeEnabled = ko.observable(isTouchMode);
    exports.isTouchModeEnabled = ko.computed({
        read: function () { return touchModeEnabled(); },
        write: function (val) {
            localStorage.setItem(isTouchModeEnabledStorageKey, val.toString());
            touchModeEnabled(val);
        }
    });
    var bwfId = Math.ceil(window.performance.now()) + 1000000;
    /**
     * @description Generate a number based on window.performance.now(), good for uniqueness on single pages
     */
    function getNextBwfId() {
        return bwfId++;
    }
    exports.getNextBwfId = getNextBwfId;
});
