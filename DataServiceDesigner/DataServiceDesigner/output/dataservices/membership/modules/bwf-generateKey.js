var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "knockout", "modules/bwf-metadata", "scripts/sprintf", "options", "modules/bwf-utilities", "loglevel", "modules/bwf-bindingHandlers", "bootstrapSelect", "scripts/bootstrap-select-binding", "modules/bwf-basePanelEditor", "clipboard"], function (require, exports, ko, mds, sprintfm, options, utils, log, bindingHandlers, bootstrapSelect, bootstrapSelectBinding, basePanelEditor, clipboard) {
    "use strict";
    var sprintf = sprintfm.sprintf;
    var bh = bindingHandlers;
    var bss = bootstrapSelect;
    var bsb = bootstrapSelectBinding;
    var GenerateKeyPanel = /** @class */ (function (_super) {
        __extends(GenerateKeyPanel, _super);
        function GenerateKeyPanel(panelEntity) {
            var _this = _super.call(this, panelEntity) || this;
            _this.keyHasBeenGenerated = ko.observable(false);
            _this.hasCreateAdministrative = ko.observable(false);
            _this.apiKey = ko.observable('');
            _this.keyLabel = ko.observable('');
            _this.availableUsers = ko.observableArray([]);
            _this.username = ko.observable(options.username);
            _this.errorMessages = ko.observableArray([]);
            _this.generatingKey = ko.observable(false);
            _this.ready = ko.observable(false);
            _this.isMobile = utils.isTouchModeEnabled;
            _this.roles = ko.observableArray([]);
            _this.rolesLoaded = ko.observable(false);
            _this.r = options.resources;
            _this.title = options.resources['bwf_generate_api_key'];
            _this.viewGrid = '';
            _this.handleFailure = function (failure) {
                if (failure.responseJSON.message)
                    _this.errorMessages.push(failure.responseJSON.message);
                if (failure.responseJSON.Messages)
                    _this.errorMessages(failure.responseJSON.Messages);
                log.error(failure);
            };
            _this.current = {
                typeMetadata: {
                    hasEditabilityToRoles: false,
                    hasVisibilityToRoles: true,
                },
                model: {
                    observables: panelEntity.observables,
                    validations: panelEntity.validations,
                    state: {
                        isCreate: function () { return true; }
                    }
                }
            };
            _this.viewGrid = panelEntity.state.gridId;
            var button = document.getElementById('copy-key');
            _this.cb = new clipboard(button);
            var getUserMetadata = mds.getType('membership', 'User');
            _this.checkHasCreateAdministrative();
            panelEntity.observables['__renderedState'].push(_this.ready);
            return _this;
        }
        GenerateKeyPanel.prototype.dispose = function () {
            _super.prototype.dispose.call(this);
            this.current = null;
            if (this.cb)
                this.cb.destroy();
        };
        GenerateKeyPanel.prototype.checkHasCreateAdministrative = function () {
            var _this = this;
            var dataService = mds.getDataService("membership");
            $.ajax({
                url: dataService.hostUrl + "/authorisation/haspermission/membership/ApiKey/CreateAdministrative",
                xhrFields: {
                    withCredentials: true
                }
            }).done(function () { return _this.hasCreateAdministrative(true); })
                .fail(function () { return _this.hasCreateAdministrative(false); })
                .always(function () { return _this.getAvailableUsers(); });
        };
        GenerateKeyPanel.prototype.getAvailableUsers = function () {
            var _this = this;
            var dataService = mds.getDataService("membership");
            var usernamesQuery = $.ajax({
                url: dataService.hostUrl + "/api/membership/query/Users",
                xhrFields: {
                    withCredentials: true
                }
            });
            usernamesQuery.done(function (result) { return _this.availableUsers(result.Records); });
            usernamesQuery.fail(this.handleFailure);
            usernamesQuery.always(function () { return _this.ready(true); });
        };
        GenerateKeyPanel.prototype.generate = function () {
            var _this = this;
            if (this.keyHasBeenGenerated())
                return;
            this.generatingKey(true);
            this.errorMessages([]);
            var url = sprintf("%s/ext/membership/user/keys/generate", mds.getDataService('membership').hostUrl);
            var visibleTo = [];
            var available = [];
            if (this.hasCreateAdministrative()) {
                visibleTo = this.current.model.observables['visibilityToRoles']();
                available = this.current.model.observables['availableRoles']();
            }
            var ownerRoles = available.filter(function (role) {
                return (visibleTo.filter(function (x) { return x == role.Name; }).length > 0);
            });
            var visibleToRoleIds = ownerRoles.map(function (role) { return (role.Id); });
            var username = this.username();
            if (username == null || username == '')
                username = options.username;
            var request = $.ajax({
                url: url,
                xhrFields: { withCredentials: true },
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    Label: this.keyLabel(),
                    Username: username,
                    VisibleToRoleIds: visibleToRoleIds
                })
            });
            request.done(function (key) {
                _this.apiKey(key);
                _this.keyHasBeenGenerated(true);
            });
            request.fail(this.handleFailure);
            request.always(function () { return _this.generatingKey(false); });
        };
        GenerateKeyPanel.prototype.close = function () {
            ko.postbox.publish(this.viewGrid + '-pop-panel');
        };
        return GenerateKeyPanel;
    }(basePanelEditor.BasePanelEditor));
    return GenerateKeyPanel;
});
