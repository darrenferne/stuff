define(["require", "exports"], function (require, exports) {
    "use strict";
    var MultiSelectControl = /** @class */ (function () {
        function MultiSelectControl(params) {
            var _this = this;
            this.selectedValues = ko.observableArray([])
                .extend({ rateLimit: { timeout: 50, method: 'notifyWhenChangesStop' }, notify: 'always' });
            this.newItemText = '';
            this.newItemValue = '';
            this.previousValues = ko.observableArray([]);
            this.values = ko.observableArray([]);
            this.control = ko.observable();
            this.setupEventHandler = function () {
                var control = _this.control();
                _this.hideCloseButton(_this.formDisabled());
                if (control != null) {
                    _this.formDisabled.subscribe(function (value) {
                        _this.hideCloseButton(value);
                    });
                    _this.selectedValues.subscribe(function (selected) {
                        _this.values().forEach(function (v) {
                            if (selected.some(function (s) { return s === v.value; }))
                                v.used = true;
                        });
                        _this.property(selected.map(function (s) { return ({ SearchCriteria: s }); }));
                    });
                    var self = _this;
                    // `this` will be the kendo control inside this event handler
                    control.bind('dataBound', function () {
                        var text = this._prev;
                        if (!text || text.trim().length === 0 || text === this.newItemText)
                            return;
                        this.newItemText = text;
                        this.newItemValue = text;
                        self.values.remove(function (i) { return !i.used; });
                        var option = { text: text, value: text, used: false };
                        self.values.push(option);
                        this.refresh();
                        this.search(option.text);
                        this.open();
                    });
                }
            };
            this.elementId = params.metadata.name;
            this.name = params.metadata.name;
            this.propertyMetadata = params.metadata;
            this.label = params.metadata.editingName;
            this.property = params.model.observables[this.propertyMetadata.name];
            this.values(this.property().map(function (p) { return ({ text: p.SearchCriteria, value: p.SearchCriteria, used: false }); }));
            this.selectedValues(this.property().map(function (p) { return p.SearchCriteria; }));
            this.formDisabled = ko.pureComputed(function () {
                return params.model.observables['formDisabled']()
                    || (params.model.state.isCreate && params.metadata.isDisabledInCreateMode)
                    || (!params.model.state.isCreate && params.metadata.isDisabledInEditMode);
            });
            this.postRender = function () {
                _this.setupEventHandler();
            };
        }
        MultiSelectControl.prototype.focus = function () {
            if (this.formDisabled())
                return true;
            var control = this.control();
            if (control) {
                control.focus();
                control.open();
            }
        };
        MultiSelectControl.prototype.hideCloseButton = function (hide) {
            if (hide)
                this.control().element.parent().find(".k-i-close").hide();
            else
                this.control().element.parent().find(".k-i-close").show();
        };
        return MultiSelectControl;
    }());
    return MultiSelectControl;
});
