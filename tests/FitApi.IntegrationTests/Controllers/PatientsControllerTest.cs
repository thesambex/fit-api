using System.Net;
using System.Net.Http.Json;
using FitApi.Core.Domain.Common;
using FitApi.Core.Domain.Patients.DTOs;
using FitApi.Core.Domain.Patients.Enums;
using FitApi.Core.Domain.Patients.Models;
using FitApi.IntegrationTests.Configuration;

namespace FitApi.IntegrationTests.Controllers;

public class PatientsControllerTest : IClassFixture<PgContainerFixture>
{
    private readonly PgContainerFixture _pgFixture;

    private readonly HttpClient _client;

    public PatientsControllerTest(PgContainerFixture containerFixture)
    {
        _pgFixture = containerFixture;

        var webFactory = new IntegrationTestWebAppFactory(_pgFixture);

        _client = webFactory.CreateClient();
    }

    [Fact(DisplayName = "Create with valid parameters should returns created")]
    public async Task Create_With_Valid_Parameters_Should_returns_Created()
    {
        var requestBody = new CreatePatientRequest("John Doe", new DateOnly(1980, 8, 3), BirthGenres.Male);

        var response = await _client.PostAsJsonAsync("api/patients", requestBody);
        response.EnsureSuccessStatusCode();

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Create with invalid parameters should returns bad request")]
    public async Task Create_With_Invalid_Parameters_Should_returns_BadRequest()
    {
        var requestBody = new CreatePatientRequest("", new DateOnly(1980, 8, 3), BirthGenres.Male);

        var response = await _client.PostAsJsonAsync("api/patients", requestBody);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Find by id with existing patient should returns ok")]
    public async Task FindById_With_Existing_Patient_Should_returns_Ok()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var patient = new Patient("Jenifer", new DateOnly(1992, 3, 5), BirthGenres.Female);

        await dbContext.Patients.AddAsync(patient);
        await dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"api/patients/{patient.ExternalId}");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadFromJsonAsync<PatientResponse>(TestUtils.SerializationOptions());

        Assert.NotNull(responseBody);
        Assert.Equal(patient.ExternalId, responseBody.Id);
        Assert.Equal(patient.BirthGenre, responseBody.BirthGenre);
        Assert.Equal(patient.Name, responseBody.Name);
        Assert.Equal(patient.BirthDate, responseBody.BirthDate);

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Find by id with not existing patient should returns not found")]
    public async Task FindById_With_NotExisting_Patient_Should_returns_NotFound()
    {
        var response = await _client.GetAsync($"api/patients/{Guid.Empty}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Find all with valid pagination parameters should returns ok")]
    public async Task FindAll_With_Valid_Pagination_Parameters_Should_returns_Ok()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var patients = new List<Patient>
        {
            new("John", new DateOnly(1980, 8, 3), BirthGenres.Male),
            new("Alex", new DateOnly(1982, 3, 5), BirthGenres.Female),
        };

        await dbContext.Patients.AddRangeAsync(patients);
        await dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"api/patients/all?pageIndex=1&pageSize=25");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadFromJsonAsync<PaginationResponse<PatientResponse>>(
            TestUtils.SerializationOptions());

        Assert.NotNull(responseBody);
        Assert.Equal(1, responseBody.PageIndex);
        Assert.Equal(25, responseBody.PageSize);
        Assert.Equal(patients.Count, responseBody.TotalCount);
        Assert.Equal(1, responseBody.TotalPages);
        Assert.False(responseBody.HasNextPage);
        Assert.False(responseBody.HasPreviousPage);
        Assert.NotNull(responseBody.Data);

        Assert.Collection(responseBody.Data,
            item1 =>
            {
                Assert.NotNull(item1);
                Assert.Equal(patients[1].ExternalId, item1.Id);
                Assert.Equal(patients[1].Name, item1.Name);
                Assert.Equal(patients[1].BirthGenre, item1.BirthGenre);
                Assert.Equal(patients[1].BirthDate, item1.BirthDate);
            },
            item2 =>
            {
                Assert.NotNull(item2);
                Assert.Equal(patients[0].ExternalId, item2.Id);
                Assert.Equal(patients[0].Name, item2.Name);
                Assert.Equal(patients[0].BirthGenre, item2.BirthGenre);
                Assert.Equal(patients[0].BirthDate, item2.BirthDate);
            });

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Find all with invalid pagination parameters should returns bad request")]
    public async Task FindAll_With_Invalid_Pagination_Parameters_Should_returns_BadRequest()
    {
        var response = await _client.GetAsync("api/patients/all?pageIndex=0&pageSize=25");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Search with valid pagination parameters should returns ok")]
    public async Task Search_With_Valid_Pagination_Parameters_Should_returns_Ok()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var patients = new List<Patient>
        {
            new("John D Roger", new DateOnly(1980, 8, 3), BirthGenres.Male),
            new("Alex D Teach", new DateOnly(1982, 3, 5), BirthGenres.Female),
        };

        await dbContext.Patients.AddRangeAsync(patients);
        await dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"api/patients/search?q=D&pageIndex=1&pageSize=25");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadFromJsonAsync<PaginationResponse<PatientResponse>>(
            TestUtils.SerializationOptions());

        Assert.NotNull(responseBody);
        Assert.Equal(1, responseBody.PageIndex);
        Assert.Equal(25, responseBody.PageSize);
        Assert.Equal(patients.Count, responseBody.TotalCount);
        Assert.Equal(1, responseBody.TotalPages);
        Assert.False(responseBody.HasNextPage);
        Assert.False(responseBody.HasPreviousPage);
        Assert.NotNull(responseBody.Data);

        Assert.Collection(responseBody.Data,
            item1 =>
            {
                Assert.NotNull(item1);
                Assert.Equal(patients[1].ExternalId, item1.Id);
                Assert.Equal(patients[1].Name, item1.Name);
                Assert.Equal(patients[1].BirthGenre, item1.BirthGenre);
                Assert.Equal(patients[1].BirthDate, item1.BirthDate);
            },
            item2 =>
            {
                Assert.NotNull(item2);
                Assert.Equal(patients[0].ExternalId, item2.Id);
                Assert.Equal(patients[0].Name, item2.Name);
                Assert.Equal(patients[0].BirthGenre, item2.BirthGenre);
                Assert.Equal(patients[0].BirthDate, item2.BirthDate);
            });

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Search with invalid pagination parameters should returns bad request")]
    public async Task Search_With_Invalid_Pagination_Parameters_Should_returns_BadRequest()
    {
        var response = await _client.GetAsync("api/patients/search?q=D&pageIndex=0&pageSize=25");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Delete with existing patient should returns no content")]
    public async Task Delete_With_Existing_Patient_Should_returns_NoContent()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var patient = new Patient("Alex", DateOnly.MinValue, BirthGenres.Female);

        await dbContext.Patients.AddAsync(patient);
        await dbContext.SaveChangesAsync();

        var response = await _client.DeleteAsync($"api/patients/{patient.ExternalId}");
        response.EnsureSuccessStatusCode();

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Delete with not existing patient should returns not found")]
    public async Task Delete_With_NotExisting_Patient_Should_returns_NotFound()
    {
        var response = await _client.DeleteAsync($"api/patients/{Guid.Empty}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Update patient with invalid parameters should returns bad request")]
    public async Task Update_Patient_With_Invalid_Parameters_Should_returns_BadRequest()
    {
        var requestBody = new UpdatePatientRequest("", DateOnly.MinValue, BirthGenres.Male);

        var response = await _client.PutAsJsonAsync($"api/patients/{Guid.Empty}", requestBody);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Update with existing patient should returns ok")]
    public async Task Update_With_Existing_Patient_Should_returns_Ok()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var patient = new Patient("Alex", DateOnly.MinValue, BirthGenres.Female);

        await dbContext.Patients.AddAsync(patient);
        await dbContext.SaveChangesAsync();

        var requestBody = new UpdatePatientRequest("Alexis", new DateOnly(1991, 2, 1), BirthGenres.Male);

        var response = await _client.PutAsJsonAsync($"api/patients/{patient.ExternalId}", requestBody);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadFromJsonAsync<PatientResponse>(TestUtils.SerializationOptions());

        Assert.NotNull(responseBody);
        Assert.Equal(patient.ExternalId, responseBody.Id);
        Assert.Equal(requestBody.Name, responseBody.Name);
        Assert.Equal(requestBody.BirthDate, responseBody.BirthDate);
        Assert.Equal(requestBody.BirthGenre, responseBody.BirthGenre);

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Update with not existing patient should returns not found")]
    public async Task Update_With_NotExisting_Patient_Should_returns_NotFound()
    {
        var requestBody = new UpdatePatientRequest("Alexis", new DateOnly(1991, 2, 1), BirthGenres.Male);

        var response = await _client.PutAsJsonAsync($"api/patients/{Guid.Empty}", requestBody);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}