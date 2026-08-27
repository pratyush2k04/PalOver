using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Risks;

namespace Pdr.Api.Validators;

public class CreateRiskCommentDtoValidator : AbstractValidator<CreateRiskCommentDto>
{
    public CreateRiskCommentDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(RiskConstants.Validation.CommentContentRequired)
            .MaximumLength(1000).WithMessage(RiskConstants.Validation.CommentContentMaxLength);
    }
}

public class UpdateRiskCommentDtoValidator : AbstractValidator<UpdateRiskCommentDto>
{
    public UpdateRiskCommentDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(RiskConstants.Validation.CommentContentRequired)
            .MaximumLength(1000).WithMessage(RiskConstants.Validation.CommentContentMaxLength);
    }
}
