using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.ChangeRequests;

namespace Pdr.Api.Validators;

public class CreateChangeRequestDtoValidator : AbstractValidator<CreateChangeRequestDto>
{
    public CreateChangeRequestDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ChangeRequestConstants.ValidationMessages.DescriptionRequired)
            .MaximumLength(ChangeRequestConstants.MaxDescriptionLength)
            .WithMessage(ChangeRequestConstants.ValidationMessages.DescriptionMaxLength);

        RuleFor(x => x.Impact)
            .NotEmpty().WithMessage(ChangeRequestConstants.ValidationMessages.ImpactRequired)
            .MaximumLength(ChangeRequestConstants.MaxImpactLength)
            .WithMessage(ChangeRequestConstants.ValidationMessages.ImpactMaxLength);
    }
}

public class RejectChangeRequestDtoValidator : AbstractValidator<RejectChangeRequestDto>
{
    public RejectChangeRequestDtoValidator()
    {
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage(ChangeRequestConstants.ValidationMessages.RejectionReasonRequired)
            .MaximumLength(ChangeRequestConstants.MaxRejectionReasonLength)
            .WithMessage(ChangeRequestConstants.ValidationMessages.RejectionReasonMaxLength);
    }
}
