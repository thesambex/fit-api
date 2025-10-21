using FitApi.Core.Domain.Professionals.DTOs;
using FluentValidation;

namespace FitApi.Core.Domain.Professionals.Validators;

public class CreateProfessionalValidator : AbstractValidator<CreateProfessionalRequest>
{
    public CreateProfessionalValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
    }
}