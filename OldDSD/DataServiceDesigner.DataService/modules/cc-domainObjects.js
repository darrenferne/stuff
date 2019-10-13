define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'modules/bwf-explorer', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService, exp) {


        function domainObjectsViewModel(data) {
            var self = this;

            self.data = data;
            self.parentModel = data.model;
            self.resources = options.resources;
            self.formDisabled = ko.observable(false);
            self.canDelete = ko.pureComputed(function () {
                if (self.selectedRecordGrid() === null || self.selectedRecordGrid().selectedRecords() === null)
                    return false;
                return self.selectedRecordGrid().selectedRecords().length > 0;
            });
            self.canClear = ko.pureComputed(function () {
                if (self.selectedRecordGrid() === null || self.selectedRecordGrid().selectedRecords() === null)
                    return false;
                return self.selectedRecordGrid().records().length > 0;
            });
            self.canSelect = ko.pureComputed(function () {
                if (self.availableRecordGrid() === null || self.selectedRecordGrid().selectedRecords() === null)
                    return false;
                return self.availableRecordGrid().selectedRecords().length > 0;
            });
            self.connectionName = ko.observable("");
            self.availableRecordGrid = ko.observable(null);
            self.availableObjects = ko.observableArray([]);
            self.selectedRecordGrid = ko.observable(null);
            self.selectedObjects = data.model.observables.DomainObjects;

            self.select = function () {
                var selectedRecords = self.availableRecordGrid().selectedRecords();
                for (var i = 0; i < selectedRecords.length; i++) {
                    var selectedRecord = selectedRecords[i].record;
                    var domainObject = {
                        Id: 0,
                        DataService: { Id: self.data.model.record.Id },
                        Schema: {
                            Id: 0,
                            Name: selectedRecord.SchemaName
                        },
                        DbName: selectedRecord.Name,
                        Name: selectedRecord.Name,
                        DisplayName: selectedRecord.Name
                    };
                    var gridItem = exp.generateBasicGridItem(domainObject, self.selectedRecordGrid().records().length + 1, self.selectedRecordGridColumns);
                    self.selectedRecordGrid().records.push(gridItem);
                    self.selectedObjects().push(domainObject);
                }
                self.availableRecordGrid().records.removeAll(selectedRecords);
            };
            self.deleteSelected = function () {
                var selectedRecords = self.selectedRecordGrid().selectedRecords();
                var selectedObjects = self.selectedRecordGrid().selectedRecords().map(function (r) { return r.record; });
                self.selectedRecordGrid().records.removeAll(selectedRecords);
                self.selectedObjects.removeAll(selectedObjects);
                self.loadAvailableObjects(self.connectionName(), true);
            };
            self.clear = function () {
                self.selectedRecordGrid().records.removeAll();
                self.selectedObjects.removeAll();
                self.loadAvailableObjects(self.connectionName(), true);
            };
        
            metadataService.getType("dataservicedesigner", "DomainObject").done(metadata => {

                var idMetadata = metadata.properties["Id"];
                idMetadata.isNotEditableInGrid = true;
                var schemaMetadata = metadata.properties["Schema"];
                schemaMetadata.isNotEditableInGrid = true;
                var dbNameMetadata = metadata.properties["DbName"];
                var nameMetadata = metadata.properties["Name"];
                var displayNameMetadata = metadata.properties["DisplayName"];

                self.selectedRecordGridColumns = [
                    new exp.ExplorerGridColumn(idMetadata, "Id", 1),
                    new exp.ExplorerGridColumn(schemaMetadata, "Schema/Name", 2),
                    new exp.ExplorerGridColumn(dbNameMetadata, "DbName", 3),
                    new exp.ExplorerGridColumn(nameMetadata, "Name", 4),
                    new exp.ExplorerGridColumn(displayNameMetadata, "DisplayName", 5)
                ];  
                
                var selectedItems = exp.generateBasicGridItems(self.selectedObjects(), self.selectedRecordGridColumns);
                var grid = exp.generateBasicGridConfiguration(selectedItems, self.selectedRecordGridColumns, "selectedObjects", true);

                grid.validate = function (record, success, failure) {
                    self.selectedRecordGrid().updateDirtyRecordWithLatestValues(record, self.selectedRecordGridColumns);
                };

                self.selectedRecordGrid(grid);
            });

            metadataService.getType("schemabrowser", "DbObject").done(metadata => {

                var schemaMetadata = metadata.properties["SchemaName"];
                var dbNameMetadata = metadata.properties["Name"];
                
                self.availableRecordGridColumns = [
                    new exp.ExplorerGridColumn(schemaMetadata, "SchemaName", 1),
                    new exp.ExplorerGridColumn(dbNameMetadata, "Name", 2)
                ];

                var availableItems = exp.generateBasicGridItems(self.availableObjects(), self.availableRecordGridColumns);
                var grid = exp.generateBasicGridConfiguration(availableItems, self.availableRecordGridColumns, "availableObjects", false);
                
                self.availableRecordGrid(grid);
            });

            //self.generateselectedRecordGridColumns = function () {
            //   metadataService.getType("dataservicedesigner", "DomainObject").done(metadata => {
            //    self.generateselectedRecordGridColumns = [
            //        new exp.ExplorerGridColumn(metadata.properties["Id"], "Id", 1),
            //        new exp.ExplorerGridColumn(metadata.properties["Schema"], "Schema", 2),
            //        new exp.ExplorerGridColumn(metadata.properties["DbName"], "DbName", 3),
            //        new exp.ExplorerGridColumn(metadata.properties["Name"], "Name", 4),
            //        new exp.ExplorerGridColumn(metadata.properties["DisplayName"], "DisplayName", 5)
            //    ];   
            //});
            //};

            //self.selectedRecordGridColumns = self.generateselectedRecordGridColumns();

            //self.generateselectedRecordGrid = function () {

            //    var selectedRecords = exp.generateBasicGridItems(self.selectedObjects(), self.selectedRecordGridColumns);

            //    return exp.generateBasicGridConfiguration(selectedRecords, self.selectedRecordGridColumns, "selectedObjects", true);
            //};

            //self.selectedRecordGrid = self.generateselectedRecordGrid();

            self.loadObjects = function () {

            };

            self.loadAvailableObjects = function (connectionName, reload) {

                if (self.connectionName() === connectionName && !reload)
                    return;
                else {
                    self.connectionName(connectionName);
                }

                var url = self.parentModel.state.dataServiceUrl.replace(self.parentModel.state.dataService, "schemabrowser");
                var availableObjectQuery = url + "/query/DbObjects?$filter=Connection/Name='" + connectionName + "'&$orderby=SchemaName,Name";

                $.ajax({
                    url: availableObjectQuery,
                    xhrFields: { withCredentials: true }
                })
                .done(function (response) {
                    self.availableRecordGrid().records.removeAll();
                    for (var i = 0; i < response.Records.length; i++) {
                        var availableRecord = response.Records[i];
                        var selected = false;
                        for (var j = 0; j < self.selectedRecordGrid().records().length; j++) {
                            var selectedRecord = self.selectedRecordGrid().records()[j].record;
                            if (availableRecord.SchemaName === selectedRecord.Schema.Name &&
                                availableRecord.Name === selectedRecord.DbName) {
                                selected = true;
                                break;
                            }
                        }
                        if (!selected) {
                            var gridItem = exp.generateBasicGridItem(availableRecord, self.availableRecordGrid().records().length + 1, self.availableRecordGridColumns);
                            self.availableRecordGrid().records.push(gridItem);
                        }
                    }
                });
            };

            //self.generateavailableRecordGridColumns = function () {
            //    var schemaMetadata = new exp.BasicGridColumnMetadata("string", "Schema", "Schema", "Schema");
            //    schemaMetadata.isNotEditableInGrid = true;

            //    var dbNameMetadata = new exp.BasicGridColumnMetadata("string", "DbName", "Db Name", "Db Name");
            //    dbNameMetadata.isNotEditableInGrid = true;

            //    return [
            //        new exp.ExplorerGridColumn(schemaMetadata, "Schema", 1),
            //        new exp.ExplorerGridColumn(dbNameMetadata, "DbName", 2)
            //    ];
            //};

            //self.availableRecordGridColumns = self.generateavailableRecordGridColumns();

            //self.generateavailableRecordGrid = function () {

            //    var availableRecords = exp.generateBasicGridItems([], self.availableRecordGridColumns);
            //    var availableRecordGrid = exp.generateBasicGridConfiguration(availableRecords, self.availableRecordGridColumns, "availableObjects", false);

            //    availableRecordGrid.validate = function (record, success, failure) {
            //        this.grid.updateDirtyRecordWithLatestValues(record, this.columns);
            //    };
            //    return availableRecordGrid;
            //};

            //self.availableRecordGrid = self.generateavailableRecordGrid();

            self.initialise = function () {
                var model = this;

                //var x = self.getAvailableObjects();
            };

            self.initialise();

            ko.postbox.subscribe("Connection" + '-property-changed', function (post) {
                self.loadAvailableObjects(post.selected().Name, false);
            });
        }

        return domainObjectsViewModel;
    }
);