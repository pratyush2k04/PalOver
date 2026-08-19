using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.ResourceRequests;

namespace Pdr.Api.Validators;

public class CreateResourceRequestDtoValidator : AbstractValidator<CreateResourceRequestDto>
{
    public CreateResourceRequestDtoValidator()
    {
        RuleFor(x => x.StandardRole)
            .IsInEnum();

        RuleFor(x => x.Skills)
            .NotEmpty().WithMessage(ResourceRequestConstants.ValidationMessages.SkillsRequired);

        RuleForEach(x => x.Skills)
            .IsInEnum();

        RuleFor(x => x.PeopleNeeded)
            .GreaterThan(0).WithMessage(ResourceRequestConstants.ValidationMessages.PeopleNeededGreaterThanZero);

        RuleFor(x => x.RequiredByDate)
            .Must(d => d >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage(ResourceRequestConstants.ValidationMessages.PastRequiredByDate);

        RuleFor(x => x.JobDescription)
            .NotEmpty().WithMessage(ResourceRequestConstants.ValidationMessages.JobDescriptionRequired)
            .Length(ResourceRequestConstants.MinJobDescriptionLength, ResourceRequestConstants.MaxJobDescriptionLength)
            .WithMessage(ResourceRequestConstants.ValidationMessages.JobDescriptionLength);
    }
}
