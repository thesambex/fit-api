using FitApi.Core.Domain.Assessments.DTOs;
using FitApi.Core.Domain.Common;

namespace FitApi.Core.Services;

public interface IAssessmentService
{
    Task<AssessmentResponse> Create(CreateAssessmentRequest requestBody);
    Task<AssessmentResponse> FindById(Guid id);
    Task<PaginationResponse<AssessmentBriefResponse>> FindAllByPatient(Guid patientId, int pageIndex, int pageSize);
    Task<AssessmentResponse> Update(Guid id, UpdateAssessmentRequest requestBody);
    Task Delete(Guid id);
}