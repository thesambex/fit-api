using FitApi.Core.Domain.Common;
using FitApi.Core.Domain.Professionals.DTOs;

namespace FitApi.Core.Services;

public interface IProfessionalService
{
    Task<ProfessionalResponse> Create(CreateProfessionalRequest requestBody);
    Task<ProfessionalResponse> FindById(Guid id);
    Task<PaginationResponse<ProfessionalResponse>> FindAll(int pageIndex, int pageSize);
    Task<PaginationResponse<ProfessionalResponse>> Search(string q, int pageIndex, int pageSize);
    Task<ProfessionalResponse> Update(Guid id, UpdateProfessionalRequest requestBody);
    Task Delete(Guid id);
}