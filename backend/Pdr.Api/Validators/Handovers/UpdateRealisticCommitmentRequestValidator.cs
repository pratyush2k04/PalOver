using FluentValidation;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Constants.Handovers;
using System.Linq;

namespace Pdr.Api.Validators.Handovers;

public class UpdateRealisticCommitmentRequestValidator : AbstractValidator<UpdateRealisticCommitmentRequest>
{
    public UpdateRealisticCommitmentRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(UpdateRealisticCommitmentRequestConstants.DescriptionRequired)
            .MaximumLength(UpdateRealisticCommitmentRequestConstants.MaxDescriptionLength).WithMessage(UpdateRealisticCommitmentRequestConstants.DescriptionMaxLength);

        RuleFor(x => x.Timeline)
            .GreaterThan(0).WithMessage(UpdateRealisticCommitmentRequestConstants.TimelineGreaterThanZero);

        RuleFor(x => x.TimelineUnit)
            .Must(unit => UpdateRealisticCommitmentRequestConstants.ValidTimelineUnits.Contains(unit))
            .WithMessage(UpdateRealisticCommitmentRequestConstants.TimelineUnitInvalid);
    }
}
