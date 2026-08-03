using FluentValidation;
using Pdr.Api.DTOs.Engagements;

namespace Pdr.Api.Validators;

public class MarkSowSignedRequestValidator : AbstractValidator<MarkSowSignedRequest>
{
    public MarkSowSignedRequestValidator()
    {
        RuleFor(x => x.EngagementDescription)
            .NotEmpty().WithMessage("Engagement description is required.");

        RuleFor(x => x.BillingStartDate)
            .NotEmpty().WithMessage("Billing start date is required.");
    }
}
