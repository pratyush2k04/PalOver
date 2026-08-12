using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Risks;

namespace Pdr.Api.Validators;

public class CreateRiskDtoValidator : AbstractValidator<CreateRiskDto>
{
    public CreateRiskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(RiskConstants.Validation.TitleRequired)
            .MaximumLength(300).WithMessage(RiskConstants.Validation.TitleMaxLength);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(RiskConstants.Validation.DescriptionRequired)
            .MaximumLength(2000).WithMessage(RiskConstants.Validation.DescriptionMaxLength);

        RuleFor(x => x.Impact)
            .IsInEnum().WithMessage(RiskConstants.Validation.ImpactInvalid);

        RuleFor(x => x.Likelihood)
            .IsInEnum().WithMessage(RiskConstants.Validation.LikelihoodInvalid);

        RuleFor(x => x.Mitigation)
            .MaximumLength(2000).WithMessage(RiskConstants.Validation.MitigationMaxLength);
    }
}

public class UpdateRiskDtoValidator : AbstractValidator<UpdateRiskDto>
{
    public UpdateRiskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(RiskConstants.Validation.TitleRequired)
            .MaximumLength(300).WithMessage(RiskConstants.Validation.TitleMaxLength);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(RiskConstants.Validation.DescriptionRequired)
            .MaximumLength(2000).WithMessage(RiskConstants.Validation.DescriptionMaxLength);

        RuleFor(x => x.Impact)
            .IsInEnum().WithMessage(RiskConstants.Validation.ImpactInvalid);

        RuleFor(x => x.Likelihood)
            .IsInEnum().WithMessage(RiskConstants.Validation.LikelihoodInvalid);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage(RiskConstants.Validation.StatusInvalid);

        RuleFor(x => x.Mitigation)
            .MaximumLength(2000).WithMessage(RiskConstants.Validation.MitigationMaxLength);
    }
}

public class CloseRiskDtoValidator : AbstractValidator<CloseRiskDto>
{
    public CloseRiskDtoValidator()
    {
        RuleFor(x => x.ResolutionNote)
            .NotEmpty().WithMessage(RiskConstants.Validation.ResolutionNoteRequired)
            .MaximumLength(2000).WithMessage(RiskConstants.Validation.ResolutionNoteMaxLength);
    }
}
