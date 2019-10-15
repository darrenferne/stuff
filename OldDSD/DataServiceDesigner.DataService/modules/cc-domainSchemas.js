﻿define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'modules/bwf-explorer', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService, exp) {


        function domainSchemasViewModel(data) {
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
            self.availableRecordGrid = ko.observable(null);
            self.availableObjects = ko.observableArray([]);
            self.selectedRecordGrid = ko.observable(null);
            self.connection = data.model.observables.Connection();
            self.selectedObjects = data.model.observables.Schemas;

            self.select = function () {
                var selectedRecords = self.availableRecordGrid().selectedRecords();
                for (var i = 0; i < selectedRecords.length; i++) {
                    var selectedRecord = selectedRecords[i].record;
                    var domainSchema = {
                        Id: 0,
                        SchemaName: selectedRecord.Name,
                        IsDefault: self.selectedObjects().length == 0 ? true : false
                    };
                    var gridItem = exp.generateBasicGridItem(domainSchema, self.selectedRecordGrid().records().length + 1, self.selectedRecordGridColumns);
                    self.selectedRecordGrid().records.push(gridItem);
                    self.selectedObjects().push(domainSchema);
                }
                self.availableRecordGrid().records.removeAll(selectedRecords);
            };
            self.deleteSelected = function () {
                var selectedRecords = self.selectedRecordGrid().selectedRecords();
                var selectedObjects = self.selectedRecordGrid().selectedRecords().map(function (r) { return r.record; });
                self.selectedRecordGrid().records.removeAll(selectedRecords);
                self.selectedObjects.removeAll(selectedObjects);
                self.loadAvailableObjects(self.connection, true);
            };
            self.clear = function () {
                self.selectedRecordGrid().records.removeAll();
                self.selectedObjects.removeAll();
                self.loadAvailableObjects(self.connection, true);
            };
        
            metadataService.getType("dataservicedesigner", "DomainSchema").done(metadata => {

                var idMetadata = metadata.properties["Id"];
                idMetadata.isNotEditableInGrid = true;
                var nameMetadata = metadata.properties["SchemaName"];
                nameMetadata.isNotEditableInGrid = true;
                var isDefaultMetadata = metadata.properties["IsDefault"];
                
                self.selectedRecordGridColumns = [
                    new exp.ExplorerGridColumn(idMetadata, "Id", 1),
                    new exp.ExplorerGridColumn(nameMetadata, "SchemaName", 2),
                    new exp.ExplorerGridColumn(isDefaultMetadata, "IsDefault", 3)
                ];  
                
                var selectedItems = exp.generateBasicGridItems(self.selectedObjects(), self.selectedRecordGridColumns);
                var grid = exp.generateBasicGridConfiguration(selectedItems, self.selectedRecordGridColumns, "selectedObjects", true);

                grid.validate = function (row, success, failure) {
                    self.selectedRecordGrid().updateDirtyRecordWithLatestValues(row.record, self.selectedRecordGridColumns);
                    success(row, row.dirtyRecord);
                    self.selectedObjects.replace(row.record, row.dirtyRecord);
                };

                self.selectedRecordGrid(grid);
            });

            metadataService.getType("schemabrowser", "DbSchema").done(metadata => {

                var dbNameMetadata = metadata.properties["Name"];
                
                self.availableRecordGridColumns = [
                    new exp.ExplorerGridColumn(dbNameMetadata, "Name", 1)
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

            self.loadAvailableObjects = function (connection, reload) {

                if (connection === null || self.connection === connection && !reload) {
                    return;
                }
                else {
                    self.connection = connection;
                }

                var url = self.parentModel.state.dataServiceUrl.replace(self.parentModel.state.dataService, "schemabrowser");
                var availableObjectQuery = url + "/query/DbSchemas?$filter=Connection/ExternalId=" + connection.Id + "&$orderby=Name";

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
                            if (availableRecord.Name === selectedRecord.Name) {
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
                self.loadAvailableObjects(post.selected(), false);
            });
        }

        return domainSchemasViewModel;
    }
);