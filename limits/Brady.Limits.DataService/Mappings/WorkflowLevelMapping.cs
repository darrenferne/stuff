using NHibernate.Mapping.ByCode.Conformist;

namespace Brady.Limits.DataService.Mappings
{
    public class WorkflowLevelMapping : ClassMapping<Domain.WorkflowLevel>
    {
        public WorkflowLevelMapping()
        {
            Id(p => p.Id);
            Property(p => p.Name);
        }       
    }
}
