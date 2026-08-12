using FluentValidation;
using Pdr.Api.DTOs.Notifications;

namespace Pdr.Api.Validators;

public class BulkDeleteNotificationsRequestValidator : AbstractValidator<BulkDeleteNotificationsRequest>
{
    public BulkDeleteNotificationsRequestValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("At least one notification id must be provided.");
    }
}
