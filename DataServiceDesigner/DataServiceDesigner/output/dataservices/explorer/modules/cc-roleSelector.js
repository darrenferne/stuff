define(["require", "exports", "knockout", "options"], function (require, exports, knockout, options) {
    "use strict";
    var RoleSelector = /** @class */ (function () {
        function RoleSelector(params) {
            var _this = this;
            // dropdown template
            this.button = '<button type="button" class="multiselect dropdown-toggle panel-type-selector" data-toggle="dropdown"><span class="left-aligned-text flex-content multiselect-selected-text"></span> <i class="large-text flex-no-shrink fa fa-caret-down"></i></button>';
            this.availableRoles = knockout.observableArray([]);
            this.visibilityRoles = knockout.observableArray([]);
            this.editabilityRoles = knockout.observableArray([]);
            this.visibilityValidation = knockout.observable('');
            this.editabilityValidation = knockout.observable('');
            this.rendered = knockout.observable(false);
            this.loaded = knockout.observable(false);
            this.ready = knockout.pureComputed(function () {
                var rendered = _this.rendered();
                var loaded = _this.loaded();
                return rendered && loaded;
            });
            this.isCreate = params.model.state.isCreate;
            this.queryUrl = options.explorerHostUrl + '/api/Membership/Query/Roles';
            this.resources = options.resources;
            this.hasEditabilityToRoles = params.typeMetadata.hasEditabilityToRoles;
            this.hasVisibilityToRoles = params.typeMetadata.hasVisibilityToRoles;
            params.model.observables['__renderedState'].push(this.ready);
            params.model.observables['availableRoles'] = this.availableRoles;
            if (this.hasEditabilityToRoles) {
                var ev = knockout.pureComputed(function () { return _this.editabilityRoles().length > 0; });
                params.model.validations.add(ev);
                params.model.observables['editableByRoles'] = this.editabilityRoles;
            }
            if (this.hasVisibilityToRoles) {
                var ev = knockout.pureComputed(function () { return _this.visibilityRoles().length > 0; });
                params.model.validations.add(ev);
                params.model.observables['visibilityToRoles'] = this.visibilityRoles;
            }
            $.ajax({
                data: { q: "$orderby=Name" },
                url: this.queryUrl,
                xhrFields: {
                    withCredentials: true
                }
            }).done(function (response) {
                _this.availableRoles(response.Records);
            }).always(function () { return _this.loaded(true); });
        }
        return RoleSelector;
    }());
    return RoleSelector;
});
