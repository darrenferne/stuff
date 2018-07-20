define(["require", "exports", "options", "knockout", "modules/bwf-help", "modules/bwf-title", "text", "bootstrap", "knockout-kendo", "knockout-postbox"], function (require, exports, options, ko, help, title) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var ChangePasswordViewModel = /** @class */ (function () {
        function ChangePasswordViewModel() {
            this.toggleHelpText = help.toggleHelpText;
            this.toggleHelp = help.toggleHelp;
            this.showHelp = help.showHelp;
            this.openHelp = help.openHelp;
            this.pageTitle = options.resources['bwf_change_password'] + ' | ' + options.resources['bwf_explorer'];
            title.setTitle(this.pageTitle);
        }
        return ChangePasswordViewModel;
    }());
    var viewModel = new ChangePasswordViewModel();
    ko.applyBindings(viewModel, document.getElementById("content"));
});
