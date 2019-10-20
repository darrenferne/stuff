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

            RuleFor(x => x.Parent)
                .NotNull();

            RuleFor(x => x.Child)
                .NotNull();
        }
    }
}