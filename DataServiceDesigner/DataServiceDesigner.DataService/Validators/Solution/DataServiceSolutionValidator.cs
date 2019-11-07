using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using FluentValidation;

namespace DataServiceDesigner.DataService
{
    public class DataServiceSolutionValidator : Validator<DataServiceSolution>
    {
        public DataServiceSolutionValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0L);

            RuleFor(x => x.ServiceName)
                .NotEmpty()
                .Length(1, 64);

            RuleFor(x => x.ServiceDisplayName)
                .NotEmpty()
                .Length(1, 64);

            RuleFor(x => x.NamespacePrefix)
                .NotEmpty()
                .Length(1, 64);

            RuleFor(x => x.CompanyName)
                .NotEmpty()
                .Length(1, 64);

            RuleFor(x => x.ServiceDescription)
                .NotEmpty()
                .Length(1, 1000);
        }
    }
}