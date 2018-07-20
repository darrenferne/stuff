define(["require", "exports", "jquery", "options", "loglevel", "knockout", "knockout-postbox", "scripts/bootstrap-submenu"], function (require, exports, $, options, log, ko) {
    "use strict";
    var load = function () {
        log.debug('Loading headers and footers with options:', options);
        var cookies = document.cookie;
        var disableChrome = cookies.toLowerCase().indexOf('bwf-disable-view-dressing=true') > -1;
        if (window.location.href.indexOf('$disableDressing') > -1)
            disableChrome = true;
        if (disableChrome) {
            var customerHeader = document.getElementById('customerHeader');
            var header = document.getElementById('header');
            var customerFooter = document.getElementById('customerFooter');
            var footer = document.getElementById('footer');
            if (customerHeader)
                customerHeader.remove();
            if (header)
                header.remove();
            if (customerFooter)
                customerFooter.remove();
            if (footer)
                footer.remove();
            $('#content').css('padding', ' 10px 0');
            return;
        }
        $('#content').css('padding', ' 69px 0 20px 0');
        $.ajax({
            xhrFields: {
                withCredentials: true
            },
            url: options.explorerHostUrl + "/customer/config"
        }).done(function (response) {
            log.debug('Customer config:', response);
            var headerReq, footerReq, customerHeaderReq, customerFooterReq;
            if (response.ReplaceExistingHeader) {
                $('#header').css('height', '0px');
                $('#content').css('padding-top', '0px');
            }
            else {
                headerReq = $.ajax({
                    xhrFields: {
                        withCredentials: true
                    },
                    url: options.explorerHostUrl + "/header"
                }).done(function (response) {
                    log.debug('Header loaded');
                    $("#header").html(response);
                    var responsiveMenuElements = $('.responsive-menu .dropdown-submenu > a');
                    responsiveMenuElements.submenupicker();
                });
            }
            ;
            if (response.HeaderHeight > 0) {
                $('#header').css('top', response.HeaderHeight);
                $('#customerHeader').css('height', response.HeaderHeight + 'px');
                var contentPaddingTop = parseInt($('#content').css('padding-top'), 10) + response.HeaderHeight;
                $('#content').css('padding-top', contentPaddingTop + 'px');
                customerHeaderReq = $.ajax({
                    xhrFields: {
                        withCredentials: true
                    },
                    url: options.explorerHostUrl + "/customer/header"
                }).done(function (response) {
                    log.debug('Customer header loaded');
                    $("#customerHeader").html(response);
                });
            }
            ;
            if (response.ReplaceExistingFooter) {
                $('#footer').css('height', '0px');
                $('#content').css('padding-bottom', '0px');
            }
            else {
                footerReq = $.ajax({
                    xhrFields: {
                        withCredentials: true
                    },
                    url: options.explorerHostUrl + "/footer"
                }).done(function (response) {
                    log.debug('Footer loaded');
                    $("#footer").html(response);
                });
            }
            ;
            if (response.FooterHeight > 0) {
                $('#footer').css('bottom', response.FooterHeight);
                $('#customerFooter').css('height', response.FooterHeight + 'px');
                var contentPaddingBottom = parseInt($('#content').css('padding-bottom'), 10) + response.FooterHeight;
                $('#content').css('padding-bottom', contentPaddingBottom + 'px');
                customerFooterReq = $.ajax({
                    xhrFields: {
                        withCredentials: true
                    },
                    url: options.explorerHostUrl + "/customer/footer"
                }).done(function (response) {
                    log.debug('Customer footer loaded');
                    $("#customerFooter").html(response);
                });
            }
            ;
            $.when(headerReq, customerHeaderReq, footerReq, customerFooterReq).done(function () {
                var responsiveMenuElements = $('.responsive-menu .dropdown-submenu > a');
                $(".close-on-click").click(function () {
                    $('.open').removeClass("open").attr("aria-expanded", "false");
                    if (!$('#toggle-responsive-button').hasClass("collapsed")) {
                        $('#toggle-responsive-button').addClass("collapsed");
                        $('#mobile-navbar').removeClass("in");
                    }
                });
                ko.postbox.publish("bwf-headersAndFooters-Loaded");
            });
        });
    };
    return { load: load };
});
