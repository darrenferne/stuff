require(['options'], function (options) {

    require(['knockout', 'modules/bwf-title', 'knockout-amd-helpers']
        , function (ko, title) {
            ko.bindingHandlers.module.baseDir = "modules";

            function tracker() {
                var self = this;

                self.title = 'Tracker | ' + options.resources['bwf_explorer'];
                title.setTitle(self.title);
            }

            var viewModel = new tracker();
            ko.applyBindings(viewModel, document.getElementById("content"));
        });
});