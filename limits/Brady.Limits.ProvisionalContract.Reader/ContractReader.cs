using System;
using System.Collections.Generic;
using System.IO;
using Brady.Limits.ProvisionalContract.DataService;
using Brady.Limits.ProvisionalContract.Domain;
using Microsoft.Office.Interop.Excel;

namespace Brady.Limits.ProvisionalContract.Reader
{
    public class ContractReader
    {
        string _fileName;
        public ContractReader(string fileName)
        {
            _fileName = Path.GetFullPath(fileName);
        }

        private T GetCellValue<T>(Range range, int row, int column)
        {
            var value = ((Range)range[row, column]).Value;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public List<Domain.ProvisionalContract> Read()
        {
            var contracts = new List<Domain.ProvisionalContract>();

            Application xlApp = new Application();
            Workbook xlBook = xlApp.Workbooks.Open(_fileName);
            try
            {
                Worksheet xlSheet = xlBook.Sheets[1] as Worksheet;
                Range xlRange = xlSheet.UsedRange;
                

                for (int r = 2; r <= xlRange.Rows.Count; r++)
                {
                    var contract = new Domain.ProvisionalContract();

                    contract.ContractId = GetCellValue<string>(xlRange, r, 1);
                    contract.ClientNumber = GetCellValue<string>(xlRange, r, 2);
                    contract.ClientName = GetCellValue<string>(xlRange, r, 3);
                    contract.Product = GetCellValue<string>(xlRange, r, 4);
                    contract.Quantity = GetCellValue<float>(xlRange, r, 5);
                    contract.QuantityUnit = GetCellValue<string>(xlRange, r, 6);
                    contract.Premium = GetCellValue<float>(xlRange, r, 7);

                    contracts.Add(contract);
                }
            }
            finally
            {
                xlBook.Close();
            }
            
            return contracts;
        }
    }
}
