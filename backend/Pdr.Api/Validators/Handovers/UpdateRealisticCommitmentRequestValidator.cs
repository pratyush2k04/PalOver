using FluentValidation;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Constants.Handovers;

namespace Pdr.Api.Validators.Handovers;

public class UpdateRealisticCommitmentRequestValidator : AbstractValidator<UpdateRealisticCommitmentRequest>
{
    private static readonly string[] ValidTimelineUnits = { "Days", "Weeks", "Months" };

    public UpdateRealisticCommitmentRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(UpdateRealisticCommitmentRequestConstants.DescriptionRequired)
            .MaximumLength(2000).WithMessage(UpdateRealisticCommitmentRequestConstants.DescriptionMaxLength);

        RuleFor(x => x.Timeline)
            .GreaterThan(0).WithMessage(UpdateRealisticCommitmentRequestConstants.TimelineGreaterThanZero);

        RuleFor(x => x.TimelineUnit)
            .Must(unit => ValidTimelineUnits.Contains(unit))
            .WithMessage(UpdateRealisticCommitmentRequestConstants.TimelineUnitInvalid);
    }
}
