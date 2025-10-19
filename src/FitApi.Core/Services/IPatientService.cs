using FitApi.Core.Domain.Common;
using FitApi.Core.Domain.Patients.DTOs;

namespace FitApi.Core.Services;

public interface IPatientService
{
    Task<PatientResponse> Create(CreatePatientRequest requestBody);
    Task<PatientResponse> FindById(Guid id);
    Task<PaginationResponse<PatientResponse>> FindAll(int pageIndex, int pageSize);
    Task<PaginationResponse<PatientResponse>> Search(string q, int pageIndex, int pageSize);
    Task<PatientResponse> Update(Guid id, UpdatePatientRequest requestBody);
    Task Delete(Guid id);
}