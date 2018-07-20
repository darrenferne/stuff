define(["require", "exports", "knockout", "cm/lib/codemirror", "loglevel", "modules/bwf-metadata", "modules/bwf-gridUtilities"], function (require, exports, knockout, codemirror, logLevel, metadataService, gridUtilities) {
    "use strict";
    var plugins = require(['knockout-postbox', 'cm/addon/edit/matchbrackets', 'cm/addon/hint/show-hint', 'cm/addon/hint/anyword-hint', 'cm/mode/selectionfilter', 'cm/addon/edit/closebrackets']);
    var FilterEditor = /** @class */ (function () {
        function FilterEditor(params) {
            var _this = this;
            this.instanceName = "cc-filterEditor";
            this.rendered = knockout.observable(false);
            this.loaded = knockout.observable(false);
            this.ready = knockout.pureComputed(function () {
                var rendered = _this.rendered();
                var loaded = _this.loaded();
                return rendered && loaded;
            });
            this.getMetadata = function (dataService, type) {
                if (!dataService || !type)
                    return;
                metadataService.getType(dataService, type).done(function (data) {
                    _this.propertyCache[type] = data.properties;
                    Object.keys(data.properties)
                        .map(function (key) { return data.properties[key]; })
                        .filter(function (property) { return property._isType; })
                        .forEach(function (property) {
                        // recusively fetch any metadata we don't have but need
                        if (_this.propertyCache[property._clrType] === undefined)
                            _this.getMetadata(dataService, property._clrType);
                    });
                    if (type === _this.BaseType()) {
                        _this.metadata(data);
                    }
                });
            };
            this.setupCodeMirror = function (filter) {
                var filterListener = filter.subscribe(function (filter) {
                    _this.editor.getDoc().setValue(filter);
                });
                var f = filter();
                var e = document.getElementById('cm-filter-editor');
                var options = {
                    lineNumbers: true,
                    lineWrapping: true,
                    matchBrackets: true,
                    readOnly: _this.formDisabled(),
                    autoCloseBrackets: true,
                    extraKeys: { "Ctrl-Space": "autocomplete" },
                    mode: { name: "filter", baseType: _this.BaseType, properties: _this.propertyCache, getTokenType: _this.getTokenType },
                };
                _this.editor = codemirror.fromTextArea(e, options);
                _this.editor.getDoc().setValue(f || "");
                codemirror.registerHelper("hint", "filter", function (cm, options) {
                    var cur = cm.getCursor();
                    var token = _this.editor.getTokenAt(cur);
                    var hints = _this.getHints(token);
                    return {
                        list: hints,
                        from: { ch: replacementStartChar(token), line: cur.line },
                        to: cur
                    };
                });
                var replacementStartChar = function (token) {
                    if (token.string.split('/').length > 1) {
                        return token.start + 1 + token.string.lastIndexOf('/');
                    }
                    else if (token.string.trim().length === 0) {
                        return token.end;
                    }
                    else {
                        return token.start;
                    }
                };
                _this.editor.on("changes", function (editor, changes) {
                    filterListener.dispose();
                    var cur = editor.getDoc().getCursor();
                    var token = _this.editor.getTokenAt(cur);
                    if (token.string.lastIndexOf('/') === token.string.length - 1 && token.string.length > 0)
                        editor.showHint();
                    filter(_this.editor.getDoc().getValue());
                });
                _this.loaded(true);
            };
            // gets the data-type for the given token. Used for suggestions and highlighting
            this.getTokenType = function (token) {
                var property = token.split('/').slice(-1)[0];
                var index = token.lastIndexOf('/');
                var lookup = index === -1
                    ? _this.BaseType()
                    : _this.BaseType() + '/' + token.substring(0, index);
                return _this.getPropertiesFromLastTypeInPath(lookup)[property].type;
            };
            this.getHints = function (token) {
                var completions = [];
                var metadata = _this.metadata();
                var equality = ["in", "notin", "=", "!="];
                var ordering = [">", "<", "<=", ">="];
                var likeable = ["like", "notLike"];
                var nullable = ["isnull", "isnotnull"];
                var conditional = ["or", "and"];
                // get the properties for the last type in the property path
                // to form the base completion list
                var key = _this.BaseType() + '/' + token.string;
                var lookup = key.substring(0, key.lastIndexOf('/'));
                var properties = _this.getPropertiesFromLastTypeInPath(lookup);
                completions = properties
                    ? Object.keys(properties)
                    : [];
                // choose the completions to use based on whether or not the most
                // recent token was an identifier, and if so what type it was
                if (lookup === _this.BaseType() && token.state.stack.length != 0) {
                    var lastToken = token.state.stack.slice(-1)[0];
                    var tokenType = token.state.types.slice(-1)[0];
                    if (lastToken === 'identifier') {
                        switch (tokenType) {
                            case 'number':
                                completions = equality.concat(ordering, nullable);
                                break;
                            case 'string':
                                completions = equality.concat(nullable, likeable);
                                break;
                            case 'atom':
                                completions = equality;
                                break;
                        }
                    }
                    else {
                        completions = completions.concat(conditional);
                    }
                }
                var text = token.string
                    .trim()
                    .split('/')
                    .slice(-1)[0];
                // remove any suggestions that do not begin with whatever text the user
                // has already typed
                var unsorted = completions.filter(function (h) { return _this.lowerEquals(h.substr(0, text.length), text); })
                    .map(function (h) { return { distance: gridUtilities.getLevenshteinDistance(text, h), value: h }; });
                // we sort by how closely the suggestion matches the fragment the user has
                // typed, where "closeness" is the levenshtein distance. If we have no 
                // distances we fallback to sorting the value itself
                var sorted = [];
                if (unsorted.every(function (i) { return i.distance === 0; })) {
                    sorted = unsorted.sort(function (l, r) { return l.value.length < r.value.length ? -1 : l.value.length > r.value.length ? 1 : 0; });
                }
                else {
                    sorted = unsorted.sort(function (l, r) { return l.distance < r.distance ? -1 : l.distance > r.distance ? 1 : 0; });
                }
                // if the property is an object append a slash both to make it obvious and improve suggestions
                return sorted.map(function (h) { return (properties[h.value] == undefined || !properties[h.value]._isType) ? h.value + ' ' : h.value + '/'; });
            };
            this.propertyCache = {};
            this.metadata = knockout.observable(null);
            this.BaseType = params.model.observables['BaseType'];
            this.dataService = params.model.observables['System'];
            if (this.dataService === undefined)
                this.dataService = ko.observable(params.metadata.dataService);
            this.formDisabled = params.model.observables['formDisabled'];
            params.model.observables['__renderedState'].push(this.ready);
            this.metadataQueryParams = knockout.computed(function () {
                var ds = _this.dataService();
                var bt = _this.BaseType();
                return (ds && bt) ? [ds, bt] : ['', ''];
            });
            this.metadataQueryParams.subscribe(function (params) {
                logLevel.debug('getting filter editor metadata for params: ' + params[0] + ' and ' + params[1]);
                if (params.every(function (item) { return item && item.length > 0; })) {
                    _this.getMetadata(params[0], params[1]);
                }
                ;
            });
            this.getMetadata(this.dataService(), this.BaseType());
            this.setupCodeMirror(params.model.observables['Filter']);
        }
        FilterEditor.prototype.getPropertiesFromLastTypeInPath = function (lookup) {
            var fragments = lookup.split('/');
            var type = this.propertyCache[this.BaseType()];
            if (fragments.length < 2)
                return type;
            // walk along the path, getting the properties for each type
            // along the way. Lots of array indices since bar the first item
            // we have property names rather than type names
            for (var i = 1; i <= fragments.length - 1; i++) {
                type = this.propertyCache[type[fragments[i]]._clrType];
            }
            return type;
        };
        FilterEditor.prototype.lowerEquals = function (l, r) {
            return l.toLowerCase() === r.toLowerCase();
        };
        return FilterEditor;
    }());
    return FilterEditor;
});
