using Brady.Limits.DataService.Nancy.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.Selection.Provider
{
    class SelectionQueryBuilder : ISelectionQueryBuilder
    {
        public string GetQuery(Domain.Models.Selection selection)
        {
            return string.Empty;
        }
    }
}
