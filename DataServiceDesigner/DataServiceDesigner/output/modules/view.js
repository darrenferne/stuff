define(["require", "exports", "options", "sammy", "knockout", "modules/bwf-help", "modules/bwf-title", "loglevel", "modules/bwf-urlParser", "jquery", "text", "bootstrap", "knockout-kendo", "knockout-postbox"], function (require, exports, options, Sammy, ko, help, title, log, urlParser, $) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var ViewViewModel = /** @class */ (function () {
        function ViewViewModel() {
            var _this = this;
            this.app = null;
            this.viewGridId = 'viewOpen';
            this.currentView = ko.observable(null);
            this.viewName = ko.pureComputed(function () {
                var view = _this.currentView();
                return view != null
                    ? view.viewName
                    : '';
            });
            this.previousUrl = ko.pureComputed(function () {
                var view = _this.currentView();
                return view != null
                    ? view.previousUrl
                    : '';
            });
            this.explorerHostUrl = options.explorerHostUrl;
            this.toggleHelpText = help.toggleHelpText;
            this.toggleHelp = help.toggleHelp;
            this.showHelp = help.showHelp;
            this.openHelp = help.openHelp;
            this.pageTitle = ko.pureComputed(function () { return _this.viewName() + " | " + options.resources['bwf_explorer']; });
            this.resize = function () {
                var height = $(window).height()
                    - $('#header').height()
                    - $('#customerHeader').height()
                    - $('#footer').height()
                    - $('#customerFooter').height();
                var width = $('#viewGrid').width();
                ko.postbox.publish(_this.viewGridId + '-viewGridResized', { height: height, width: width });
            };
            this.loadView = function (viewParameters) {
                if (_this.viewName() === viewParameters.viewName)
                    return;
                _this.currentView(viewParameters);
                ko.postbox.publish(_this.viewGridId + '-loadView', viewParameters);
            };
            this.changeViewName = function (viewName) { return _this.app.runRoute('get', "/view/#open/" + viewName); };
            this.handlers = {
                viewOpen: function (context) {
                    if (context.params.viewName === _this.viewName())
                        return;
                    var url = urlParser.parseUrl(window.location.hash);
                    var viewParams = {
                        viewName: context.params.viewName,
                        previousUrl: window.location.href,
                        urlParameters: url.urlParameters,
                        urlFilteredBy: url.urlFilteredby
                    };
                    _this.loadView(viewParams);
                },
                defaultView: function (context) {
                    var baseUrl = _this.explorerHostUrl + "/view/getDefaultView/" + context.params.dataService + "/" + context.params.baseType;
                    var request = $.ajax({
                        url: baseUrl,
                        xhrFields: {
                            withCredentials: true
                        }
                    });
                    request.done(function (viewName) {
                        var index = window.location.hash.indexOf('?parameters=');
                        var params = '';
                        if (index !== -1)
                            params = window.location.hash.substring(index);
                        else {
                            var filteredByPosition = location.hash.toLowerCase().indexOf('?filteredby=');
                            if (filteredByPosition !== -1)
                                params = location.hash.substring(filteredByPosition);
                        }
                        if (_this.previousUrl() !== '')
                            history.replaceState(null, _this.pageTitle(), _this.previousUrl());
                        context.redirect("#open/" + (viewName + params));
                    });
                    request.fail(function (response) {
                        log.error('Error getting default view:', response);
                        var redirectUrl = response.status === 401
                            ? "/authentication/login?returnUrl=/view/" + location.hash
                            : "/error/" + response.responseJSON.message;
                        debugger;
                        window.location.assign(redirectUrl);
                    });
                }
            };
            this.viewGridModuleRendered = function () {
                var self = _this;
                _this.app = Sammy(function () {
                    this.get('/view/#open/:viewName', self.handlers.viewOpen);
                    this.get('/view/#default/:dataService/:baseType', self.handlers.defaultView);
                }).run();
            };
            title.setTitle(this.pageTitle);
            $(window).resize(this.resize);
            ko.postbox.subscribe(this.viewGridId + '-changeViewName', this.changeViewName);
            ko.postbox.subscribe(this.viewGridId + '-resizeRequired', this.resize);
        }
        return ViewViewModel;
    }());
    window.onbeforeunload = function () {
        log.debug('onbeforeunload event fired');
        ko.postbox.publish('onbeforeunload');
    };
    var viewModel = new ViewViewModel();
    ko.applyBindings(viewModel, document.getElementById('content'));
});
