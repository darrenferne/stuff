define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService) {

        var isNullOrEmpty = function (str) {
            return str == null || str === "";
        }

        function dataServiceConnectionViewModel(data) {
            var self = this;
            
            self.data = data;
            self.parentModel = data.model;
            self.formDisabled = data.model.observables.formDisabled;
            self.connectionString = data.model.observables["ConnectionString"];
            
            self.enableEdit = ko.observable(true);
            self.edit = function () {
                var dbConnection = self.parentModel.record.DbConnection; 
                var onCompletion = function (params) {
                    self.parentModel.record.DbConnection = params.record;
                    self.parentModel.record.ConnectionString = params.record.ConnectionString;
                }

                var actionArgs = {
                    action: dbConnection == null ? 'create' : 'edit',
                    baseType: 'DbConnection',
                    component: 'bwf-panel-editor',
                    onCompletion: onCompletion,
                    parentIsSource: true,
                    dataService: 'schemabrowser',
                    toEdit: [ dbConnection ]
                }

                ko.postbox.publish(self.parentModel.state.gridId + '-doAction', actionArgs)
            };

            self.initialise = function (model, record) {
                model.connectionString(record.ConnectionString);
            }
            self.initialise(self, data.model.record);
        }

        return dataServiceConnectionViewModel;
    }
);