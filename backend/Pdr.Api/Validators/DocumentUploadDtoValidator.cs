using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Documents;
using Pdr.Api.Models.Documents;
using System.IO;
using System.Linq;

namespace Pdr.Api.Validators;

public class DocumentUploadDtoValidator : AbstractValidator<DocumentUploadDto>
{
    public DocumentUploadDtoValidator()
    {
        RuleFor(x => x.File)
            .Cascade(CascadeMode.Stop)

            .NotNull().WithMessage(DocumentValidationConstants.Messages.FileRequired)

            .Must(f => f.Length > 0)
            .WithMessage(DocumentValidationConstants.Messages.FileCannotBeEmpty)

            .Must(f => f.Length <= DocumentValidationConstants.MaxFileSizeBytes)
            .WithMessage(DocumentValidationConstants.Messages.FileTooLarge)

            .Must(f => DocumentValidationConstants.AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLowerInvariant()))
            .WithMessage(DocumentValidationConstants.Messages.FileTypeNotSupported);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage(DocumentValidationConstants.Messages.CategoryIdRequired);

        RuleFor(x => x.Visibility)
            .Must(v => v == DocumentVisibilityNames.PmOnly || v == DocumentVisibilityNames.TeamWide)
            .WithMessage(DocumentValidationConstants.Messages.VisibilityInvalid);
    }
}
