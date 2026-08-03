using FluentValidation;
using Pdr.Api.DTOs.Handovers;
using System.Linq;

namespace Pdr.Api.Validators.Handovers;

public class SaveHandoverDraftRequestValidator : AbstractValidator<SaveHandoverDraftRequest>
{
    private readonly string[] _validSentiments = { "New", "Warm", "Strained" };

    public SaveHandoverDraftRequestValidator()
    {
        RuleFor(x => x.ClientBackground)
            .MaximumLength(2000).WithMessage("Client Background cannot exceed 2000 characters.");

        RuleFor(x => x.BusinessProblem)
            .MaximumLength(2000).WithMessage("Business Problem cannot exceed 2000 characters.");

        RuleFor(x => x.ClientSentiment)
            .Must(s => string.IsNullOrEmpty(s) || _validSentiments.Contains(s))
            .WithMessage("Client Sentiment must be one of: New, Warm, Strained.");

        RuleFor(x => x.DealOrigin)
            .MaximumLength(2000).WithMessage("Deal Origin cannot exceed 2000 characters.");
    }
}
