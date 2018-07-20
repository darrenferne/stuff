define(["require", "exports", "knockout"], function (require, exports, ko) {
    "use strict";
    var GridAggregates = /** @class */ (function () {
        function GridAggregates(params) {
            this.aggregates = params.aggregates;
            this.aggregateRowsSelected = params.aggregateRowsSelected;
            this.columns = params.columns;
            // get an array where each element is simply its index, based on whatever 
            // property has the most aggregations. We don't care about the values here, 
            // we just need something we can iterate over in knockout
            this.iterations = params.aggregates().reduce(function (prev, curr) { return prev.length > curr.Values.length ? prev : curr.Values; }, [])
                .map(function (_, i) { return i; });
        }
        GridAggregates.prototype.aggregateRowClasses = function (index) {
            var _this = this;
            // we don't have a record for the aggregate row to hang a `selected` observable
            // off of, so lets generate a computed one for each row
            return ko.pureComputed(function () {
                if (_this.aggregateRowsSelected.indexOf(index) > -1)
                    return 'bwf-aggregate-row bwf-selected';
                return 'bwf-aggregate-row';
            });
        };
        GridAggregates.prototype.hasAggregation = function (column, index) {
            var aggregate = this.aggregates().filter(function (a) { return a.Key === column.path; })[0];
            if (aggregate == null)
                return false;
            return (index < aggregate.Values.length);
        };
        GridAggregates.prototype.getAggregateLabel = function (column, index) {
            var aggregate = this.aggregates()
                .filter(function (a) { return a.Key === column.path; })[0].Values[index];
            switch (aggregate.Function.toLowerCase()) {
                case "sum":
                    return 'Sum';
                case "min":
                    return 'Min';
                case "max":
                    return 'Max';
                case "count":
                    return 'Count';
                case "avg":
                case "average":
                    return 'Average';
                case "wgt avg":
                case "weighted average":
                    return 'Wgt. Avg';
            }
        };
        GridAggregates.prototype.getAggregateValue = function (column, index) {
            var aggregate = this.aggregates()
                .filter(function (a) { return a.Key === column.path; })[0].Values[index];
            if (aggregate.Function.toLocaleLowerCase() === "count") {
                return aggregate.TotalAggregate.toString();
            }
            else {
                return column.formatter(aggregate.TotalAggregate);
            }
        };
        return GridAggregates;
    }());
    return GridAggregates;
});
