﻿using AutoMapper;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Create")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
    public class DomainObjectRecordType : ObservableRecordType<DomainObject, long, DomainObjectBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainObject, DomainObject>()
                .ForMember(x => x.Schema, m => m.Ignore())
                .ForMember(x => x.Properties, m => m.Ignore());
        }
    }
}
