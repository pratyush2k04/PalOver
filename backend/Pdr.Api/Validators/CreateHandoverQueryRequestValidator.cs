using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Models.Handover;

namespace Pdr.Api.Validators;

public class CreateHandoverQueryRequestValidator : AbstractValidator<CreateHandoverQueryRequest>
{
    public CreateHandoverQueryRequestValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty().WithMessage(HandoverQueryConstants.QuestionRequired)
            .MaximumLength(HandoverQueryConstants.MaxQuestionLength).WithMessage(HandoverQueryConstants.QuestionMaxLength);

        RuleFor(x => x.FieldSection)
            .Must(section => Enum.TryParse<HandoverQueryFieldSectionEnum>(section, out _))
            .WithMessage(HandoverQueryConstants.FieldSectionInvalid);

        RuleFor(x => x.ReferenceId)
            .NotNull().WithMessage(HandoverQueryConstants.ReferenceIdRequired)
            .When(x => HandoverQueryFieldSectionNames.SectionsRequiringReference.Contains(x.FieldSection));

        RuleFor(x => x.ReferenceId)
            .Null().WithMessage(HandoverQueryConstants.ReferenceIdNotAllowed)
            .When(x => !HandoverQueryFieldSectionNames.SectionsRequiringReference.Contains(x.FieldSection));
    }
}
