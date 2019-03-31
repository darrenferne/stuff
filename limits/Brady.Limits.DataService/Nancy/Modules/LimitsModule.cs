using BWF.DataServices.Nancy.Abstract;
using BWF.Globalisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brady.Limits.DataService.Nancy.Modules
{
    public abstract class LimitsModule : GlobalisedNancyModule
    {
        public LimitsModule(string moduleUrl, IGlobalisationProvider globalisationProvider)
            : base(moduleUrl, globalisationProvider)
        {
            this.SessionTimeoutEnabled = true; 
        }

        public bool SessionTimeoutEnabled { get; set; }
    }
}
