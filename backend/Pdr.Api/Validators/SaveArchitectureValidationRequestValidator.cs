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

        When(x => x.Status == ArchitectureStatus.Modified, () =>
        {
            RuleFor(x => x.Rationale)
                .NotEmpty().WithMessage(ArchitectureValidationConstants.Messages.RationaleRequiredWhenModified)
                .MaximumLength(2000).WithMessage("Rationale cannot exceed 2000 characters.");
        });
    }
}
