using BWF.DataServices.Support.NHibernate.Abstract;
using FluentValidation;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ProcessingStateValidator : Validator<ProcessingState>
    {
        public ProcessingStateValidator()
        {
            RuleFor(p => p.Id)
                .NotNull();

            RuleFor(p => p.ExternalId)
                .NotNull()
                .Length(1, 64);

            RuleFor(p => p.StateType)
                .NotNull()
                .Length(1, 64);
        }
    }
}
