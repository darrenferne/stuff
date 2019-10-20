define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService) {

        function childObjectViewModel(data) {
            var self = this;
            
            self.data = data;
            self.parentId = '#' + data.grid + '-' + data.metadata.name + '-bwf-property-control';

            self.selectedChild = self.data.model.observables["Child"];
            self.selectedChildChange = ko.observable(null)

            self.setParentObjectQuery = function(child) {
                self.data.typeMetadata.properties.Parent.populateChoiceQuery = "'dataservicedesigner/query/DomainObjects?$filter=Id!=' + " + child + " + '&$orderby=ObjectName'"
                self.data.model.observables.selectedChildChange(child);
            };

            self.initialise = function(x) {

                self.selectedChild.subscribe(function(child) {
                    self.setParentObjectQuery(child);
                });

                self.data.model.observables.selectedChildChange = self.selectedChildChange;
                self.data.typeMetadata.properties.Parent.refreshChoiceOnChangesTo = "ChildChange";
            };

            self.initialise();
        }

        return childObjectViewModel;
    }
);