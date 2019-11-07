using System;
using AutoMapper;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Solution")]
    [EditAction("Solution")]
    [DeleteAction("Solution")]
    public class DataServiceSolutionRecordType : ChangeableRecordType<DataServiceSolution, long, DataServiceSolutionBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DataServiceSolution, DataServiceSolution>();
        }
    }
}
