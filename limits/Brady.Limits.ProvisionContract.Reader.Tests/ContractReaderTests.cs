using System;
using Brady.Limits.ProvisionalContract.Reader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Brady.Limits.ProvisionContract.Reader.Tests
{
    [TestClass]
    public class ContractReaderTests
    {
        [TestMethod]
        public void Read_book_test()
        {
            var reader = new ContractReader(@".\Book1.xlsx");

            var contracts = reader.Read();
        }
    }
}
