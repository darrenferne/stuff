using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.Data;
using SchemaBrowser.DataService;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            SchemaBrowserUtils browser = new SchemaBrowserUtils();
            
            var connectionString = @"Data Source=localhost\sqlexpress;Initial Catalog=dsd-latest;Integrated Security=True;Pooling=False";
            var keys = browser.GetObjectPrimaryKeys(SchemaBrowser.Domain.DbType.SqlServer, connectionString);
            //var schema = connection.GetSchema();
            //var tables = connection.GetSchema("Tables");
            //var fks = connection.GetSchema("ForeignKeys");
            //var ixs = connection.GetSchema("Indexes");
            //var ixColumns = connection.GetSchema("IndexColumns");
        }

        void DoIt()
        {
            var factories = DbProviderFactories.GetFactoryClasses();
            foreach (DataRow factoryRow in factories.Rows)
            {
                Console.WriteLine(factoryRow[0]);
                Console.WriteLine(factoryRow[1]);
                Console.WriteLine(factoryRow[2]);
                Console.WriteLine(factoryRow[3]);
            }
            
        }
    }
}
