using FluentValidation;
using Pdr.Api.DTOs.Documents;

namespace Pdr.Api.Validators;

public class UpdateDocumentVisibilityDtoValidator : AbstractValidator<UpdateDocumentVisibilityDto>
{
    public UpdateDocumentVisibilityDtoValidator()
    {
        RuleFor(x => x.Visibility)
            .NotEmpty().WithMessage("Visibility is required.")
            .Must(v => v == "PM-only" || v == "Team-wide")
            .WithMessage("Visibility must be either 'PM-only' or 'Team-wide'.");
    }
}
