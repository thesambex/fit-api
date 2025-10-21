using FitApi.Core.Domain.Common;
using FitApi.Core.Domain.Professionals.DTOs;
using FitApi.Core.Domain.Professionals.Models;
using FitApi.Core.Exceptions;
using FitApi.Core.Repositories;
using FitApi.Core.Repositories.Professionals;
using FitApi.Core.Services;
using Microsoft.Extensions.Logging;

namespace FitApi.Infra.Services;

public class ProfessionalService(
    IProfessionalRepository professionalRepository,
    IUnitOfWork unitOfWork,
    ILogger<ProfessionalService> logger
) : IProfessionalService
{
    public async Task<ProfessionalResponse> Create(CreateProfessionalRequest requestBody)
    {
        var professional = new Professional(requestBody.Name);

        await professionalRepository.Add(professional);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Professional {id} created", professional.ExternalId);

        return new ProfessionalResponse(professional.ExternalId, professional.Name);
    }

    public async Task<ProfessionalResponse> FindById(Guid id)
    {
        var professional = await professionalRepository.FindByExternalId(id);
        if (professional == null)
        {
            throw new NotFoundException("Professional not found");
        }

        return new ProfessionalResponse(professional.ExternalId, professional.Name);
    }

    public async Task<PaginationResponse<ProfessionalResponse>> FindAll(int pageIndex, int pageSize)
    {
        if (pageIndex <= 0 || pageSize <= 0)
        {
            throw new PaginationException("Invalid pagination parameters");
        }

        var records = await professionalRepository.FindAll(pageIndex, pageSize);
        var totalCount = await professionalRepository.Count();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var responseData = records.Select(e => new ProfessionalResponse(e.ExternalId, e.Name)).ToList();
        
        return new  PaginationResponse<ProfessionalResponse>(responseData, pageIndex, pageSize, totalPages, totalCount);
    }

    public async Task<PaginationResponse<ProfessionalResponse>> Search(string q, int pageIndex, int pageSize)
    {
        if (pageIndex <= 0 || pageSize <= 0)
        {
            throw new PaginationException("Invalid pagination parameters");
        }

        var searchQuery = q.Replace(" ", "%");

        var records = await professionalRepository.Search(searchQuery, pageIndex, pageSize);
        var totalCount = await professionalRepository.SearchCount(searchQuery);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        var responseData = records.Select(e => new ProfessionalResponse(e.ExternalId, e.Name)).ToList();
        
        return new  PaginationResponse<ProfessionalResponse>(responseData, pageIndex, pageSize, totalPages, totalCount);
    }
}