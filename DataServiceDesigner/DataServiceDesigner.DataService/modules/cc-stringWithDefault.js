define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService) {

        function stringWithDefaultModel(data) {
            var self = this;

            self.data = data;
            self.parentId = '#' + data.grid + '-' + data.metadata.name + '-bwf-property-control';

            var parameter = JSON.parse(data.metadata.customControlParameter);
            self.property = data.model.observables[data.metadata.name];
            self.default = parameter.default;

            self.initialise = function () {
                if (self.property() == "") {
                    self.property(self.default);
                }
            };
            self.initialise();
        }

        return stringWithDefaultModel;
    }
);