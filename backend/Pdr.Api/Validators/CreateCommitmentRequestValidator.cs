using FluentValidation;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Constants;
using System.Linq;

namespace Pdr.Api.Validators;

public class CreateCommitmentRequestValidator : AbstractValidator<CreateCommitmentRequest>
{
    public CreateCommitmentRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(CreateCommitmentRequestConstants.DescriptionRequired)
            .MaximumLength(CreateCommitmentRequestConstants.MaxDescriptionLength).WithMessage(CreateCommitmentRequestConstants.DescriptionMaxLength);

        RuleFor(x => x.CommitmentTypeId)
            .GreaterThan(0).WithMessage(CreateCommitmentRequestConstants.CommitmentTypeIdRequired);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(CreateCommitmentRequestConstants.TitleRequired)
            .MaximumLength(CreateCommitmentRequestConstants.MaxTitleLength).WithMessage(CreateCommitmentRequestConstants.TitleMaxLength);

        RuleFor(x => x.Timeline)
            .GreaterThan(0).WithMessage(CreateCommitmentRequestConstants.TimelineGreaterThanZero);

        RuleFor(x => x.TimelineUnit)
            .Must(unit => CreateCommitmentRequestConstants.ValidTimelineUnits.Contains(unit))
            .WithMessage(CreateCommitmentRequestConstants.TimelineUnitInvalid);
    }
}
