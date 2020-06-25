using BWF.DataServices.Support.NHibernate.Abstract;
using FluentValidation;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ProcessingRequestValidator : Validator<ProcessingRequest>
    {
        public ProcessingRequestValidator()
        {
            RuleFor(p => p.Id)
                .NotNull();

            RuleFor(p => p.RequestId)
                .NotNull()
                .Length(1, 36);

            RuleFor(p => p.RequestType)
                .NotNull()
                .Length(1, 64);
            
        }
    }
}
