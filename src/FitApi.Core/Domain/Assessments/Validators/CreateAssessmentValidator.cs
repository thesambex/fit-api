using FitApi.Core.Domain.Assessments.DTOs;
using FluentValidation;

namespace FitApi.Core.Domain.Assessments.Validators;

public class CreateAssessmentValidator : AbstractValidator<CreateAssessmentRequest>
{
    public CreateAssessmentValidator()
    {
        RuleFor(x => x.ProfessionalId).NotNull();
        RuleFor(x => x.PatientId).NotNull();
        RuleFor(x => x.Height).NotNull().GreaterThan(0m);
        RuleFor(x => x.Weight).NotNull().GreaterThan(0m);
        RuleFor(x => x.Folds).NotNull();
        RuleFor(x => x.Folds.Sum()).GreaterThan(0m);
    }
}