using FitApi.Core.Domain.Professionals.DTOs;
using FluentValidation;

namespace FitApi.Core.Domain.Professionals.Validators;

public class UpdateProfessionalValidator : AbstractValidator<UpdateProfessionalRequest>
{
    public UpdateProfessionalValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
    }
}