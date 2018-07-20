define(["require", "exports", "knockout", "options", "scripts/sprintf", "modules/bwf-metadata"], function (require, exports, knockout, options, sprintf, metadataService) {
    "use strict";
    var IsLocked = /** @class */ (function () {
        function IsLocked(params) {
            var _this = this;
            this.displayName = knockout.observable('');
            this.lockHeader = options.resources['bwf_locked_single'];
            this.lockWarning = options.resources['bwf_lock_modify_warning'];
            this.rendered = knockout.observable(false);
            this.checkedForLock = knockout.observable(false);
            this.checkedForPermission = knockout.observable(false);
            this.ready = knockout.pureComputed(function () {
                var rendered = _this.rendered();
                var checkedForLock = _this.checkedForLock();
                var checkedForPermission = _this.checkedForPermission();
                return rendered && checkedForLock && checkedForPermission;
            });
            this.displayName(params.typeMetadata.displayName);
            var ds = metadataService.getDataService(params.model.state.dataService);
            params.model.observables['__renderedState'].push(this.ready);
            this.IsLocked = params.model.observables['isLocked'];
            this.HasIgnoreLock = params.model.observables['hasIgnoreLock'];
            var record = params.model.record;
            var isLockedUrl = sprintf.sprintf("%s/%s/%s/islocked", ds.url, params.model.state.typeName, record.Id);
            var hasIgnoreLock = sprintf.sprintf("%s/authorisation/haspermission/%s/%s/IgnoreLock", ds.hostUrl, ds.name, params.permissions.lockPermissionType || params.model.state.typeName);
            $.ajax({
                url: isLockedUrl,
                xhrFields: { withCredentials: true },
                type: "GET"
            }).done(function (result) { return _this.IsLocked(result); })
                .always(function () { return _this.checkedForLock(true); });
            $.ajax({
                url: hasIgnoreLock,
                xhrFields: { withCredentials: true },
                type: "GET"
            }).done(function (result) { return _this.HasIgnoreLock(result); })
                .fail(function () { return _this.HasIgnoreLock(false); })
                .always(function () { return _this.checkedForPermission(true); });
        }
        return IsLocked;
    }());
    return IsLocked;
});
