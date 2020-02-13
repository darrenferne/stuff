using System;
using AutoMapper;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Metadata.Enums;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Solution")]
    [EditAction("Solution")]
    [DeleteAction("Solution")]
    [JavascriptAction(1,
        Name = "Generate Template",
        DisplayName = "Generate Template",
        Explanation = "Generate template code for the data service",
        InvokableFor = InvokableFor.One,
        ScriptModule =
            @"
            var baseUrl = scope.dataServiceUrl();
            var dataServiceName = scope.selectedRecords()[0].record.Name;
            var url = baseUrl  + '/generatetemplate/' + dataServiceName;
            $(""#template"").remove();
            $(""<iframe id='template' style='dsplay:none' src='"" + url +""'></iframe>"").appendTo('body');"
    )]

    public class DataServiceSolutionRecordType : ChangeableRecordType<DataServiceSolution, long, DataServiceSolutionBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DataServiceSolution, DataServiceSolution>()
                .ForMember(m => m.DataServices, o => o.Ignore()); 
        }
    }
}
