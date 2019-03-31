using BWF.DataServices.Nancy;
using BWF.DataServices.Nancy.Abstract;
using BWF.DataServices.Nancy.Interfaces;
using BWF.Globalisation.Interfaces;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brady.Limits.DataService.Nancy.Models
{
    public abstract class LimitsModel : GlobalisedModel
    {
        public LimitsModel(IGlobalisationProvider globalisationProvider, IUserIdentity userIdentity, IDataServiceHostSettings settings, bool sessionTimeoutEnabled)
            : base(globalisationProvider, userIdentity as UserIdentity)
        {
            this.SessionTimeoutEnabled = sessionTimeoutEnabled;
        }

        public bool SessionTimeoutEnabled { get; set; }
    }
}
