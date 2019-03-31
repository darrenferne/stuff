define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'modules/bwf-idgen', 'knockout-amd-helpers', 'knockout-postbox'],
    function (ko, options, log, metadataService, idgen) {

        function workflowLevelsViewModel(data) {
            var self = this;

            self.parentModel = data.model;
            self.subscribed = false;
            self.getNextId = function () {
                var id = idgen.nextId();
                return id === 0 ? idgen.nextId() : id;
            };
            self.controlHeight = ko.observable(data.metadata.customControlHeight + "px");

            self.levels = self.parentModel.observables["Levels"]() === null ? ko.observableArray([]) : data.model.observables['Levels'];
            self.levels().forEach(function (level) {
                level.internalId = self.getNextId();
            });

            self.displayColumns = [
                {
                    title: 'Level',
                    field: 'Name',
                    template: '#= Name == null ? "" : Name #'
                }
            ];

            self.selectedLevels = ko.observableArray([]);
            self.selectedLevelsChange = function () {
                self.selectedLevels([]);
                var grid = $("#workflowLevelsGrid").data("kendoGrid");
                var selection = grid.select();
                for (var i = 0; i < selection.length; i++) {
                    var dataItem = grid.dataItem(selection[i]);
                    self.selectedLevels.push(dataItem);
                }
            };

            self.showCreate = ko.computed(function () {
                return true;
            });
            self.showEdit = ko.computed(function () {
                return true;
            });
            self.showDelete = ko.computed(function () {
                return true;
            });
            self.enableCreate = ko.computed(function () {
                return true;
            });
            self.enableEdit = ko.computed(function () {
                return (self.selectedLevels().length === 1);
            });
            self.enableDelete = ko.computed(function () {
                return (self.selectedLevels().length > 0);
            });

            self.createLevel = function () {

                var onCompletion = function (params) {
                    params.record.internalId = self.getNextId();
                    if (self.parentModel.record.Levels === null)
                        self.parentModel.record.Levels = [];
                    self.parentModel.record.Levels.push(params.record);
                };

                var actionArgs = {
                    action: 'create',
                    baseType: 'WorkflowLevel',
                    component: 'bwf-panel-editor',
                    onCompletion: onCompletion,
                    parentIsSource: true,
                    dataService: 'limitsprototype'
                };

                ko.postbox.publish(self.parentModel.state.gridId + '-doAction', actionArgs);
            };

            self.editLevel = function () {
                var recordToEdit = $.grep(self.levels(), function (item, index) {
                    return item.internalId === self.selectedLevels()[0].internalId;
                });

                var onCompletion = function (params) {
                    var editedLevel = params.record;
                    for (var i = 0; i < self.parentModel.record.Levels.length; i++) {
                        if (self.parentModel.record.Levels[i].internalId === editedLevel.internalId) {
                            self.parentModel.record.Levels[i] = editedLevel;
                            break;
                        }
                    }
                };

                var actionArgs = {
                    action: 'edit',
                    baseType: 'WorkflowLevel',
                    component: 'bwf-panel-editor',
                    onCompletion: onCompletion,
                    parentIsSource: true,
                    dataService: 'limitsprototype',
                    toEdit: recordToEdit
                };

                ko.postbox.publish(self.parentModel.state.gridId + '-doAction', actionArgs);
            };

            self.deleteLevel = function () {
                var recordTodelete = $.grep(self.levels(), function (item, index) {
                    return item.internalId === self.selectedLevels()[0].internalId;
                });

                var onCompletion = function (params) {
                    var deletedLevel = params.record[0];
                    for (var i = 0; i < self.parentModel.record.Levels.length; i++) {
                        if (self.parentModel.record.Levels[i].internalId === editedLevel.internalId) {
                            self.parentModel.record.Levels.splice(i, 1);
                            break;
                        }
                    }
                };

                var actionArgs = {
                    action: 'delete',
                    baseType: 'WorkflowLevel',
                    component: 'bwf-delete',
                    onCompletion: onCompletion,
                    parentIsSource: true,
                    dataService: 'limitsprototype',
                    toDelete: recordToDelete
                };

                ko.postbox.publish(self.parentModel.state.gridId + '-doAction', actionArgs);
            };
        }

        return workflowLevelsViewModel;
    }
);