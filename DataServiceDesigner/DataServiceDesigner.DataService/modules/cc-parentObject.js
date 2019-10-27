define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService) {

        function parentObjectViewModel(data) {
            var self = this;
            
            self.data = data;
            self.parentId = '#' + data.grid + '-' + data.metadata.name + '-bwf-property-control';

            self.selectedChildChange = ko.observable(null);

            self.setParentObjectQuery = function (child) {
                if (child === null || child === undefined) {
                    child = 0;
                }

                self.data.typeMetadata.properties.Parent.populateChoiceQuery = "'dataservicedesigner/query/DomainObjects?$filter=Id!=' + " + child + " + '&$orderby=ObjectName'";
                self.selectedChildChange(child);
            };

            self.initialise = function() {

                self.data.model.observables["Child"].subscribe(function (child) {
                    self.setParentObjectQuery(child);
                });

                self.data.model.observables.selectedChildChange = self.selectedChildChange;
                self.data.typeMetadata.properties.Parent.refreshChoiceOnChangesTo = "ChildChange";
                self.setParentObjectQuery(0);

                ko.postbox.subscribe(data.grid + '-clear-panel', function () {
                    delete self.data.model.observables.selectedChildChange;
                    self.data.typeMetadata.properties.Parent.refreshChoiceOnChangesTo = "";
                });
            };

            self.initialise();
        }

        return parentObjectViewModel;
    }
);