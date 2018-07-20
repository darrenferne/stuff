define(["require", "exports", "clipboard", "loglevel", "options"], function (require, exports, clipboard, log, options) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    if (!ko.bindingHandlers.clipboardButton) {
        ko.bindingHandlers.clipboardButton = {
            init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                var c = new clipboard(element);
                element.dataset.clipboardText = ko.unwrap(valueAccessor());
                c.on('success', function (e) {
                    log.debug("Copy success");
                    ko.postbox.publish("bwf-transient-notification", options.resources["bwf_copied_to_clipboard"]);
                });
                c.on('error', function (e) {
                    log.warn("Copy was not successful", e);
                });
            },
            update: function (element, valueAccessor) {
                element.dataset.clipboardText = ko.unwrap(valueAccessor());
            }
        };
    }
});
