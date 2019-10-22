define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService) {

        function childObjectViewModel(data) {
            var self = this;
            
            self.data = data;
            self.parentId = '#' + data.grid + '-' + data.metadata.name + '-bwf-property-control';

            self.selectedParentChange = ko.observable(null);

            self.setChildObjectQuery = function (parent) {
                if (parent === null || parent === undefined) {
                    parent = 0;
                }
                
                self.data.typeMetadata.properties.Child.populateChoiceQuery = "'dataservicedesigner/query/DomainObjects?$filter=Id!=' + " + parent + " + '&$orderby=ObjectName'";
                self.selectedParentChange(parent);
            };

            self.initialise = function () {

                self.data.model.observables["Parent"].subscribe(function (parent) {
                    self.setChildObjectQuery(parent);
                });

                self.data.model.observables.selectedParentChange = self.selectedParentChange;
                self.data.typeMetadata.properties.Child.refreshChoiceOnChangesTo = "ParentChange";
                self.setChildObjectQuery(0);

                ko.postbox.subscribe(data.grid + '-pop-panel', function () {
                    delete self.data.model.observables.selectedParentChange;
                    self.data.typeMetadata.properties.Child.refreshChoiceOnChangesTo = "";
                });
            };

            self.initialise();
        }

        return childObjectViewModel;
    }
);