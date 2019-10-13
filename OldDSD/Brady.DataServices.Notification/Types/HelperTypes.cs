using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace Brady.DataServices.Notification
{
    internal static class ReaderExtensions
    {
        public static void Initialise(this INotificationQuery query, IDbConnection connection)
        {
            NotificationTable table = query as NotificationTable;
            if (table != null)
            {
                if (table.ColumnNames == null || table.ColumnNames.Length == 0)
                {
                    if (connection != null)
                    {
                        if (connection.State != ConnectionState.Open)
                            connection.Open();

                        using (IDbCommand command = connection.CreateCommand())
                        {
                            command.CommandText = table.Sql;

                            using (IDataReader reader = command.ExecuteReader())
                            {
                                List<string> columns = new List<string>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i).ToLower();
                                    if (!columns.Contains(columnName))
                                        columns.Add(columnName); 
                                }
                                table.ColumnNames = columns.ToArray();
                                reader.Close();
                            }
                        }
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        public static Comparable GetComparable(this OracleDataReader reader, string fieldName)
        {
            return reader.GetComparable(new string[] { fieldName });
        }
        public static Comparable GetComparable(this OracleDataReader reader, string[] fieldNames)
        {
            object[] values = new object[fieldNames.Length];
            for (int fn = 0; fn < fieldNames.Length; fn++)
            {
                int ordinal = reader.GetOrdinal(fieldNames[fn]);
                try
                {
                    values[fn] = reader.GetValue(ordinal);
                }
                catch (OverflowException) //dirty hack for oracle decimals with greater precision than .NET
                { 
                    if (reader.GetFieldType(ordinal) == typeof(Decimal))
                        values[fn] = Convert.ToDecimal(reader.GetOracleDecimal(ordinal).ToString());
                }
            }
            return new Comparable(values);
        }
        [DebuggerHidden]
        public static Comparable GetComparable(this OracleDataReader reader)
        {
            object[] values = new object[reader.FieldCount];
            for (int f = 0; f < reader.FieldCount; f++)
            {
                try
                {
                    values[f] = reader.GetValue(f);
                }
                catch (OverflowException) //dirty hack for oracle decimals with greater precision than .NET
                {
                    if (!reader.IsDBNull(f) && reader.GetFieldType(f) == typeof(Decimal))
                        values[f] = (double)reader.GetOracleDecimal(f);
                }
            }
            return new Comparable(values);
        }
        public static Comparable GetComparable(this SqlDataReader reader, string fieldName)
        {
            return reader.GetComparable(new string[] { fieldName });
        }
        public static Comparable GetComparable(this SqlDataReader reader, string[] fieldNames)
        {
            object[] values = new object[fieldNames.Length];
            for (int fn = 0; fn < fieldNames.Length; fn++)
                values[fn] = reader.GetValue(reader.GetOrdinal(fieldNames[fn]));
            return new Comparable(values);
        }
        public static Comparable GetComparable(this SqlDataReader reader)
        {
            object[] values = new object[reader.FieldCount];
            reader.GetProviderSpecificValues(values);
            return new Comparable(values);
        }
    
        public static Row GetRow(this OracleDataReader reader, string fieldName)
        {
            return new Row(reader.GetComparable(fieldName));
        }
        public static Row GetRow(this OracleDataReader reader, string[] fieldNames)
        {
            return new Row(reader.GetComparable(fieldNames));
        }
        public static Row GetRow(this OracleDataReader reader)
        {
            return new Row(reader.GetComparable());
        }
        public static Row GetRow(this SqlDataReader reader, string fieldName)
        {
            return new Row(reader.GetComparable(fieldName));
        }
        public static Row GetRow(this SqlDataReader reader, string[] fieldNames)
        {
            return new Row(reader.GetComparable(fieldNames));
        }
        public static Row GetRow(this SqlDataReader reader)
        {
            return new Row(reader.GetComparable());
        }

        public static Key GetKey(this OracleDataReader reader, string keyFieldName)
        {
            return new Key(reader.GetComparable(keyFieldName));
        }
        public static Key GetKey(this OracleDataReader reader, string[] keyFieldNames)
        {
            return new Key(reader.GetComparable(keyFieldNames));
        }
        public static Key GetKey(this SqlDataReader reader, string keyFieldName)
        {
            return new Key(reader.GetComparable(keyFieldName));
        }
        public static Key GetKey(this SqlDataReader reader, string[] keyFieldNames)
        {
            return new Key(reader.GetComparable(keyFieldNames));
        }
    }

    internal class Comparable
    {
        int _hash;
        bool _hashSet;

        public Comparable(object value)
            : this(new object[] { value })
        { }
        public Comparable(object[] values)
        {
            this.Values = values;
        }
        
        public object[] Values { get; set; }

        public object this[int index]
        {
            get { return Values[index]; }
        }

        public int Hash
        {
            get { return GetHashCode(); }
        }
        public override int GetHashCode()
        {
            if (!_hashSet)
            {
                int hash;
                if (Values.Length == 1)
                    hash = this[0].GetHashCode();
                else
                {
                    hash = 13;
                    unchecked
                    {
                        for (int fv = 0; fv < Values.Length; fv++)
                            hash = hash * 17 + this[fv].GetHashCode();
                    }
                }
                _hash = hash;
                _hashSet = true;
            }
            return _hash;
        }

        public override bool Equals(object obj)
        {
            if (Values.Length == 1)
                return this[0].Equals(((Comparable)obj)[0]);
            else
            {
                bool match = true;
                Comparable compare = obj as Comparable;
                if (compare != null)
                {
                    for (int fv = 0; fv < Values.Length; fv++)
                    {
                        match = match && this[fv].Equals(compare[fv]);
                        if (!match)
                            break;
                    }
                }
                return match;
            }
        }
    }

    internal class Key : Comparable
    {
        public Key(object value)
            : base(new object[] { value })
        { }
        public Key(object[] values)
            : base(values)
        { }
        public Key(Comparable comparable)
            : base(comparable.Values)
        { }
    }

    internal class Row : Comparable
    {
        public Row(object[] values)
            : base(values)
        { }

        public Row(Comparable comparable)
            : base(comparable.Values)
        { }
    }

    internal class KeyAndRowHash
    {
        public KeyAndRowHash(Key key, int rowHash)
        {
            this.Key = key;
            this.RowHash = rowHash;
        }
        public Key Key { get; set; }
        public int RowHash { get; set; }
    }

    internal class KeyAndRow
    {
        public KeyAndRow(Key key, Row row)
        {
            this.Key = key;
            this.Row = row;
        }
        public Key Key { get; set; }
        public Row Row { get; set; }
    }
}
