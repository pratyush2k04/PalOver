using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.ResourceRequests;

namespace Pdr.Api.Validators;

public class ConfirmAllocationDtoValidator : AbstractValidator<ConfirmAllocationDto>
{
    public ConfirmAllocationDtoValidator()
    {
        RuleFor(x => x.ConfirmedDate)
            .Must(d => d <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage(ResourceRequestConstants.ValidationMessages.FutureConfirmedDate);
    }
}
