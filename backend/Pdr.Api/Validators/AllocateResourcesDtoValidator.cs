using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.ResourceRequests;

namespace Pdr.Api.Validators;

public class AllocateResourcesDtoValidator : AbstractValidator<AllocateResourcesDto>
{
    public AllocateResourcesDtoValidator()
    {
        RuleFor(x => x.Names)
            .NotEmpty().WithMessage(ResourceRequestConstants.ValidationMessages.NamesRequired);

        RuleForEach(x => x.Names)
            .MaximumLength(255).WithMessage(ResourceRequestConstants.ValidationMessages.NameTooLong);
    }
}
