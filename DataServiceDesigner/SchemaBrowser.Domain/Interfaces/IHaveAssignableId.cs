using BWF.DataServices.Metadata.Interfaces;

namespace SchemaBrowser.Domain
{
    public interface IHaveAssignableId<Tid> : IHaveId<Tid>
    {
        new Tid Id { get; set; }
    }
}
