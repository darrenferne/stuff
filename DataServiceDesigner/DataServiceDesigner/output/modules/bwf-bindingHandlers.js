define(["require", "exports", "knockout"], function (require, exports, ko) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    if (!ko.bindingHandlers.anchorClick) {
        // for <= IE10 - we need to check to see if anchor button is disabled before we perform the click action
        ko.bindingHandlers.anchorClick = {
            init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                var originalFunction = valueAccessor();
                var newValueAccessor = function () {
                    return function (viewModel, event) {
                        if (!$(element).hasClass('disabled') && event.which === 1)
                            originalFunction.apply(viewModel, arguments);
                        return true; // allows normal click action, e.g. middle click for new tab
                    };
                };
                ko.bindingHandlers.click.init(element, newValueAccessor, allBindingsAccessor, viewModel, bindingContext);
            }
        };
    }
    if (!ko.bindingHandlers.readonly) {
        // there is no readonly binding in ko 
        // using attr: { readonly: <code> } works in Chrome
        // however it is badly formed i.e. it does <input readonly="true">
        // where it should be just <input readonly> 
        // (or <input readonly="readonly"> for XHTML)
        // main issue is <input readonly="false"> will still make it readonly
        // this hander removes it as it should
        // https://github.com/knockout/knockout/issues/1100
        ko.bindingHandlers.readonly = {
            update: function (element, valueAccessor) {
                var value = !!ko.utils.unwrapObservable(valueAccessor());
                if (value !== element.readOnly)
                    element.readOnly = value;
            }
        };
    }
    if (!ko.bindingHandlers.highlightUpdates) {
        ko.bindingHandlers.highlightUpdates = {
            'update': function (element, valueAccessor, allBindings, cell, context) {
                // access the value so that we subscribe to it and this function gets
                // called when it changes.s
                var value = ko.unwrap(valueAccessor());
                if (!context || !context.record || context.record.updateType() === 'None')
                    return;
                if (!cell.isChangeTrackingDisabled && cell.isDirty()) {
                    cell.doHighlight();
                }
            }
        };
    }
    if (!ko.bindingHandlers.draggable) {
        ko.bindingHandlers.draggable = {
            update: function (element, valueAccessor) {
                var value = !!ko.utils.unwrapObservable(valueAccessor());
                if (value !== element.draggable)
                    element.draggable = value;
            }
        };
    }
});
