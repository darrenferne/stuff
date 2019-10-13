using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using FluentValidation;

namespace DataServiceDesigner.DataService
{
    public class DomainObjectPropertyValidator : Validator<DomainObjectProperty>
    {
        public DomainObjectPropertyValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0L);

            RuleFor(x => x.ColumnName)
                .NotEmpty()
                .Length(1, 64);

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1, 64);

            RuleFor(x => x.DisplayName)
                .Length(0, 64);
        }
    }
}