define(["require", "exports", "knockout"], function (require, exports, ko) {
    "use strict";
    var BreadcrumbsModel = /** @class */ (function () {
        function BreadcrumbsModel(params) {
            this.breadcrumbs = params.breadcrumbs;
            this.disabled = params.disabled || ko.observable(false);
        }
        BreadcrumbsModel.prototype.getAttributes = function (item) {
            if (item.url == null)
                return {};
            return {
                href: item.url
            };
        };
        ;
        return BreadcrumbsModel;
    }());
    return BreadcrumbsModel;
});
