using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Validators;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    public class ConnectionBatchValidator : BatchValidator<long, DesignerConnection>
    {
        protected override void SetupValidators(ChangeSetItems<long, DesignerConnection> items)
        {
            validator = new ConnectionValidator();
            deletionValidator = new EmptyValidator<DesignerConnection>();
        }
    }
}