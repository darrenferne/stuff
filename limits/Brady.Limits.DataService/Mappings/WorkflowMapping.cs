using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Brady.Limits.DataService.Mappings
{
    public class WorkflowMapping : ClassMapping<Domain.Workflow>
    {
        public WorkflowMapping()
        {
            Id(p => p.Id);
            Property(p => p.Name);

            Bag(x => x.Levels, m =>
            {
                m.Key(k => k.Column("workflowid"));
                m.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                m.Lazy(CollectionLazy.NoLazy);
            }, r => r.OneToMany());
        }       
    }
}
