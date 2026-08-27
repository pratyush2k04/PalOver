using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.ResourceRequests;

namespace Pdr.Api.Validators;

public class RemoveAllocationDtoValidator : AbstractValidator<RemoveAllocationDto>
{
    public RemoveAllocationDtoValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage(ResourceRequestConstants.ValidationMessages.RemovalReasonRequired)
            .Length(10, 500).WithMessage(ResourceRequestConstants.ValidationMessages.RemovalReasonLength);
    }
}
