using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using FluentValidation;

namespace DataServiceDesigner.DataService
{
    public class DomainSchemaValidator : Validator<DomainSchema>
    {
        public DomainSchemaValidator()
        {
            RuleFor(x => x.Id)
                 .GreaterThanOrEqualTo(0L);

            RuleFor(x => x.Name)
                .Length(1, 64);
        }
    }
}