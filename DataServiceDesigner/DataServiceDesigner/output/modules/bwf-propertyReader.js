define(["require", "exports", "knockout"], function (require, exports, ko) {
    "use strict";
    var propertyReader = /** @class */ (function () {
        function propertyReader() {
        }
        propertyReader.getPropertyValue = function (propertyPath, sourceObject) {
            var ps = propertyPath.split("/");
            var obj = sourceObject;
            if (typeof obj === 'object' && obj !== null) {
                for (var i = 0; i < ps.length; i++) {
                    var propertyExists = Object.keys(obj).indexOf(ps[i]) !== -1;
                    obj = ko.unwrap(propertyExists ? obj[ps[i]] : obj[ps[i]] = "");
                    if (obj === null) {
                        return null;
                    }
                }
                return obj;
            }
            else {
                return null;
            }
        };
        propertyReader.setPropertyValue = function (propertyPath, rootObject, newValue) {
            var fragments = propertyPath.split('/');
            var obj = rootObject;
            for (var i = 0; i < fragments.length - 1; i++) {
                obj = obj[fragments[i]];
            }
            obj[fragments[fragments.length - 1]] = newValue;
        };
        return propertyReader;
    }());
    return propertyReader;
});
