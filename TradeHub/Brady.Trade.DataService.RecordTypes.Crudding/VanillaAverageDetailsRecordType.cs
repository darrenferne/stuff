﻿using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.DataService.Core.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BWF.DataServices.Core.Interfaces;
using Brady.Trade.Domain;

namespace Brady.Trade.DataService.RecordTypes
{
    public class VanillaAverageDetailsRecordType : AverageDetailsRecordType<VanillaAverageDetails>
    {
        public VanillaAverageDetailsRecordType(IMetadataProvider metadataProvider) 
            : base(metadataProvider)
        {
        }
    }
}