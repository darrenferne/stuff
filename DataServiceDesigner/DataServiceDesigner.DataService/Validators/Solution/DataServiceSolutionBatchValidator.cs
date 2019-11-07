using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Validators;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    public class DataServiceSolutionBatchValidator : BatchValidator<long, DataServiceSolution>
    {
        protected override void SetupValidators(ChangeSetItems<long, DataServiceSolution> items)
        {
            validator = new DataServiceSolutionValidator();
            deletionValidator = new EmptyValidator<DataServiceSolution>();
        }
    }
}