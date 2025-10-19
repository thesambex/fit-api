using FitApi.Core.Domain.Patients.Enums;

namespace FitApi.Core.Domain.Patients.DTOs;

public record PatientResponse(Guid Id, string Name, DateOnly BirthDate, BirthGenres BirthGenre);