var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "modules/bwf-gridUtilities", "modules/bwf-propertyReader", "modules/bwf-metadata", "knockout", "options", "sprintf", "modules/grid/cells/cell-base"], function (require, exports, utilities, reader, metadataService, ko, options, sprintfM, cell) {
    "use strict";
    var sprintf = sprintfM.sprintf;
    var EditableHasChoiceCell = /** @class */ (function (_super) {
        __extends(EditableHasChoiceCell, _super);
        function EditableHasChoiceCell(params) {
            var _this = _super.call(this, params, true) || this;
            _this.optionsCaption = "[" + options.resources['bwf_none'] + "]";
            _this.pasteValue = function (value) {
                EditableHasChoiceCell.Requests[_this.choiceUrl].done(function () { return _this.value(value); });
            };
            _this.unfilteredChoices = ko.observableArray();
            _this.filter = function () { return true; };
            _this.filteredOn = [];
            _this.choices = ko.pureComputed(function () {
                // go and get dependencies on any value derived from one of the values
                // we filter on so that if the filtered on value changes we re-evaluate
                // which options are valid to show in the dropdown
                _this.filteredOn.forEach(function (f) { return Object.keys(_this.row.values)
                    .filter(function (key) { return key.indexOf(f.split(',')[0] + '/') == 0; })
                    .forEach(function (v) { return _this.row.values[v](); }); });
                return _this.unfilteredChoices().filter(function (i) { return _this.filter(i); });
            });
            _this.isType = _this.column.path.indexOf('/') > -1 && params.column.metadata.name !== 'BaseType';
            _this.isEnum = _this.column.metadata.type === 'enum';
            _this.originalValue = ko.toJSON(_this.recordValue());
            _this.isChangeTrackingDisabled = params.row.isChangeTrackingDisabled;
            if (!_this.isChangeTrackingDisabled)
                _this.recordValue.resetIsDirty = function () {
                    _this.originalValue = ko.toJSON(_this.recordValue());
                    _this.isDirty(false);
                };
            if (_this.isType) {
                var pieces = _this.column.path.split('/');
                if (pieces.length > 2)
                    _this.isEditable(false);
                else {
                    _this.subTypeMetadata = _this.typeMetadata.properties[pieces[0]];
                    _this.isEditable(!_this.subTypeMetadata.isNotEditableInGrid && !_this.value.isReadonly());
                }
            }
            else {
                _this.isEditable(!_this.column.metadata.isNotEditableInGrid && !_this.value.isReadonly());
            }
            _this.value = ko.computed({
                read: function () {
                    var value = _this.recordValue();
                    if (_this.isEnum) {
                        if (_this.isEditable())
                            return value == null ? null : value["Value"];
                        else
                            return value == null ? null : value["Text"];
                    }
                    else {
                        return value;
                    }
                },
                write: function (newValue) {
                    var selectedObject = _this.choices().filter(function (c) { return c[_this.valueField] === newValue; })[0] || null;
                    // if we don't have a selected object, we probably have a pasted in value
                    // so we check the text field because that is what we copy from the grid
                    if (!selectedObject)
                        selectedObject = _this.choices().filter(function (c) { return c[_this.textField] === newValue; })[0] || null;
                    if (_this.isEnum)
                        _this.recordValue(selectedObject);
                    else if (selectedObject != null)
                        _this.recordValue(selectedObject[_this.valueField]);
                    else
                        _this.recordValue(null);
                    _this.valueChanged(selectedObject);
                }
            });
            if (_this.isEditable)
                _this.configureForChoices();
            _this.validateObject(_this.recordValue());
            return _this;
        }
        EditableHasChoiceCell.prototype.dispose = function () {
            var url = this.choiceUrl;
            this.value.dispose();
            if (!this.isChangeTrackingDisabled) {
                this.recordValue.resetIsDirty = function () { return; };
                this.recordValue.isDirty(false);
            }
            EditableHasChoiceCell.Count[url]--;
            if (EditableHasChoiceCell.Requests[url]
                && EditableHasChoiceCell.Count[url] <= 0) {
                var request = EditableHasChoiceCell.Requests[url];
                if (request['abort'])
                    request.abort();
                EditableHasChoiceCell.Options[url].removeAll();
                delete EditableHasChoiceCell.Requests[url];
                delete EditableHasChoiceCell.Options[url];
                delete EditableHasChoiceCell.Count[url];
            }
        };
        EditableHasChoiceCell.prototype.getClipboardValue = function () {
            var _this = this;
            var value = this.value();
            if (value == null)
                return '';
            if (this.isEnum)
                return value;
            var selectedObject = this.choices()
                .filter(function (c) { return c[_this.valueField] === value; })[0] || null;
            if (selectedObject == null)
                return '';
            return selectedObject[this.textField];
        };
        EditableHasChoiceCell.prototype.validateObject = function (selectedObject) {
            if (selectedObject)
                this.isValid(true);
            else
                this.isValid(this.column.metadata.isNullable);
            if (this.isValid())
                this.recordValue.validationMessages.remove(EditableHasChoiceCell.NotNullMessage);
            else {
                if (!this.recordValue.validationMessages().some(function (m) { return m == EditableHasChoiceCell.NotNullMessage; }))
                    this.recordValue.validationMessages.push(EditableHasChoiceCell.NotNullMessage);
            }
        };
        EditableHasChoiceCell.prototype.valueChanged = function (selectedObject) {
            var _this = this;
            if (this.row.updateType() === 'None')
                this.row.updateType('Edited');
            // for choices this is not a computed
            if (!this.isChangeTrackingDisabled)
                this.recordValue.isDirty(this.originalValue !== ko.toJSON(this.recordValue()));
            if (this.isType) {
                this.row.dirtyRecord[this.column.path.split('/')[0]] = selectedObject;
                var typePropertyName = this.column.path.split('/')[0];
                Object.keys(this.row.values)
                    .filter(function (key) { return key.indexOf(typePropertyName) === 0; })
                    .filter(function (key) { return key !== _this.column.path; })
                    .forEach(function (key) {
                    if (selectedObject)
                        _this.row.values[key](reader.getPropertyValue(key, _this.row.dirtyRecord));
                    else
                        _this.row.values[key](null);
                });
            }
            else if (this.column.metadata.type === 'enum') {
                this.row.dirtyRecord[this.column.path] = selectedObject;
            }
            else {
                this.row.dirtyRecord[this.column.path] = (selectedObject == null || this.valueField == null) ? null : selectedObject[this.valueField];
            }
            this.validateObject(selectedObject);
            ko.postbox.publish(this.gridId + '-update-clipboard-value');
        };
        EditableHasChoiceCell.prototype.configureForChoices = function () {
            var _this = this;
            if (this.isType) {
                var propertyName = this.column.path.split('/')[0];
                this.valueField = this.column.path.split('/')[1];
                this.textField = this.typeMetadata.properties[propertyName].displayFieldInEditorChoice;
            }
            else if (this.column.metadata.type === 'enum') {
                this.valueField = 'Value';
                this.textField = 'Text';
            }
            else {
                this.valueField = this.column.metadata.valueFieldInEditorChoice;
                this.textField = this.column.metadata.displayFieldInEditorChoice;
            }
            if (this.isType) {
                if (this.subTypeMetadata.filteredOn.length > 0) {
                    this.filteredOn = this.subTypeMetadata.filteredOn;
                    this.filter = function (item) {
                        return _this.filteredOn.every(function (f) {
                            var pieces = f.split(',');
                            var property = pieces[0], path = pieces[1];
                            var metadata = metadataService.getPropertyWithPrefix(_this.typeMetadata.dataService, _this.typeMetadata, property);
                            var value = _this.row.dirtyRecord[property];
                            if (metadata._isType)
                                value = value[metadata.valueFieldInEditorChoice];
                            var iv = reader.getPropertyValue(path, item);
                            return iv && value && (iv.toString().toLowerCase() == value.toString().toLowerCase());
                        });
                    };
                }
            }
            var refreshOnChangesValue = {};
            if (this.column.metadata.refreshChoiceOnChangesTo) {
                this.column.metadata.refreshChoiceOnChangesTo.split(';').forEach(function (c) {
                    refreshOnChangesValue['selected' + c] = function (item) { return _this.row.dirtyRecord[c]; };
                });
            }
            this.choiceUrl = utilities.constructChoiceUrl(this.typeMetadata, this.column.metadata, refreshOnChangesValue, this.column.path, null);
            this.fetchOptions();
        };
        EditableHasChoiceCell.prototype.fetchOptions = function () {
            var _this = this;
            var choiceUrl = this.choiceUrl;
            if (EditableHasChoiceCell.Requests[choiceUrl] === undefined) {
                EditableHasChoiceCell.Options[choiceUrl] = ko.observableArray([]);
                EditableHasChoiceCell.Count[choiceUrl] = 1;
                if (!choiceUrl) {
                    EditableHasChoiceCell.Requests[choiceUrl] = $.Deferred();
                    return;
                }
                EditableHasChoiceCell.Requests[choiceUrl] = $.ajax({
                    url: choiceUrl,
                    xhrFields: {
                        withCredentials: true
                    }
                });
                EditableHasChoiceCell.Requests[choiceUrl].done(function (response) {
                    var items;
                    if (Array.isArray(response))
                        items = response;
                    else
                        items = response.Records;
                    ko.utils.arrayPushAll(_this.unfilteredChoices, items);
                });
            }
            else {
                EditableHasChoiceCell.Count[choiceUrl]++;
            }
            this.unfilteredChoices = EditableHasChoiceCell.Options[choiceUrl];
        };
        EditableHasChoiceCell.Requests = {};
        EditableHasChoiceCell.Options = {};
        EditableHasChoiceCell.Count = {};
        EditableHasChoiceCell.NotNullMessage = "Value must not be null or empty";
        return EditableHasChoiceCell;
    }(cell.BwfWrapperCell));
    return EditableHasChoiceCell;
});
