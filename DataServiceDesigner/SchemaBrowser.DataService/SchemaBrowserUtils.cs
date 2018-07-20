using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using SBD = SchemaBrowser.Domain;

namespace SchemaBrowser.DataService
{
    internal class SchemaBrowserUtils
    {
        private DbProviderFactory GetDbProviderFactory(SBD.DbType dbType)
        {
            DataTable availableFactories = DbProviderFactories.GetFactoryClasses();
            DataColumn providerNameCol = availableFactories.Columns["InvariantName"];
            DbProviderFactory selectedFactory = null;

            foreach (DataRow factoryRow in availableFactories.Rows)
            {
                var providerName = (string)factoryRow[providerNameCol.Ordinal];
                switch (dbType)
                {
                    case SBD.DbType.SqlServer:
                        if (providerName == "System.Data.SqlClient")
                            selectedFactory = DbProviderFactories.GetFactory(factoryRow);
                        break;
                    case SBD.DbType.Oracle:
                        if (providerName == "Oracle.DataAccess.Client")
                            selectedFactory = DbProviderFactories.GetFactory(factoryRow);
                        break;
                }
                if (selectedFactory != null)
                    break;
            }
            return selectedFactory;
        }

        private DbConnection GetDbConnection(SBD.DbType dbType, string connectionString)
        {
            var factory = GetDbProviderFactory(dbType);
            var dbConnection = factory.CreateConnection();

            if (!string.IsNullOrEmpty(connectionString))
            {
                dbConnection.ConnectionString = connectionString;
                dbConnection.Open();
            }
            return dbConnection;
        }

        public bool TestConnection(SBD.DbType dbType, string connectionString, out string message)
        {
            message = string.Empty;
            try
            {
                using (var connection = GetDbConnection(dbType, connectionString))
                {
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return string.IsNullOrEmpty(message);
        }

        public List<SBD.DbObject> GetDbObjects(SBD.DbType dbType, string connectionString, bool includeProperties = true)
        {
            var tables = GetDbObjects(dbType, SBD.DbObjectType.Table, connectionString, includeProperties);
            var views = GetDbObjects(dbType, SBD.DbObjectType.View, connectionString, includeProperties);

            return tables.Union(views).ToList();
        }

        public List<SBD.DbObject> GetDbObjects(SBD.DbType dbType, SBD.DbObjectType objectType, string connectionString, bool includeProperties = true)
        {
            List<SBD.DbObject> dbObjects = new List<SBD.DbObject>();

            using (var dbConnection = GetDbConnection(dbType, connectionString))
            {
                var tables = dbConnection.GetSchema(objectType == SBD.DbObjectType.Table ? "Tables" : "Views");
                var catalogCol = dbType == SBD.DbType.SqlServer ? tables.Columns["TABLE_CATALOG"] : null;
                var ownerCol = dbType == SBD.DbType.SqlServer ? tables.Columns["TABLE_SCHEMA"] : tables.Columns["OWNER"];
                var tableNameCol = objectType == SBD.DbObjectType.View && dbType != SBD.DbType.SqlServer ? tables.Columns["VIEW_NAME"] : tables.Columns["TABLE_NAME"];

                foreach (DataRow tableRow in tables.Rows)
                {
                    var objectCatalog = dbType == SBD.DbType.SqlServer ? (string)tableRow[catalogCol] : string.Empty;
                    var objectOwner = (string)tableRow[ownerCol];
                    var objectName = (string)tableRow[tableNameCol];

                    var dbObject = new SBD.DbObject()
                    {
                        SchemaName = objectOwner,
                        Name = objectName,
                        ObjectType = objectType,
                        //Properties = includeProperties ? GetDbObjectProperties(dbType, objectType, dbConnection, objectCatalog, objectOwner, objectName) 
                        //                               : new List<SBD.DbObjectProperty>()
                    };

                    dbObjects.Add(dbObject);
                }
            }

            return dbObjects;
        }

        public List<SBD.DbObjectProperty> GetDbObjectProperties(SBD.DbType dbType, string connectionString)
        {
            using (var dbConnection = GetDbConnection(dbType, connectionString))
            {
                var tableCols = GetDbObjectProperties(dbType, SBD.DbObjectType.Table, dbConnection);
                var viewCols = GetDbObjectProperties(dbType, SBD.DbObjectType.View, dbConnection);

                return tableCols.Union(viewCols).ToList();
            }
        }

        public List<SBD.DbObjectProperty> GetDbObjectProperties(SBD.DbType dbType, SBD.DbObjectType objectType, DbConnection dbConnection)
        {
            List<SBD.DbObjectProperty> dbObjectProperties = new List<SBD.DbObjectProperty>();

            var columns = dbType == SBD.DbType.SqlServer ?
                    dbConnection.GetSchema(objectType == SBD.DbObjectType.Table ? "Columns" : "ViewColumns") :
                    dbConnection.GetSchema("Columns");

            var ownerCol = dbType == SBD.DbType.SqlServer ? columns.Columns["TABLE_SCHEMA"] : columns.Columns["OWNER"];
            var tableNameCol = columns.Columns["TABLE_NAME"];
            var columnNameCol = columns.Columns["COLUMN_NAME"];
            var columnTypeCol = dbType == SBD.DbType.SqlServer ? columns.Columns["DATA_TYPE"] : columns.Columns["DATATYPE"];
            var lengthCol = dbType == SBD.DbType.SqlServer ? columns.Columns["CHARACTER_MAXIMUM_LENGTH"] : columns.Columns["LENGTH"];
            var nullableCol = dbType == SBD.DbType.SqlServer ? columns.Columns["IS_NULLABLE"] : columns.Columns["NULLABLE"];

            foreach (DataRow columnRow in columns.Rows)
            {
                var objectOwner = (string)columnRow[ownerCol];
                var objectName = (string)columnRow[tableNameCol];
                var columnName = (string)columnRow[columnNameCol];
                var columnType = columnTypeCol == null ? null : (string)columnRow[columnTypeCol];
                var columnLength = lengthCol == null || columnRow.IsNull(lengthCol.Ordinal) ? 0 : int.Parse(columnRow[lengthCol].ToString());
                var isNullable = nullableCol == null || columnRow.IsNull(nullableCol.Ordinal) ? true : ((string)columnRow[nullableCol]).ToLower()[0] == 'y';

                var dbObjectProperty = new SBD.DbObjectProperty()
                {
                    SchemaName = objectOwner,
                    ObjectName = objectName,
                    Name = columnName
                };

                dbObjectProperties.Add(dbObjectProperty);
            }
            return dbObjectProperties;
        }

        public List<SBD.DbObjectProperty> GetDbObjectProperties(SBD.DbType dbType, SBD.DbObjectType objectType, DbConnection dbConnection, string objectCatalog, string objectOwner, string objectName)
        {
            List<SBD.DbObjectProperty> dbObjectProperties = new List<SBD.DbObjectProperty>();

            var columns = dbType == SBD.DbType.SqlServer ?
                    dbConnection.GetSchema(objectType == SBD.DbObjectType.Table ? "Columns" : "ViewColumns", new string[] { objectCatalog, objectOwner, objectName }) :
                    dbConnection.GetSchema("Columns", new string[] { objectOwner, objectName });

            var columnNameCol = columns.Columns["COLUMN_NAME"];
            var columnTypeCol = dbType == SBD.DbType.SqlServer ? columns.Columns["DATA_TYPE"] : columns.Columns["DATATYPE"];
            var lengthCol = dbType == SBD.DbType.SqlServer ? columns.Columns["CHARACTER_MAXIMUM_LENGTH"] : columns.Columns["LENGTH"];
            var nullableCol = dbType == SBD.DbType.SqlServer ? columns.Columns["IS_NULLABLE"] : columns.Columns["NULLABLE"];

            foreach (DataRow columnRow in columns.Rows)
            {
                var columnName = (string)columnRow[columnNameCol];
                var columnType = columnTypeCol == null ? null : (string)columnRow[columnTypeCol];
                var columnLength = lengthCol == null || columnRow.IsNull(lengthCol.Ordinal) ? 0 : int.Parse(columnRow[lengthCol].ToString()) ;
                var isNullable = nullableCol == null || columnRow.IsNull(nullableCol.Ordinal) ? true : ((string)columnRow[nullableCol]).ToLower()[0] == 'y';

                var dbObjectProperty = new SBD.DbObjectProperty()
                {
                    SchemaName = objectOwner,
                    ObjectName = objectName,
                    Name = columnName
                };

                dbObjectProperties.Add(dbObjectProperty);
            }
            return dbObjectProperties;
        }
    }
}
