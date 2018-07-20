define(["require", "exports", "options", "loglevel", "knockout"], function (require, exports, options, log, ko) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    function compareMenuItems(a, b) {
        if (a.Position < b.Position)
            return -1;
        if (a.Position > b.Position)
            return 1;
        var textA = a.Text.toUpperCase();
        var textB = b.Text.toUpperCase();
        if (textA < textB)
            return -1;
        if (textA > textB)
            return 1;
        return 0;
    }
    function orderSubmenu(subMenu) {
        subMenu.sort(compareMenuItems);
        subMenu.forEach(function (item) {
            if (item.Items.length > 0)
                orderSubmenu(item.Items);
        });
        return;
    }
    var Menu = /** @class */ (function () {
        function Menu() {
            var _this = this;
            this.menuItems = ko.observableArray([]);
            $.ajax({
                url: options.explorerHostUrl + '/menu',
                xhrFields: { withCredentials: true },
                dataType: 'json',
                contentType: 'application/json'
            }).done(function (result) {
                log.debug('Menus loaded', result);
                var menus = result.Items.slice();
                menus.sort(compareMenuItems);
                menus.forEach(function (item) {
                    if (item.Items.length > 0)
                        orderSubmenu(item.Items);
                    _this.menuItems.push(item);
                });
            }).fail(function (error) {
                log.error("Error loading menus: ", error);
            });
        }
        Menu.prototype.setupMenu = function () {
            var responsiveMenuElements = $('.responsive-menu .dropdown-submenu > a');
            responsiveMenuElements.submenupicker();
            $(".close-on-click").click(function () {
                $('.open').removeClass("open");
                if (!$('#toggle-responsive-button').hasClass("collapsed")) {
                    $('#toggle-responsive-button').addClass("collapsed");
                    $('#mobile-navbar').removeClass("in");
                }
            });
            ko.postbox.publish("bwf-menu-Loaded");
        };
        return Menu;
    }());
    exports.default = Menu;
});
