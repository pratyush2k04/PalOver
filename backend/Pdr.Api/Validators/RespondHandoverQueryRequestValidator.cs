using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Handovers;

namespace Pdr.Api.Validators;

public class RespondHandoverQueryRequestValidator : AbstractValidator<RespondHandoverQueryRequest>
{
    public RespondHandoverQueryRequestValidator()
    {
        RuleFor(x => x.ResponseText)
            .NotEmpty().WithMessage(HandoverQueryConstants.ResponseTextRequired)
            .MaximumLength(HandoverQueryConstants.MaxResponseLength).WithMessage(HandoverQueryConstants.ResponseTextMaxLength);
    }
}
