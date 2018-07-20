using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using FluentValidation;

namespace DataServiceDesigner.DataService
{
    public class DomainObjectValidator : Validator<DesignerDomainObject>
    {
        public DomainObjectValidator()
        {
            RuleFor(x => x.Id)
                 .GreaterThanOrEqualTo(0L);

            RuleFor(x => x.DbSchema)
                .NotEmpty()
                .Length(1, 64);

            RuleFor(x => x.DbObject)
                .NotEmpty()
                .Length(1, 64);

            RuleFor(x => x.Name)
                .Length(0, 64);

            RuleFor(x => x.DisplayName)
                .Length(0, 64);

            RuleFor(x => x.PluralisedDisplayName)
                .Length(0, 64);
        }
    }
}