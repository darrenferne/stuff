using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using FluentValidation;

namespace DataServiceDesigner.DataService
{
    public class DomainObjectReferencePropertyValidator : Validator<DomainObjectReferenceProperty>
    {
        public DomainObjectReferencePropertyValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0L);

            RuleFor(x => x.ReferenceId)
                .NotNull();

            RuleFor(x => x.ParentPropertyId)
                .NotNull();

            RuleFor(x => x.ChildPropertyId)
                .NotNull()
                .Must((o, i) => i != o.ParentPropertyId)
                    .WithMessage("'Child Property Id' must not be the same as 'Parent Property Id'");
        }
    }
}