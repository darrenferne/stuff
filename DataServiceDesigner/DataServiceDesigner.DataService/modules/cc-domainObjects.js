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
            self.availableRecordGrid = ko.observable(null);
            self.availableObjects = ko.observableArray([]);
            self.selectedRecordGrid = ko.observable(null);
            self.dataService = data.model.observables.DataService();
            self.schemaName = data.model.observables.SchemaName();
            self.selectedObjects = data.model.observables.Objects;

            self.toDisplayName = function (name, pluralise) {
                var displayName = name.charAt(0).toUpperCase();
                for (var i = 1; i < name.length; i++) {
                    var char = name.charAt(i);
                    if (char === char.toUpperCase()) {
                        displayName = displayName + ' ' + char;
                    }
                    else {
                        displayName = displayName + char;
                    }
                }
                if (pluralise) {
                    displayName = displayName + 's';
                }
                return displayName;
            };

            self.select = function () {
                var selectedRecords = self.availableRecordGrid().selectedRecords();
                for (var i = 0; i < selectedRecords.length; i++) {
                    var selectedRecord = selectedRecords[i].record;
                    var domainObject = {
                        Id: 0,
                        TableName: selectedRecord.Name,
                        ObjectName: selectedRecord.Name,
                        DisplayName: self.toDisplayName(selectedRecord.Name),
                        PluralisedDisplayName: self.toDisplayName(selectedRecord.Name, true)
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
                self.loadAvailableObjects(self.connection, true);
            };
            self.clear = function () {
                self.selectedRecordGrid().records.removeAll();
                self.selectedObjects.removeAll();
                self.loadAvailableObjects(self.connection, true);
            };

            metadataService.getType("dataservicedesigner", "DomainObject").done(metadata => {

                var idMetadata = metadata.properties["Id"];
                idMetadata.isNotEditableInGrid = true;
                var tableNameMetadata = metadata.properties["TableName"];
                var objectNameMetadata = metadata.properties["ObjectName"];
                var displayNameMetadata = metadata.properties["DisplayName"];
                var pluralisedDisplayNameMetadata = metadata.properties["PluralisedDisplayName"];

                self.selectedRecordGridColumns = [
                    new exp.ExplorerGridColumn(idMetadata, "Id", 1),
                    new exp.ExplorerGridColumn(tableNameMetadata, "TableName", 2),
                    new exp.ExplorerGridColumn(objectNameMetadata, "ObjectName", 3),
                    new exp.ExplorerGridColumn(displayNameMetadata, "DisplayName", 4),
                    new exp.ExplorerGridColumn(pluralisedDisplayNameMetadata, "PluralisedDisplayName", 5)];

                var selectedItems = exp.generateBasicGridItems(self.selectedObjects(), self.selectedRecordGridColumns);
                var grid = exp.generateBasicGridConfiguration(selectedItems, self.selectedRecordGridColumns, "selectedObjects", true);

                grid.validate = function (row, success, failure) {
                    self.selectedRecordGrid().updateDirtyRecordWithLatestValues(row, self.selectedRecordGridColumns);
                    success(row, row.dirtyRecord);
                    self.selectedObjects.replace(row.record, row.dirtyRecord);
                };

                self.selectedRecordGrid(grid);
            });

            metadataService.getType("schemabrowser", "DbObject").done(metadata => {

                var dbNameMetadata = metadata.properties["Name"];

                self.availableRecordGridColumns = [
                    new exp.ExplorerGridColumn(dbNameMetadata, "Name", 1)
                ];

                var availableItems = exp.generateBasicGridItems(self.availableObjects(), self.availableRecordGridColumns);
                var grid = exp.generateBasicGridConfiguration(availableItems, self.availableRecordGridColumns, "availableObjects", false);

                self.availableRecordGrid(grid);
            });

            self.loadAvailableObjects = function (dataService, schemaName, reload) {

                if (dataService == null || schemaName === null) {
                    return;
                }
                else if (self.dataService == dataService && self.schemaName == schemaName && !reload) {
                    return;
                }
                else {
                    self.dataService = dataService;
                    self.schemaName = schemaName;
                }

                var url = self.parentModel.state.dataServiceUrl;
                var dataServiceQuery = url + "/query/DomainDataServices?$filter=Id=" + dataService + "&$expands=Connection";

                $.ajax({
                    url: dataServiceQuery,
                    xhrFields: { withCredentials: true }
                })
                .done(function (response) {

                    url = self.parentModel.state.dataServiceUrl.replace(self.parentModel.state.dataService, "schemabrowser");
                    var availableObjectQuery = url + "/query/DbObjects?$filter=SchemaName='" + schemaName + "'&$orderby=Name";

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
                });
            };

            self.initialise = function () {
                self.loadAvailableObjects(self.dataService, self.schemaName, true);
            };

            ko.postbox.subscribe("DataService" + '-property-changed', function (post) {
                self.loadAvailableObjects(post.value, self.schemaName, false);
            });

            ko.postbox.subscribe("SchemaName" + '-property-changed', function (post) {
                self.loadAvailableObjects(self.dataService, post.value, false);
            });

            self.initialise();
        }

        return domainObjectsViewModel;
    }
);