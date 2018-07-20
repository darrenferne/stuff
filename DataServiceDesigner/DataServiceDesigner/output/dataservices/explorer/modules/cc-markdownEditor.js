define(["require", "exports", "knockout", "scripts/sprintf", "modules/bwf-markdownEditorBinding"], function (require, exports, knockout, sprintfM, editor) {
    "use strict";
    var markdown = editor;
    var sprintf = sprintfM.sprintf;
    var MarkdownEditor = /** @class */ (function () {
        function MarkdownEditor(params, data) {
            this.rendered = knockout.observable(false);
            this.formDisabled = params.model.observables['formDisabled'];
            params.model.observables['__renderedState'].push(this.rendered);
            this.params = params;
            this.editingProperty = params.model.observables[params.metadata.name];
            this.label = params.metadata.displayName;
            if (!this.editingProperty())
                this.editingProperty("");
        }
        MarkdownEditor.prototype.afterRender = function () {
            this.rendered(true);
        };
        return MarkdownEditor;
    }());
    return MarkdownEditor;
});
