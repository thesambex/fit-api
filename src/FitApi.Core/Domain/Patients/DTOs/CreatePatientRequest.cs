using FitApi.Core.Domain.Patients.Enums;

namespace FitApi.Core.Domain.Patients.DTOs;

public record CreatePatientRequest(string Name, DateOnly BirthDate, BirthGenres BirthGenre);