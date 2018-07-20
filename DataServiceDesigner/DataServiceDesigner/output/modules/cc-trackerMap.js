define(['knockout', 'options', 'knockout-kendo']
        , function (ko, options) {

            function trackerMapViewModel() {
                var self = this;

                kendo.culture(options.formattingCulture);

                self.tag = ko.observable("");
                self.longitude = ko.observable(0.0);
                self.latitude = ko.observable(0.0);

                self.options = {
                    enableHighAccuracy: true,
                    timeout: 5000,
                    maximumAge: 0
                };

                this.locationSuccess = function (pos) {
                    self.longitude(pos.coords.longitude);
                    self.latitude(pos.coords.latitude);
                }

                this.locationError = function (err) {
                    self.tag(err.message);
                }

                navigator.geolocation.getCurrentPosition(self.locationSuccess, self.locationError, self.options);

            }

            return trackerMapViewModel;
        });
