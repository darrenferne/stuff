define(["require", "exports", "knockout", "modules/bwf-idgen", "bootstrap", "knockout-postbox"], function (require, exports, ko, idgen) {
    "use strict";
    var TransientNotificationHandler = /** @class */ (function () {
        function TransientNotificationHandler() {
            var _this = this;
            this.transientNotification = {
                id: 0,
                title: ko.observable(''),
                message: ko.observable(''),
                isVisible: ko.observable(false),
                styleClass: ko.observable('alert-success'),
                requireDismissal: ko.observable(false)
            };
            this.dismiss = function () { return _this.transientNotification.isVisible(false); };
            this.create = function (notification) {
                var title = '';
                var styleClass = 'alert-success';
                var requireDismissal = false;
                var message = '';
                if (typeof notification === 'string') {
                    message = notification;
                }
                else {
                    title = notification.title || '';
                    styleClass = notification.styleClass || 'alert-success';
                    requireDismissal = notification.requireDismissal || false;
                    message = notification.message || '';
                }
                var nextId = idgen.nextId();
                _this.transientNotification.id = nextId;
                _this.transientNotification.title(title);
                _this.transientNotification.message(message);
                _this.transientNotification.styleClass(styleClass);
                _this.transientNotification.requireDismissal(requireDismissal);
                _this.transientNotification.isVisible(true);
                if (!requireDismissal) {
                    setTimeout(function (id) {
                        if (_this.transientNotification.id === nextId)
                            _this.dismiss();
                    }, 3000);
                }
            };
            ko.postbox.subscribe('bwf-transient-notification', this.create);
        }
        return TransientNotificationHandler;
    }());
    var handler = new TransientNotificationHandler();
    return handler;
});
