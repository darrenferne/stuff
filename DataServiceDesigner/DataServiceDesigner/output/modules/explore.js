define(["require", "exports", "knockout", "modules/bwf-help", "text", "bootstrap", "knockout-kendo", "knockout-postbox"], function (require, exports, ko, help) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var ExploreViewModel = /** @class */ (function () {
        function ExploreViewModel() {
            this.toggleHelpText = help.toggleHelpText;
            this.toggleHelp = help.toggleHelp;
            this.showHelp = help.showHelp;
            this.openHelp = help.openHelp;
        }
        return ExploreViewModel;
    }());
    ko.applyBindings(new ExploreViewModel());
});
