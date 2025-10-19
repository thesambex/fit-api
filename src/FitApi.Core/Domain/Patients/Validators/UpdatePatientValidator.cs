using FitApi.Core.Domain.Patients.DTOs;
using FluentValidation;

namespace FitApi.Core.Domain.Patients.Validators;

public class UpdatePatientValidator : AbstractValidator<UpdatePatientRequest>
{
    public UpdatePatientValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
    }
}