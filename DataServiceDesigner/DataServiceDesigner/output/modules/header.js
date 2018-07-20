define(["require", "exports", "knockout", "modules/bwf-utilities", "options"], function (require, exports, ko, utils, options) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var ReponsiveHeader = /** @class */ (function () {
        function ReponsiveHeader() {
            var _this = this;
            this.username = options.username;
            var self = this;
            this.imageSource = "/usersettings/image/" + this.username;
            this.inTouchMode = utils.isTouchModeEnabled;
            this.shouldCancelHide = ko.observable(false);
            this.sub = this.inTouchMode.subscribe(function (x) { return _this.shouldCancelHide(true); });
            $('#userMenu').on('hide.bs.dropdown', function (e) {
                if (self.shouldCancelHide()) {
                    e.preventDefault();
                    self.shouldCancelHide(false);
                }
            });
        }
        ReponsiveHeader.prototype.dispose = function () {
            if (this.sub)
                this.sub.dispose();
        };
        return ReponsiveHeader;
    }());
    var viewModel = new ReponsiveHeader();
    ko.applyBindings(viewModel, document.querySelector('nav.navbar.navbar-static-top'));
});
