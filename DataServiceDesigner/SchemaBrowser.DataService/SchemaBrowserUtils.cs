using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using SBD = SchemaBrowser.Domain;

namespace SchemaBrowser.DataService
{
    public class SchemaBrowserUtils
    {
        private DbProviderFactory GetDbProviderFactory(SBD.DatabaseType dbType)
        {
            DataTable availableFactories = DbProviderFactories.GetFactoryClasses();
            DataColumn providerNameCol = availableFactories.Columns["InvariantName"];
            DbProviderFactory selectedFactory = null;

            foreach (DataRow factoryRow in availableFactories.Rows)
            {
                var providerName = (string)factoryRow[providerNameCol.Ordinal];
                switch (dbType)
                {
                    case SBD.DatabaseType.SqlServer:
                        if (providerName == "System.Data.SqlClient")
                            selectedFactory = DbProviderFactories.GetFactory(factoryRow);
                        break;
                    case SBD.DatabaseType.Oracle:
                        if (providerName == "Oracle.DataAccess.Client" || providerName == "Oracle.ManagedDataAccess.Client")
                            selectedFactory = DbProviderFactories.GetFactory(factoryRow);
                        break;
                }
                if (selectedFactory != null)
                    break;
            }
            return selectedFactory;
        }

        public DbConnection GetDbConnection(SBD.DatabaseType dbType, string connectionString)
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

        public bool TestConnection(SBD.DatabaseType dbType, string connectionString, out string message)
        {
            message = string.Empty;
            try
            {
                if (!connectionString.Contains("Connection Timeout"))
                    connectionString = connectionString + ";Connection Timeout=120";

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

        public List<SBD.DbObject> GetDbObjects(SBD.DatabaseType dbType, string connectionString, bool includeProperties = true)
        {
            var tables = GetDbObjects(dbType, SBD.DbObjectType.Table, connectionString, includeProperties);
            var views = GetDbObjects(dbType, SBD.DbObjectType.View, connectionString, includeProperties);

            return tables.Union(views).ToList();
        }

        public List<SBD.DbObject> GetDbObjects(SBD.DatabaseType dbType, SBD.DbObjectType objectType, string connectionString, bool includeProperties = true)
        {
            using (var dbConnection = GetDbConnection(dbType, connectionString))
            {
                return GetDbObjects(dbType, dbConnection, objectType);
            }
        }

        public List<SBD.DbObject> GetDbObjects(SBD.DatabaseType dbType, DbConnection dbConnection, SBD.DbObjectType objectType)
        {
            List<SBD.DbObject> dbObjects = new List<SBD.DbObject>();

            var tables = dbConnection.GetSchema(objectType == SBD.DbObjectType.Table ? "Tables" : "Views");
            var catalogCol = dbType == SBD.DatabaseType.SqlServer ? tables.Columns["TABLE_CATALOG"] : null;
            var ownerCol = dbType == SBD.DatabaseType.SqlServer ? tables.Columns["TABLE_SCHEMA"] : tables.Columns["OWNER"];
            var tableNameCol = objectType == SBD.DbObjectType.View && dbType != SBD.DatabaseType.SqlServer ? tables.Columns["VIEW_NAME"] : tables.Columns["TABLE_NAME"];

            foreach (DataRow tableRow in tables.Rows)
            {
                var objectCatalog = dbType == SBD.DatabaseType.SqlServer ? (string)tableRow[catalogCol] : string.Empty;
                var objectOwner = (string)tableRow[ownerCol];
                var objectName = (string)tableRow[tableNameCol];

                var dbObject = new SBD.DbObject()
                {
                    SchemaName = objectOwner,
                    Name = objectName,
                    ObjectType = objectType
                };

                dbObjects.Add(dbObject);
            }
            
            return dbObjects;
        }

        public List<SBD.DbObjectProperty> GetDbObjectProperties(SBD.DatabaseType dbType, string connectionString)
        {
            using (var dbConnection = GetDbConnection(dbType, connectionString))
            {
                var tableCols = GetDbObjectProperties(dbType, SBD.DbObjectType.Table, dbConnection);
                var viewCols = GetDbObjectProperties(dbType, SBD.DbObjectType.View, dbConnection);

                return tableCols.Union(viewCols).ToList();
            }
        }

        public List<SBD.DbObjectProperty> GetDbObjectProperties(SBD.DatabaseType dbType, SBD.DbObjectType objectType, DbConnection dbConnection)
        {
            List<SBD.DbObject> dbObjects = null;
            List<SBD.DbObjectProperty> dbObjectProperties = new List<SBD.DbObjectProperty>();

            var columns = dbType == SBD.DatabaseType.SqlServer ?
                    dbConnection.GetSchema(objectType == SBD.DbObjectType.Table ? "Columns" : "ViewColumns") :
                    dbConnection.GetSchema("Columns");

            if (dbType == SBD.DatabaseType.Oracle)
                dbObjects = GetDbObjects(dbType, dbConnection, objectType);
            
            var ownerCol = dbType == SBD.DatabaseType.SqlServer ? columns.Columns["TABLE_SCHEMA"] : columns.Columns["OWNER"];
            var tableNameCol = columns.Columns["TABLE_NAME"];
            var columnNameCol = columns.Columns["COLUMN_NAME"];
            var columnTypeCol = dbType == SBD.DatabaseType.SqlServer ? columns.Columns["DATA_TYPE"] : columns.Columns["DATATYPE"];
            var lengthCol = dbType == SBD.DatabaseType.SqlServer ? columns.Columns["CHARACTER_MAXIMUM_LENGTH"] : columns.Columns["LENGTH"];
            var nullableCol = dbType == SBD.DatabaseType.SqlServer ? columns.Columns["IS_NULLABLE"] : columns.Columns["NULLABLE"];

            foreach (DataRow columnRow in columns.Rows)
            {
                var objectOwner = (string)columnRow[ownerCol];
                var objectName = (string)columnRow[tableNameCol];
                if (dbType == SBD.DatabaseType.SqlServer || (dbType == SBD.DatabaseType.Oracle && dbObjects.Any(o => o.SchemaName == objectOwner && o.Name == objectName)))
                {
                    var columnName = (string)columnRow[columnNameCol];
                    var columnType = columnTypeCol == null ? null : ((string)columnRow[columnTypeCol]).ToUpper();
                    var columnLength = lengthCol == null || columnRow.IsNull(lengthCol.Ordinal) ? 0 : int.Parse(columnRow[lengthCol].ToString());
                    var isNullable = nullableCol == null || columnRow.IsNull(nullableCol.Ordinal) ? true : ((string)columnRow[nullableCol]).ToLower()[0] == 'y';

                    var dbObjectProperty = new SBD.DbObjectProperty()
                    {
                        SchemaName = objectOwner,
                        ObjectName = objectName,
                        Name = columnName,
                        ColumnType = columnType,
                        NetType = SBD.DbTypeConversion.SqlTypeToNetType(columnType),
                        ColumnLength = columnLength,
                        IsNullable = isNullable
                    };

                    dbObjectProperties.Add(dbObjectProperty);
                }
            }
            return dbObjectProperties;
        }

        public List<SBD.DbObjectProperty> GetDbObjectProperties(SBD.DatabaseType dbType, SBD.DbObjectType objectType, DbConnection dbConnection, string objectCatalog, string objectOwner, string objectName)
        {
            List<SBD.DbObjectProperty> dbObjectProperties = new List<SBD.DbObjectProperty>();

            var columns = dbType == SBD.DatabaseType.SqlServer ?
                    dbConnection.GetSchema(objectType == SBD.DbObjectType.Table ? "Columns" : "ViewColumns", new string[] { objectCatalog, objectOwner, objectName }) :
                    dbConnection.GetSchema("Columns", new string[] { objectOwner, objectName });

            var columnNameCol = columns.Columns["COLUMN_NAME"];
            var columnTypeCol = dbType == SBD.DatabaseType.SqlServer ? columns.Columns["DATA_TYPE"] : columns.Columns["DATATYPE"];
            var lengthCol = dbType == SBD.DatabaseType.SqlServer ? columns.Columns["CHARACTER_MAXIMUM_LENGTH"] : columns.Columns["LENGTH"];
            var nullableCol = dbType == SBD.DatabaseType.SqlServer ? columns.Columns["IS_NULLABLE"] : columns.Columns["NULLABLE"];

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

        private string GetObjectPrimaryKeySQL(SBD.DatabaseType dbType, bool filtered = false)
        {
            var sql = string.Empty;
            if (dbType == SBD.DatabaseType.SqlServer)
            {
                sql = @"SELECT
                            s.name              schema_name,
                            t.name              table_name,
                            i.name              index_name,
                            c.name              column_name
                        FROM
                            sys.tables t
                            JOIN
                            sys.schemas s ON t.schema_id = s.schema_id
                            JOIN
                            sys.indexes i ON i.object_id = t.object_id
                            JOIN
                            sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                            JOIN
                            sys.columns c ON c.object_id = t.object_id AND c.column_id = ic.column_id
                        WHERE
                            i.is_primary_key = 1";

                if (filtered)
                {
                    sql += @" AND s.name = @1 AND t.name = @2";
                }

                sql += @" ORDER BY s.name, t.name, i.name, ic.index_column_id";
            }
            else
            {
                sql = @"SELECT
                            cc.owner            schema_name,
                            cc.table_name       table_name, 
                            c.constraint_name   index_name,
                            cc.column_name      column_name
                        FROM 
                            dba_constraints c
                            JOIN
                            dba_cons_columns cc ON c.constraint_name = cc.constraint_name AND c.owner = cc.owner
                        WHERE 
                            c.constraint_type = 'P'";
                
                if (filtered)
                {
                    sql += @" AND s.owner = @1 AND t.table_name = @2";
                }

                sql += @" ORDER BY cc.owner, cc.table_name, c.constraint_name, cc.column_name, cc.position";
            }
            return sql;
        }

        public SBD.DbObjectIndex GetObjectPrimaryKey(SBD.DatabaseType dbType, string connectionString, string objectOwner, string objectName)
        {
            var sql = GetObjectPrimaryKeySQL(dbType, true);

            var connection = GetDbConnection(dbType, connectionString);
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            
            var param1 = command.CreateParameter();
            param1.DbType = DbType.String;
            param1.Value = objectOwner;
            command.Parameters.Add(param1);
            
            var param2 = command.CreateParameter();
            param2.DbType = DbType.String;
            param2.Value = objectOwner;
            command.Parameters.Add(param2);

            var reader = command.ExecuteReader();
            var primaryKey = default(SBD.DbObjectIndex);
            while (reader.Read())
            {
                if (primaryKey is null)
                {
                    primaryKey.SchemaName = reader.GetString(reader.GetOrdinal("schema_name"));
                    primaryKey.TableName = reader.GetString(reader.GetOrdinal("table_name"));
                    primaryKey.IndexName = reader.GetString(reader.GetOrdinal("index_name"));
                }
                primaryKey.Columns.Add(reader.GetString(reader.GetOrdinal("column_name")));
            }

            return primaryKey;
        }

        public List<SBD.DbObjectIndex> GetObjectPrimaryKeys(SBD.DatabaseType dbType, string connectionString)
        {
            var primaryKeys = new List<SBD.DbObjectIndex>();
            var sql = GetObjectPrimaryKeySQL(dbType);

            using (var connection = GetDbConnection(dbType, connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        var primaryKey = default(SBD.DbObjectIndex);
                        var lastKeyName = string.Empty;
                        while (reader.Read())
                        {
                            var schemaName = reader.GetString(reader.GetOrdinal("schema_name"));
                            var tableName = reader.GetString(reader.GetOrdinal("table_name"));
                            var indexName = reader.GetString(reader.GetOrdinal("index_name"));
                            var keyName = $"{schemaName}.{tableName}.{indexName}";
                            if (keyName != lastKeyName)
                            {
                                primaryKey = new SBD.DbObjectIndex()
                                {
                                    SchemaName = schemaName,
                                    TableName = tableName,
                                    IndexName = indexName
                                };
                                primaryKeys.Add(primaryKey);

                                lastKeyName = keyName; 
                            }
                            primaryKey.Columns.Add(reader.GetString(reader.GetOrdinal("column_name")));
                        }
                    }
                }
            }
            return primaryKeys;
        }

        private string GetObjectForeignKeySQL(SBD.DatabaseType dbType, bool filtered = false)
        {
            var sql = string.Empty;
            if (dbType == SBD.DatabaseType.SqlServer)
            {
                sql = @"SELECT
                            s.name						schema_name,
                            t.name						table_name,
                            fk.name						constraint_name,
	                        cfk.name					constraint_column_name,
	                        rs.name						referenced_schema_name,
	                        rt.name						referenced_table_name,
	                        ri.name						referenced_index_name,
	                        crt.name					referenced_column_name
                        FROM
                            sys.tables t
                            JOIN
                            sys.schemas s ON s.schema_id = t.schema_id
                            JOIN
                            sys.foreign_keys fk ON fk.parent_object_id = t.object_id
                            JOIN
                            sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
	                        JOIN
	                        sys.columns cfk ON cfk.object_id = fkc.parent_object_id AND cfk.column_id = fkc.parent_column_id
	                        JOIN
	                        sys.tables rt ON rt.object_id = fkc.referenced_object_id
	                        JOIN
                            sys.schemas rs ON rs.schema_id = rt.schema_id
	                        JOIN
                            sys.columns crt ON crt.object_id = fkc.referenced_object_id AND crt.column_id = fkc.referenced_column_id
	                        JOIN
	                        sys.indexes ri ON ri.object_id = fk.referenced_object_id AND ri.index_id = fk.key_index_id";

                if (filtered)
                {
                    sql += @" AND s.name = @1 AND t.name = @2";
                }

                sql += @" ORDER BY s.name, t.name, fk.name, fkc.constraint_column_id";
            }
            else
            {
                sql = @"SELECT 
                           c.owner              schema_name,
                           c.table_name         table_name,
                           c.constraint_name    constraint_name,
                           cc.column_name       constraint_column_name,
                           rc.owner             referenced_schema_name,
                           rc.table_name        referenced_table_name,
                           rc.constraint_name   referenced_index_name,
                           rcc.column_name      referenced_column_name
                        FROM 
                           dba_constraints c
                           JOIN
                           dba_cons_columns cc ON cc.owner = c.owner AND cc.constraint_name = c.constraint_name 
                           JOIN
                           dba_constraints rc ON rc.owner = c.r_owner AND rc.constraint_name = c.r_constraint_name
                           JOIN
                           dba_cons_columns rcc ON rcc.owner = rc.owner AND rcc.constraint_name = rc.constraint_name AND rcc.position = cc.position
                        WHERE   
                           c.constraint_type='R'";

                if (filtered)
                {
                    sql += @" AND c.owner = @1 AND c.table_name = @2";
                }

                sql += @" ORDER BY c.owner, c.table_name, c.constraint_name, cc.position";
            }
            return sql;
        }

        public SBD.DbObjectForeignKey GetObjectForeignKey(SBD.DatabaseType dbType, string connectionString, string objectOwner, string objectName)
        {
            var sql = GetObjectForeignKeySQL(dbType, true);

            var connection = GetDbConnection(dbType, connectionString);
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            var param1 = command.CreateParameter();
            param1.DbType = DbType.String;
            param1.Value = objectOwner;
            command.Parameters.Add(param1);

            var param2 = command.CreateParameter();
            param2.DbType = DbType.String;
            param2.Value = objectOwner;
            command.Parameters.Add(param2);

            var reader = command.ExecuteReader();
            var foreignKey = default(SBD.DbObjectForeignKey);
            while (reader.Read())
            {
                if (foreignKey is null)
                {
                    foreignKey.SchemaName = reader.GetString(reader.GetOrdinal("schema_name"));
                    foreignKey.TableName = reader.GetString(reader.GetOrdinal("table_name"));
                    foreignKey.ConstraintName = reader.GetString(reader.GetOrdinal("constraint_name"));
                    foreignKey.ReferencedIndex = new SBD.DbObjectIndex()
                    {
                        SchemaName = reader.GetString(reader.GetOrdinal("referenced_schema_name")),
                        TableName = reader.GetString(reader.GetOrdinal("referenced_table_name")),
                        IndexName = reader.GetString(reader.GetOrdinal("referenced_index_name"))
                    };
                }
                foreignKey.Columns.Add(reader.GetString(reader.GetOrdinal("constraint_column_name")));
                foreignKey.ReferencedIndex.Columns.Add(reader.GetString(reader.GetOrdinal("referenced_column_name")));
            }

            return foreignKey;
        }

        public List<SBD.DbObjectForeignKey> GetObjectForeignKeys(SBD.DatabaseType dbType, string connectionString)
        {
            var foreignKeys = new List<SBD.DbObjectForeignKey>();
            var sql = GetObjectForeignKeySQL(dbType);

            using (var connection = GetDbConnection(dbType, connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        var foreignKey = default(SBD.DbObjectForeignKey);
                        var lastKeyName = string.Empty;
                        while (reader.Read())
                        {
                            var schemaName = reader.GetString(reader.GetOrdinal("schema_name"));
                            var tableName = reader.GetString(reader.GetOrdinal("table_name"));
                            var constraintName = reader.GetString(reader.GetOrdinal("constraint_name"));
                            var keyName = $"{schemaName}.{tableName}.{constraintName}";
                            if (keyName != lastKeyName)
                            {
                                foreignKey = new SBD.DbObjectForeignKey()
                                {
                                    SchemaName = schemaName,
                                    TableName = tableName,
                                    ConstraintName = constraintName,
                                    ReferencedIndex = new SBD.DbObjectIndex() 
                                    {
                                        SchemaName = reader.GetString(reader.GetOrdinal("referenced_schema_name")),
                                        TableName = reader.GetString(reader.GetOrdinal("referenced_table_name")),
                                        IndexName = reader.GetString(reader.GetOrdinal("referenced_index_name"))
                                    }
                                };
                                foreignKeys.Add(foreignKey);

                                lastKeyName = keyName;
                            }
                            foreignKey.Columns.Add(reader.GetString(reader.GetOrdinal("constraint_column_name")));
                            foreignKey.ReferencedIndex.Columns.Add(reader.GetString(reader.GetOrdinal("referenced_column_name")));
                        }
                    }
                }
            }
            return foreignKeys;
        }
    }
}
