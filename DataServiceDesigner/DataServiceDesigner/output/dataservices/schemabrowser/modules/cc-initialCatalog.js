define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService) {

        function initialCatalogViewModel(data) {
            var self = this;

            self.data = data;
            self.parentId = '#' + data.grid + '-' + data.metadata.name + '-bwf-property-control';
            self.databaseType = data.model.observables["DatabaseType"];
            self.showOrHide = function (show) {
                var parent = $(self.parentId);
                if (show)
                    parent.show();
                else
                    parent.hide();
            }

            self.initialise = function () {
                var model = this;
                self.databaseType.subscribe(function (databaseType) {
                    model.showOrHide(databaseType == 'SQLServer');
                })
                model.showOrHide(self.databaseType() == 'SQLServer');
            }
            self.initialise();
        }

        return initialCatalogViewModel;
    }
);