define(["require", "exports", "modules/notifications/bwf-notificationService", "knockout"], function (require, exports, service, ko) {
    "use strict";
    var NotificationBadge = /** @class */ (function () {
        function NotificationBadge() {
            var _this = this;
            this.Count = service.newNotifications;
            this.Visible = ko.pureComputed(function () { return _this.Count() > 0; });
        }
        return NotificationBadge;
    }());
    return NotificationBadge;
});
