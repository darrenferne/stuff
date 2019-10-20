using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Validators;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    public class DomainObjectReferenceBatchValidator : BatchValidator<long, DomainObjectReference>
    {
        protected override void SetupValidators(ChangeSetItems<long, DomainObjectReference> items)
        {
            validator = new DomainObjectReferenceValidator();
            deletionValidator = new EmptyValidator<DomainObjectReference>();
        }
    }
}