using FluentValidation;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Constants.Handovers;

namespace Pdr.Api.Validators.Handovers;

public class UpdateCommitmentRequestValidator : AbstractValidator<UpdateCommitmentRequest>
{
    private static readonly string[] ValidTimelineUnits = { "Days", "Weeks", "Months" };

    public UpdateCommitmentRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(UpdateCommitmentRequestConstants.DescriptionRequired)
            .MaximumLength(2000).WithMessage(UpdateCommitmentRequestConstants.DescriptionMaxLength);

        RuleFor(x => x.CommitmentTypeId)
            .GreaterThan(0).WithMessage(UpdateCommitmentRequestConstants.CommitmentTypeIdRequired);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(UpdateCommitmentRequestConstants.TitleRequired)
            .MaximumLength(100).WithMessage(UpdateCommitmentRequestConstants.TitleMaxLength);

        RuleFor(x => x.Timeline)
            .GreaterThan(0).WithMessage(UpdateCommitmentRequestConstants.TimelineGreaterThanZero);

        RuleFor(x => x.TimelineUnit)
            .Must(unit => ValidTimelineUnits.Contains(unit))
            .WithMessage(UpdateCommitmentRequestConstants.TimelineUnitInvalid);
    }
}
