define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService) {

        function initialCatalogViewModel(data) {
            var self = this;

            self.data = data;
            self.parentId = '#' + data.grid + '-' + data.metadata.name + '-bwf-property-control';
            self.useIntegratedSecurity = data.model.observables["UseIntegratedSecurity"];
            self.showOrHide = function (show) {
                var parent = $(self.parentId);
                if (show)
                    parent.show();
                else
                    parent.hide();
            };

            self.initialise = function () {
                var model = this;
                self.useIntegratedSecurity.subscribe(function (useIntegratedSecurity) {
                    model.showOrHide(!useIntegratedSecurity);
                })
                model.showOrHide(!self.useIntegratedSecurity());
            };
            self.initialise();
        }

        return initialCatalogViewModel;
    }
);