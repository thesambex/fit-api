namespace FitApi.Core.Domain.Assessments.DTOs;

public record AssessmentResult(
    decimal BodyDensity,
    decimal BodyFatPercent,
    decimal BodyFarWeight,
    decimal LeanMass,
    decimal Bmi,
    SkinFoldsReqResp SkinFolds,
    decimal FoldsSum
);