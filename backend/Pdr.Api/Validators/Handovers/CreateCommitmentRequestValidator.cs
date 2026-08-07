using FluentValidation;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Constants.Handovers;

namespace Pdr.Api.Validators.Handovers;

public class CreateCommitmentRequestValidator : AbstractValidator<CreateCommitmentRequest>
{
    private static readonly string[] ValidTimelineUnits = { "Days", "Weeks", "Months" };

    public CreateCommitmentRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(CreateCommitmentRequestConstants.DescriptionRequired)
            .MaximumLength(2000).WithMessage(CreateCommitmentRequestConstants.DescriptionMaxLength);

        RuleFor(x => x.CommitmentTypeId)
            .GreaterThan(0).WithMessage(CreateCommitmentRequestConstants.CommitmentTypeIdRequired);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(CreateCommitmentRequestConstants.TitleRequired)
            .MaximumLength(100).WithMessage(CreateCommitmentRequestConstants.TitleMaxLength);

        RuleFor(x => x.Timeline)
            .GreaterThan(0).WithMessage(CreateCommitmentRequestConstants.TimelineGreaterThanZero);

        RuleFor(x => x.TimelineUnit)
            .Must(unit => ValidTimelineUnits.Contains(unit))
            .WithMessage(CreateCommitmentRequestConstants.TimelineUnitInvalid);
    }
}
