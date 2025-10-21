using FitApi.Core.Domain.Patients.DTOs;
using FitApi.Core.Domain.Patients.Enums;
using FitApi.Core.Domain.Patients.Models;
using FitApi.Core.Exceptions;
using FitApi.Core.Repositories;
using FitApi.Core.Repositories.Patients;
using FitApi.Core.Services;
using FitApi.Infra.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace FitApi.UnitTests.Services;

public class PatientServiceTest
{
    private readonly Mock<IPatientRepository> _patientRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    private readonly IPatientService _patientService;

    public PatientServiceTest()
    {
        _patientRepository = new Mock<IPatientRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        var logger = new Mock<ILogger<PatientService>>();

        _patientService = new PatientService(_patientRepository.Object, _unitOfWork.Object, logger.Object);
    }

    [Fact(DisplayName = "Create should create a patient")]
    public async Task Create_Should_Create_Patient()
    {
        var requestBody = new CreatePatientRequest("John", DateOnly.MinValue, BirthGenres.Male);

        var response = await _patientService.Create(requestBody);

        _patientRepository.Verify(r => r.Add(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);

        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(requestBody.Name, response.Name);
        Assert.Equal(requestBody.BirthDate, response.BirthDate);
        Assert.Equal(requestBody.BirthGenre, response.BirthGenre);
    }

    [Fact(DisplayName = "Find by id with existing id should returns patient")]
    public async Task FindById_With_ExistingId_Should_returns_Patient()
    {
        var patient = new Patient("John Doe", DateOnly.MinValue, BirthGenres.Male);

        _patientRepository.Setup(r => r.FindByExternalId(patient.ExternalId, CancellationToken.None))
            .ReturnsAsync(patient);

        var response = await _patientService.FindById(patient.ExternalId);

        Assert.NotNull(response);
        Assert.Equal(patient.ExternalId, response.Id);
        Assert.Equal(patient.Name, response.Name);
        Assert.Equal(patient.BirthDate, response.BirthDate);
        Assert.Equal(patient.BirthGenre, response.BirthGenre);
    }

    [Fact(DisplayName = "Find by id with not existing id should throws not found exception")]
    public async Task FindById_With_NotExistingId_Should_throws_NotFoundException()
    {
        var exception =
            await Assert.ThrowsAsync<NotFoundException>(async () => await _patientService.FindById(Guid.Empty));

        Assert.NotNull(exception);
        Assert.Equal("Patient not found", exception.Message);
    }

    [Fact(DisplayName = "Find all with valid pagination parameters should returns paginated list")]
    public async Task FindAll_With_Valid_Pagination_Parameters_Should_returns_PaginatedList()
    {
        const int pageIndex = 1;
        const int pageSize = 25;

        var patients = new List<Patient>
        {
            new("John Doe", DateOnly.MinValue, BirthGenres.Male),
            new("Jenifer", DateOnly.MinValue, BirthGenres.Female),
        };

        _patientRepository.Setup(r => r.FindAll(pageIndex, pageSize, CancellationToken.None)).ReturnsAsync(patients);
        _patientRepository.Setup(r => r.Count(CancellationToken.None)).ReturnsAsync(patients.Count);

        var response = await _patientService.FindAll(pageIndex, pageSize);

        _patientRepository.Verify(r => r.FindAll(pageIndex, pageSize, CancellationToken.None), Times.Once);
        _patientRepository.Verify(r => r.Count(CancellationToken.None), Times.Once);

        Assert.NotNull(response);
        Assert.Equal(pageIndex, response.PageIndex);
        Assert.Equal(pageSize, response.PageSize);
        Assert.Equal(patients.Count, response.TotalCount);
        Assert.Equal(1, response.TotalPages);
        Assert.False(response.HasNextPage);
        Assert.False(response.HasPreviousPage);
        Assert.NotNull(response.Data);

        Assert.Collection(response.Data,
            item1 =>
            {
                Assert.NotNull(item1);
                Assert.Equal(patients[0].ExternalId, item1.Id);
                Assert.Equal(patients[0].Name, item1.Name);
                Assert.Equal(patients[0].BirthGenre, item1.BirthGenre);
                Assert.Equal(patients[0].BirthDate, item1.BirthDate);
            },
            item2 =>
            {
                Assert.NotNull(item2);
                Assert.Equal(patients[1].ExternalId, item2.Id);
                Assert.Equal(patients[1].Name, item2.Name);
                Assert.Equal(patients[1].BirthGenre, item2.BirthGenre);
                Assert.Equal(patients[1].BirthDate, item2.BirthDate);
            });
    }

    [Fact(DisplayName = "Find all with invalid pagination parameters should throws pagination exception")]
    public async Task FindAll_With_Invalid_Pagination_Parameters_Should_throws_PaginationException()
    {
        var exception = await Assert.ThrowsAsync<PaginationException>(async () => await _patientService.FindAll(0, 25));

        Assert.NotNull(exception);
        Assert.Equal("Invalid pagination parameters", exception.Message);
    }

    [Fact(DisplayName = "Search with valid pagination parameters should returns paginated list")]
    public async Task Search_With_Valid_Pagination_Parameters_Should_returns_PaginatedList()
    {
        const int pageIndex = 1;
        const int pageSize = 25;
        const string q = "D";

        var patients = new List<Patient>
        {
            new("John D Doe", DateOnly.MinValue, BirthGenres.Male),
            new("Jenifer D Teach", DateOnly.MinValue, BirthGenres.Female),
        };

        _patientRepository.Setup(r => r.Search(q, pageIndex, pageSize, CancellationToken.None)).ReturnsAsync(patients);
        _patientRepository.Setup(r => r.SearchCount(q, CancellationToken.None)).ReturnsAsync(patients.Count);

        var response = await _patientService.Search(q, pageIndex, pageSize);

        _patientRepository.Verify(r => r.Search(q, pageIndex, pageSize, CancellationToken.None), Times.Once);
        _patientRepository.Verify(r => r.SearchCount(q, CancellationToken.None), Times.Once);

        Assert.NotNull(response);
        Assert.Equal(pageIndex, response.PageIndex);
        Assert.Equal(pageSize, response.PageSize);
        Assert.Equal(patients.Count, response.TotalCount);
        Assert.Equal(1, response.TotalPages);
        Assert.False(response.HasNextPage);
        Assert.False(response.HasPreviousPage);
        Assert.NotNull(response.Data);

        Assert.Collection(response.Data,
            item1 =>
            {
                Assert.NotNull(item1);
                Assert.Equal(patients[0].ExternalId, item1.Id);
                Assert.Equal(patients[0].Name, item1.Name);
                Assert.Equal(patients[0].BirthGenre, item1.BirthGenre);
                Assert.Equal(patients[0].BirthDate, item1.BirthDate);
            },
            item2 =>
            {
                Assert.NotNull(item2);
                Assert.Equal(patients[1].ExternalId, item2.Id);
                Assert.Equal(patients[1].Name, item2.Name);
                Assert.Equal(patients[1].BirthGenre, item2.BirthGenre);
                Assert.Equal(patients[1].BirthDate, item2.BirthDate);
            });
    }

    [Fact(DisplayName = "Search with invalid pagination parameters should throws pagination exception")]
    public async Task Search_With_Invalid_Pagination_Parameters_Should_throws_PaginationException()
    {
        var exception = await Assert.ThrowsAsync<PaginationException>(async () =>
            await _patientService.Search(string.Empty, 0, 25));

        Assert.NotNull(exception);
        Assert.Equal("Invalid pagination parameters", exception.Message);
    }

    [Fact(DisplayName = "Delete with existing id should returns success")]
    public async Task Delete_With_ExistingId_Should_returns_Success()
    {
        var patient = new Patient("Alex", DateOnly.MinValue, BirthGenres.Female)
        {
            Id = 1
        };

        _patientRepository.Setup(r => r.FindByExternalId(patient.ExternalId, CancellationToken.None))
            .ReturnsAsync(patient);

        await _patientService.Delete(patient.ExternalId);

        _patientRepository.Verify(r => r.FindByExternalId(patient.ExternalId, CancellationToken.None), Times.Once);
        _patientRepository.Verify(r => r.DeleteById(patient.Id, CancellationToken.None));

        _unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact(DisplayName = "Delete with not existing id should throws not found exception")]
    public async Task Delete_With_NotExistingId_Should_throws_NotFoundException()
    {
        var exception =
            await Assert.ThrowsAsync<NotFoundException>(async () => await _patientService.Delete(Guid.Empty));

        Assert.NotNull(exception);
        Assert.Equal("Patient not found", exception.Message);
    }

    [Fact(DisplayName = "Update with existing id should returns success")]
    public async Task Update_With_ExistingId_Should_returns_Success()
    {
        var requestBody = new UpdatePatientRequest("Alexis", new DateOnly(1991, 2, 1), BirthGenres.Male);

        var patient = new Patient("Alex", DateOnly.MinValue, BirthGenres.Female);

        _patientRepository.Setup(r => r.FindByExternalId(patient.ExternalId, CancellationToken.None))
            .ReturnsAsync(patient);

        var response = await _patientService.Update(patient.ExternalId, requestBody);

        _patientRepository.Verify(r => r.FindByExternalId(patient.ExternalId, CancellationToken.None), Times.Once);

        _unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);

        Assert.NotNull(response);
        Assert.Equal(patient.ExternalId, response.Id);
        Assert.Equal(requestBody.Name, response.Name);
        Assert.Equal(requestBody.BirthDate, response.BirthDate);
        Assert.Equal(requestBody.BirthGenre, response.BirthGenre);
    }

    [Fact(DisplayName = "Update with not existing id should throws not found exception")]
    public async Task Update_With_NotExistingId_Should_throws_NotFoundException()
    {
        var requestBody = new UpdatePatientRequest("Alexis", new DateOnly(1991, 2, 1), BirthGenres.Male);

        var exception =
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _patientService.Update(Guid.Empty, requestBody));

        Assert.NotNull(exception);
        Assert.Equal("Patient not found", exception.Message);
    }
}