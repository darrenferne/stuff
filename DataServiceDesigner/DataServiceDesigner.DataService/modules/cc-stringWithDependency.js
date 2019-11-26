define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService) {

        function stringWithDependencyModel(data) {
            var self = this;

            self.data = data;
            self.parentId = '#' + data.grid + '-' + data.metadata.name + '-bwf-property-control';

            self.isNullOrEmpty = function (value) {
                return value == null || value === "";
            }

            var parameter = JSON.parse(data.metadata.customControlParameter);
            self.property = data.model.observables[data.metadata.name];
            self.propertyChanged = ko.observable(!self.isNullOrEmpty(self.property()));
            self.dependencyChanged = ko.observable(false);
            self.dependency = data.model.observables[parameter.dependency];
            self.action = new Function(parameter.action).bind({
                property: self.property,
                dependency: self.dependency
            });

            self.initialise = function () {
                self.property.subscribe(function () {
                    if (!self.dependencyChanged()) {
                        self.propertyChanged(!self.isNullOrEmpty(self.property()));
                    }
                    self.dependencyChanged(false);
                });
                self.dependency.subscribe(function () {
                    if (!self.propertyChanged() && self.action != null) {
                        self.dependencyChanged(true);
                        self.action();
                    }
                });
            };

            self.initialise();
        }

        return stringWithDependencyModel;
    }
);