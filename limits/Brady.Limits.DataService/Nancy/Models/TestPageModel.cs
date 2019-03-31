using BWF.DataServices.Nancy.Interfaces;
using BWF.Globalisation.Interfaces;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brady.Limits.DataService.Nancy.Models
{
    public class TestPageModel : LimitsModel
    {
        public string Message { get; set; }
        public TestPageModel(IGlobalisationProvider globalisationProvider, IUserIdentity userIdentity, IDataServiceHostSettings settings, bool sessionTimeoutEnabled)
            : base(globalisationProvider, userIdentity, settings, sessionTimeoutEnabled)
        {
            Message = "Test Page";
        }
    }
}
