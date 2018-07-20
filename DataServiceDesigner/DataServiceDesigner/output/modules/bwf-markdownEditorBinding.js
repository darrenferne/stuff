define(["require", "exports", "knockout", "cm/lib/codemirror"], function (require, exports, ko, CodeMirror) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var plugins = require(['cm/mode/markdown/markdown', 'cm/addon/edit/matchbrackets', 'cm/addon/edit/closebrackets']);
    if (!ko.bindingHandlers.markdownEditor) {
        ko.bindingHandlers.markdownEditor = {
            init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                var options = viewModel.options || {};
                options.value = ko.unwrap(valueAccessor());
                if (!options.value)
                    options.value = "";
                options.mode = 'markdown';
                options.lineNumbers = true;
                // These options are from the plug ins
                options.matchBrackets = true;
                options.autoCloseBrackets = true;
                element.editor = CodeMirror.fromTextArea(element, ko.toJS(options));
                element.editor.on('change', function (cm) {
                    var value = valueAccessor();
                    value(cm.getValue());
                });
                var disabled = allBindings.get('disabled');
                element.editor.setOption('readOnly', ko.unwrap(disabled));
                disabled.subscribe(function (v) { return element.editor.setOption('readonly', v); });
                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                    var wrapper = element.editor.getWrapperElement();
                    wrapper.parentNode.removeChild(wrapper);
                });
            },
            update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
                var value = ko.unwrap(valueAccessor());
                if (element.editor) {
                    var cur = element.editor.getCursor();
                    element.editor.setValue(value ? value : "");
                    element.editor.setCursor(cur);
                    element.editor.refresh();
                }
            }
        };
    }
});
