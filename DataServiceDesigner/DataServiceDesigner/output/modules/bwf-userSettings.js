define(["require", "exports", "options", "knockout", "modules/bwf-globalisation", "knockout-kendo", "knockout-postbox"], function (require, exports, options, ko, globalisation) {
    "use strict";
    var UserSettingsViewModel = /** @class */ (function () {
        function UserSettingsViewModel() {
            var _this = this;
            this.options = options;
            this.r = options.resources;
            this.title = options.resources['bwf_user_settings'];
            this.username = options.username;
            this.userPictureUrl = ko.observable("/usersettings/image/" + options.username);
            this.avatarHasChanged = ko.observable(false);
            this.userImageType = ko.observable(options.imageType);
            this.fileUploadIsVisible = ko.pureComputed(function () { return _this.userImageType() == 'image'; });
            this.userImageVisible = ko.pureComputed(function () { return _this.fileUploadIsVisible() && _this.avatarHasChanged(); });
            this.availableLanguageCultures = ko.observable(options.languageCultures);
            this.languageCulture = ko.observable(options.languageCulture);
            this.availableFormattingCultures = ko.observableArray(options.formattingCultures);
            this.formattingCulture = ko.observable(options.formattingCulture);
            this.dateTimeDisplayFormat = ko.observable(options.dateTimeDisplayFormat);
            this.dateDisplayFormat = ko.observable(options.dateDisplayFormat);
            this.availableTimezones = ko.observableArray(options.timezones);
            this.timezone = ko.observable(options.timezone);
            this.fileUpload = function (data, e) {
                var input = e.target;
                var file = input.files[0];
                if (file == null)
                    return;
                var reader = new FileReader();
                reader.onload = function () {
                    _this.userPictureUrl(reader.result);
                    _this.avatarHasChanged(true);
                };
                reader.readAsDataURL(file);
            };
            this.canSave = ko.pureComputed(function () {
                if (_this.saveInProgress())
                    return false;
                if (!_this.dateDisplayFormat.isValid() || !_this.dateTimeDisplayFormat.isValid())
                    return false;
                return true;
            });
            this.saveInProgress = ko.observable(false);
            this.saveSettings = function () {
                if (_this.saveInProgress())
                    return;
                if (!_this.dateDisplayFormat.isValid() || !_this.dateTimeDisplayFormat.isValid())
                    return;
                _this.saveInProgress(true);
                var userSettings = {
                    LanguageCulture: _this.languageCulture(),
                    FormattingCulture: _this.formattingCulture(),
                    DateDisplayFormat: _this.dateDisplayFormat(),
                    DateTimeDisplayFormat: _this.dateTimeDisplayFormat(),
                    UserImageType: _this.userImageType(),
                    UserImageData: _this.userPictureUrl(),
                    Timezone: _this.timezone() == '' ? null : _this.timezone()
                };
                var request = $.ajax({
                    url: '',
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify(userSettings)
                });
                request.done(function () { return ko.postbox.publish('bwf-transient-notification', _this.r['bwf_user_settings_saved']); });
                request.fail(function () { return alert(_this.r['bwf_user_settings_save_error']); });
                request.always(function () { return _this.saveInProgress(false); });
            };
            this.dateTimeDisplayFormat.isValid = ko.computed(function () {
                var valid = globalisation.checkDateTimeFormatInFormatCulture(_this.dateTimeDisplayFormat(), _this.formattingCulture(), options.formattingCultures);
                if (!valid)
                    _this.dateTimeDisplayFormat('');
                return valid;
            });
            this.dateDisplayFormat.isValid = ko.computed(function () {
                var valid = globalisation.checkDateFormatInFormatCulture(_this.dateDisplayFormat(), _this.formattingCulture(), options.formattingCultures);
                if (!valid)
                    _this.dateDisplayFormat('');
                return valid;
            });
            this.availableDateFormats = ko.pureComputed(function () {
                var culture = _this.formattingCulture();
                if (culture == '' || culture == null)
                    return [];
                return globalisation.getCultureDateFormats(culture, options.formattingCultures);
            });
            this.availableDateTimeFormats = ko.pureComputed(function () {
                var culture = _this.formattingCulture();
                if (culture == '' || culture == null)
                    return [];
                return globalisation.getCultureDateTimeFormats(culture, options.formattingCultures);
            });
        }
        return UserSettingsViewModel;
    }());
    return UserSettingsViewModel;
});
