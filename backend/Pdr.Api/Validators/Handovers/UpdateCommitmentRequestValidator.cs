using FluentValidation;
using Pdr.Api.DTOs.Handovers;

namespace Pdr.Api.Validators.Handovers;

public class UpdateCommitmentRequestValidator : AbstractValidator<UpdateCommitmentRequest>
{
    public UpdateCommitmentRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.CommitmentTypeId)
            .GreaterThan(0).WithMessage("Commitment Type ID is required.");
    }
}
