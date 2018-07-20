define(["require", "exports", "knockout", "loglevel", "options", "jquery"], function (require, exports, ko, log, options, $) {
    "use strict";
    var HelpModel = /** @class */ (function () {
        function HelpModel() {
            var _this = this;
            this.r = options.resources;
            this.window = null;
            this.showHelp = ko.observable(false);
            this.toggleHelpText = ko.pureComputed(function () {
                return (_this.showHelp())
                    ? _this.r['bwf_hide_help']
                    : _this.r['bwf_show_help'];
            });
            this.openHelp = function (product, module, item, data) {
                if (_this.window == null) {
                    log.debug('Creating help window');
                    $("<div id='bwf-helpWindow' />").appendTo(document.body).kendoWindow({
                        width: "640px",
                        height: "480px",
                        visible: false,
                        modal: false,
                        actions: ['Maximize', 'Close'],
                        iframe: true
                    });
                    _this.window = $("#bwf-helpWindow").data("kendoWindow");
                    _this.window.title(_this.r['bwf_help']);
                }
                ;
                var linkUrl = options.explorerHostUrl + "/helplink/" + product + "/" + module + "/" + item;
                log.debug('Help link url:', linkUrl);
                $.ajax({
                    xhrFields: {
                        withCredentials: true
                    },
                    url: linkUrl
                }).done(function (response) {
                    _this.window.refresh(response);
                    if (!_this.window.options.visible) {
                        _this.window.center();
                        _this.window.open();
                    }
                    ;
                });
            };
            this.toggleHelp = function () {
                if (_this.window != null && _this.window.options.visible) {
                    log.debug('Closing open help window');
                    self.window.close();
                }
                _this.showHelp(!_this.showHelp());
            };
            log.debug('Instantiating help with options:', options);
        }
        return HelpModel;
    }());
    var help = new HelpModel();
    return help;
});
