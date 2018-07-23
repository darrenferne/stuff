using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Validators;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    public class DataServiceConnectionBatchValidator : BatchValidator<long, DataServiceConnection>
    {
        protected override void SetupValidators(ChangeSetItems<long, DataServiceConnection> items)
        {
            validator = new DataServiceConnectionValidator();
            deletionValidator = new EmptyValidator<DataServiceConnection>();
        }
    }
}