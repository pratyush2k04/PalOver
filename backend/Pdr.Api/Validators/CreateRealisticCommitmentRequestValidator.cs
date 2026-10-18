using FluentValidation;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Constants;

namespace Pdr.Api.Validators;

public class CreateRealisticCommitmentRequestValidator : AbstractValidator<CreateRealisticCommitmentRequest>
{
    public CreateRealisticCommitmentRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(CreateRealisticCommitmentRequestConstants.DescriptionRequired)
            .MaximumLength(CreateRealisticCommitmentRequestConstants.MaxDescriptionLength).WithMessage(CreateRealisticCommitmentRequestConstants.DescriptionMaxLength);

        RuleFor(x => x.Timeline)
            .GreaterThan(0).WithMessage(CreateRealisticCommitmentRequestConstants.TimelineGreaterThanZero);

        RuleFor(x => x.TimelineUnit)
            .IsInEnum().WithMessage(CreateRealisticCommitmentRequestConstants.TimelineUnitInvalid);
    }
}
