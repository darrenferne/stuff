define(["require", "exports", "knockout", "jquery", "loglevel", "options"], function (require, exports, ko, $, log, bwfOptions) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    function observableFrom(value, def) {
        if (ko.isObservable(value))
            return value;
        var v = (value == null && def != null
            ? def
            : value);
        return ko.observable(v);
    }
    function getOptions(options) {
        if (typeof options === 'undefined' || options == null)
            return null;
        var required = options.required || false;
        var canSetNull = (options.canSetNull === null || options.canSetNull === undefined) ? true : options.canSetNull;
        return {
            canSetNull: canSetNull,
            disable: observableFrom(options.disable, false),
            required: required,
            filterEnabled: options.filterEnabled,
            mobile: observableFrom(options.mobile, false)
        };
    }
    function setDisabled(element, disabled) {
        if (disabled) {
            element.addClass("disabled");
            element.attr('disabled', 'disabled');
        }
        else {
            element.removeAttr('disabled');
            element.removeClass("disabled");
        }
        ko["tasks"].schedule(function () { return element.selectpicker('refresh'); });
    }
    ;
    function setMobile(element, isMobile) {
        if (isMobile) {
            element.selectpicker('mobile');
        }
        else {
            element.removeClass('mobile-device');
        }
    }
    if (!ko.bindingHandlers.bootstrapSelect) {
        ko.bindingHandlers.bootstrapSelect = {
            after: ["value", "options", "valueAllowUnset"],
            init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                var $selectElement = $(element);
                if ($selectElement.is('select')) {
                    var selectPickerOptions = valueAccessor();
                    if (typeof selectPickerOptions === 'undefined' && selectPickerOptions === null) {
                        log.error("No options were passed into selectPicker binding");
                        return;
                    }
                    var subscriptions = [];
                    var selectSettings = {
                        liveSearch: false,
                        liveSearchNormalize: true,
                        liveSearchPlaceholder: bwfOptions.resources["bwf_search"],
                        size: 8,
                        width: '100%',
                        mobile: false
                    };
                    var prefs = getOptions(selectPickerOptions);
                    var value = allBindingsAccessor.get("value");
                    var optionsArray = allBindingsAccessor.get("options");
                    var optionsDisplayProperty = allBindingsAccessor.get('optionsText');
                    var optionsValueProperty = allBindingsAccessor.get('optionsValue');
                    var noOptionsAvailable = optionsArray().length == 0;
                    if (!prefs.canSetNull && !value() && !noOptionsAvailable)
                        value(optionsArray()[0][optionsValueProperty]);
                    // Static options
                    if (prefs.required === true)
                        $selectElement.attr('required', "true");
                    if (prefs.filterEnabled)
                        selectSettings.liveSearch = true;
                    if (prefs.mobile)
                        selectSettings.mobile = true;
                    // initiate selectpicker
                    $selectElement.addClass('selectpicker').selectpicker(selectSettings);
                    // Dynamic options
                    if (prefs.disable)
                        subscriptions.push(prefs.disable.subscribe(function (disabled) { return setDisabled($selectElement, disabled); }));
                    if (prefs.mobile) {
                        setMobile($selectElement, prefs.mobile());
                        subscriptions.push(prefs.mobile.subscribe(function (isMobile) { return setMobile($selectElement, isMobile); }));
                    }
                    if (ko.isObservable(allBindingsAccessor.get("options"))) {
                        // we need this refresh so that when the array is empty, we still 
                        // refresh as the optionsAfterRender callback will not be called
                        subscriptions.push(optionsArray.subscribe(function (x) {
                            ko["tasks"].schedule(function () { return $selectElement.selectpicker('refresh'); });
                        }));
                    }
                }
                ko.utils.domNodeDisposal.addDisposeCallback(element, function (element) {
                    subscriptions.forEach(function (x) { return x.dispose(); });
                    if ($(element).is('.selectpicker'))
                        $(element).selectpicker('destroy');
                });
            },
            update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                var $selectElement = $(element);
                if ($selectElement.is('select')) {
                    var value = allBindingsAccessor.get("value");
                    var optionsArray = allBindingsAccessor.get("options");
                    var prefs = getOptions(valueAccessor());
                    var optionsValueProperty = allBindingsAccessor.get('optionsValue');
                    var noOptionsAvailable = optionsArray().length == 0;
                    if (!prefs.canSetNull && !value() && !noOptionsAvailable)
                        value(optionsArray()[0][optionsValueProperty]);
                    if (noOptionsAvailable)
                        setDisabled($selectElement, true);
                    else
                        setDisabled($selectElement, prefs.disable());
                    ko.bindingHandlers.value.update(element, value, allBindingsAccessor, viewModel, bindingContext);
                    $selectElement.attr("value", ko.unwrap(value));
                    ko["tasks"].schedule(function () { return $selectElement.selectpicker('refresh'); });
                }
            }
        };
    }
});
