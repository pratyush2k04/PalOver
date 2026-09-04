using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.ArchitectureValidation;
using Pdr.Api.Models.ArchitectureValidation;

namespace Pdr.Api.Validators;

public class SaveArchitectureValidationRequestValidator : AbstractValidator<SaveArchitectureValidationRequest>
{
    public SaveArchitectureValidationRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage(ArchitectureValidationConstants.Messages.InvalidArchitectureStatus);

        RuleFor(x => x.Rationale)
            .MaximumLength(500).WithMessage("Rationale cannot exceed 500 characters.");

        When(x => x.Status == ArchitectureStatus.Modified, () =>
        {
            RuleFor(x => x.Rationale)
                .NotEmpty().WithMessage(ArchitectureValidationConstants.Messages.RationaleRequiredWhenModified);
        });
    }
}
