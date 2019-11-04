define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'modules/bwf-explorer', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService, exp) {

        function domainObjectPropertiesViewModel(data) {
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
            self.schema = data.model.observables.Schema();
            self.objectName = data.model.observables.ObjectName();
            self.selectedObjects = data.model.observables.Properties;

            self.toDisplayName = function (name, pluralise) {
                if (name.includes('_')) {
                    var parts = name.Split('_');
                    displayName = parts.map(self.toDisplayName).join(' ');
                }
                else {
                    if (name === name.toUpperCase()) {
                        name = name.toLowerCase();
                    }
                    var displayName = name.charAt(0).toUpperCase();
                    for (var i = 1; i < name.length; i++) {
                        var char = name.charAt(i);
                        var charM1 = name.charAt(i - 1);
                        if (char === char.toUpperCase() && charM1 !==charM1.toUpperCase()) {
                            displayName = displayName + ' ' + char;
                        }
                        else {
                            displayName = displayName + char;
                        }
                    }
                }
                if (pluralise) {
                    if (displayName.substr(-2).toLower() === 'ty') {
                        displayName = displayName.substr(displayName.length - 2) + 'ties';
                    }
                    else {
                        displayName = displayName + 's';
                    }
                }
                return displayName;
            };

            self.select = function () {
                var selectedRecords = self.availableRecordGrid().selectedRecords();
                for (var i = 0; i < selectedRecords.length; i++) {
                    var selectedRecord = selectedRecords[i].record;
                    var domainProperty = {
                        Id: 0,
                        ColumnName: selectedRecord.Name,
                        PropertyName: selectedRecord.Name,
                        DisplayName: self.toDisplayName(selectedRecord.Name),
                        PropertyType: selectedRecord.NetType,
                        Length: selectedRecord.ColumnLength,
                        IsNullable: selectedRecord.IsNullable
                    };
                    var gridItem = exp.generateBasicGridItem(domainProperty, self.selectedRecordGrid().records().length + 1, self.selectedRecordGridColumns);
                    self.selectedRecordGrid().records.push(gridItem);
                    self.selectedObjects().push(domainProperty);
                }
                self.availableRecordGrid().records.removeAll(selectedRecords);
            };
            self.deleteSelected = function () {
                var selectedRecords = self.selectedRecordGrid().selectedRecords();
                var selectedObjects = self.selectedRecordGrid().selectedRecords().map(function (r) { return r.record; });
                self.selectedRecordGrid().records.removeAll(selectedRecords);
                self.selectedObjects.removeAll(selectedObjects);
                self.loadAvailableObjects(self.schema, self.objectName, true);
            };
            self.clear = function () {
                self.selectedRecordGrid().records.removeAll();
                self.selectedObjects.removeAll();
                self.loadAvailableObjects(self.schema, self.objectName, true);
            };

            metadataService.getType("dataservicedesigner", "DomainObjectProperty").done(metadata => {

                //var idMetadata = metadata.properties["Id"];
                //idMetadata.isNotEditableInGrid = true;
                var columnNameMetadata = metadata.properties["ColumnName"];
                var propertyNameMetadata = metadata.properties["PropertyName"];
                var displayNameMetadata = metadata.properties["DisplayName"];
                var propertyTypeMetadata = metadata.properties["PropertyType"];
                var lengthMetadata = metadata.properties["Length"];
                var isNullableMetadata = metadata.properties["IsNullable"];
                var isPartOfKeyMetadata = metadata.properties["IsPartOfKey"];
                var includeInDefaultViewMetadata = metadata.properties["IncludeInDefaultView"];

                self.selectedRecordGridColumns = [
                    //new exp.ExplorerGridColumn(idMetadata, "Id", 1),
                    new exp.ExplorerGridColumn(columnNameMetadata, "ColumnName", 1),
                    new exp.ExplorerGridColumn(propertyNameMetadata, "PropertyName", 2),
                    new exp.ExplorerGridColumn(displayNameMetadata, "DisplayName", 3),
                    new exp.ExplorerGridColumn(propertyTypeMetadata, "PropertyType", 4),
                    new exp.ExplorerGridColumn(lengthMetadata, "Length", 5),
                    new exp.ExplorerGridColumn(isNullableMetadata, "IsNullable", 6),
                    new exp.ExplorerGridColumn(isPartOfKeyMetadata, "IsPartOfKey", 7),
                    new exp.ExplorerGridColumn(includeInDefaultViewMetadata, "IncludeInDefaultView", 8)];

                var selectedItems = exp.generateBasicGridItems(self.selectedObjects(), self.selectedRecordGridColumns);
                var grid = exp.generateBasicGridConfiguration(selectedItems, self.selectedRecordGridColumns, "selectedObjects", true);

                grid.validate = function (row, success, failure) {
                    self.selectedRecordGrid().updateDirtyRecordWithLatestValues(row, self.selectedRecordGridColumns);
                    success(row, row.dirtyRecord);
                    self.selectedObjects.replace(row.record, row.dirtyRecord);
                };

                self.selectedRecordGrid(grid);
            });

            metadataService.getType("schemabrowser", "DbObjectProperty").done(metadata => {

                var dbNameMetadata = metadata.properties["Name"];

                self.availableRecordGridColumns = [
                    new exp.ExplorerGridColumn(dbNameMetadata, "Name", 1)
                ];

                var availableItems = exp.generateBasicGridItems(self.availableObjects(), self.availableRecordGridColumns);
                var grid = exp.generateBasicGridConfiguration(availableItems, self.availableRecordGridColumns, "availableObjects", false);

                self.availableRecordGrid(grid);
            });

            self.loadAvailableObjects = function (schema, objectName, reload) {

                if (schema == null || objectName === null) {
                    return;
                }
                else if (self.schema == schema && self.objectName === objectName && !reload) {
                    return;
                }

                var url = self.parentModel.state.dataServiceUrl;
                var dataServiceQuery = url + "/query/DomainSchemas?$filter=Id=" + schema + "&$expands=DataService/Connection";

                $.ajax({
                    url: dataServiceQuery,
                    xhrFields: { withCredentials: true }
                })
                .done(function (response) {

                    url = self.parentModel.state.dataServiceUrl.replace(self.parentModel.state.dataService, "schemabrowser");
                    var availableObjectQuery = url + "/query/DbObjectPropertys?$filter=ObjectName='" + objectName + "'&$orderby=Name";

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
                                if (availableRecord.Name === selectedRecord.ColumnName) {
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
                self.loadAvailableObjects(self.schema, self.objectName, true);
            
                ko.postbox.subscribe("Schema" + '-property-changed', function (post) {
                    self.loadAvailableObjects(post.value, self.objectName, false);
                    self.schema = post.value;
                });

                ko.postbox.subscribe("ObjectName" + '-property-changed', function (post) {
                    self.loadAvailableObjects(self.schema, post.value, false);
                    self.objectName = post.value;
                });

                ko.postbox.publish(self.data.grid + '-togglePanelWidth', true);
            };

            self.initialise();
        }

        return domainObjectPropertiesViewModel;
    }
);