using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.Data;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DoIt();
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
