using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Validators;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    public class DataServiceBatchValidator : BatchValidator<long, DesignerDataService>
    {
        protected override void SetupValidators(ChangeSetItems<long, DesignerDataService> items)
        {
            validator = new DataServiceValidator();
            deletionValidator = new EmptyValidator<DesignerDataService>();
        }
    }
}