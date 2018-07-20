define(["require", "exports", "knockout"], function (require, exports, knockout) {
    "use strict";
    var TemplateEditor = /** @class */ (function () {
        function TemplateEditor(params) {
            var _this = this;
            this.rendered = knockout.observable(false);
            this.id = window.performance.now();
            this.value = params.model.observables[params.metadata.name];
            this.label = params.metadata.editingName;
            this.formDisabled = params.model.observables['formDisabled'];
            params.model.observables['__renderedState'].push(this.rendered);
            this.editorConfig = function () { return params; };
            this.type = params.model.observables[params.metadata.customControlParameter];
            this.editor = ko.pureComputed(function () {
                var value = ko.unwrap(_this.type);
                if (value == null)
                    return '';
                if (typeof value === 'string')
                    return value;
                else
                    return value.Value;
            });
            this.editorControl = ko.pureComputed(function () {
                var editor = _this.editor();
                switch (editor) {
                    case 'Markdown':
                        return 'ds-explorer-cc-markdownEditor';
                    default:
                        return 'bwf-empty';
                }
            });
        }
        return TemplateEditor;
    }());
    return TemplateEditor;
});
