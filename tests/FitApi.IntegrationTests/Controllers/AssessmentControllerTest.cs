using System.Net;
using System.Net.Http.Json;
using FitApi.Core.Domain.Assessments.DTOs;
using FitApi.Core.Domain.Assessments.Models;
using FitApi.Core.Domain.Common;
using FitApi.Core.Domain.Patients.Enums;
using FitApi.Core.Domain.Patients.Models;
using FitApi.Core.Domain.Professionals.Models;
using FitApi.IntegrationTests.Configuration;

namespace FitApi.IntegrationTests.Controllers;

public class AssessmentControllerTest : IClassFixture<PgContainerFixture>
{
    private readonly PgContainerFixture _pgFixture;

    private readonly HttpClient _client;

    public AssessmentControllerTest(PgContainerFixture pgFixture)
    {
        _pgFixture = pgFixture;

        var webFactory = new IntegrationTestWebAppFactory(_pgFixture);

        _client = webFactory.CreateClient();
    }

    [Fact(DisplayName = "Create with valid parameters should returns created")]
    public async Task Create_With_Valid_Parameters_Should_returns_Created()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var patient = new Patient("Jenifer", new DateOnly(1992, 3, 5), BirthGenres.Female);
        var professional = new Professional("Mary");

        await dbContext.Patients.AddAsync(patient);
        await dbContext.Professionals.AddAsync(professional);

        await dbContext.SaveChangesAsync();

        var requestBody = new CreateAssessmentRequest(patient.ExternalId, professional.ExternalId, 1.80m, 90.7m,
            new SkinFoldsReqResp(1.2m, 0, 0, 0, 0, 0, 0, 0, 0, 0));

        var response = await _client.PostAsJsonAsync("api/assessments", requestBody);
        response.EnsureSuccessStatusCode();

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Create with invalid parameters should returns bad request")]
    public async Task Create_With_Invalid_Parameters_Should_returns_BadRequest()
    {
        var requestBody = new CreateAssessmentRequest(Guid.Empty, Guid.Empty, 0m, 0m,
            new SkinFoldsReqResp(0m, 0, 0, 0, 0, 0, 0, 0, 0, 0));

        var response = await _client.PostAsJsonAsync("api/assessments", requestBody);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Create with not existing patient should returns not found")]
    public async Task Create_With_NotExisting_Patient_Should_returns_NotFound()
    {
        var requestBody = new CreateAssessmentRequest(Guid.Empty, Guid.Empty, 1.5m, 87m,
            new SkinFoldsReqResp(0m, 0, 1.3m, 0, 0, 0, 0, 0, 0, 0));

        var response = await _client.PostAsJsonAsync("api/assessments", requestBody);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Create with not existing professional should returns not found")]
    public async Task Create_With_NotExisting_Professional_Should_returns_NotFound()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var patient = new Patient("Test", new DateOnly(1992, 3, 5), BirthGenres.Female);

        await dbContext.Patients.AddAsync(patient);
        await dbContext.SaveChangesAsync();

        var requestBody = new CreateAssessmentRequest(patient.ExternalId, Guid.Empty, 1.45m, 50.4m,
            new SkinFoldsReqResp(0m, 0, 0, 0, 0, 0, 0, 1.7m, 0, 0));

        var response = await _client.PostAsJsonAsync("api/assessments", requestBody);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Find by id with existing assessment should returns ok")]
    public async Task FindById_With_Existing_Assessment_Should_returns_Ok()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var patient = new Patient("Jenifer", new DateOnly(1992, 3, 5), BirthGenres.Female);
        var professional = new Professional("Mary");

        await dbContext.Patients.AddAsync(patient);
        await dbContext.Professionals.AddAsync(professional);

        await dbContext.SaveChangesAsync();

        var skinFolds = new SkinFolds { Thigh = 1.2m };
        var bodyAssessment =
            new BodyAssessment(patient.Id, professional.Id, 30, BirthGenres.Male, 1.70m, 70m, skinFolds);

        await dbContext.BodyAssessments.AddAsync(bodyAssessment);
        await dbContext.SaveChangesAsync();

        var bodyAssessmentSkinFolds = new BodyAssessmentSkinFolds(bodyAssessment.Id, skinFolds);

        await dbContext.BodyAssessmentSkinFolds.AddAsync(bodyAssessmentSkinFolds);
        await dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"api/assessments/{bodyAssessment.ExternalId}");
        response.EnsureSuccessStatusCode();

        var responseBody =
            await response.Content.ReadFromJsonAsync<AssessmentResponse>(TestUtils.SerializationOptions());

        Assert.NotNull(responseBody);
        Assert.Equal(bodyAssessment.ExternalId, responseBody.Id);
        Assert.Equal(professional.ExternalId, responseBody.ProfessionalResponse.Id);
        Assert.Equal(professional.Name, responseBody.ProfessionalResponse.Name);
        Assert.Equal(patient.ExternalId, responseBody.PatientResponse.Id);
        Assert.Equal(patient.Name, responseBody.PatientResponse.Name);
        Assert.Equal(patient.BirthDate, responseBody.PatientResponse.BirthDate);
        Assert.Equal(patient.BirthGenre, responseBody.PatientResponse.BirthGenre);
        Assert.Equal(bodyAssessment.Height, responseBody.Height);
        Assert.Equal(bodyAssessment.Weight, responseBody.Weight);
        Assert.Equal(bodyAssessment.FoldsSum, responseBody.FoldsSum);

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Find by id with not existing assessment should returns not found")]
    public async Task FindById_With_NotExisting_Assessment_Should_returns_NotFound()
    {
        var response = await _client.GetAsync($"api/assessments/{Guid.Empty}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Find all by parent id with valid pagination parameters should returns paginated list")]
    public async Task FindAllByParentId_With_Valid_Pagination_Parameters_Should_returns_PaginatedList()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var patient = new Patient("Jenifer", new DateOnly(1992, 3, 5), BirthGenres.Female);
        var professional = new Professional("Mary");

        await dbContext.Patients.AddAsync(patient);
        await dbContext.Professionals.AddAsync(professional);

        await dbContext.SaveChangesAsync();

        var skinFolds = new SkinFolds { Thigh = 1.2m };
        var bodyAssessment =
            new BodyAssessment(patient.Id, professional.Id, 30, BirthGenres.Male, 1.70m, 70m, skinFolds);

        await dbContext.BodyAssessments.AddAsync(bodyAssessment);
        await dbContext.SaveChangesAsync();

        const int pageIndex = 1;
        const int pageSize = 25;

        var response = await _client.GetAsync(
            $"api/assessments/patient/{patient.ExternalId}/all?pageIndex={pageIndex}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadFromJsonAsync<PaginationResponse<AssessmentBriefResponse>>();

        Assert.NotNull(responseBody);
        Assert.Equal(1, responseBody.PageIndex);
        Assert.Equal(25, responseBody.PageSize);
        Assert.Equal(1, responseBody.TotalCount);
        Assert.Equal(1, responseBody.TotalPages);
        Assert.False(responseBody.HasNextPage);
        Assert.False(responseBody.HasPreviousPage);
        Assert.NotNull(responseBody.Data);

        Assert.Collection(responseBody.Data, item1 =>
        {
            Assert.Equal(bodyAssessment.ExternalId, item1.Id);
            Assert.Equal(DateOnly.FromDateTime(bodyAssessment.AssessmentDate.DateTime), item1.Date);
            Assert.Equal(professional.Name, item1.ProfessionalName);
            Assert.Equal(bodyAssessment.Weight, item1.Weight);
        });

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Find all by parent id with invalid pagination parameters should returns bad request")]
    public async Task FindAllByParentId_With_Invalid_Pagination_Parameters_Should_returns_BadRequest()
    {
        const int pageIndex = 0;
        const int pageSize = 25;

        var response = await _client.GetAsync(
            $"api/assessments/patient/{Guid.Empty}/all?pageIndex={pageIndex}&pageSize={pageSize}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Update assessment with invalid parameters should returns bad request")]
    public async Task Update_Assessment_With_Invalid_Parameters_Should_returns_BadRequest()
    {
        var requestBody =
            new UpdateAssessmentRequest(0m, 0m, new SkinFoldsReqResp(0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m));

        var response = await _client.PutAsJsonAsync($"api/assessments/{Guid.Empty}", requestBody);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Update with existing assessment should returns ok")]
    public async Task Update_With_Existing_Assessment_Should_returns_Ok()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var patient = new Patient("Jenifer", new DateOnly(1992, 3, 5), BirthGenres.Female);
        var professional = new Professional("Mary");

        await dbContext.Patients.AddAsync(patient);
        await dbContext.Professionals.AddAsync(professional);

        await dbContext.SaveChangesAsync();

        var skinFolds = new SkinFolds { Thigh = 1.2m };
        var bodyAssessment =
            new BodyAssessment(patient.Id, professional.Id, 30, BirthGenres.Male, 1.70m, 70m, skinFolds);

        await dbContext.BodyAssessments.AddAsync(bodyAssessment);
        await dbContext.SaveChangesAsync();

        var bodyAssessmentSkinFolds = new BodyAssessmentSkinFolds(bodyAssessment.Id, skinFolds);

        await dbContext.BodyAssessmentSkinFolds.AddAsync(bodyAssessmentSkinFolds);
        await dbContext.SaveChangesAsync();

        var requestBody =
            new UpdateAssessmentRequest(2m, 80m, new SkinFoldsReqResp(1.1m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m));

        var response = await _client.PutAsJsonAsync($"api/assessments/{bodyAssessment.ExternalId}", requestBody);
        response.EnsureSuccessStatusCode();

        var responseBody =
            await response.Content.ReadFromJsonAsync<AssessmentResponse>(TestUtils.SerializationOptions());

        Assert.NotNull(responseBody);
        Assert.Equal(bodyAssessment.ExternalId, responseBody.Id);
        Assert.Equal(professional.ExternalId, responseBody.ProfessionalResponse.Id);
        Assert.Equal(professional.Name, responseBody.ProfessionalResponse.Name);
        Assert.Equal(patient.ExternalId, responseBody.PatientResponse.Id);
        Assert.Equal(patient.Name, responseBody.PatientResponse.Name);
        Assert.Equal(patient.BirthDate, responseBody.PatientResponse.BirthDate);
        Assert.Equal(patient.BirthGenre, responseBody.PatientResponse.BirthGenre);
        Assert.Equal(requestBody.Height, responseBody.Height);
        Assert.Equal(requestBody.Weight, responseBody.Weight);
        Assert.Equal(requestBody.Folds.Sum(), responseBody.FoldsSum);

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Update with not existing assessment should returns not found")]
    public async Task Update_With_NotExisting_Assessment_Should_returns_NotFound()
    {
        var requestBody =
            new UpdateAssessmentRequest(2m, 80m, new SkinFoldsReqResp(1.1m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m));

        var response = await _client.PutAsJsonAsync($"api/assessments/{Guid.Empty}", requestBody);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Delete with existing assessment should returns no content")]
    public async Task Delete_With_Existing_Assessment_Should_returns_NoContent()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var patient = new Patient("Jenifer", new DateOnly(1992, 3, 5), BirthGenres.Female);
        var professional = new Professional("Mary");

        await dbContext.Patients.AddAsync(patient);
        await dbContext.Professionals.AddAsync(professional);

        await dbContext.SaveChangesAsync();

        var skinFolds = new SkinFolds { Thigh = 1.2m };
        var bodyAssessment =
            new BodyAssessment(patient.Id, professional.Id, 30, BirthGenres.Male, 1.70m, 70m, skinFolds);

        await dbContext.BodyAssessments.AddAsync(bodyAssessment);
        await dbContext.SaveChangesAsync();

        var bodyAssessmentSkinFolds = new BodyAssessmentSkinFolds(bodyAssessment.Id, skinFolds);

        await dbContext.BodyAssessmentSkinFolds.AddAsync(bodyAssessmentSkinFolds);
        await dbContext.SaveChangesAsync();

        var response = await _client.DeleteAsync($"api/assessments/{bodyAssessment.ExternalId}");
        response.EnsureSuccessStatusCode();

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Delete with not existing assessment should returns not found")]
    public async Task Delete_With_NotExisting_Assessment_Should_returns_NotFound()
    {
        var response = await _client.DeleteAsync($"api/assessments/{Guid.Empty}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}