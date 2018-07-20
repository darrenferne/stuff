define(["require", "exports", "knockout", "options"], function (require, exports, ko, options) {
    "use strict";
    var titleSource = ko.observable(null);
    var title = ko.pureComputed(function () {
        var source = titleSource();
        if (ko.isObservable(source))
            return source();
        return options.resources['bwf_explorer'];
    });
    var titleViewModel = {
        pageTitle: title
    };
    ko.applyBindings(titleViewModel, document.getElementsByTagName("head")[0]);
    var Title = /** @class */ (function () {
        function Title() {
        }
        Title.prototype.setTitle = function (title) {
            if (ko.isObservable(title))
                titleSource(title);
            else if (title != null)
                titleSource(ko.observable(title));
            else
                titleSource(null);
        };
        Title.prototype.clearTitle = function () {
            titleSource(null);
        };
        return Title;
    }());
    var titleFacade = new Title();
    return titleFacade;
});
