define(["require", "exports", "knockout"], function (require, exports, ko) {
    "use strict";
    var Validations = /** @class */ (function () {
        function Validations() {
            var _this = this;
            this.validators = ko.observableArray([]);
            this.messages = {};
            this.isValid = ko.pureComputed(function () {
                var validators = _this.validators();
                var isValid = validators.map(function (v) { return v(); })
                    .every(function (v) { return v; });
                return isValid;
            });
        }
        Validations.prototype.add = function (v) {
            this.validators.push(v);
        };
        return Validations;
    }());
    var PanelState = /** @class */ (function () {
        function PanelState(args, gridId) {
            var _this = this;
            this.onCompletion = function (_) { };
            this.action = args.action;
            this.component = args.component;
            this.parentIsSource = args.parentIsSource || false;
            this.isWide = ko.observable(!!args.isWidePane);
            this.gridId = gridId;
            this.resizeDisabled = !!args.resizeDisabled;
            this.typeName = args.baseType;
            this._stackId = window.performance.now();
            this.requiresUpdate = args.action !== 'create' && !this.parentIsSource;
            if (args.onCompletion) {
                this.onCompletion = args.onCompletion;
            }
            var keysFromSelf = Object.keys(this).concat(['isWidePane', 'baseType']);
            var keysFromArgs = Object.keys(args);
            var keysToCopy = keysFromArgs.filter(function (key) { return keysFromSelf.indexOf(key) === -1; });
            keysToCopy.forEach(function (key) { return _this[key] = args[key]; });
        }
        Object.defineProperty(PanelState.prototype, "isCreate", {
            get: function () { return this.action == 'create' || this.action == 'copy'; },
            enumerable: true,
            configurable: true
        });
        return PanelState;
    }());
    var StackItem = /** @class */ (function () {
        function StackItem(record, state) {
            this.record = record;
            this.observables = {};
            this.validations = new Validations();
            this.state = state;
            this.observables['__renderedState'] = ko.observableArray([]);
        }
        return StackItem;
    }());
    var PanelViewModel = /** @class */ (function () {
        function PanelViewModel(viewGridId) {
            var _this = this;
            this.stack = ko.observableArray([]);
            this.component = ko.observable('bwf-empty').extend({ notify: 'always' });
            this.isPanelWide = ko.pureComputed(function () {
                var s = _this.stack();
                var head = s.length > 0 ? s[0] : null;
                return head ? head.state.isWide() : false;
            });
            this.isResizeDisabled = ko.pureComputed(function () {
                var s = _this.stack();
                var head = s.length > 0 ? s[0] : null;
                return head ? head.state.resizeDisabled : false;
            });
            this.grid = viewGridId;
            this.subscriptions = ko.observableArray([]);
            this.component.extend({ notify: 'always' });
            this.subscriptions.push(ko.postbox.subscribe(this.grid + '-doAction', function (message) {
                ko.postbox.publish(_this.grid + '-showPane');
                if (!message.preserveStack && !message.parentIsSource) {
                    _this.clear();
                }
                var record = _this.getRecordData(message);
                var state = new PanelState(message, _this.grid);
                if (message.enqueue == null || message.enqueue == false)
                    _this.push(record, state);
                else
                    _this.unshift(record, state);
            }));
            this.subscriptions.push(ko.postbox.subscribe(this.grid + '-doAction-withoutPanel', function (message) {
                var record = _this.getRecordData(message);
                var state = new PanelState(message, _this.grid);
                var args = new StackItem(record, state);
                ko.components.get(args.state.component, function (com) {
                    com.createViewModel(args, undefined);
                });
            }));
            this.subscriptions.push(ko.postbox.subscribe(this.grid + '-pop-panel', function () { return _this.pop(); }));
            this.subscriptions.push(ko.postbox.subscribe(this.grid + '-gridInEditMode', function () { return _this.hide(); }));
            this.subscriptions.push(ko.postbox.subscribe(this.grid + '-loadView', function () { return _this.hide(); }));
            this.subscriptions.push(ko.postbox.subscribe(this.grid + '-clear-panel', function () { return _this.clear(); }));
            this.subscriptions.push(ko.postbox.subscribe(this.grid + '-togglePanelWidth', function (wide) { return _this.togglePanelWidth(wide); }));
            this.subscriptions.push(this.isPanelWide.subscribe(function (isWide) { return ko.postbox.publish(_this.grid + '-isWidePane', isWide); }));
        }
        PanelViewModel.prototype.getRecordData = function (args) {
            if (args.data) {
                return args.data;
            }
            switch (args.action.toLowerCase()) {
                case 'edit':
                case 'copy':
                case 'view':
                    return args.toEdit[0];
                case 'lock':
                    return args.toLock;
                case 'delete':
                    return args.toDelete;
                default:
                    return {};
            }
        };
        PanelViewModel.prototype.dispose = function () {
            this.subscriptions().forEach(function (s) { return s.dispose(); });
        };
        // these are called push and pop but use unshift and shift so that the item
        // at 0 index in the array is always the "top" of the stack, and we don't need 
        // to get the length and index based on that to get the current item
        PanelViewModel.prototype.push = function (record, state) {
            var next = new StackItem(record, state);
            this.component('bwf-empty');
            this.stack.unshift(next);
            this.component(next.state.component);
        };
        PanelViewModel.prototype.unshift = function (record, state) {
            var next = new StackItem(record, state);
            this.stack.push(next);
        };
        PanelViewModel.prototype.pop = function () {
            this.component('bwf-empty');
            var item = this.stack.shift();
            var closeTopic = item.state.action;
            if (item.state.gridId !== null || item.state.gridId !== undefined || item.state.gridId !== '')
                closeTopic += "-" + item.state.gridId;
            if (item.state.typeName !== null || item.state.typeName !== undefined || item.state.typeName !== '')
                closeTopic += "-" + item.state.typeName;
            ko.postbox.publish(closeTopic + "-panelClosed");
            if (this.stack().length === 0) {
                this.hide();
            }
            else {
                var next = this.stack.peek()[0];
                this.component(next.state.component);
            }
        };
        PanelViewModel.prototype.hide = function () {
            this.component('bwf-empty');
            this.stack([]);
            ko.postbox.publish(this.grid + '-hidePane', []);
        };
        PanelViewModel.prototype.clear = function () {
            this.component('bwf-empty');
            this.stack([]);
        };
        PanelViewModel.prototype.togglePanelWidth = function (wide) {
            var s = this.stack();
            var head = s.length > 0 ? s[0] : null;
            // we have to check in case 'wide' hasn't been passed in as
            // a knockout binding will also pass in the ko viewmodel so
            // we need to make sure 'wide' is a boolean
            if (typeof wide !== "boolean") {
                if (head)
                    head.state.isWide(!head.state.isWide());
            }
            else
                head.state.isWide(wide);
        };
        return PanelViewModel;
    }());
    return PanelViewModel;
});
