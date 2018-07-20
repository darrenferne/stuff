define(["require", "exports", "knockout", "sprintf", "loglevel", "modules/bwf-utilities", "modules/bwf-datetimeUtilities", "options"], function (require, exports, knockout, sprintfM, log, utils, datetimeUtils, options) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var sprintf = sprintfM.sprintf;
    var BasePanelEditor = /** @class */ (function () {
        function BasePanelEditor(entity) {
            var _this = this;
            entity.observables['__renderedState'] = knockout.observableArray([]);
            this.timeoutElapsed = knockout.observable(false);
            this.errorMessages = knockout.observableArray([]);
            this.inTouchMode = utils.isTouchModeEnabled;
            this.notInTouchMode = knockout.pureComputed(function () { return !_this.inTouchMode(); });
            this.loadPanelTimeout = setTimeout(function () {
                var states = entity.observables['__renderedState'];
                _this.timeoutElapsed(true);
                log.warn("Timeout occurred when loading panel editor.");
            }, 10 * 1000);
            this.rendered = knockout.computed(function () {
                var children = entity.observables['__renderedState'];
                var unwrapped = children();
                var rendered = unwrapped.map(function (c) { return c(); }).every(function (c) { return c; });
                var timedout = _this.timeoutElapsed();
                if (unwrapped.length == 0)
                    return false;
                var isRendered = rendered || timedout;
                if (isRendered && _this.loadPanelTimeout) {
                    clearTimeout(_this.loadPanelTimeout);
                    _this.loadPanelTimeout = null;
                }
                return isRendered;
            }).extend({ rateLimit: { method: "notifyWhenChangesStop", timeout: 50 } });
        }
        BasePanelEditor.prototype.dispose = function () {
            this.rendered.dispose();
            if (this.loadPanelTimeout) {
                clearTimeout(this.loadPanelTimeout);
                this.loadPanelTimeout = null;
            }
        };
        BasePanelEditor.prototype.createSubscription = function (property, c) {
            var key = property.name;
            if (property.useCustomControl) {
                return;
            }
            var selected = 'selected' + key;
            // for properties with a 'selected' computed observable we hook our change message onto that
            // since knockout notifies subsribers in order of registration. This ends up notifying the
            // sub-components that the value has changed before the selected observable has had a chance
            // to update, meaning that the sub-component reads a stale value.
            var target = (property._isType || property.isEnum || property.hasChoice) ? selected : key;
            var subscription = c.observables[target].subscribe(function (newValue) {
                var changed = {
                    property: key,
                    value: c.observables[key](),
                    selected: c.observables[selected]
                };
                knockout.postbox.publish(key + '-property-changed', changed);
            });
            return subscription;
        };
        BasePanelEditor.prototype.createObservablesForChoice = function (property, c) {
            // observable which holds the currently selected item, identified by the 
            // valueField property, not the actual entity
            c.observables[property.name] = knockout.observable();
            // collection of items that can be selected from
            var available = sprintf('available%ss', property.name);
            c.observables[available] = knockout.observableArray([]);
            // observable for the object representation of of the chosen item. 
            c.observables['selected' + property.name] = knockout.pureComputed(function () {
                var keyValue = c.observables[property.name]();
                var availableItems = c.observables[available]();
                var selected = availableItems.filter(function (item) {
                    var v = item[property.valueFieldInEditorChoice];
                    return v && keyValue && v.toString() === keyValue.toString();
                });
                // if there are somehow multiple matching items just take the first one
                return selected.length > 0 ? selected[0] : undefined;
            });
        };
        BasePanelEditor.prototype.listenForChangesTo = function (property, c) {
            var topic = property + '-property-set-by-custom-control';
            return knockout.postbox.subscribe(topic, function (value) {
                c.observables[property](value);
            });
        };
        BasePanelEditor.prototype.getComponentNameFromProperty = function (property) {
            if (property.hasChoice || property._isType) {
                return 'ds-explorer-cc-hasChoice';
            }
            switch (property.type.toLowerCase()) {
                case 'integer':
                case 'numeric':
                    return 'ds-explorer-cc-numeric';
                case 'time':
                    return 'ds-explorer-cc-time';
                case 'date':
                    return 'ds-explorer-cc-date';
                case 'measure':
                    return 'ds-explorer-cc-valueWithUnit';
                case 'boolean':
                    return 'ds-explorer-cc-boolean';
                case 'image':
                    return 'ds-explorer-cc-image';
                case 'collection':
                    return 'ds-explorer-cc-readOnlyCollection';
                default:
                    return 'ds-explorer-cc-string';
            }
        };
        BasePanelEditor.prototype.createObservables = function (m, c, formDisabled, createObservablesForChoice, callback) {
            var _this = this;
            var properties;
            if (!Array.isArray(m)) {
                properties = Object.keys(m.properties)
                    .map(function (key) { return m.properties[key]; });
            }
            else {
                properties = m;
            }
            var propertiesUsedForRefresh = [];
            properties.forEach(function (property) {
                if (property._isCollection) {
                    c.observables[property.name] = knockout.observableArray([]);
                }
                else if ((property._isType || property.isEnum || property.hasChoice) && _this.isPropertyVisible(property, c.state.isCreate)) {
                    createObservablesForChoice(property, c);
                }
                else {
                    c.observables[property.name] = knockout.observable(null);
                }
                if (property.refreshChoiceOnChangesTo) {
                    property.refreshChoiceOnChangesTo.split(';').forEach(function (r) {
                        if (propertiesUsedForRefresh.indexOf(r) < 0)
                            propertiesUsedForRefresh.push(r);
                    });
                }
            });
            propertiesUsedForRefresh.forEach(function (p) {
                if (!c.observables['selected' + p]) {
                    c.observables['selected' + p] = c.observables[p];
                }
            });
            c.observables['isLocked'] = knockout.observable(false);
            c.observables['hasIgnoreLock'] = knockout.observable(true);
            c.observables['customControlDisableSave'] = knockout.observable(false);
            c.observables['formDisabled'] = formDisabled;
            Object.keys(c.observables)
                .map(function (key) { return c.observables[key]; })
                .forEach(function (obs) {
                if (obs && knockout.isObservable(obs)) {
                    obs.extend({
                        rateLimit: {
                            timeout: 250, method: 'notifyWhenChangesStop'
                        }
                    });
                }
            });
            if (typeof callback === 'function')
                callback(m, c);
        };
        BasePanelEditor.prototype.populateObservables = function (properties, c, record) {
            properties.forEach(function (property) {
                var currentValue = record[property.name];
                if (property._isCollection) {
                    c.observables[property.name](currentValue);
                }
                else if (property.hasChoice) {
                    var value = property._isType || property.isEnum
                        ? (currentValue
                            ? currentValue[property.valueFieldInEditorChoice]
                            : property.defaultValue)
                        : currentValue;
                    c.observables[property.name](value);
                }
                else if (property.type == 'time' && !c.state.isCreate) {
                    var convertedValue = datetimeUtils.convertToTZ(currentValue, options.derivedTimezone, 'YYYY-MM-DDTHH:mm:ss');
                    var newValue = kendo.toString(kendo.parseDate(convertedValue), 'u', options.formattingCulture);
                    c.observables[property.name](newValue);
                }
                else {
                    c.observables[property.name](currentValue);
                }
            });
        };
        BasePanelEditor.prototype.setupValidation = function (properties, c) {
            var _this = this;
            properties.forEach(function (property) {
                var observable = c.observables[property.name];
                c.validations.messages[property.name] = knockout.observable('');
                if (property._isType || !_this.isPropertyVisible(property, c.state.isCreate) || property.hasChoice) {
                    return;
                }
                switch (property.type.toLowerCase()) {
                    case 'integer':
                        observable.extend({
                            validInteger: {
                                allowNull: property.isNullable,
                                message: sprintf("'%s' must be a valid integer", property.name)
                            }
                        });
                        break;
                    case 'numeric':
                        observable.extend({
                            validNumeric: {
                                allowNull: property.isNullable,
                                message: sprintf("'%s' must be a valid number", property.name)
                            }
                        });
                        break;
                    case 'measure':
                        // setup the isValid observable so it is picked up by the saveDisabled computed
                        // we do the actual validation in the valueWithUnit component
                        observable.isValid = knockout.observable(true);
                        break;
                    case 'string':
                        // we can't do anything here unless we either hardcode in property names to apply
                        // validation to or define some new metadata.
                        break;
                    default:
                        break;
                }
            });
        };
        BasePanelEditor.prototype.getPropertiesFromMetadata = function (m, isCreate, doNotFilterHidden) {
            var _this = this;
            var properties = Object.keys(m.properties)
                .map(function (key) { return m.properties[key]; });
            if (!doNotFilterHidden) {
                properties = properties.filter(function (p) {
                    return _this.isPropertyVisible(p, isCreate);
                });
            }
            return properties;
        };
        BasePanelEditor.prototype.isPropertyVisible = function (p, isCreate) {
            if (p.isHiddenInEditor)
                return false;
            return (isCreate && !p.isHiddenInCreateMode) || (!isCreate && !p.isHiddenInEditMode);
        };
        return BasePanelEditor;
    }());
    exports.BasePanelEditor = BasePanelEditor;
});
