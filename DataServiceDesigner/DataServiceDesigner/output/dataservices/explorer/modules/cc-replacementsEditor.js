define(["require", "exports", "knockout", "modules/bwf-metadata", "modules/bwf-explorer", "options"], function (require, exports, ko, metadataService, explorer, options) {
    "use strict";
    var ReplacementsEditor = /** @class */ (function () {
        function ReplacementsEditor(params) {
            var _this = this;
            this.disposables = [];
            this.gridConfiguration = null;
            this.label = "Replacements";
            this.replacements = ko.observableArray([]);
            this.replacementsAsGridItems = ko.observableArray([]);
            this.rendered = ko.observable(false);
            this.loaded = ko.observable(false);
            this.ready = ko.pureComputed(function () {
                var rendered = _this.rendered();
                var loaded = _this.loaded();
                return rendered && loaded;
            });
            this.canAdd = ko.pureComputed(function () {
                var disabled = _this.formDisabled();
                var ready = _this.ready();
                return ready && !disabled;
            });
            this.canDelete = ko.pureComputed(function () {
                var disabled = _this.formDisabled();
                var ready = _this.ready();
                if (disabled || !ready)
                    return false;
                return _this.selectedReplacements().length > 0;
            });
            this.add = function () { return ko.postbox.publish(_this.gridId + '-replacementEditor-insert-row', { insertAbove: false, callback: _this.sync }); };
            this.onDelete = function () { return ko.postbox.publish(_this.gridId + '-replacementEditor-delete-row', { callback: _this.sync }); };
            this.clearReplacements = function () {
                _this.gridConfiguration.records([]);
                _this.sync();
            };
            this.sync = function () {
                var current = _this.replacements();
                var newReplacements = _this.replacementsAsGridItems().map(function (r) {
                    var replacement = {
                        Id: r.record.Id,
                        Key: r.values['Key'](),
                        Value: r.values['Value']()
                    };
                    return replacement;
                });
                _this.replacements(newReplacements);
            };
            this.formDisabled = params.model.observables['formDisabled'];
            this.r = options.resources;
            this.replacements = (params.model.observables['Replacements']);
            this.createGridConfiguration();
            params.model.observables['__renderedState'].push(this.ready);
        }
        ReplacementsEditor.prototype.dipose = function () {
            for (var _i = 0, _a = this.disposables; _i < _a.length; _i++) {
                var disposable = _a[_i];
                disposable.dispose();
            }
        };
        ReplacementsEditor.prototype.createGridConfiguration = function () {
            var _this = this;
            var promise = metadataService.getType('explorer', 'replacement');
            promise.done(function (metadata) {
                var grid = explorer.generateIdentificationSummaryGridConfiguration(_this.gridId + '-replacementEditor', metadata, _this.formDisabled);
                _this.replacementsAsGridItems = grid.gridItems;
                var config = grid.configuration;
                grid.setRecords(_this.replacements());
                config.inEditMode(true);
                config.disableInsertRecords = false;
                config.disableRemoveRecords = false;
                config.canSelectIndividualCells(true);
                config.createNewRecord = function () {
                    var dsr = {
                        BaseTypeName: "Replacement",
                        TypeName: "Replacement",
                        Data: {},
                        Position: _this.replacements().length + 1,
                    };
                    var i = new explorer.ExplorerGridItem(dsr, config.selectedColumns(), "explorer");
                    return i;
                };
                config.validate = function (record, success, failure) {
                    _this.sync();
                    var pattern = /[A-z_\.]+[A-z_\.\d]*/;
                    var hasValidKey = pattern.test(record.values['Key']());
                    var hasValidValue = record.values['Value']() != null
                        && record.values['Value']().length > 0;
                    var validations = {
                        ModelValidations: [],
                        PropertyValidations: {},
                        Summary: '',
                        Type: 'Replacement'
                    };
                    if (!hasValidKey)
                        validations.PropertyValidations['Key'] = "Must only contain alphanumerics and the characters '.', '_', and '/'";
                    if (!hasValidValue)
                        validations.PropertyValidations['Value'] = "Cannot be empty";
                    validations.Summary = Object.keys(validations.PropertyValidations)
                        .map(function (k) { return validations.PropertyValidations[k]; }).join('; ');
                    failure(record, validations);
                };
                _this.selectedReplacements = config.selectedRecords;
                _this.gridConfiguration = config;
                _this.loaded(true);
            });
        };
        return ReplacementsEditor;
    }());
    return ReplacementsEditor;
});
