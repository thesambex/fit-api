using FitApi.Core.Domain.Common;
using FitApi.Core.Domain.Patients.DTOs;
using FitApi.Core.Domain.Patients.Models;
using FitApi.Core.Exceptions;
using FitApi.Core.Repositories;
using FitApi.Core.Repositories.Patients;
using FitApi.Core.Services;
using Microsoft.Extensions.Logging;

namespace FitApi.Infra.Services;

public class PatientService(
    IPatientRepository patientRepository,
    IUnitOfWork unitOfWork,
    ILogger<PatientService> logger
) : IPatientService
{
    public async Task<PatientResponse> Create(CreatePatientRequest requestBody)
    {
        var patient = new Patient(requestBody.Name, requestBody.BirthDate, requestBody.BirthGenre);

        await patientRepository.Add(patient);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Patient {id} created", patient.ExternalId);

        return new PatientResponse(patient.ExternalId, patient.Name, patient.BirthDate, patient.BirthGenre);
    }

    public async Task<PatientResponse> FindById(Guid id)
    {
        var patient = await patientRepository.FindByExternalId(id);
        if (patient == null)
        {
            throw new NotFoundException("Patient not found");
        }

        return new PatientResponse(patient.ExternalId, patient.Name, patient.BirthDate, patient.BirthGenre);
    }

    public async Task<PaginationResponse<PatientResponse>> FindAll(int pageIndex, int pageSize)
    {
        if (pageIndex <= 0 || pageSize <= 0)
        {
            throw new PaginationException("Invalid pagination parameters");
        }

        var records = await patientRepository.FindAll(pageIndex, pageSize);
        var totalCount = await patientRepository.Count();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var responseData = records.Select(e => new PatientResponse(e.ExternalId, e.Name, e.BirthDate, e.BirthGenre))
            .ToList();

        return new PaginationResponse<PatientResponse>(responseData, pageIndex, pageSize, totalPages, totalCount);
    }

    public async Task<PaginationResponse<PatientResponse>> Search(string q, int pageIndex, int pageSize)
    {
        if (pageIndex <= 0 || pageSize <= 0)
        {
            throw new PaginationException("Invalid pagination parameters");
        }

        var searchQuery = q.Replace(" ", "%");

        var records = await patientRepository.Search(searchQuery, pageIndex, pageSize);
        var totalCount = await patientRepository.SearchCount(searchQuery);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var responseData = records.Select(e => new PatientResponse(e.ExternalId, e.Name, e.BirthDate, e.BirthGenre))
            .ToList();

        return new PaginationResponse<PatientResponse>(responseData, pageIndex, pageSize, totalPages, totalCount);
    }

    public async Task<PatientResponse> Update(Guid id, UpdatePatientRequest requestBody)
    {
        var patient = await patientRepository.FindByExternalId(id);
        if (patient == null)
        {
            throw new NotFoundException("Patient not found");
        }
        
        patient.SetName(requestBody.Name);
        patient.SetBirthDate(requestBody.BirthDate);
        patient.SetBirthGenre(requestBody.BirthGenre);
        
        await unitOfWork.SaveChangesAsync();
        
        return new PatientResponse(id, patient.Name, patient.BirthDate, patient.BirthGenre);
    }

    public async Task Delete(Guid id)
    {
        var patient = await patientRepository.FindByExternalId(id);
        if (patient == null)
        {
            throw new NotFoundException("Patient not found");
        }

        await patientRepository.DeleteById(patient.Id);
        await unitOfWork.SaveChangesAsync();
    }
}