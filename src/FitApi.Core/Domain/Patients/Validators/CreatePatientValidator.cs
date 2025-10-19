using FitApi.Core.Domain.Patients.DTOs;
using FluentValidation;

namespace FitApi.Core.Domain.Patients.Validators;

public sealed class CreatePatientValidator : AbstractValidator<CreatePatientRequest>
{
    public CreatePatientValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
    }
}