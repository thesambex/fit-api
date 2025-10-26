using FitApi.Core.Domain.Assessments.DTOs;
using FitApi.Core.Domain.Assessments.Models;
using FitApi.Core.Domain.Patients.Enums;
using FitApi.Core.Domain.Patients.Models;
using FitApi.Core.Domain.Professionals.Models;
using FitApi.Core.Exceptions;
using FitApi.Core.Repositories;
using FitApi.Core.Repositories.Assessments;
using FitApi.Core.Repositories.Patients;
using FitApi.Core.Repositories.Professionals;
using FitApi.Core.Services;
using FitApi.Infra.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace FitApi.UnitTests.Services;

public class AssessmentServiceTest
{
    private readonly Mock<IProfessionalRepository> _professionalRepository;
    private readonly Mock<IPatientRepository> _patientRepository;
    private readonly Mock<IBodyAssessmentRepository> _bodyAssessmentRepository;
    private readonly Mock<IBodyAssessmentSkinFoldsRepository> _bodyAssessmentSkinFoldsRepository;

    private readonly Mock<IUnitOfWork> _unitOfWork;

    private readonly IAssessmentService _assessmentService;

    public AssessmentServiceTest()
    {
        _professionalRepository = new Mock<IProfessionalRepository>();
        _patientRepository = new Mock<IPatientRepository>();
        _bodyAssessmentRepository = new Mock<IBodyAssessmentRepository>();
        _bodyAssessmentSkinFoldsRepository = new Mock<IBodyAssessmentSkinFoldsRepository>();

        _unitOfWork = new Mock<IUnitOfWork>();

        var logger = new Mock<ILogger<AssessmentService>>();

        _assessmentService = new AssessmentService(_professionalRepository.Object, _patientRepository.Object,
            _bodyAssessmentRepository.Object, _bodyAssessmentSkinFoldsRepository.Object, _unitOfWork.Object,
            logger.Object);
    }

    [Fact(DisplayName = "Create should create assessment")]
    public async Task Create_Should_Create_Assessment()
    {
        var requestBody = new CreateAssessmentRequest(Guid.NewGuid(), Guid.NewGuid(), 1.80m, 90.7m,
            new SkinFoldsReqResp(1.2m, 0, 0, 0, 0, 0, 0, 0, 0, 0));

        var patient = new Patient("Test", DateOnly.MinValue, BirthGenres.Female)
        {
            ExternalId = requestBody.PatientId
        };

        var professional = new Professional("Test")
        {
            ExternalId = requestBody.ProfessionalId
        };

        _patientRepository.Setup(r => r.FindByExternalId(requestBody.PatientId, CancellationToken.None))
            .ReturnsAsync(patient);

        _professionalRepository.Setup(r => r.FindByExternalId(requestBody.ProfessionalId, CancellationToken.None))
            .ReturnsAsync(professional);

        var response = await _assessmentService.Create(requestBody);

        _patientRepository.Verify(r => r.FindByExternalId(requestBody.PatientId, CancellationToken.None), Times.Once);
        _professionalRepository.Verify(r => r.FindByExternalId(requestBody.ProfessionalId, CancellationToken.None),
            Times.Once);

        _bodyAssessmentRepository.Verify(r => r.Add(It.IsAny<BodyAssessment>(), CancellationToken.None), Times.Once);

        _bodyAssessmentSkinFoldsRepository.Verify(
            r => r.Add(It.IsAny<BodyAssessmentSkinFolds>(), CancellationToken.None), Times.Once);

        _unitOfWork.Verify(u => u.BeginAsync(CancellationToken.None), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Exactly(2));
        _unitOfWork.Verify(u => u.CommitAsync(CancellationToken.None), Times.Once);

        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(professional.ExternalId, response.ProfessionalResponse.Id);
        Assert.Equal(professional.Name, response.ProfessionalResponse.Name);
        Assert.Equal(patient.ExternalId, response.PatientResponse.Id);
        Assert.Equal(patient.Name, response.PatientResponse.Name);
        Assert.Equal(patient.BirthDate, response.PatientResponse.BirthDate);
        Assert.Equal(patient.BirthGenre, response.PatientResponse.BirthGenre);
        Assert.Equal(requestBody.Height, response.Height);
        Assert.Equal(requestBody.Weight, response.Weight);
        Assert.Equal(requestBody.Folds.Triceps, response.Folds.Triceps);
        Assert.Equal(requestBody.Folds.Thoracic, response.Folds.Thoracic);
        Assert.Equal(requestBody.Folds.Thigh, response.Folds.Thigh);
        Assert.Equal(requestBody.Folds.Subscapular, response.Folds.Subscapular);
        Assert.Equal(requestBody.Folds.Suprailiac, response.Folds.Suprailiac);
        Assert.Equal(requestBody.Folds.Supraspinal, response.Folds.Supraspinal);
        Assert.Equal(requestBody.Folds.Abs, response.Folds.Abs);
        Assert.Equal(requestBody.Folds.Biceps, response.Folds.Biceps);
        Assert.Equal(requestBody.Folds.MedianAxillary, response.Folds.MedianAxillary);
        Assert.Equal(requestBody.Folds.Calf, response.Folds.Calf);
    }

    [Fact(DisplayName = "Create with not existing patient id should throws not found exception")]
    public async Task Create_With_NotExisting_PatientId_Should_throws_NotFoundException()
    {
        var requestBody = new CreateAssessmentRequest(Guid.Empty, Guid.NewGuid(), 1.80m, 90.7m,
            new SkinFoldsReqResp(1.2m, 0, 0, 0, 0, 0, 0, 0, 0, 0));

        var exception =
            await Assert.ThrowsAsync<NotFoundException>(async () => await _assessmentService.Create(requestBody));

        Assert.NotNull(exception);
        Assert.NotNull(exception.Data);
        Assert.Equal("patient", exception.Data["participant"]);
        Assert.Equal("Patient not found", exception.Message);
    }

    [Fact(DisplayName = "Create with not existing professional id should throws not found exception")]
    public async Task Create_With_NotExisting_ProfessionalId_Should_throws_NotFoundException()
    {
        var requestBody = new CreateAssessmentRequest(Guid.NewGuid(), Guid.Empty, 1.80m, 90.7m,
            new SkinFoldsReqResp(1.2m, 0, 0, 0, 0, 0, 0, 0, 0, 0));

        var patient = new Patient("Test", DateOnly.MinValue, BirthGenres.Female)
        {
            ExternalId = requestBody.PatientId
        };

        _patientRepository.Setup(r => r.FindByExternalId(requestBody.PatientId, CancellationToken.None))
            .ReturnsAsync(patient);

        var exception =
            await Assert.ThrowsAsync<NotFoundException>(async () => await _assessmentService.Create(requestBody));

        Assert.NotNull(exception);
        Assert.NotNull(exception.Data);
        Assert.Equal("professional", exception.Data["participant"]);
        Assert.Equal("Patient not found", exception.Message);
    }

    [Fact(DisplayName = "Find by id with existing id should returns assessment")]
    public async Task FindById_With_ExistingId_Should_returns_Assessment()
    {
        var patient = new Patient("Test", DateOnly.MinValue, BirthGenres.Female)
        {
            ExternalId = Guid.NewGuid()
        };

        var professional = new Professional("Test")
        {
            ExternalId = Guid.NewGuid()
        };

        var skinFolds = new SkinFolds()
        {
            Triceps = 1.2m
        };

        var bodyAssessment = new BodyAssessment(30, BirthGenres.Male, 1.96m, 78.4m, skinFolds)
        {
            Id = 1,
            Patient = patient,
            Professional = professional,
            AssessmentSkinFolds = new BodyAssessmentSkinFolds(1, skinFolds)
        };

        _bodyAssessmentRepository.Setup(r => r.FindByExternalId(bodyAssessment.ExternalId, CancellationToken.None))
            .ReturnsAsync(bodyAssessment);

        var response = await _assessmentService.FindById(bodyAssessment.ExternalId);

        _bodyAssessmentRepository.Verify(r => r.FindByExternalId(bodyAssessment.ExternalId, CancellationToken.None),
            Times.Once);

        Assert.NotNull(response);
        Assert.Equal(bodyAssessment.ExternalId, response.Id);
        Assert.Equal(professional.ExternalId, response.ProfessionalResponse.Id);
        Assert.Equal(professional.Name, response.ProfessionalResponse.Name);
        Assert.Equal(patient.ExternalId, response.PatientResponse.Id);
        Assert.Equal(patient.Name, response.PatientResponse.Name);
        Assert.Equal(patient.BirthDate, response.PatientResponse.BirthDate);
        Assert.Equal(patient.BirthGenre, response.PatientResponse.BirthGenre);
        Assert.Equal(bodyAssessment.Height, response.Height);
        Assert.Equal(bodyAssessment.Weight, response.Weight);
        Assert.Equal(bodyAssessment.AssessmentSkinFolds.Triceps, response.Folds.Triceps);
    }

    [Fact(DisplayName = "Find by id with not existing id should throws not found exception")]
    public async Task FindById_With_NotExistingId_Should_throws_NotFoundException()
    {
        var exception =
            await Assert.ThrowsAsync<NotFoundException>(async () => await _assessmentService.FindById(Guid.Empty));

        Assert.NotNull(exception);
        Assert.Equal("Body assessment not found", exception.Message);
    }

    [Fact(DisplayName = "Find all by parent id with valid pagination parameters should returns paginated list")]
    public async Task FindAllByParentId_With_Valid_Pagination_Parameters_Should_returns_PaginatedList()
    {
        const int pageIndex = 1;
        const int pageSize = 25;
        var patientId = Guid.NewGuid();

        var assessmentBriefs = new List<BodyAssessmentBrief>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateTimeOffset.MinValue,
                PatientExternalId = patientId,
                ProfessionalName = "Alex",
                Weight = 1m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Date = DateTimeOffset.MinValue,
                PatientExternalId = patientId,
                ProfessionalName = "Alex",
                Weight = 2m
            }
        };

        _bodyAssessmentRepository
            .Setup(r => r.FindAllByPatientId(patientId, pageIndex, pageSize, CancellationToken.None))
            .ReturnsAsync(assessmentBriefs);

        _bodyAssessmentRepository.Setup(r => r.CountByPatientId(patientId, CancellationToken.None))
            .ReturnsAsync(assessmentBriefs.Count);

        var response = await _assessmentService.FindAllByPatient(patientId, pageIndex, pageSize);

        Assert.NotNull(response);
        Assert.Equal(1, response.PageIndex);
        Assert.Equal(25, response.PageSize);
        Assert.Equal(2, response.TotalCount);
        Assert.Equal(1, response.TotalPages);
        Assert.False(response.HasNextPage);
        Assert.False(response.HasPreviousPage);
        Assert.NotNull(response.Data);

        Assert.Collection(response.Data,
            item1 =>
            {
                Assert.Equal(assessmentBriefs[0].Id, item1.Id);
                Assert.Equal(DateOnly.FromDateTime(assessmentBriefs[0].Date.DateTime), item1.Date);
                Assert.Equal(assessmentBriefs[0].ProfessionalName, item1.ProfessionalName);
                Assert.Equal(assessmentBriefs[0].Weight, item1.Weight);
            },
            item2 =>
            {
                Assert.Equal(assessmentBriefs[1].Id, item2.Id);
                Assert.Equal(DateOnly.FromDateTime(assessmentBriefs[1].Date.DateTime), item2.Date);
                Assert.Equal(assessmentBriefs[1].ProfessionalName, item2.ProfessionalName);
                Assert.Equal(assessmentBriefs[1].Weight, item2.Weight);
            });
    }

    [Fact(DisplayName = "Find all by parent id with invalid pagination parameters should throws pagination exception")]
    public async Task FindAllByParentId_With_Invalid_Pagination_Parameters_Should_throws_PaginationException()
    {
        var exception = await Assert.ThrowsAsync<PaginationException>(async () =>
            await _assessmentService.FindAllByPatient(Guid.Empty, 0, 0));

        Assert.NotNull(exception);
        Assert.Equal("Invalid pagination parameters", exception.Message);
    }

    [Fact(DisplayName = "Update with existing id should returns success")]
    public async Task Update_With_ExistingId_Should_returns_Success()
    {
        var patient = new Patient("Test", DateOnly.MinValue, BirthGenres.Female)
        {
            ExternalId = Guid.NewGuid()
        };

        var professional = new Professional("Test")
        {
            ExternalId = Guid.NewGuid()
        };

        var skinFolds = new SkinFolds()
        {
            Triceps = 1.2m
        };

        var bodyAssessment = new BodyAssessment(30, BirthGenres.Male, 1.96m, 78.4m, skinFolds)
        {
            Id = 1,
            Patient = patient,
            Professional = professional,
            AssessmentSkinFolds = new BodyAssessmentSkinFolds(1, skinFolds)
        };

        _bodyAssessmentRepository.Setup(r => r.FindByExternalId(bodyAssessment.ExternalId, CancellationToken.None))
            .ReturnsAsync(bodyAssessment);

        var requestBody =
            new UpdateAssessmentRequest(2m, 80m, new SkinFoldsReqResp(1.1m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m));

        var response = await _assessmentService.Update(bodyAssessment.ExternalId, requestBody);

        _bodyAssessmentRepository.Verify(r => r.FindByExternalId(bodyAssessment.ExternalId, CancellationToken.None),
            Times.Once);

        _unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);

        Assert.NotNull(response);
        Assert.Equal(bodyAssessment.ExternalId, response.Id);
        Assert.Equal(bodyAssessment.Weight, response.Weight);
        Assert.Equal(bodyAssessment.Height, response.Height);
        Assert.Equal(professional.ExternalId, response.ProfessionalResponse.Id);
        Assert.Equal(professional.Name, response.ProfessionalResponse.Name);
        Assert.Equal(patient.ExternalId, response.PatientResponse.Id);
        Assert.Equal(patient.Name, response.PatientResponse.Name);
        Assert.Equal(patient.BirthDate, response.PatientResponse.BirthDate);
        Assert.Equal(patient.BirthGenre, response.PatientResponse.BirthGenre);
        Assert.Equal(bodyAssessment.AssessmentSkinFolds.Triceps, response.Folds.Triceps);
    }

    [Fact(DisplayName = "Update with not existing id should throws not found exception")]
    public async Task Update_With_NotExistingId_Should_throws_NotFoundException()
    {
        var requestBody =
            new UpdateAssessmentRequest(2m, 80m, new SkinFoldsReqResp(1.1m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m));

        var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _assessmentService.Update(Guid.Empty, requestBody));

        Assert.NotNull(exception);
        Assert.Equal("Body assessment not found", exception.Message);
    }

    [Fact(DisplayName = "Delete with existing id should returns success")]
    public async Task Delete_With_ExistingId_Should_returns_Success()
    {
        var patient = new Patient("Test", DateOnly.MinValue, BirthGenres.Female)
        {
            ExternalId = Guid.NewGuid()
        };

        var professional = new Professional("Test")
        {
            ExternalId = Guid.NewGuid()
        };

        var skinFolds = new SkinFolds()
        {
            Triceps = 1.2m
        };

        var bodyAssessment = new BodyAssessment(30, BirthGenres.Male, 1.96m, 78.4m, skinFolds)
        {
            Id = 1,
            Patient = patient,
            Professional = professional,
            AssessmentSkinFolds = new BodyAssessmentSkinFolds(1, skinFolds)
        };

        _bodyAssessmentRepository.Setup(r => r.FindByExternalId(bodyAssessment.ExternalId, CancellationToken.None))
            .ReturnsAsync(bodyAssessment);

        await _assessmentService.Delete(bodyAssessment.ExternalId);

        _bodyAssessmentRepository.Verify(r => r.FindByExternalId(bodyAssessment.ExternalId, CancellationToken.None),
            Times.Once);
        _bodyAssessmentRepository.Verify(r => r.DeleteById(bodyAssessment.Id, CancellationToken.None), Times.Once);

        _unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact(DisplayName = "Delete with not existing id should throws not found exception")]
    public async Task Delete_With_NotExistingId_Should_throws_NotFoundException()
    {
        var exception =
            await Assert.ThrowsAsync<NotFoundException>(async () => await _assessmentService.Delete(Guid.Empty));
        
        Assert.NotNull(exception);
        Assert.Equal("Body assessment not found", exception.Message);
    }
}