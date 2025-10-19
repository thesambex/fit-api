using FitApi.Core.Domain.Patients.Enums;

namespace FitApi.Core.Domain.Patients.DTOs;

public record UpdatePatientRequest(string Name, DateOnly BirthDate, BirthGenres BirthGenre);