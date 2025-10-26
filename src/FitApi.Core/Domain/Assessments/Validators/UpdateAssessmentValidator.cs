using FitApi.Core.Domain.Assessments.DTOs;
using FluentValidation;

namespace FitApi.Core.Domain.Assessments.Validators;

public class UpdateAssessmentValidator : AbstractValidator<UpdateAssessmentRequest>
{
    public UpdateAssessmentValidator()
    {
        RuleFor(x => x.Height).NotNull().GreaterThan(0m);
        RuleFor(x => x.Weight).NotNull().GreaterThan(0m);
        RuleFor(x => x.Folds).NotNull();
        RuleFor(x => x.Folds.Sum()).GreaterThan(0m);
    }
}