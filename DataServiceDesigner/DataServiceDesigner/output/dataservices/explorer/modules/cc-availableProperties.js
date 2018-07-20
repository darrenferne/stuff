define(["require", "exports", "knockout", "loglevel", "modules/bwf-metadata", "scripts/knockout-selection", "knockout-postbox"], function (require, exports, ko, log, metadataService) {
    "use strict";
    var AvailableProperties = /** @class */ (function () {
        function AvailableProperties(data) {
            var _this = this;
            this.breadCrumbClicked = function (item) {
                log.debug('Bread crumb clicked', item);
                _this.breadCrumbs.remove(function (i) { return i.depth > item.depth; });
                _this.updatePropertySet(item.originalProperties);
            };
            this.propertyClicked = function (item) {
                log.debug('Property clicked', item);
                var length = _this.breadCrumbs().length;
                var namePrefix = length > 1 ? (_this.breadCrumbs()[length - 1]).name : '';
                var name = namePrefix + item.name;
                var displayNamePrefix = length > 1 ? (_this.breadCrumbs()[length - 1]).displayName : '';
                var displayName = displayNamePrefix === '' ? item.displayName : displayNamePrefix + ' ' + item.displayName;
                if (!item.hasChildren) {
                    ko.postbox.publish(_this.instanceName + '-cc-available-property-selected', { name: name, displayName: displayName, item: item.originalItem });
                    return;
                }
                metadataService.getType(_this.dataService(), item.originalItem._clrType).done(function (metadata) {
                    _this.breadCrumbs.push({
                        depth: length + 1,
                        displayName: item.displayName,
                        name: name + '/',
                        originalProperties: metadata.properties
                    });
                    _this.updatePropertySet(metadata.properties);
                });
            };
            log.debug('Data passed into available properties view model:', data);
            this.subscriptions = ko.observableArray([]);
            this.instanceName = data.instanceName;
            this.baseType = data.baseType;
            this.dataService = data.dataService;
            if (!ko.isObservable(this.dataService))
                throw new Error("Observable could not be found for data service");
            this.breadCrumbs = ko.observableArray([]);
            this.propertySet = ko.observableArray([]);
            this.availableProperties = ko.observableArray([]);
            this.isCreate = data.isCreate;
            this.rendered = ko.observable(false);
            this.loaded = ko.observable(false);
            this.ready = ko.pureComputed(function () {
                var rendered = _this.rendered();
                var loaded = _this.loaded();
                return rendered && loaded;
            });
            this.formDisabled = data.formDisabled;
            if (data.renderedState)
                data.renderedState.push(this.ready);
            this.subscriptions.push(this.dataService.subscribe(function (ds) {
                if (!ds)
                    _this.resetAvailableProperties();
            }));
            this.subscriptions.push(this.breadCrumbs.subscribe(function (breadCrumbs) {
                log.debug('bread crumbs changed', breadCrumbs);
            }));
            this.subscriptions.push(this.baseType.subscribe(function (baseType) {
                log.debug('base type changed', baseType);
                _this.resetAvailableProperties();
            }));
            this.subscriptions.push(this.availableProperties.subscribe(function (availableProperties) {
                log.debug('available properties changed', availableProperties);
                _this.updatePropertySet(availableProperties);
            }));
            this.subscriptions.push(this.propertySet.subscribe(function (propertySet) {
                log.debug('property set changed', propertySet);
            }));
            this.resetAvailableProperties();
            ko.postbox.subscribe(this.instanceName + '-available-properties-rendered', function () {
            });
        }
        AvailableProperties.prototype.dispose = function () {
            this.subscriptions().forEach(function (sub) {
                sub.dispose();
            });
        };
        AvailableProperties.prototype.updatePropertySet = function (availableProperties) {
            var filteredProperties = Object.keys(availableProperties).map(function (key) { return availableProperties[key]; }).filter(function (item) {
                return !item.isHidden && !item._isCollection;
            });
            var orderedProperties = filteredProperties.sort(function (left, right) {
                return left.displayName == right.displayName ? 0 : (left.displayName < right.displayName ? -1 : 1);
            });
            var initialPropertySet = orderedProperties.map(function (item, index) {
                var hasChildren = item._isType;
                var displayNameWithIndicator = hasChildren ? item.displayName + '...' : item.displayName;
                return {
                    index: index,
                    displayNameWithIndicator: displayNameWithIndicator,
                    displayName: item.displayName,
                    name: item.name,
                    hasChildren: hasChildren,
                    originalItem: item,
                    title: item.description
                };
            });
            this.propertySet([]);
            this.propertySet(initialPropertySet);
        };
        AvailableProperties.prototype.resetAvailableProperties = function () {
            var _this = this;
            log.debug('Resetting available properties');
            if (!this.dataService() || !this.baseType()) {
                this.breadCrumbs([]);
                this.availableProperties([]);
                if (this.isCreate) {
                    this.loaded(true);
                }
                return;
            }
            metadataService.getType(this.dataService(), this.baseType()).done(function (response) {
                log.debug('Success', response);
                _this.breadCrumbs([
                    {
                        depth: 1,
                        displayName: response.displayName,
                        name: '',
                        originalProperties: response.properties
                    }
                ]);
                var asArray = Object.keys(response.properties).map(function (key) { return response.properties[key]; });
                _this.availableProperties(asArray);
            }).always(function () { return _this.loaded(true); });
        };
        return AvailableProperties;
    }());
    return AvailableProperties;
});
