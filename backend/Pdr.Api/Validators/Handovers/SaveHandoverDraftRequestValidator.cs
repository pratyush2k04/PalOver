using FluentValidation;
using Pdr.Api.DTOs.Handovers;
using System.Linq;
using Pdr.Api.Constants.Handovers;

namespace Pdr.Api.Validators.Handovers;

public class SaveHandoverDraftRequestValidator : AbstractValidator<SaveHandoverDraftRequest>
{
    public SaveHandoverDraftRequestValidator()
    {
        RuleFor(x => x.ClientBackground)
            .MaximumLength(SaveHandoverDraftRequestConstants.MaxFieldLength).WithMessage(SaveHandoverDraftRequestConstants.ClientBackgroundMaxLength);

        RuleFor(x => x.BusinessProblem)
            .MaximumLength(SaveHandoverDraftRequestConstants.MaxFieldLength).WithMessage(SaveHandoverDraftRequestConstants.BusinessProblemMaxLength);

        RuleFor(x => x.ClientSentiment)
            .Must(s => string.IsNullOrEmpty(s) || SaveHandoverDraftRequestConstants.ValidSentiments.Contains(s))
            .WithMessage(SaveHandoverDraftRequestConstants.ClientSentimentInvalid);

        RuleFor(x => x.DealOrigin)
            .MaximumLength(SaveHandoverDraftRequestConstants.MaxFieldLength).WithMessage(SaveHandoverDraftRequestConstants.DealOriginMaxLength);
    }
}
