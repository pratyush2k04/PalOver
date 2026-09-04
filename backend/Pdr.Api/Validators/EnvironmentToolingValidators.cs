using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.EnvironmentTooling;
using Pdr.Api.Models.EnvironmentTooling;

namespace Pdr.Api.Validators;

public class AddCustomEnvironmentToolingItemRequestValidator : AbstractValidator<AddCustomEnvironmentToolingItemRequest>
{
    public AddCustomEnvironmentToolingItemRequestValidator()
    {
        RuleFor(x => x.CustomName)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.OwnerId)
            .GreaterThan(0).WithMessage("A valid Owner must be selected.");
    }
}

public class UpdateEnvironmentToolingStatusRequestValidator : AbstractValidator<UpdateEnvironmentToolingStatusRequest>
{
    public UpdateEnvironmentToolingStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("A valid Status must be provided.");

        RuleFor(x => x.BlockReason)
            .MaximumLength(500).WithMessage("Block reason must not exceed 500 characters.");

        When(x => x.Status == EnvironmentToolingStatus.Blocked, () =>
        {
            RuleFor(x => x.BlockReason)
                .NotEmpty().WithMessage(EnvironmentToolingConstants.Messages.BlockReasonRequired);
        });
    }
}

public class UpdateEnvironmentToolingOwnerRequestValidator : AbstractValidator<UpdateEnvironmentToolingOwnerRequest>
{
    public UpdateEnvironmentToolingOwnerRequestValidator()
    {
        RuleFor(x => x.OwnerId)
            .GreaterThan(0).WithMessage("A valid Owner must be selected.");
    }
}
