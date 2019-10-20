define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService) {

        function parentObjectViewModel(data) {
            var self = this;
            
            self.data = data;
            self.parentId = '#' + data.grid + '-' + data.metadata.name + '-bwf-property-control';

            self.selectedParent = self.data.model.observables["Parent"];
            self.selectedParentChange = ko.observable(null);

            self.setChildObjectQuery = function(parent) {
                self.data.typeMetadata.properties.Child.populateChoiceQuery = "'dataservicedesigner/query/DomainObjects?$filter=Id!=' + " + parent + " + '&$orderby=ObjectName'"
                self.data.model.observables.selectedParentChange(parent);
            };

            self.initialise = function(x) {

                self.selectedParent.subscribe(function(parent) {
                    self.setChildObjectQuery(parent);
                });

                self.data.model.observables.selectedParentChange = self.selectedParentChange;
                self.data.typeMetadata.properties.Child.refreshChoiceOnChangesTo = "ParentChange";
            };

            self.initialise();
        }

        return parentObjectViewModel;
    }
);