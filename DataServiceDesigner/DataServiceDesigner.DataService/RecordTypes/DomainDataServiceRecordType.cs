﻿using AutoMapper;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Create")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
    public class DataServiceRecordType : ChangeableRecordType<DomainDataService, long, DomainDataServiceBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainDataService, DomainDataService>()
                .ForMember(x => x.DbConnection, m => m.Ignore())
                .ForMember(x => x.DbSchema, m => m.Ignore())
                .ForMember(x => x.DomainObjects, m => m.Ignore());
        }
    }
}