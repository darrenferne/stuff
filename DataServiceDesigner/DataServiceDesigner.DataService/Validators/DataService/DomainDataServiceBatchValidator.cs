using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Validators;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    public class DomainDataServiceBatchValidator : BatchValidator<long, DomainDataService>
    {
        protected override void SetupValidators(ChangeSetItems<long, DomainDataService> items)
        {
            validator = new DomainDataServiceValidator();
            deletionValidator = new EmptyValidator<DomainDataService>();
        }
    }
}