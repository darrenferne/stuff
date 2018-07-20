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
define(["require", "exports", "knockout", "modules/bwf-metadata", "modules/grid/cells/cell-base"], function (require, exports, ko, metadata, cell) {
    "use strict";
    var OperatorCell = /** @class */ (function (_super) {
        __extends(OperatorCell, _super);
        function OperatorCell(params) {
            var _this = _super.call(this, params, true) || this;
            _this.operatorOptions = [
                { DisplayName: '=', Value: '=' },
                { DisplayName: '!=', Value: '!=' }
            ];
            var propertyMetadata = metadata.getPropertyWithPrefix(params.typeMetadata.dataService, params.typeMetadata, params.row.record.Parameter);
            switch (propertyMetadata.type) {
                case 'string':
                    _this.operatorOptions.push({ DisplayName: 'like', Value: 'like' }, { DisplayName: 'not like', Value: 'notLike' });
                    break;
                case 'boolean':
                    // don't add any extra operators
                    break;
                default:
                    _this.operatorOptions.push({ DisplayName: '<=', Value: '<=' }, { DisplayName: '<', Value: '<' }, { DisplayName: '>', Value: '>' }, { DisplayName: '>=', Value: '>=' });
                    break;
            }
            _this.selectedText = ko.pureComputed(function () {
                var op = _this.recordValue();
                var matching = _this.operatorOptions.filter(function (x) { return x.Value === op; });
                // two characters worth of space to be clear of the dropdown arrow
                return matching.length > 0 ? matching[0].DisplayName + '__' : '';
            });
            _this.recordValue(params.row.record.Operator);
            _this.value = ko.computed({
                read: function () { return _this.recordValue(); },
                write: function (newValue) { return _this.recordValue(newValue); }
            });
            return _this;
        }
        OperatorCell.prototype.dispose = function () {
            this.value.dispose();
            _super.prototype.dispose.call(this);
        };
        return OperatorCell;
    }(cell.BwfWrapperCell));
    return OperatorCell;
});
