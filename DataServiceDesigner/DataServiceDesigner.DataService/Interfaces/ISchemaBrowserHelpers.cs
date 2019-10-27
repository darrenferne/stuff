using BWF.DataServices.Domain.Interfaces;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    public interface ISchemaBrowserHelpers
    {
        void SynchConnections(IChangeSet changeSet, string token);
        void AddDefaultObjectsToSchema(DomainSchema domainSchema);
        void AddDefaultRelationshipsToSchema(DomainSchema domainSchema);
        void AddDefaultPropertiesToObject(DomainObject domainObject);
    }
}
