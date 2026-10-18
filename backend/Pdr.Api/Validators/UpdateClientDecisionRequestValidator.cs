using FluentValidation;
using Pdr.Api.DTOs.BrownField;

namespace Pdr.Api.Validators;

public class UpdateClientDecisionRequestValidator : AbstractValidator<UpdateClientDecisionRequest>
{
    public UpdateClientDecisionRequestValidator()
    {
        RuleFor(x => x.ClientDecision)
            .IsInEnum().WithMessage("A valid Client Decision must be provided.");
    }
}
