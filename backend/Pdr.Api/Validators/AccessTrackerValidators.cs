using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.AccessTracker;
using Pdr.Api.Models.AccessTracker;

namespace Pdr.Api.Validators;

public class AddCustomAccessItemRequestValidator : AbstractValidator<AddCustomAccessItemRequest>
{
    public AddCustomAccessItemRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.OwnerId)
            .GreaterThan(0).WithMessage("A valid Owner must be selected.");

        RuleFor(x => x.TimeZoneNote)
            .MaximumLength(100).WithMessage("Timezone note must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.TimeZoneNote));
    }
}

public class UpdateAccessItemStatusRequestValidator : AbstractValidator<UpdateAccessItemStatusRequest>
{
    public UpdateAccessItemStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("A valid Status must be provided.");

        RuleFor(x => x.BlockReason)
            .MaximumLength(500).WithMessage("Block reason must not exceed 500 characters.");

        When(x => x.Status == AccessItemStatus.Blocked, () =>
        {
            RuleFor(x => x.BlockReason)
                .NotEmpty().WithMessage(AccessTrackerConstants.Messages.BlockReasonRequired);
        });
    }
}

public class UpdateAccessItemOwnerRequestValidator : AbstractValidator<UpdateAccessItemOwnerRequest>
{
    public UpdateAccessItemOwnerRequestValidator()
    {
        RuleFor(x => x.OwnerId)
            .GreaterThan(0).WithMessage("A valid Owner must be selected.");
    }
}
