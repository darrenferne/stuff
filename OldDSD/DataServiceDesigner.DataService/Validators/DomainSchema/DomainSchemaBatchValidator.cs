using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Validators;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    public class DomainSchemaBatchValidator : BatchValidator<long, DomainSchema>
    {
        protected override void SetupValidators(ChangeSetItems<long, DomainSchema> items)
        {
            validator = new DomainSchemaValidator();
            deletionValidator = new EmptyValidator<DomainSchema>();
        }
    }
}