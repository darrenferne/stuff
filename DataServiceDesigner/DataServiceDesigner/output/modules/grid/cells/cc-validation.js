define(["require", "exports", "knockout", "knockout-kendo"], function (require, exports, ko, kk) {
    "use strict";
    var knockoutKendo = kk;
    var ValidationCell = /** @class */ (function () {
        function ValidationCell(config) {
            var _this = this;
            this.validationMessages = ko.observableArray([]);
            this.renderedTemplate = ko.computed(function () {
                var validationMessages = _this.validationMessages()
                    .filter(function (vm) { return vm.messages().length > 0; });
                var data = validationMessages.map(function (vm) {
                    return {
                        property: vm.property,
                        message: vm.messages().join(', ')
                    };
                });
                return ValidationCell.unRenderedTemplate(data);
            });
            this.validationErrors = ko.pureComputed(function () {
                return _this.validationMessages().reduce(function (prev, curr) { return prev + curr.messages().length; }, 0);
            });
            this.visible = ko.pureComputed(function () { return _this.validationErrors() > 0; });
            this.columns = config.columns;
            this.id = 'validation-cell-' + ValidationCell.validationId++;
            this.row = config.row;
            this.validationMessages(this.columns().map(function (column) {
                return {
                    property: column.displayName(),
                    messages: _this.row.values[column.path].validationMessages
                };
            }));
            this.validationMessages.push({ property: 'Row', messages: this.row.modelValidations });
        }
        ValidationCell.prototype.dispose = function () {
            this.validationErrors.dispose();
            var tt = $(document.getElementById(this.id)).data('kendoTooltip');
            if (tt)
                tt.destroy();
        };
        ValidationCell.prototype.onRendered = function () {
            var tt = $(document.getElementById(this.id)).data('kendoTooltip');
            tt.bind('show', function (e) { this.popup.element.addClass('bwf-error-tooltip'); });
        };
        ValidationCell.unRenderedTemplate = kendo.template('<table><tbody># for (var i = 0; i < vm.length; i++) { #<tr><td>#= vm[i].property #</td><td>#= vm[i].message #</td></tr># } #</tbody></table>', { useWithBlock: false, paramName: 'vm' });
        ValidationCell.validationId = 20000;
        return ValidationCell;
    }());
    return ValidationCell;
});
