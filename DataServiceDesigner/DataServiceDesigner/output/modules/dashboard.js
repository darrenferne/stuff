define(["require", "exports", "knockout", "sprintf", "options", "modules/bwf-title"], function (require, exports, ko, sprintfM, options, title) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var sprintf = sprintfM.sprintf;
    var DashboardPage = /** @class */ (function () {
        function DashboardPage() {
            var _this = this;
            this.r = options.resources;
            this.dashboardName = ko.observable("Dashboard");
            this.pageTitle = ko.pureComputed(function () { return _this.dashboardName() + " | " + _this.r["bwf_explorer"]; });
            title.setTitle(this.pageTitle);
        }
        DashboardPage.prototype.dispose = function () {
            title.clearTitle();
        };
        ;
        DashboardPage.prototype.dashboardConfiguration = function () {
            return {
                elementId: "dashboardPage",
                dashboardName: this.dashboardName
            };
        };
        return DashboardPage;
    }());
    var viewModel = new DashboardPage();
    ko.applyBindings(viewModel, document.getElementById("content"));
});
