using System.Net;
using System.Net.Http.Json;
using FitApi.Core.Domain.Common;
using FitApi.Core.Domain.Professionals.DTOs;
using FitApi.Core.Domain.Professionals.Models;
using FitApi.IntegrationTests.Configuration;

namespace FitApi.IntegrationTests.Controllers;

public class ProfessionalsControllerTest : IClassFixture<PgContainerFixture>
{
    private readonly PgContainerFixture _pgFixture;

    private readonly HttpClient _client;

    public ProfessionalsControllerTest(PgContainerFixture containerFixture)
    {
        _pgFixture = containerFixture;

        var webFactory = new IntegrationTestWebAppFactory(_pgFixture);

        _client = webFactory.CreateClient();
    }

    [Fact(DisplayName = "Create with valid parameters should returns created")]
    public async Task Create_With_Valid_Parameters_Should_returns_Created()
    {
        var requestBody = new CreateProfessionalRequest("Coy Smith");

        var response = await _client.PostAsJsonAsync("api/professionals", requestBody);
        response.EnsureSuccessStatusCode();

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Create with invalid parameters should returns bad request")]
    public async Task Create_With_Invalid_Parameters_Should_returns_BadRequest()
    {
        var requestBody = new CreateProfessionalRequest("");

        var response = await _client.PostAsJsonAsync("api/professionals", requestBody);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Find by id with existing professional should returns ok")]
    public async Task FindById_With_Existing_Professional_Should_returns_Ok()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var professional = new Professional("Coy Smith");

        await dbContext.Professionals.AddAsync(professional);
        await dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"api/professionals/{professional.ExternalId}");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadFromJsonAsync<ProfessionalResponse>();

        Assert.NotNull(responseBody);
        Assert.Equal(professional.ExternalId, responseBody.Id);
        Assert.Equal(professional.Name, responseBody.Name);

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Find by id with not existing professional should returns not found")]
    public async Task FindById_With_NotExisting_Professional_Should_returns_NotFound()
    {
        var response = await _client.GetAsync($"api/professionals/{Guid.Empty}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Find all with valid pagination parameters should returns ok")]
    public async Task FindAll_With_Valid_Pagination_Parameters_Should_returns_Ok()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var professionals = new List<Professional>()
        {
            new("Marshall D Fenders"),
            new("Marshall D Teach"),
        };

        await dbContext.Professionals.AddRangeAsync(professionals);
        await dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"api/professionals/all");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadFromJsonAsync<PaginationResponse<ProfessionalResponse>>();

        Assert.NotNull(responseBody);
        Assert.Equal(1, responseBody.PageIndex);
        Assert.Equal(25, responseBody.PageSize);
        Assert.Equal(professionals.Count, responseBody.TotalCount);
        Assert.Equal(1, responseBody.TotalPages);
        Assert.False(responseBody.HasNextPage);
        Assert.False(responseBody.HasPreviousPage);
        Assert.NotNull(responseBody.Data);

        Assert.Collection(responseBody.Data,
            item1 =>
            {
                Assert.NotNull(item1);
                Assert.Equal(professionals[0].ExternalId, item1.Id);
                Assert.Equal(professionals[0].Name, item1.Name);
            },
            item2 =>
            {
                Assert.NotNull(item2);
                Assert.Equal(professionals[1].ExternalId, item2.Id);
                Assert.Equal(professionals[1].Name, item2.Name);
            });

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Find all with invalid pagination parameters should returns bad request")]
    public async Task FindAll_With_Invalid_Pagination_Parameters_Should_returns_BadRequest()
    {
        var response = await _client.GetAsync($"api/professionals/all?pageIndex=0&pageSize=25");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Search with valid pagination parameters should returns ok")]
    public async Task Search_With_Valid_Pagination_Parameters_Should_returns_Ok()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var professionals = new List<Professional>()
        {
            new("Marshall D Fenders"),
            new("Marshall D Teach"),
            new("Mc Coy Smith"),
        };

        await dbContext.Professionals.AddRangeAsync(professionals);
        await dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"api/professionals/search?q=D&pageIndex=1&pageSize=25");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadFromJsonAsync<PaginationResponse<ProfessionalResponse>>();

        Assert.NotNull(responseBody);
        Assert.Equal(1, responseBody.PageIndex);
        Assert.Equal(25, responseBody.PageSize);
        Assert.Equal(2, responseBody.TotalCount);
        Assert.Equal(1, responseBody.TotalPages);
        Assert.False(responseBody.HasNextPage);
        Assert.False(responseBody.HasPreviousPage);
        Assert.NotNull(responseBody.Data);

        Assert.Collection(responseBody.Data,
            item1 =>
            {
                Assert.NotNull(item1);
                Assert.Equal(professionals[0].ExternalId, item1.Id);
                Assert.Equal(professionals[0].Name, item1.Name);
            },
            item2 =>
            {
                Assert.NotNull(item2);
                Assert.Equal(professionals[1].ExternalId, item2.Id);
                Assert.Equal(professionals[1].Name, item2.Name);
            });

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Search with invalid pagination parameters should returns bad request")]
    public async Task Search_With_Invalid_Pagination_Parameters_Should_returns_BadRequest()
    {
        var response = await _client.GetAsync("api/professionals/search?q=D&pageIndex=0&pageSize=25");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Delete with existing professional should returns no content")]
    public async Task Delete_With_Existing_Professional_Should_returns_NoContent()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var professional = new Professional("Test");

        await dbContext.Professionals.AddAsync(professional);
        await dbContext.SaveChangesAsync();

        var response = await _client.DeleteAsync($"api/professionals/{professional.ExternalId}");
        response.EnsureSuccessStatusCode();

        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Delete with not existing professional should returns not found")]
    public async Task Delete_With_NotExisting_Professional_Should_returns_NotFound()
    {
        var response = await _client.DeleteAsync($"api/professionals/{Guid.Empty}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Update professional with invalid parameters should returns bad request")]
    public async Task Update_Professional_With_Invalid_Parameters_Should_returns_BadRequest()
    {
        var requestBody = new UpdateProfessionalRequest("");

        var response = await _client.PutAsJsonAsync($"api/professionals/{Guid.Empty}", requestBody);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Update with existing professional should returns ok")]
    public async Task Update_With_Existing_Professional_Should_returns_Ok()
    {
        await using var dbContext = _pgFixture.CreateContext();

        var requestBody = new UpdateProfessionalRequest("Coy");

        var professional = new Professional("Test");

        await dbContext.Professionals.AddAsync(professional);
        await dbContext.SaveChangesAsync();

        var response = await _client.PutAsJsonAsync($"api/professionals/{professional.ExternalId}", requestBody);
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadFromJsonAsync<ProfessionalResponse>();

        Assert.NotNull(responseBody);
        Assert.Equal(professional.ExternalId, responseBody.Id);
        Assert.Equal(requestBody.Name, responseBody.Name);
        
        await _pgFixture.ClearDatabaseAsync();
    }

    [Fact(DisplayName = "Update with not existing professional should returns not found")]
    public async Task Update_With_NotExisting_Professional_Should_returns_NotFound()
    {
        var requestBody = new UpdateProfessionalRequest("Coy");

        var response = await _client.PutAsJsonAsync($"api/professionals/{Guid.Empty}", requestBody);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}