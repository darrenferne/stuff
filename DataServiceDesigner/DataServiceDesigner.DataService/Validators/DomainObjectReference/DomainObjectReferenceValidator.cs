using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using FluentValidation;

namespace DataServiceDesigner.DataService
{
    public class DomainObjectReferenceValidator : Validator<DomainObjectReference>
    {
        public DomainObjectReferenceValidator()
        {
            RuleFor(x => x.Id)
                 .GreaterThanOrEqualTo(0L);
            
            RuleFor(x => x.ReferenceName)
                .Length(1, 64);

            RuleFor(x => x.ConstraintName)
                .Length(0, 64);

            RuleFor(x => x.Parent)
                .NotNull();

            RuleFor(x => x.Child)
                .NotNull();
        }
    }
}