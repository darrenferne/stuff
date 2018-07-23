using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using FluentValidation;

namespace DataServiceDesigner.DataService
{
    public class DomainDataServiceValidator : Validator<DomainDataService>
    {
        public DomainDataServiceValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0L);

            RuleFor(x => x.Name)
                .Length(0, 64);
        }
    }
}