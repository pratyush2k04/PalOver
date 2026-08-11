using System;
using System.Linq;
using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Documents;
using Pdr.Api.Models;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Validators;

public class UpdateDocumentVisibilityDtoValidator : AbstractValidator<UpdateDocumentVisibilityDto>
{
    public UpdateDocumentVisibilityDtoValidator()
    {
        // Visibility is only required when not resetting to the category default
        When(x => !x.ResetToCategoryDefault, () =>
        {
            RuleFor(x => x.Visibility)
                .NotEmpty().WithMessage(DocumentValidationConstants.Messages.VisibilityRequired)
                .Must(v => v == DocumentVisibilityNames.PmOnly || v == DocumentVisibilityNames.TeamWide || v == DocumentVisibilityNames.Custom)
                .WithMessage(DocumentValidationConstants.Messages.UpdateVisibilityInvalid);

            When(x => x.Visibility == DocumentVisibilityNames.Custom, () =>
            {
                RuleFor(x => x.AllowedRoles)
                    .NotNull().WithMessage(DocumentValidationConstants.Messages.CustomVisibilityRolesRequired)
                    .Must(roles => roles != null && roles.Count > 0)
                    .WithMessage(DocumentValidationConstants.Messages.CustomVisibilityAtLeastOneRole)
                    .Must(roles => roles == null || roles.All(r => Enum.IsDefined(typeof(RoleEnum), r)))
                    .WithMessage(DocumentValidationConstants.Messages.InvalidRoleIds);
            });
        });
    }
}
