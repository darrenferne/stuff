define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'modules/bwf-explorer', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService, exp) {

        function domainObjectsViewModel(data) {
            var self = this;

            self.data = data;
            self.resources = options.resources
            self.formDisabled = ko.observable(false);
            self.canDelete = ko.observable(true);
            self.canClear = ko.observable(true);
            self.canSelect = ko.observable(true);
            self.select = function () {
                var selectedRecords = self.availableGrid.selectedRecords().map(r => r.record);
            }
            self.deleteSelected = function () {
                var selectedRecords = self.selectedGrid.selectedRecords().map(r => r.record);
            }
            self.clear = function () {
                var selectedRecords = self.selectedGrid.selectedRecords().map(r => r.record);
            }
            
            self.generateSelectedGrid = function () {
                var idMetadata = new exp.BasicGridColumnMetadata("integer", "Id");
                idMetadata.isNotEditableInGrid = true;

                var schemaMetadata = new exp.BasicGridColumnMetadata("string", "DbSchema", "Db Schema", "Db Schema");
                schemaMetadata.isNotEditableInGrid = true;

                var dbNameMetadata = new exp.BasicGridColumnMetadata("string", "DbObject", "Db Object", "Db Object");
                dbNameMetadata.isNotEditableInGrid = true;

                var objectNameMetadata = new exp.BasicGridColumnMetadata("string", "Name");
                var displayNameMetadata = new exp.BasicGridColumnMetadata("string", "Display Name", "Display Name");

                var selected = [
                    { Id: 1, DbSchema: "Schema1", DbObject: "DbName1", Name: "Name1", DisplayName: "Display1" },
                    { Id: 2, DbSchema: "Schema2", DbObject: "DbName2", Name: "Name2", DisplayName: "Display2" },
                    { Id: 3, DbSchema: "Schema3", DbObject: "DbName3", Name: "Name3", DisplayName: "Display3" }
                ];

                var columns = [
                        new exp.ExplorerGridColumn(idMetadata, "Id", 1),
                        new exp.ExplorerGridColumn(schemaMetadata, "DbSchema", 2),
                        new exp.ExplorerGridColumn(dbNameMetadata, "DbObject", 3),
                        new exp.ExplorerGridColumn(objectNameMetadata, "Name", 4),
                        new exp.ExplorerGridColumn(displayNameMetadata, "DisplayName", 5)];

                var selectedRecords = exp.generateBasicGridItems(selected, columns);

                return exp.generateBasicGridConfiguration(selectedRecords, columns, "selectedObjects", true);
            }

            self.selectedGrid = self.generateSelectedGrid();
            
            self.generateAvailableGrid = function () {
                var dbNameMetadata = new exp.BasicGridColumnMetadata("string", "DbObject", "Db Object", "Db Object");

                var available = [
                    { DbObject: "DbName1" },
                    { DbObject: "DbName2" },
                    { DbObject: "DbName3" }
                ];

                var columns = [
                    new exp.ExplorerGridColumn(dbNameMetadata, "DbObject", 1)
                ];

                var availableRecords = exp.generateBasicGridItems(available, columns);
                var availableGrid = exp.generateBasicGridConfiguration(availableRecords, columns, "availableObjects", false);

                availableGrid.validate = function (record, success, failure) {

                };

                return availableGrid;
            }

            self.availableGrid = self.generateAvailableGrid();

            self.initialise = function () {
                var model = this;
                
            }
            self.initialise();
        }

        return domainObjectsViewModel;
    }
);