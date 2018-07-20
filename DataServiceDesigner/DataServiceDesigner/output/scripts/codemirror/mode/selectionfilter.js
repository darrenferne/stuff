(function (mod) {
    if (typeof exports == "object" && typeof module == "object") // CommonJS
        mod(require("../lib/codemirror"));
    else if (typeof define == "function" && define.amd) // AMD
        define(["../lib/codemirror"], mod);
    else // Plain browser env
        mod(CodeMirror);
})(function (CodeMirror) {
    "use strict";

    CodeMirror.defineMode("filter", function (config, parserConfig) {
        "use strict";

        var getTokenType = parserConfig.getTokenType;

        function isValidValue(lastToken){
            return (lastToken === 'semicolon' || lastToken === 'value-operator' || lastToken === 'open-bracket');
        }

        var binaryOperators = ['<', '>', '=', '!'];
        var unaryOperators = ['isnull', 'isnotnull'];
        var conditionals = ['and', 'or'];
        var likes = ['like', 'notLike'];
        var constants = ['true', 'false'];
        var listFilters = ['in', 'notin'];
        var dateFormatters = ['date', 'datetime'];

        function isIn(array, value) {
            for (var i = 0; i < array.length; i++) {
                if (array[i] === value) {
                    return true;
                }
            }
            return false;
        }

        function isValidProperty(property) {
            var fullPath = parserConfig.baseType() + '/' + property;
            var objectPath = fullPath.substring(0, fullPath.lastIndexOf('/'))
            var pathFragments = objectPath.split('/');
            var object = parserConfig.properties[parserConfig.baseType()];

            if (pathFragments.length > 1) {
                for (var i = 1; i <= pathFragments.length - 1; i++) {
                    object = parserConfig.properties[object[pathFragments[i]]._clrType];
                }
            }

            if (object === undefined)
            {
                return false;
            }

            if (typeof object === 'object') {
                var keys = Object.keys(object)
                var p = fullPath.split('/').slice(-1)[0];
                var matching = keys.filter(function (i) {
                    return i === p;
                }).length
                return matching === 1;
            }
        }

        function eatString(stream) {
            var escaped = false, ch;
            while ((ch = stream.next()) != null) {
                if (ch == "'" && !escaped) {
                    break;
                }
                escaped = !escaped && ch == "\\";
            }
        }

        function foldTypeName(type) {
            type = type !== undefined ? type.toLowerCase() : '';
            if (type === 'integer' || type === 'time' || type === 'date' || type === 'numeric' || type === 'measure') {
                return 'number';
            } else if (type === 'boolean') {
                return 'atom'
            } else if (type === 'string' || type === 'enum') {
                return 'string';
            } else {
                return '';
            }
        }

        /*
        *   a regular expression that should match either a date or time
        * 
        *   (
        *       \d{3}-[01]\d-[0-3]\d[T][0-2]\d:[0-5]\d:[0-5]\d([+-][0-2]\d:[0-5]\d|Z))    a datetime in format 'yyyy-MM-ddTHH:mm:sszzz'
        *   )
        *   |   or
        *   (
        *       \d{3}-[01]\d-[0-3]\d    a date in format 'yyyy-MM-dd'
        *   )

        *   a regular expression that should match a number
        *   (
        *       -?[0-9]*\.?[0-9]+       a number that may negative and may have decimal places
        *       ([eE][-+]?[0-9]+)?      an optional standard form 
        *   )/
        * 
        */

        function getTokenStyle(stream, state) {
            var previous = state.stack.slice(-1)[0];
            var type = state.types.slice(-1)[0];

            var char = stream.next();
            var code = char.charCodeAt(0);
            if (code === 45 || (code > 47 && code < 58)) {
                if (previous === 'open-paren-date' && code !== 45) { //DateTime
                    stream.match(/^(\d{3}-[01]\d-[0-3]\d[T][0-2]\d:[0-5]\d:[0-5]\d([+-][0-2]\d:[0-5]\d|Z))|(\d{3}-[01]\d-[0-3]\d)/);
                    state.stack.push('value');
                    return type === 'number' ? 'number' : 'number error';
                }
                else { //Number
                    stream.match(/^(\-?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)/);
                    state.stack.push('value');
                    return isValidValue(previous) && type === 'number' ? 'number' : 'number error';
                }                

            } else if (char === ';') {
                state.stack.push('semicolon');
                return previous === 'value' ? 'bracket' : 'bracket error';

            } else if (char === "'") {
                eatString(stream);
                state.stack.push('value');
                return isValidValue(previous) && type === 'string' ? 'string' : 'string error';

            } else if (isIn(binaryOperators, char)) {
                stream.eatWhile(/^[<>!=]/);
                var c = stream.current();
                var valid = previous === 'identifier' && (c.length === 1 || isIn(['<=', '>=', '!='], c));
                state.stack.push('value-operator');
                return valid ? 'operator' : 'operator error';

            } else if (char === '[') {
                state.stack.push('open-bracket');
                return isIn(listFilters, previous) ? 'bracket' : 'bracket error';

            } else if (char === ']') {
                state.stack.push('close-bracket');
                return previous === 'value' ? 'bracket' : 'bracket error';

            } else if (char === '(') {
                if (previous === 'date-formatter') {
                    state.stack.push('open-paren-date');
                    return 'bracket';
                }
                else {
                    state.stack.push('open-paren');
                    return previous == null || previous === 'conditional' ? 'bracket' : 'bracket error';
                }

            } else if (char === ')') {
                state.stack.push('close-paren');
                return previous === 'value' || previous === 'close-bracket' ? 'bracket' : 'bracket error';

            } else {
                stream.eatWhile(/^[\w\/]/)
                var originalCase = stream.current();
                var word = originalCase.toLowerCase();

                if (isIn(unaryOperators, word)) {
                    state.stack.push('value');
                    return previous === 'identifier' ? 'atom' : 'error';

                } else if (isIn(conditionals, word)) {
                    var valid = previous === 'value' || previous === 'close-bracket' || previous === 'close-paren';
                    state.stack.push('conditional');
                    return valid ? 'keyword' : 'keyword error';

                } else if (isIn(constants, word)) {
                    state.stack.push('value');
                    return isValidValue(previous) && type === 'atom' ? 'atom' : 'atom error';

                } else if (isIn(listFilters, word)) {
                    state.stack.push('in');
                    return previous === 'identifier' ? 'keyword' : 'keyword error';

                } else if (isIn(likes, word)) {
                    state.stack.push('value-operator');
                    return previous === 'identifier' ? 'operator' : 'operator error';

                } else if (isIn(dateFormatters, word)) {
                    state.stack.push('date-formatter');
                    return previous === 'value-operator' ? 'keyword' : 'keyword error';

                } else {
                    var validPosition = previous == null || previous == 'open-paren' || previous == 'conditional'
                    var valid = validPosition && isValidProperty(originalCase);

                    if (valid) {
                        state.stack.push('identifier');
                        state.types.push(foldTypeName(getTokenType(originalCase)));
                    }

                    return valid ? 'variable-1' : 'variable-1 error';
                }
            }
        }

        return {
            startState: function () {
                return {
                    stack: [],
                    types: []
                }
            },

            token: function (stream, state) {
                if (stream.eatSpace()) { return null; }
                return getTokenStyle(stream, state);
            }
        };
    });
});