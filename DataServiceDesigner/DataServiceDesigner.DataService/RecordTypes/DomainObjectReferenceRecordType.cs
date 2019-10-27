using AutoMapper;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Metadata.Enums;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using System;
using System.Linq;

namespace DataServiceDesigner.DataService
{
    
    [JavascriptAction(1,
        Name = "Create",
        DisplayName = "Create",
        Explanation = "Create a new Domain Object Reference",
        IconType = ActionIconType.FontAwesome,
        Icon = "fa-plus",
        InvokableFor = InvokableFor.All,
        ScriptModule =
            @"
            var actionArgs = {
                action: 'create',
                baseType: 'DomainObjectReference', 
                component: 'bwf-panel-editor',
                dataService: scope.dataService(),
                dataServiceUrl: scope.dataServiceUrl(),
                metadata: scope.metadata()
            };
            ko.postbox.publish(scope.viewGridId + '-clear-panel', actionArgs);
            ko.postbox.publish(scope.viewGridId + '-doAction', actionArgs);"
    )]
    [JavascriptAction(2,
        Name = "Edit",
        DisplayName = "Edit",
        Explanation = "Edit the selected Domain Object Reference",
        IconType = ActionIconType.FontAwesome,
        Icon = "fa-edit",
        InvokableFor = InvokableFor.One,
        ScriptModule =
            @"
            var actionArgs = {
                action: 'edit',
                baseType: 'DomainObjectReference', 
                component: 'bwf-panel-editor',
                dataService: scope.dataService(),
                dataServiceUrl: scope.dataServiceUrl(),
                metadata: scope.metadata()
            };
            ko.postbox.publish(scope.viewGridId + '-clear-panel', actionArgs);
            ko.postbox.publish(scope.viewGridId + '-doAction', actionArgs);"
    )]
    [DeleteAction("Domain Reference", Position = 3)]
    public class DomainObjectReferenceRecordType : ChangeableRecordType<DomainObjectReference, long, DomainObjectReferenceBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainObjectReference, DomainObjectReference>()
                .ForMember(m => m.Schema, o => o.Ignore())
                .ForMember(m => m.Parent, o => o.Ignore())
                .ForMember(m => m.Child, o => o.Ignore())
                .ForMember(m => m.Properties, o => o.Ignore());
        }

        public override Action<ChangeSet<long, DomainObjectReference>, BatchSaveContext<long, DomainObjectReference>, string> PreSaveAction
        {
            get
            {
                return (changeSet, context, username) =>
                {
                    base.PreSaveAction(changeSet, context, username);
                };
            }
        }
    }
}
