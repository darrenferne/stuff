define(["require", "exports", "options", "knockout", "modules/bwf-help", "modules/bwf-title", "text", "bootstrap", "knockout-kendo", "knockout-postbox"], function (require, exports, options, ko, help, title) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var UserSettingsViewModel = /** @class */ (function () {
        function UserSettingsViewModel() {
            this.toggleHelpText = help.toggleHelpText;
            this.toggleHelp = help.toggleHelp;
            this.showHelp = help.showHelp;
            this.openHelp = help.openHelp;
            this.pageTitle = options.resources['bwf_user_settings'] + ' | ' + options.resources['bwf_explorer'];
            title.setTitle(this.pageTitle);
        }
        return UserSettingsViewModel;
    }());
    var viewModel = new UserSettingsViewModel();
    ko.applyBindings(viewModel, document.getElementById("content"));
});
