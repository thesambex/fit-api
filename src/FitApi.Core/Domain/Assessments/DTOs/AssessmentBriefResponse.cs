namespace FitApi.Core.Domain.Assessments.DTOs;

public record AssessmentBriefResponse(Guid Id, DateOnly Date, decimal Weight, string ProfessionalName, string PatientName);