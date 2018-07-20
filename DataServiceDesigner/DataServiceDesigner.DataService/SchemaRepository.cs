using System.Collections.Generic;
using System.Linq;
using DataServiceDesigner.Domain;
using System.Data;
using System.Data.Common;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.QueryConverters.InMemory;

namespace DataServiceDesigner.DataService
{
    public class SchemaRepository : InMemoryDataServiceRepository, ISchemaRepository
    {
        Dictionary<long, Dictionary<string, Dictionary<string, DesignerDomainObject>>> _schemaObjects;

        public SchemaRepository(IAuthorisation authorisation, IMetadataProvider metadataProvider)
            : base("schemabrowser", authorisation, metadataProvider)
        {
            _schemaObjects = new Dictionary<long, Dictionary<string, Dictionary<string, DesignerDomainObject>>>();
        }

        
        public void FetchSchema(DesignerConnection connection, bool update = false)
        {
            if (update && _schemaObjects.ContainsKey(connection.Id))
                _schemaObjects.Remove(connection.Id);
            
            if (!_schemaObjects.ContainsKey(connection.Id))
            {
                var dbObjects = GetDbObjects(connection, DesignerObjectType.Table);
                
                var schemaObjects = dbObjects
                            .GroupBy(o => o.DbSchema)
                            .Select(g => new { Schema = g.Key, Objects = g.ToList() })
                            .ToDictionary(s => s.Schema, s => s.Objects.ToDictionary(o => o.DbObject));

                _schemaObjects.Add(connection.Id, schemaObjects);
            }
        }

        public IList<string> GetAvailableSchemas(DesignerConnection connection)
        {
            FetchSchema(connection);

            return _schemaObjects[connection.Id].Keys.ToList();
        }

        public IList<DesignerDomainObjectProperty> GetObjectProperties(DesignerConnection connection, string owner, string objectName)
        {
            FetchSchema(connection);

            var dbObject = _schemaObjects[connection.Id][owner][objectName];
            
            return dbObject.Properties;
        }

        public IList<DesignerDomainObject> GetObjects(DesignerConnection connection, string owner)
        {
            FetchSchema(connection);

            var dbObjects = _schemaObjects[connection.Id][owner].Values.ToList();

            return dbObjects;
        }

        internal DbProviderFactory GetDbProviderFactory(DesignerConnection connection)
        {
            DataTable availableFactories = DbProviderFactories.GetFactoryClasses();
            DataColumn providerNameCol = availableFactories.Columns["InvariantName"];
            DbProviderFactory selectedFactory = null;

            foreach (DataRow factoryRow in availableFactories.Rows)
            {
                var providerName = (string)factoryRow[providerNameCol.Ordinal];
                switch (connection.DatabaseType)
                {
                    case DesignerDatabaseType.SqlServer:
                        if (providerName == "System.Data.SqlClient")
                            selectedFactory = DbProviderFactories.GetFactory(factoryRow);
                        break;
                    case DesignerDatabaseType.Oracle:
                        if (providerName == "Oracle.DataAccess.Client")
                            selectedFactory = DbProviderFactories.GetFactory(factoryRow);
                        break;
                }
                if (selectedFactory != null)
                    break;
            }
            return selectedFactory;
        }

        internal DbConnection GetDbConnection(DesignerConnection connection)
        {
            var factory = GetDbProviderFactory(connection);
            var dbConnection = factory.CreateConnection();

            dbConnection.ConnectionString = connection.ConnectionString;
            dbConnection.Open();

            return dbConnection;
        }

        internal IList<DesignerDomainObject> GetDbObjects(DesignerConnection connection, DesignerObjectType objectType)
        {
            List<DesignerDomainObject> dbObjects = new List<DesignerDomainObject>();

            using (var dbConnection = GetDbConnection(connection))
            {
                var tables = dbConnection.GetSchema(objectType == DesignerObjectType.Table ? "Tables" : "Views");
                var catalogCol = connection.DatabaseType == DesignerDatabaseType.SqlServer ? tables.Columns["TABLE_CATALOG"] : null;
                var ownerCol = connection.DatabaseType == DesignerDatabaseType.SqlServer ? tables.Columns["TABLE_SCHEMA"] : tables.Columns["OWNER"];
                var tableNameCol = objectType == DesignerObjectType.View && connection.DatabaseType == DesignerDatabaseType.SqlServer ? tables.Columns["VIEW_NAME"] : tables.Columns["TABLE_NAME"];

                foreach (DataRow table in tables.Rows)
                {
                    var objectCatalog = connection.DatabaseType == DesignerDatabaseType.SqlServer ? (string)table[catalogCol] : string.Empty;
                    var objectOwner = (string)table[ownerCol];
                    var objectName = (string)table[tableNameCol];
                    
                    var dbObject = new DesignerDomainObject()
                    {
                        DbSchema = objectOwner,
                        DbObject = objectName,
                        Name = objectName,
                        DisplayName = objectName,
                        PluralisedDisplayName = objectName + "s",

                        Properties = GetDbObjectProperties(connection.DatabaseType, objectType, dbConnection, objectCatalog, objectOwner, objectName)
                    };

                    dbObjects.Add(dbObject);
                }
            }

            return dbObjects;
        }
        
        internal IList<DesignerDomainObjectProperty> GetDbObjectProperties(DesignerDatabaseType dbType, DesignerObjectType objectType, DbConnection dbConnection, string objectCatalog, string objectOwner, string objectName)
        {
            List<DesignerDomainObjectProperty> dbObjectProperties = new List<DesignerDomainObjectProperty>();

            var columns = dbType == DesignerDatabaseType.SqlServer ? 
                    dbConnection.GetSchema(objectType == DesignerObjectType.Table ? "Columns" : "ViewColumns", new string[] { objectCatalog, objectOwner, objectName }):
                    dbConnection.GetSchema("Columns", new string[] { objectOwner, objectName });

            var columnNameCol = columns.Columns["COLUMN_NAME"];
            var columnTypeCol = dbType == DesignerDatabaseType.SqlServer ? columns.Columns["DATA_TYPE"] : columns.Columns["DATATYPE"];
            var lengthCol = dbType == DesignerDatabaseType.SqlServer ? columns.Columns["CHARACTER_MAXIMUM_LENGTH"] : columns.Columns["LENGTH"];
            var nullableCol = dbType == DesignerDatabaseType.SqlServer ? columns.Columns["IS_NULLABLE"] : columns.Columns["NULLABLE"];

            return dbObjectProperties;
        }

        protected override IQueryExecutor GetQueryExecutor(string username, string token)
        {
            return new InMemoryQueryExecutor(this);
        }
        public override IEnumerable<T> Get<T>()
        {
            return Enumerable.Empty<T>();
            
        }

        public override IEnumerable<long> GetRoleIdsThatHaveServiceLevelPermission(string permissionType, string permissionName)
        {
            return Enumerable.Empty<long>();
        }

        public override void AddDataLevelPermissions(IEnumerable<BwfDataLevelPermission> dataLevelPermissions)
        {
            //Do nothing
        }

        public override void RemoveDataLevelPermissions(IEnumerable<BwfDataLevelPermission> dataLevelPermissions)
        {
            //Do nothing
        }
    }
}
