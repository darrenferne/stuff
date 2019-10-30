using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using FluentValidation;

namespace DataServiceDesigner.DataService
{
    public class DataServiceConnectionValidator : Validator<DataServiceConnection>
    {
        public DataServiceConnectionValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0L);

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1, 64);

            RuleFor(x => x.DataSource)
                .NotEmpty()
                .Length(1, 1000);

            RuleFor(x => x.InitialCatalog)
                .Length(0, 64);

            RuleFor(x => x.Username)
                .Length(0, 64);

            RuleFor(x => x.Password)
                .Length(0, 64);

            RuleFor(x => x.ConnectionString)
                .NotEmpty()
                .Length(1, 2000);
        }
    }
}