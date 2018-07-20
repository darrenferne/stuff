define(["require", "exports", "knockout", "modules/bwf-metadata", "options", "loglevel", "jquery", "modules/bwf-explorer"], function (require, exports, ko, metadata, options, log, $, explorer) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Tag = /** @class */ (function () {
        function Tag() {
        }
        return Tag;
    }());
    exports.Tag = Tag;
    var TagEditor = /** @class */ (function () {
        function TagEditor(params) {
            var _this = this;
            this.allTags = ko.observableArray([]);
            this.filter = ko.observable('');
            this.ready = ko.observable(false);
            this.subscriptions = [];
            this.setupGrid = function (metadata) {
                var selectedGrid = explorer.generateIdentificationSummaryGridConfiguration(_this.gridId + '-selectedTags', metadata, _this.formDisabled);
                _this.selectedGrid = selectedGrid.configuration;
                _this.subscriptions.push(_this.property.subscribe(function (sr) { return selectedGrid.setRecords(sr); }));
                selectedGrid.setRecords(_this.property());
                _this.ready(true);
            };
            this.availableTags = ko.pureComputed(function () {
                var all = _this.allTags();
                if (all.length == 0)
                    return [];
                var filter = _this.filter();
                var selected = _this.property();
                var available = all.filter(function (t) { return !selected.some(function (s) { return s.Id.toLowerCase() == t.Id.toLowerCase(); }); });
                if (filter == '' || filter == null)
                    return available;
                return available.filter(function (t) { return t.Id.toLowerCase().indexOf(filter.toLowerCase()) > -1; });
            });
            this.addNew = function () {
                var newTag = { Id: _this.filter() };
                var existing = _this.allTags().filter(function (t) { return t.Id.toLowerCase() == newTag.Id.toLowerCase(); })[0];
                if (existing == null) {
                    _this.allTags.push(newTag);
                    _this.property.push(newTag);
                }
                else {
                    _this.add(existing);
                }
                _this.filter('');
            };
            this.add = function (item) {
                if (typeof _this.property === 'function') {
                    var itemIdIndex = _this.property().map(function (t) { return t.Id; }).indexOf(item.Id);
                    if (itemIdIndex < 0)
                        _this.property.push(item);
                }
            };
            this.canClear = ko.pureComputed(function () { return !_this.formDisabled() && _this.property().length > 0; });
            this.clear = function () { return _this.property([]); };
            this.canRemove = ko.pureComputed(function () { return !_this.formDisabled() && _this.selectedGrid.selectedRecords().length > 0; });
            this.remove = function () {
                var selected = _this.selectedGrid.selectedRecords().map(function (s) { return s.record; });
                var current = _this.property();
                var toKeep = current.filter(function (c) { return !selected.some(function (d) { return d.Id === c.Id; }); });
                _this.property(toKeep);
            };
            params.model.observables['__renderedState'].push(this.ready);
            this.property = params.model.observables[params.metadata.name];
            this.r = options.resources;
            this.formDisabled = params.model.observables['formDisabled'];
            this.gridId = params.grid;
            var query = $.ajax(options.explorerHostUrl + '/api/explorer/query/Tags', {
                xhrFields: {
                    withCredentials: true
                },
            });
            query.done(function (result) {
                _this.allTags(result.Records);
            });
            query.fail(log.error);
            this.subscriptions.push({ dispose: function () { return query.abort(); } });
            metadata.getType('explorer', 'Tag').done(this.setupGrid);
        }
        TagEditor.prototype.dispose = function () {
            this.subscriptions.forEach(function (s) { return s.dispose(); });
        };
        return TagEditor;
    }());
    exports.default = TagEditor;
});
