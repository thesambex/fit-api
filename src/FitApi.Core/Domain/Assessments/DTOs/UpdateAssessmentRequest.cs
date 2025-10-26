namespace FitApi.Core.Domain.Assessments.DTOs;

public record UpdateAssessmentRequest(
    decimal Height,
    decimal Weight,
    SkinFoldsReqResp Folds
);