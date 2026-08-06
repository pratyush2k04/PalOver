using FluentValidation;
using Pdr.Api.DTOs.Documents;

namespace Pdr.Api.Validators;

public class UpdateDocumentVisibilityDtoValidator : AbstractValidator<UpdateDocumentVisibilityDto>
{
    private static readonly HashSet<string> ValidRoles = new()
    {
        "Pre Sales", "Project Management", "Tech Lead", "Leadership"
    };

    public UpdateDocumentVisibilityDtoValidator()
    {
        // Visibility is only required when not resetting to the category default
        When(x => !x.ResetToCategoryDefault, () =>
        {
            RuleFor(x => x.Visibility)
                .NotEmpty().WithMessage("Visibility is required.")
                .Must(v => v == "PM-only" || v == "Team-wide" || v == "Custom")
                .WithMessage("Visibility must be 'Team-wide', 'PM-only', or 'Custom'.");

            When(x => x.Visibility == "Custom", () =>
            {
                RuleFor(x => x.AllowedRoles)
                    .NotNull().WithMessage("Allowed roles must be provided when setting custom visibility.")
                    .Must(roles => roles != null && roles.Count > 0)
                    .WithMessage("At least one role must be selected for custom visibility.")
                    .Must(roles => roles == null || roles.All(r => ValidRoles.Contains(r)))
                    .WithMessage($"One or more roles are invalid. Valid roles: {string.Join(", ", ValidRoles)}.");
            });
        });
    }
}
