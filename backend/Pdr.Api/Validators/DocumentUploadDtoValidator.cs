using FluentValidation;
using Pdr.Api.DTOs.Documents;
using System.IO;
using System.Linq;

namespace Pdr.Api.Validators;

public class DocumentUploadDtoValidator : AbstractValidator<DocumentUploadDto>
{
    private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50MB
    private readonly string[] _allowedExtensions = { ".pdf", ".docx", ".xlsx", ".pptx", ".png", ".jpg", ".msg", ".eml" };

    public DocumentUploadDtoValidator()
    {
        RuleFor(x => x.File)
            .Cascade(CascadeMode.Stop)

            .NotNull().WithMessage("File is required.")

            .Must(f => f.Length > 0)
            .WithMessage("File cannot be empty.")

            .Must(f => f.Length <= MaxFileSizeBytes)
            .WithMessage($"Files must be 50 MB or smaller.")

            .Must(f => _allowedExtensions.Contains(Path.GetExtension(f.FileName).ToLowerInvariant()))
            .WithMessage("That file type is not supported.");

        RuleFor(x => x.EngagementId)
            .GreaterThan(0).WithMessage("EngagementId must be greater than 0.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("A valid CategoryId is required.");

        RuleFor(x => x.Visibility)
            .Must(v => v == "PM-only" || v == "Team-wide")
            .WithMessage("Visibility must be either 'PM-only' or 'Team-wide'.");
    }
}
