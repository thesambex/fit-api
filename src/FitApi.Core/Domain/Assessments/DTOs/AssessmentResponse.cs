using FitApi.Core.Domain.Patients.DTOs;
using FitApi.Core.Domain.Professionals.DTOs;

namespace FitApi.Core.Domain.Assessments.DTOs;

public record AssessmentResponse(
    Guid Id,
    PatientResponse PatientResponse,
    ProfessionalResponse ProfessionalResponse,
    decimal Height,
    decimal Weight,
    SkinFoldsReqResp Folds,
    decimal FoldsSum
);