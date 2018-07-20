"use strict";
define(["knockout", "linqjs", "modules/bwf-idgen", "bootstrap", "knockout-kendo", 'knockout-postbox'], function (ko, Enumerable, idgen) {
    var notificationsViewModel = function (options) {
        var self = this;
        if (typeof options == "string" || options instanceof String)
            options = { instance: options, divCss: "well well-sm" };
        self.divCss = options.divCss;
        self.errorsToolbarTemplate = kendo.template('<div class="toolbar"><button type="button" class="btn btn-xs" data-bind="click: errorsClearAll">Clear All</button></div>');
        self.warningsToolbarTemplate = kendo.template('<div class="toolbar"><button type="button" class="btn btn-xs" data-bind="click: warningsClearAll">Clear All</button></div>');
        self.infosToolbarTemplate = kendo.template('<div class="toolbar"><button type="button" class="btn btn-xs" data-bind="click: infosClearAll">Clear All</button></div>');
        self.isErrorsOpen = ko.observable(false);
        self.isWarningsOpen = ko.observable(false);
        self.isInfosOpen = ko.observable(false);
        self.errors = ko.observableArray([]);
        self.warnings = ko.observableArray([]);
        self.infos = ko.observableArray([]);
        self.notificationsExist = ko.computed(function () {
            return (self.errors().length + self.warnings().length + self.infos().length) != 0;
        });
        self.lastPublishedNotificationsExist = null;
        self.notificationsExist.subscribe(function (newValue) {
            if (newValue !== self.lastPublishedNotificationsExist) {
                self.lastPublishedNotificationsExist = newValue;
                ko.postbox.publish(options.instance + ".notificationsExist", newValue);
            }
        });
        self.lastNotification = ko.computed(function () {
            var last = Enumerable.from(self.errors()).firstOrDefault();
            if (last === null)
                last = Enumerable.from(self.warnings()).firstOrDefault();
            if (last === null)
                last = Enumerable.from(self.infos()).firstOrDefault();
            if (last === null)
                return { level: "", message: "" };
            return { level: last.level, message: '(' + kendo.toString(last.received, 'HH:mm:ss') + ') ' + last.message };
        });
        var clearRow = function (context, e, existing) {
            var rowId = context.dataItem($(e.currentTarget).closest("tr")).id;
            existing.remove(function (row) { return row.id === rowId; });
        };
        self.errorsClick = function () {
            self.isErrorsOpen(!self.isErrorsOpen());
        };
        self.warningsClick = function () {
            self.isWarningsOpen(!self.isWarningsOpen());
        };
        self.infosClick = function () {
            self.isInfosOpen(!self.isInfosOpen());
        };
        self.errorsClearAll = function () {
            self.errors.removeAll();
            self.isErrorsOpen(false);
        };
        self.warningsClearAll = function () {
            self.warnings.removeAll();
            self.isWarningsOpen(false);
        };
        self.infosClearAll = function () {
            self.infos.removeAll();
            self.isInfosOpen(false);
        };
        self.clearAll = function () {
            self.errorsClearAll();
            self.warningsClearAll();
            self.infosClearAll();
        };
        self.errorsClearRow = function (e) {
            clearRow(this, e, self.errors);
        };
        self.warningsClearRow = function (e) {
            clearRow(this, e, self.warnings);
        };
        self.infosClearRow = function (e) {
            clearRow(this, e, self.infos);
        };
        self.errorsTotal = ko.computed(function () {
            return self.errors().length;
        });
        self.warningsTotal = ko.computed(function () {
            return self.warnings().length;
        });
        self.infosTotal = ko.computed(function () {
            return self.infos().length;
        });
        var updateNotifications = function (notifications, existing, level) {
            var notifications = Enumerable.from(notifications).distinct();
            var existingErrors = Enumerable.from(existing());
            var toRemove = existingErrors.where(function (x) { return notifications.contains(x.message); }).toArray();
            existing.removeAll(toRemove);
            var transformed = notifications.select(function (x) { return { "id": idgen.nextId(), "received": new Date(), "message": x, "level": level }; }).toArray();
            ko.utils.arrayPushAll(existing(), transformed);
            existing.sort(function (left, right) { return left.id > right.id ? -1 : 1; });
            existing.valueHasMutated();
        };
        ko.postbox.subscribe(options.instance + ".clearAll", function () {
            self.clearAll();
        });
        ko.postbox.subscribe(options.instance + ".addError", function (notifications) {
            updateNotifications(notifications, self.errors, "Error");
        });
        ko.postbox.subscribe(options.instance + ".addWarning", function (notifications) {
            updateNotifications(notifications, self.warnings, "Warning");
        });
        ko.postbox.subscribe(options.instance + ".addInfo", function (notifications) {
            updateNotifications(notifications, self.infos, "Info");
        });
    };
    return notificationsViewModel;
});
