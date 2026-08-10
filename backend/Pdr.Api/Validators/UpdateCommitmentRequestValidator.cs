using FluentValidation;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Constants;
using System.Linq;

namespace Pdr.Api.Validators;

public class UpdateCommitmentRequestValidator : AbstractValidator<UpdateCommitmentRequest>
{
    public UpdateCommitmentRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(UpdateCommitmentRequestConstants.DescriptionRequired)
            .MaximumLength(UpdateCommitmentRequestConstants.MaxDescriptionLength).WithMessage(UpdateCommitmentRequestConstants.DescriptionMaxLength);

        RuleFor(x => x.CommitmentTypeId)
            .GreaterThan(0).WithMessage(UpdateCommitmentRequestConstants.CommitmentTypeIdRequired);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(UpdateCommitmentRequestConstants.TitleRequired)
            .MaximumLength(UpdateCommitmentRequestConstants.MaxTitleLength).WithMessage(UpdateCommitmentRequestConstants.TitleMaxLength);

        RuleFor(x => x.Timeline)
            .GreaterThan(0).WithMessage(UpdateCommitmentRequestConstants.TimelineGreaterThanZero);

        RuleFor(x => x.TimelineUnit)
            .Must(unit => UpdateCommitmentRequestConstants.ValidTimelineUnits.Contains(unit))
            .WithMessage(UpdateCommitmentRequestConstants.TimelineUnitInvalid);
    }
}
