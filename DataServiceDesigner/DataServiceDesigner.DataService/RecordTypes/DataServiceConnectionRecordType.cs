using System;
using AutoMapper;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Create")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
    public class DataServiceConnectionRecordType : ChangeableRecordType<DataServiceConnection, long, DataServiceConnectionBatchValidator>
    {
        public DataServiceConnectionRecordType()
        { }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DataServiceConnection, DataServiceConnection>();
        }
    }
}
