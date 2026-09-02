using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Handovers;

namespace Pdr.Api.Validators;

public class AddHandoverQueryMessageRequestValidator : AbstractValidator<AddHandoverQueryMessageRequest>
{
    public AddHandoverQueryMessageRequestValidator()
    {
        RuleFor(x => x.MessageText)
            .NotEmpty().WithMessage(HandoverQueryConstants.MessageTextRequired)
            .MaximumLength(HandoverQueryConstants.MaxMessageLength).WithMessage(HandoverQueryConstants.MessageTextMaxLength);
    }
}
