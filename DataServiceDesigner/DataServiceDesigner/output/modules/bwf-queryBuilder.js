define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var OrderByDirection;
    (function (OrderByDirection) {
        OrderByDirection[OrderByDirection["Ascending"] = 0] = "Ascending";
        OrderByDirection[OrderByDirection["Descending"] = 1] = "Descending";
    })(OrderByDirection || (OrderByDirection = {}));
    var QueryBuilder = /** @class */ (function () {
        function QueryBuilder(type) {
            this.type = type;
            if (typeof type !== 'string')
                throw "A type must be provided for the query builder";
            this.expandProperties = [];
            this.filters = [];
            this.orderBys = [];
            this.aggregates = [];
            this.groupBys = [];
            this.groupByAggregates = [];
            this.top = null;
            this.skip = null;
            this.distinctBy = null;
            this.includeExpandsForEdit = null;
            this.excludeTotalCount = null;
        }
        QueryBuilder.prototype.Expand = function () {
            var _this = this;
            var properties = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                properties[_i] = arguments[_i];
            }
            properties.forEach(function (property) {
                if (_this.expandProperties.indexOf(property) === -1)
                    _this.expandProperties.push(property);
            });
            return this;
        };
        QueryBuilder.prototype.IncludeExpandsForEditing = function () {
            this.includeExpandsForEdit = true;
            return this;
        };
        QueryBuilder.prototype.ExcludeTotalCount = function () {
            this.excludeTotalCount = true;
            return this;
        };
        QueryBuilder.prototype.OrderBy = function (property, ascending) {
            if (ascending === void 0) { ascending = true; }
            var direction = ascending ? OrderByDirection.Ascending : OrderByDirection.Descending;
            if (this.orderBys.filter(function (x) { return x.Direction == direction && x.Property == property; }).length > 0)
                this.orderBys = this.orderBys.filter(function (x) { return x.Direction == direction && x.Property == property; });
            this.orderBys.push({
                Property: property,
                Direction: direction
            });
            return this;
        };
        QueryBuilder.prototype.DistinctBy = function (property) {
            this.distinctBy = property;
            return this;
        };
        QueryBuilder.prototype.Filter = function (callback) {
            var filterBuilder = new FilterBuilder();
            callback(filterBuilder);
            this.filters.push(filterBuilder.getFilter());
            return this;
        };
        ;
        QueryBuilder.prototype.Top = function (maxRecords) {
            this.top = maxRecords;
            return this;
        };
        QueryBuilder.prototype.Skip = function (toSkip) {
            this.skip = toSkip;
            return this;
        };
        QueryBuilder.prototype.GroupBy = function (property) {
            this.groupBys.push(property);
            return this;
        };
        QueryBuilder.prototype.Aggregate = function (builder) {
            var aggregateBuilder = new AggregateBuilder();
            builder(aggregateBuilder);
            this.aggregates.push(aggregateBuilder.getAggregateString());
            return this;
        };
        QueryBuilder.prototype.GroupByAggregate = function (builder) {
            var aggregateBuilder = new AggregateBuilder();
            builder(aggregateBuilder);
            this.groupByAggregates.push(aggregateBuilder.getAggregateString());
            return this;
        };
        QueryBuilder.prototype.buildQuery = function (urlEncode) {
            if (urlEncode === void 0) { urlEncode = false; }
            var baseQuery = this.type + "s";
            var queryParts = [];
            if (this.orderBys.length > 0)
                queryParts.push("$orderby=" + this.orderBys.map(function (x) {
                    return "" + x.Property + (x.Direction == OrderByDirection.Descending ? ' desc' : '');
                }));
            if (this.expandProperties.length > 0)
                queryParts.push("$expand=" + this.expandProperties.join(","));
            if (this.filters.length > 0)
                queryParts.push("$filter=" + this.filters.join(" and "));
            if (this.aggregates.length > 0)
                queryParts.push("$aggregate=" + this.aggregates.join(","));
            if (this.groupByAggregates.length > 0)
                queryParts.push("$groupbyaggregate=" + this.groupByAggregates.join(","));
            if (this.groupBys.length > 0)
                queryParts.push("$groupby=" + this.groupBys.join(","));
            if (this.distinctBy != null)
                queryParts.push("$distinctby=" + this.distinctBy);
            if (this.excludeTotalCount != null)
                queryParts.push("$includetotalcount=false");
            if (this.includeExpandsForEdit)
                queryParts.push("$foredit=true");
            if (this.skip != null)
                queryParts.push("$skip=" + this.skip);
            if (this.top != null)
                queryParts.push("$top=" + this.top);
            var toReturn = queryParts.length === 0 ? baseQuery : baseQuery + "?" + queryParts.join("&");
            if (urlEncode)
                return encodeURIComponent(toReturn);
            return toReturn;
        };
        QueryBuilder.prototype.GetQuery = function () {
            return this.buildQuery();
        };
        return QueryBuilder;
    }());
    exports.QueryBuilder = QueryBuilder;
    var FilterBuilder = /** @class */ (function () {
        function FilterBuilder() {
            this.filterParts = [];
            this.resetLast();
        }
        FilterBuilder.prototype.resetLast = function () {
            this.lastWasAnd = false;
            this.lastWasOr = false;
            this.lastWasProperty = false;
        };
        FilterBuilder.prototype.Property = function (property) {
            if (this.lastWasProperty)
                throw "Cannot call property twice in a row";
            this.resetLast();
            this.lastWasProperty = true;
            return new FilterOperator(this, property);
        };
        FilterBuilder.prototype.And = function (subFilterSetup) {
            if (subFilterSetup != null) {
                this.filterParts.push(' and (');
                var subFilterBuilder = new FilterBuilder();
                subFilterSetup(subFilterBuilder);
                this.filterParts.push(subFilterBuilder.getFilter());
                this.filterParts.push(')');
            }
            else {
                if (this.lastWasAnd || this.lastWasOr)
                    throw "Cannot call 'and' after 'and' or 'or'";
                this.resetLast();
                this.lastWasAnd = true;
                this.filterParts.push(' and ');
            }
            return this;
        };
        FilterBuilder.prototype.Or = function (subFilterSetup) {
            if (subFilterSetup != null) {
                this.filterParts.push(' or (');
                var subFilterBuilder = new FilterBuilder();
                subFilterSetup(subFilterBuilder);
                this.filterParts.push(subFilterBuilder.getFilter());
                this.filterParts.push(')');
            }
            else {
                if (this.lastWasAnd || this.lastWasOr)
                    throw "Cannot call 'or' after 'and' or 'or'";
                this.resetLast();
                this.lastWasOr = true;
                this.filterParts.push(' or ');
            }
            return this;
        };
        FilterBuilder.prototype.Parenthesis = function (subFilterSetup) {
            this.filterParts.push('(');
            var subFilterBuilder = new FilterBuilder();
            subFilterSetup(subFilterBuilder);
            this.filterParts.push(subFilterBuilder.getFilter());
            this.filterParts.push(')');
            return this;
        };
        FilterBuilder.prototype.getFilter = function () {
            return this.filterParts.join("");
        };
        return FilterBuilder;
    }());
    exports.FilterBuilder = FilterBuilder;
    var FilterOperator = /** @class */ (function () {
        function FilterOperator(fb, property) {
            this.filterBuilder = fb;
            this.property = property;
        }
        FilterOperator.prototype.getFilterParameterString = function (item, allowNulls) {
            if (allowNulls === void 0) { allowNulls = true; }
            var isNull = item === null || item === void 0;
            if (!allowNulls && isNull)
                throw "Null or empty value is not allowed";
            if (typeof item === 'boolean')
                return item === true ? 'true' : 'false';
            else if (typeof item === 'string') {
                var escaped = encodeURIComponent(item.replace("\\", "\\\\")).replace("'", "%5C%27");
                return "'" + escaped + "'";
            }
            else if (typeof item === 'number')
                return "" + item;
            else if (item instanceof Date)
                throw "Dates are no longer supported as a parameter type. Use the Date or DateTime specific version of the function with an ISO 8601 formatted string.";
            else if (isNull)
                return 'null';
            else
                throw "Unknown or invalid item passed into filter builder";
        };
        FilterOperator.prototype.getFilterParameterStringDate = function (item, allowNulls) {
            if (allowNulls === void 0) { allowNulls = true; }
            var isNull = item === null || item === void 0;
            if (!allowNulls && isNull)
                throw "Null or empty value is not allowed";
            if (typeof item === 'string')
                return "date(" + item + ")";
            else if (isNull)
                return 'null';
            else
                throw "Unknown or invalid item passed into filter builder";
        };
        FilterOperator.prototype.getFilterParameterStringDateTime = function (item, allowNulls) {
            if (allowNulls === void 0) { allowNulls = true; }
            var isNull = item === null || item === void 0;
            if (!allowNulls && isNull)
                throw "Null or empty value is not allowed";
            if (typeof item === 'string')
                return "datetime(" + item + ")";
            else if (isNull)
                return 'null';
            else
                throw "Unknown or invalid item passed into filter builder";
        };
        FilterOperator.prototype.Like = function (item) {
            var toAppend = this.getFilterParameterString(item, false);
            this.filterBuilder.filterParts.push(this.property + " like " + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.NotLike = function (item) {
            var toAppend = this.getFilterParameterString(item, false);
            this.filterBuilder.filterParts.push(this.property + " notlike " + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.GreaterThan = function (item) {
            var toAppend = this.getFilterParameterString(item, false);
            this.filterBuilder.filterParts.push(this.property + ">" + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.GreaterThanDate = function (item) {
            var toAppend = this.getFilterParameterStringDate(item, false);
            this.filterBuilder.filterParts.push(this.property + ">" + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.GreaterThanDateTime = function (item) {
            var toAppend = this.getFilterParameterStringDateTime(item, false);
            this.filterBuilder.filterParts.push(this.property + ">" + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.GreaterThanEqualTo = function (item) {
            var toAppend = this.getFilterParameterString(item);
            this.filterBuilder.filterParts.push(this.property + ">=" + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.GreaterThanEqualToDate = function (item) {
            var toAppend = this.getFilterParameterStringDate(item);
            this.filterBuilder.filterParts.push(this.property + ">=" + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.GreaterThanEqualToDateTime = function (item) {
            var toAppend = this.getFilterParameterStringDateTime(item);
            this.filterBuilder.filterParts.push(this.property + ">=" + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.LessThan = function (item) {
            var toAppend = this.getFilterParameterString(item, false);
            this.filterBuilder.filterParts.push(this.property + "<" + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.LessThanDate = function (item) {
            var toAppend = this.getFilterParameterStringDate(item, false);
            this.filterBuilder.filterParts.push(this.property + "<" + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.LessThanDateTime = function (item) {
            var toAppend = this.getFilterParameterStringDateTime(item, false);
            this.filterBuilder.filterParts.push(this.property + "<" + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.LessThanEqualTo = function (item) {
            var toAppend = this.getFilterParameterString(item);
            this.filterBuilder.filterParts.push(this.property + "<=" + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.LessThanEqualToDate = function (item) {
            var toAppend = this.getFilterParameterStringDate(item);
            this.filterBuilder.filterParts.push(this.property + "<=" + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.LessThanEqualToDateTime = function (item) {
            var toAppend = this.getFilterParameterStringDateTime(item);
            this.filterBuilder.filterParts.push(this.property + "<=" + toAppend);
            return this.filterBuilder;
        };
        FilterOperator.prototype.EqualTo = function (item) {
            if (item === null || item === void 0) {
                this.IsNull();
            }
            else {
                var toAppend = this.getFilterParameterString(item);
                this.filterBuilder.filterParts.push(this.property + "=" + toAppend);
            }
            return this.filterBuilder;
        };
        FilterOperator.prototype.EqualToDate = function (item) {
            if (item === null || item === void 0) {
                this.IsNull();
            }
            else {
                var toAppend = this.getFilterParameterStringDate(item);
                this.filterBuilder.filterParts.push(this.property + "=" + toAppend);
            }
            return this.filterBuilder;
        };
        FilterOperator.prototype.EqualToDateTime = function (item) {
            if (item === null || item === void 0) {
                this.IsNull();
            }
            else {
                var toAppend = this.getFilterParameterStringDateTime(item);
                this.filterBuilder.filterParts.push(this.property + "=" + toAppend);
            }
            return this.filterBuilder;
        };
        FilterOperator.prototype.NotEqualTo = function (item) {
            if (item === null || item === void 0) {
                this.IsNotNull();
            }
            else {
                var toAppend = this.getFilterParameterString(item);
                this.filterBuilder.filterParts.push(this.property + "!=" + toAppend);
            }
            return this.filterBuilder;
        };
        FilterOperator.prototype.NotEqualToDate = function (item) {
            if (item === null || item === void 0) {
                this.IsNotNull();
            }
            else {
                var toAppend = this.getFilterParameterStringDate(item);
                this.filterBuilder.filterParts.push(this.property + "!=" + toAppend);
            }
            return this.filterBuilder;
        };
        FilterOperator.prototype.NotEqualToDateTime = function (item) {
            if (item === null || item === void 0) {
                this.IsNotNull();
            }
            else {
                var toAppend = this.getFilterParameterStringDateTime(item);
                this.filterBuilder.filterParts.push(this.property + "!=" + toAppend);
            }
            return this.filterBuilder;
        };
        FilterOperator.prototype.IsNull = function () {
            this.filterBuilder.filterParts.push(this.property + " isnull");
            return this.filterBuilder;
        };
        FilterOperator.prototype.IsNotNull = function () {
            this.filterBuilder.filterParts.push(this.property + " isnotnull");
            return this.filterBuilder;
        };
        FilterOperator.prototype.In = function () {
            var _this = this;
            var items = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                items[_i] = arguments[_i];
            }
            var itemsString = this.property + " in [" + items.map(function (i) { return _this.getFilterParameterString(i); }).join(";") + "]";
            this.filterBuilder.filterParts.push(itemsString);
            return this.filterBuilder;
        };
        return FilterOperator;
    }());
    exports.FilterOperator = FilterOperator;
    var AggregateBuilder = /** @class */ (function () {
        function AggregateBuilder() {
            this.aggregates = [];
        }
        AggregateBuilder.prototype.getError = function (name, numberOfPropertiesRequired) {
            if (numberOfPropertiesRequired === void 0) { numberOfPropertiesRequired = 1; }
            var propertyString = numberOfPropertiesRequired > 1 ? "properties" : "property";
            return "'" + name + "' requires " + numberOfPropertiesRequired + " " + propertyString + " to aggregate";
        };
        AggregateBuilder.prototype.isValidProperty = function (property) {
            return typeof property === 'string' && property.length > 0;
        };
        AggregateBuilder.prototype.Sum = function (property) {
            if (!this.isValidProperty(property))
                throw this.getError("Sum");
            this.aggregates.push("sum(" + property + ")");
            return this;
        };
        AggregateBuilder.prototype.Min = function (property) {
            if (!this.isValidProperty(property))
                throw this.getError("Min");
            this.aggregates.push("min(" + property + ")");
            return this;
        };
        AggregateBuilder.prototype.Max = function (property) {
            if (!this.isValidProperty(property))
                throw this.getError("Max");
            this.aggregates.push("max(" + property + ")");
            return this;
        };
        AggregateBuilder.prototype.Average = function (property) {
            if (!this.isValidProperty(property))
                throw this.getError("Average");
            this.aggregates.push("average(" + property + ")");
            return this;
        };
        AggregateBuilder.prototype.Count = function (property) {
            if (!this.isValidProperty(property))
                throw this.getError("Count");
            this.aggregates.push("count(" + property + ")");
            return this;
        };
        AggregateBuilder.prototype.WeightedAverage = function (property1, property2) {
            if (!this.isValidProperty(property1) || !this.isValidProperty(property2))
                throw this.getError("WeightedAverage", 2);
            this.aggregates.push("weightedaverage(" + property1 + ";" + property2 + ")");
            return this;
        };
        AggregateBuilder.prototype.getAggregateString = function () {
            return this.aggregates.join(",");
        };
        return AggregateBuilder;
    }());
});
