using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Nancy.Abstract;
using BWF.Globalisation.Interfaces;
using BWF.Enums;
using Nancy;
using SchemaBrowser.Domain;
using System;

namespace SchemaBrowser.DataService
{
    public class ApiModule : GlobalisedNancyModule
    {
        IAuthentication _authentication;
        ISchemaBrowserRepository _repository;
        SchemaBrowserUtils _utils;

        public ApiModule(IAuthentication authentication, IGlobalisationProvider globalisationProvider, ISchemaBrowserRepository repository)
            : base("api/schemabrowser/ext", globalisationProvider)
        {
            _authentication = authentication;
            _repository = repository;
            _utils = new SchemaBrowserUtils();

            Get[@"/TestConnection/{databaseType}"] = args =>
            {
                string databaseType = args.databaseType;
                DbType dbType = databaseType.FromDatabaseValue<DbType>();
                string connectionString = Request.Query["cs"]; //can't get this to work via normal parameter as it's encoded
                string message = string.Empty;

                _utils.TestConnection(dbType, connectionString, out message);

                return Response.AsJson(message);
            };
        }
    }
}
