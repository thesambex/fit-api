using FitApi.Core.Domain.Assessments.DTOs;
using FitApi.Core.Domain.Assessments.Models;
using FitApi.Core.Domain.Common;
using FitApi.Core.Domain.Patients.DTOs;
using FitApi.Core.Domain.Professionals.DTOs;
using FitApi.Core.Exceptions;
using FitApi.Core.Repositories;
using FitApi.Core.Repositories.Assessments;
using FitApi.Core.Repositories.Patients;
using FitApi.Core.Repositories.Professionals;
using FitApi.Core.Services;
using Microsoft.Extensions.Logging;

namespace FitApi.Infra.Services;

public class AssessmentService(
    IProfessionalRepository professionalRepository,
    IPatientRepository patientRepository,
    IBodyAssessmentRepository bodyAssessmentRepository,
    IBodyAssessmentSkinFoldsRepository bodyAssessmentSkinFoldsRepository,
    IUnitOfWork unitOfWork,
    ILogger<AssessmentService> logger
) : IAssessmentService
{
    public async Task<AssessmentResponse> Create(CreateAssessmentRequest requestBody)
    {
        try
        {
            var patient = await patientRepository.FindByExternalId(requestBody.PatientId);
            if (patient == null)
            {
                throw new NotFoundException("Patient not found")
                {
                    Data = { ["participant"] = "patient" }
                };
            }

            var professional = await professionalRepository.FindByExternalId(requestBody.ProfessionalId);
            if (professional == null)
            {
                throw new NotFoundException("Patient not found")
                {
                    Data = { ["participant"] = "professional" }
                };
            }

            await unitOfWork.BeginAsync();

            var skinFolds = new SkinFolds
            {
                Triceps = requestBody.Folds.Triceps,
                Biceps = requestBody.Folds.Biceps,
                Subscapular = requestBody.Folds.Subscapular,
                Suprailiac = requestBody.Folds.Suprailiac,
                MedianAxillary = requestBody.Folds.MedianAxillary,
                Abs = requestBody.Folds.Abs,
                Thoracic = requestBody.Folds.Thoracic,
                Supraspinal = requestBody.Folds.Supraspinal,
                Thigh = requestBody.Folds.Thigh,
                Calf = requestBody.Folds.Calf
            };

            var bodyAssessment = new BodyAssessment(patient.Id, professional.Id, patient.GetAge(), patient.BirthGenre,
                requestBody.Height, requestBody.Weight, skinFolds);

            await bodyAssessmentRepository.Add(bodyAssessment);
            await unitOfWork.SaveChangesAsync();

            var bodyAssessmentSkinFolds = new BodyAssessmentSkinFolds(bodyAssessment.Id, skinFolds);

            await bodyAssessmentSkinFoldsRepository.Add(bodyAssessmentSkinFolds);
            await unitOfWork.SaveChangesAsync();

            await unitOfWork.CommitAsync();

            logger.LogInformation("Created assessment {id}", bodyAssessment.ExternalId);

            return new AssessmentResponse(
                bodyAssessment.ExternalId,
                new PatientResponse(patient.ExternalId, patient.Name, patient.BirthDate, patient.BirthGenre),
                new ProfessionalResponse(professional.ExternalId, professional.Name),
                bodyAssessment.Height, bodyAssessment.Weight, requestBody.Folds, requestBody.Folds.Sum()
            );
        }
        catch (Exception _)
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<AssessmentResponse> FindById(Guid id)
    {
        var bodyAssessment = await bodyAssessmentRepository.FindByExternalId(id);
        if (bodyAssessment == null)
        {
            throw new NotFoundException("Body assessment not found");
        }

        var patient = bodyAssessment.Patient!;
        var professional = bodyAssessment.Professional!;

        var skinFolds = new SkinFoldsReqResp(
            bodyAssessment.AssessmentSkinFolds?.Triceps ?? 0m,
            bodyAssessment.AssessmentSkinFolds?.Biceps ?? 0m,
            bodyAssessment.AssessmentSkinFolds?.Subscapular ?? 0m,
            bodyAssessment.AssessmentSkinFolds?.Suprailiac ?? 0m,
            bodyAssessment.AssessmentSkinFolds?.MedianAxillary ?? 0m,
            bodyAssessment.AssessmentSkinFolds?.Abs ?? 0m,
            bodyAssessment.AssessmentSkinFolds?.Thoracic ?? 0m,
            bodyAssessment.AssessmentSkinFolds?.Supraspinal ?? 0m,
            bodyAssessment.AssessmentSkinFolds?.Thigh ?? 0m,
            bodyAssessment.AssessmentSkinFolds?.Calf ?? 0m
        );

        return new AssessmentResponse(
            bodyAssessment.ExternalId,
            new PatientResponse(patient.ExternalId, patient.Name, patient.BirthDate, patient.BirthGenre),
            new ProfessionalResponse(professional.ExternalId, professional.Name),
            bodyAssessment.Height, bodyAssessment.Weight, skinFolds, skinFolds.Sum()
        );
    }

    public async Task<PaginationResponse<AssessmentBriefResponse>> FindAllByPatient(
        Guid patientId,
        int pageIndex,
        int pageSize
    )
    {
        if (pageIndex <= 0 || pageSize <= 0)
        {
            throw new PaginationException("Invalid pagination parameters");
        }

        var records = await bodyAssessmentRepository.FindAllByPatientId(patientId, pageIndex, pageSize);
        var totalCount = await bodyAssessmentRepository.CountByPatientId(patientId);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var responseData = records.Select(e => new AssessmentBriefResponse(e.Id,
            DateOnly.FromDateTime(e.Date.LocalDateTime), e.Weight,
            e.ProfessionalName)).ToList();

        return new PaginationResponse<AssessmentBriefResponse>(responseData, pageIndex, pageSize, totalPages,
            totalCount);
    }

    public async Task<AssessmentResponse> Update(Guid id, UpdateAssessmentRequest requestBody)
    {
        var bodyAssessment = await bodyAssessmentRepository.FindByExternalId(id);
        if (bodyAssessment == null)
        {
            throw new NotFoundException("Body assessment not found");
        }

        var patient = bodyAssessment.Patient!;
        var professional = bodyAssessment.Professional!;

        var skinFolds = new SkinFolds
        {
            Triceps = requestBody.Folds.Triceps,
            Biceps = requestBody.Folds.Biceps,
            Subscapular = requestBody.Folds.Subscapular,
            Suprailiac = requestBody.Folds.Suprailiac,
            MedianAxillary = requestBody.Folds.MedianAxillary,
            Abs = requestBody.Folds.Abs,
            Thoracic = requestBody.Folds.Thoracic,
            Supraspinal = requestBody.Folds.Supraspinal,
            Thigh = requestBody.Folds.Thigh,
            Calf = requestBody.Folds.Calf
        };

        var bodyAssessmentSkinFolds = bodyAssessment.AssessmentSkinFolds;
        bodyAssessmentSkinFolds?.SetFolds(skinFolds);

        bodyAssessment.SetHeight(requestBody.Height);
        bodyAssessment.SetWeight(requestBody.Weight);
        bodyAssessment.AssessmentSkinFolds = bodyAssessmentSkinFolds;

        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Updated assessment {id}", id);

        return new AssessmentResponse(
            bodyAssessment.ExternalId,
            new PatientResponse(patient.ExternalId, patient.Name, patient.BirthDate, patient.BirthGenre),
            new ProfessionalResponse(professional.ExternalId, professional.Name),
            bodyAssessment.Height, bodyAssessment.Weight, requestBody.Folds, requestBody.Folds.Sum()
        );
    }

    public async Task Delete(Guid id)
    {
        var bodyAssessment = await bodyAssessmentRepository.FindByExternalId(id);
        if (bodyAssessment == null)
        {
            throw new NotFoundException("Body assessment not found");
        }

        await bodyAssessmentRepository.DeleteById(bodyAssessment.Id);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Deleted assessment {id}", bodyAssessment.ExternalId);
    }
}