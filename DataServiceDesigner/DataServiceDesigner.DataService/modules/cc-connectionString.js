define(['knockout', 'options', 'loglevel', 'modules/bwf-metadata', 'knockout-amd-helpers', 'knockout-postbox', 'jquery'],
    function (ko, options, log, metadataService) {

        var isNullOrEmpty = function (str) {
            return str === null || str === "";
        };

        function initialCatalogViewModel(data) {
            var self = this;
            
            self.data = data;
            self.databaseType = data.model.observables["DatabaseType"];
            self.dataSource = data.model.observables["DataSource"];
            self.initialCatalog = data.model.observables["InitialCatalog"];
            self.useIntegratedSecurity = data.model.observables["UseIntegratedSecurity"];
            self.username = data.model.observables["Username"];
            self.password = data.model.observables["Password"];
            self.connectionString = data.model.observables["ConnectionString"];
            
            self.generateConnectionString = function (model) {

                var connectionString = "";
                if (!isNullOrEmpty(model.dataSource())) {
                    connectionString += "Data Source=" + model.dataSource() + ";";
                }
                if (self.databaseType() === "SQLServer" && !isNullOrEmpty(model.initialCatalog())) {
                    connectionString += "Initial Catalog=" + model.initialCatalog() + ";";
                }
                if (model.useIntegratedSecurity()) {
                    connectionString += "Integrated Security=" + model.useIntegratedSecurity() + ";";
                }
                else {
                    if (!isNullOrEmpty(model.username())) {
                        connectionString += "User Id=" + model.username() + ";";
                    }
                    if (!isNullOrEmpty(model.password())) {
                        connectionString += "Password=" + model.password() + ";";
                    }
                }
                self.connectionString(connectionString);
            };
            self.testConnection = function () {
                var databaseType = self.databaseType();
                var dataSource = self.dataSource();
                var initialCatalog = self.initialCatalog();
                var catalogRequired = databaseType === "SQLServer";
                var useIntegrated = self.useIntegratedSecurity();
                var userName = self.username();
                var password = self.password();
                var connectionString = self.connectionString();

                if (!useIntegrated && isNullOrEmpty(userName)) {
                    alert("'User Name' required");
                }
                else if (!useIntegrated && isNullOrEmpty(password)) {
                    alert("'Password' required");
                }
                else if (isNullOrEmpty(dataSource)) {
                    alert("'Data Source' required");
                }
                else if (catalogRequired && isNullOrEmpty(initialCatalog)) {
                    alert("'Initial Catalog' required");
                }
                else if (isNullOrEmpty(connectionString)) {
                    alert("'Connection String' required");
                }
                else {

                    var testConnectionQuery = options.explorerHostUrl + '/api/schemabrowser/ext/TestConnection/' + databaseType + '/?cs=' + encodeURIComponent(connectionString);

                    $.ajax({
                        url: testConnectionQuery,
                        xhrFields: { withCredentials: true }
                    })
                        .done(function (result) {
                            if (result === "")
                                alert("Test connection succeeded");
                            else {
                                alert(result);
                            }
                        })
                        .fail(function (message) {
                            alert("Connection test failed\n" + message.status);
                        });
                }
            };
            self.initialise = function () {
                var model = this;
                self.dataSource.subscribe(function () {
                    model.generateConnectionString(model);
                })
                self.initialCatalog.subscribe(function () {
                    model.generateConnectionString(model);
                })
                self.useIntegratedSecurity.subscribe(function () {
                    model.generateConnectionString(model);
                })
                self.username.subscribe(function () {
                    model.generateConnectionString(model);
                })
                self.password.subscribe(function () {
                    model.generateConnectionString(model);
                })
                model.generateConnectionString(model);
            };
            self.initialise();
        }

        return initialCatalogViewModel;
    }
);