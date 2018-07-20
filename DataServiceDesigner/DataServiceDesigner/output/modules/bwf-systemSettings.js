define(["require", "exports", "options", "knockout", "modules/bwf-help", "loglevel", "knockout-kendo", "knockout-postbox"], function (require, exports, options, ko, help, log) {
    "use strict";
    var SystemSettingsViewModel = /** @class */ (function () {
        function SystemSettingsViewModel() {
            var _this = this;
            this.r = options.resources;
            this.sessionTimeout = ko.observable(options.sessionTimeout);
            this.sessionTimeoutValidation = ko.observable('');
            this.sessionTimeoutEnabled = ko.observable(options.sessionTimeoutEnabled);
            this.onlyAllowDefaultViewsForAdministrators = ko.observable(options.onlyAllowDefaultViewsForAdministrators);
            this.passwordResetTimeout = ko.observable(options.passwordResetTimeout);
            // timezones
            this.availableTimezones = ko.observableArray(options.timezones);
            this.timezoneValidation = ko.observable('');
            this.timezone = ko.observable(options.timezone);
            // smtp
            this.smtpFromAddress = ko.observable(options.smtpFromAddress);
            this.smtpHostUri = ko.observable(options.smtpHostUri);
            this.smtpUseSSL = ko.observable(options.smtpUseSSL);
            this.smtpAuthenticationRequired = ko.observable(options.smtpAuthenticationRequired);
            this.smtpHostUsername = ko.observable(options.smtpHostUsername);
            this.smtpHostPassword = ko.observable(options.smtpHostPassword);
            this.smtpHostPort = ko.observable(options.smtpHostPort);
            // sms
            this.smsProviders = options.smsProviders.split(";");
            this.smsSelectedProvider = ko.observable(options.smsProvider);
            this.smsUsername = ko.observable(options.smsUsername);
            this.smsPassword = ko.observable(options.smsPassword);
            // help
            this.toggleHelpText = help.toggleHelpText;
            this.toggleHelp = help.toggleHelp;
            this.showHelp = help.showHelp;
            this.openHelp = help.openHelp;
            this.addSuccessNotification = function (message) {
                ko.postbox.publish("bwf-transient-notification", message);
            };
            this.canSave = ko.computed(function () {
                var sessionTimeout = _this.sessionTimeout();
                var canSave = sessionTimeout !== undefined
                    && sessionTimeout !== null
                    && sessionTimeout > 0;
                return canSave;
            });
            this.save = function () {
                log.debug('Saving system settings, session timeout:', _this.sessionTimeout());
                log.debug('Saving system settings, session timeout enabled:', _this.sessionTimeoutEnabled());
                var jqxhr = $.ajax({
                    url: '',
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        SessionTimeout: _this.sessionTimeout(),
                        SessionTimeoutEnabled: _this.sessionTimeoutEnabled(),
                        PasswordResetTimeout: _this.passwordResetTimeout(),
                        OnlyAllowDefaultViewsForAdministrators: _this.onlyAllowDefaultViewsForAdministrators(),
                        Timezone: _this.timezone(),
                        SmtpSettings: {
                            Uri: _this.smtpHostUri(),
                            Username: _this.smtpHostUsername(),
                            Password: _this.smtpHostPassword(),
                            Port: _this.smtpHostPort(),
                            AuthenticationRequired: _this.smtpAuthenticationRequired(),
                            UseSSL: _this.smtpUseSSL(),
                            FromEmail: _this.smtpFromAddress()
                        },
                        SmsSettings: {
                            Provider: _this.smsSelectedProvider(),
                            Username: _this.smsUsername(),
                            Password: _this.smsPassword()
                        }
                    })
                }).done(function (response) {
                    log.debug('System settings successfully saved');
                    _this.addSuccessNotification("System settings successfully saved.");
                }).fail(function (message) {
                    log.error('Error saving system settings:', message);
                    alert('Error saving system settings, see console logging messages for further information.');
                });
            };
        }
        return SystemSettingsViewModel;
    }());
    return SystemSettingsViewModel;
});
