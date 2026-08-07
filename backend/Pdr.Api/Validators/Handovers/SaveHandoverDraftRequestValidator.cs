using FluentValidation;
using Pdr.Api.DTOs.Handovers;
using System.Linq;
using Pdr.Api.Constants.Handovers;

namespace Pdr.Api.Validators.Handovers;

public class SaveHandoverDraftRequestValidator : AbstractValidator<SaveHandoverDraftRequest>
{
    private readonly string[] _validSentiments = { "New", "Warm", "Strained" };

    public SaveHandoverDraftRequestValidator()
    {
        RuleFor(x => x.ClientBackground)
            .MaximumLength(2000).WithMessage(SaveHandoverDraftRequestConstants.ClientBackgroundMaxLength);

        RuleFor(x => x.BusinessProblem)
            .MaximumLength(2000).WithMessage(SaveHandoverDraftRequestConstants.BusinessProblemMaxLength);

        RuleFor(x => x.ClientSentiment)
            .Must(s => string.IsNullOrEmpty(s) || _validSentiments.Contains(s))
            .WithMessage(SaveHandoverDraftRequestConstants.ClientSentimentInvalid);

        RuleFor(x => x.DealOrigin)
            .MaximumLength(2000).WithMessage(SaveHandoverDraftRequestConstants.DealOriginMaxLength);
    }
}
