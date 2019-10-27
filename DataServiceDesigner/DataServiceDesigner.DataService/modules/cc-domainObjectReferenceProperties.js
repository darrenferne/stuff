define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'modules/bwf-explorer', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService, exp) {

        function domainObjectReferencePropertiesViewModel(data) {
            var self = this;

            self.data = data;
            self.parentModel = data.model;
            self.resources = options.resources;
            self.formDisabled = ko.observable(false);
            self.referencePropertyGrid = ko.observable(null);
            self.referenceProperties = data.model.observables.Properties;
            self.id = data.model.observables.Id;
            self.selectedParentObject = data.model.observables.Parent;
            self.selectedChildObject = data.model.observables.Child;

            self.canAdd = ko.pureComputed(function() {
                return self.selectedParentObject() != null && self.selectedChildObject() != null;
            });
            self.canDelete = ko.pureComputed(function () {
                if (self.referencePropertyGrid() === null || self.referencePropertyGrid().selectedRecords() === null)
                    return false;
                return self.referencePropertyGrid().selectedRecords().length > 0;
            });
            self.canClear = ko.pureComputed(function () {
                if (self.referencePropertyGrid() === null || self.referencePropertyGrid().selectedRecords() === null)
                    return false;
                return self.referencePropertyGrid().records().length > 0;
            });
            self.add = function () {
                var referenceProperty = {
                    Id: 0,
                    ReferenceId: self.id(), 
                    ParentObjectId: self.selectedParentObject(),
                    ChildObjectId: self.selectedChildObject(),
                    ParentPropertyId: null,
                    ChildPropertyId: null
                };
                var gridItem = exp.generateBasicGridItem(referenceProperty, self.referencePropertyGrid().records().length + 1, self.referencePropertyGridColumns);
                self.referencePropertyGrid().records.push(gridItem);
                self.referenceProperties().push(referenceProperty);
            };
            self.deleteSelected = function () {
                var selectedRecords = self.referencePropertyGrid().selectedRecords();
                var selectedObjects = self.referencePropertyGrid().selectedRecords().map(function (r) { return r.record; });
                self.referencePropertyGrid().records.removeAll(selectedRecords);
                self.referenceProperties.removeAll(selectedObjects);

            };
            self.clear = function () {
                self.referencePropertyGrid().records.removeAll();
                self.referenceProperties.removeAll();
            };

            metadataService.getType("dataservicedesigner", "DomainObjectReferenceProperty").done(metadata => {

                var parentMetadata = metadata.properties["ParentPropertyId"];
                parentMetadata.refreshChoiceOnChangesTo = "ParentObjectId";
                parentMetadata.populateChoiceQuery = "'dataservicedesigner/query/DomainObjectPropertys?' + (this.selectedParentObjectId() == null ? '' : '$filter=Object/Id=' + this.selectedParentObjectId() + '&') + '$expand=Object&$orderby=PropertyName'";
                var childMetadata = metadata.properties["ChildPropertyId"];
                childMetadata.refreshChoiceOnChangesTo = "ChildObjectId";
                childMetadata.populateChoiceQuery = "'dataservicedesigner/query/DomainObjectPropertys?' + (this.selectedChildObjectId() == null ? '' : '$filter=Object/Id=' + this.selectedChildObjectId() + '&') + '$expand=Object&$orderby=PropertyName'";

                self.referencePropertyGridColumns = [
                    new exp.ExplorerGridColumn(parentMetadata, "ParentPropertyId", 1),
                    new exp.ExplorerGridColumn(childMetadata, "ChildPropertyId", 2)
                ];

                var properties = self.referenceProperties().map(function (r) {
                    return {
                        Id: r.Id,
                        ReferenceId: r.ReferenceId, 
                        ParentObjectId: self.selectedParentObject(),
                        ChildObjectId: self.selectedChildObject(),
                        ParentPropertyId: r.ParentProperty.Id,
                        ChildPropertyId: r.ChildProperty.Id
                    };
                });

                var gridItems = exp.generateBasicGridItems(properties, self.referencePropertyGridColumns);
                var grid = exp.generateBasicGridConfiguration(gridItems, self.referencePropertyGridColumns, "referenceProperties", true);

                grid.validate = function (row, success, failure) {
                    self.referencePropertyGrid().updateDirtyRecordWithLatestValues(row, self.referencePropertyGridColumns);
                    success(row, row.dirtyRecord);
                    self.referenceProperties.replace(row.record, row.dirtyRecord);
                };

                self.referencePropertyGrid(grid);
            });

            self.initialise = function() {

                self.selectedParentObject.subscribe(function() {
                    self.clear();
                });
                self.selectedChildObject.subscribe(function() {
                    self.clear();
                });
            };

            self.initialise();
        }

        return domainObjectReferencePropertiesViewModel;
    }
);