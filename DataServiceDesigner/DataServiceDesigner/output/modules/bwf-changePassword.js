define(["require", "exports", "options", "knockout", "modules/bwf-help", "loglevel"], function (require, exports, options, ko, help, log) {
    "use strict";
    var libs = require(['knockout-amd-helpers', 'knockout-kendo', 'knockout-postbox']);
    var ChangePasswordViewModel = /** @class */ (function () {
        function ChangePasswordViewModel() {
            var _this = this;
            this.title = options.resources['bwf_change_password'];
            this.currentPasswordLabelText = options.resources['bwf_current_password'];
            this.newPasswordLabelText = options.resources['bwf_new_password'];
            this.confirmNewPasswordLabelText = options.resources['bwf_confirm_new_password'];
            this.currentPassword = ko.observable('');
            this.newPassword = ko.observable('');
            this.confirmNewPassword = ko.observable('');
            this.changeFailedMessage = ko.observable('');
            this.changeButtonText = options.resources['bwf_apply'];
            this.toggleHelpText = help.toggleHelpText;
            this.toggleHelp = help.toggleHelp;
            this.showHelp = help.showHelp;
            this.openHelp = help.openHelp;
            this.addSuccessNotification = function (message) { return ko.postbox.publish("bwf-transient-notification", message); };
            this.doNotMatchMessage = ko.pureComputed(function () {
                return _this.newPassword() !== _this.confirmNewPassword()
                    ? options.resources['bwf_passwords_do_not_match']
                    : '';
            });
            this.canChangePassword = ko.pureComputed(function () {
                var canChange = _this.doNotMatchMessage() === '' &&
                    _this.currentPassword() !== undefined && _this.currentPassword() !== null && _this.currentPassword() !== '' &&
                    _this.newPassword() !== undefined && _this.newPassword() !== null && _this.newPassword() !== '' &&
                    _this.confirmNewPassword() !== undefined && _this.confirmNewPassword() !== null && _this.confirmNewPassword() !== '';
                return canChange;
            });
        }
        ChangePasswordViewModel.prototype.changePassword = function () {
            var _this = this;
            log.debug('Changing user password');
            this.changeFailedMessage('');
            var jqxhr = $.ajax({
                url: '',
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({ Current: this.currentPassword(), New: this.newPassword() })
            }).done(function (response) {
                log.debug('Password successfully changed');
                _this.currentPassword('');
                _this.newPassword('');
                _this.confirmNewPassword('');
                _this.addSuccessNotification(options.resources['bwf_password_successfully_changed']);
            }).fail(function (message) {
                log.warn('Password change failed:', message);
                _this.currentPassword('');
                _this.newPassword('');
                _this.confirmNewPassword('');
                _this.changeFailedMessage(message.responseText);
            });
        };
        ;
        return ChangePasswordViewModel;
    }());
    return ChangePasswordViewModel;
});
