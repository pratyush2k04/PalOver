using FluentValidation;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Constants.Handovers;

namespace Pdr.Api.Validators.Handovers;

public class CreateRealisticCommitmentRequestValidator : AbstractValidator<CreateRealisticCommitmentRequest>
{
    private static readonly string[] ValidTimelineUnits = { "Days", "Weeks", "Months" };

    public CreateRealisticCommitmentRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(CreateRealisticCommitmentRequestConstants.DescriptionRequired)
            .MaximumLength(2000).WithMessage(CreateRealisticCommitmentRequestConstants.DescriptionMaxLength);

        RuleFor(x => x.Timeline)
            .GreaterThan(0).WithMessage(CreateRealisticCommitmentRequestConstants.TimelineGreaterThanZero);

        RuleFor(x => x.TimelineUnit)
            .Must(unit => ValidTimelineUnits.Contains(unit))
            .WithMessage(CreateRealisticCommitmentRequestConstants.TimelineUnitInvalid);
    }
}
