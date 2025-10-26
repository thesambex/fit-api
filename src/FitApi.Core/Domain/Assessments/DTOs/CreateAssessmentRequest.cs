namespace FitApi.Core.Domain.Assessments.DTOs;

public record CreateAssessmentRequest(
    Guid PatientId,
    Guid ProfessionalId,
    decimal Height,
    decimal Weight,
    SkinFoldsReqResp Folds
);