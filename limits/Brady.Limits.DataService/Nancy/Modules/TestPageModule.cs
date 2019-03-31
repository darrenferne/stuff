using Brady.Limits.DataService.Nancy.Models;
using BWF.DataServices.Nancy.Interfaces;
using BWF.Globalisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brady.Limits.DataService.Nancy.Modules
{
    public class TestPageModule : LimitsModule
    {
        public TestPageModule(IGlobalisationProvider globalisationProvider, IDataServiceHostSettings settings)
               : base("ext/limitsprototype", globalisationProvider)
        {
            Get["/testpage"] = args =>
            {
                return View["testpage", new TestPageModel(globalisationProvider, Context.CurrentUser, settings, base.SessionTimeoutEnabled)];
            };
        }
    }
}
