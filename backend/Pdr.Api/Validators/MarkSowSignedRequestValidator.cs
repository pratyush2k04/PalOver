using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Engagements;

namespace Pdr.Api.Validators;

public class MarkSowSignedRequestValidator : AbstractValidator<MarkSowSignedRequest>
{
    public MarkSowSignedRequestValidator()
    {
        RuleFor(x => x.EngagementDescription)
            .NotEmpty().WithMessage(EngagementValidationConstants.DescriptionRequired);

        RuleFor(x => x.BillingStartDate)
            .NotEmpty().WithMessage(EngagementValidationConstants.BillingStartDateRequired);
    }
}
