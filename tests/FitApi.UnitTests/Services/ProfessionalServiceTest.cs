using FitApi.Core.Domain.Professionals.DTOs;
using FitApi.Core.Domain.Professionals.Models;
using FitApi.Core.Exceptions;
using FitApi.Core.Repositories;
using FitApi.Core.Repositories.Professionals;
using FitApi.Core.Services;
using FitApi.Infra.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace FitApi.UnitTests.Services;

public class ProfessionalServiceTest
{
    private readonly Mock<IProfessionalRepository> _professionalRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    private readonly IProfessionalService _professionalService;

    public ProfessionalServiceTest()
    {
        _professionalRepository = new Mock<IProfessionalRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        var logger = new Mock<ILogger<ProfessionalService>>();

        _professionalService =
            new ProfessionalService(_professionalRepository.Object, _unitOfWork.Object, logger.Object);
    }

    [Fact(DisplayName = "Create should create a professional")]
    public async Task Create_Should_Create_Professional()
    {
        var requestBody = new CreateProfessionalRequest("Coy");

        var response = await _professionalService.Create(requestBody);

        _professionalRepository.Verify(r => r.Add(It.IsAny<Professional>(), CancellationToken.None), Times.Once);

        _unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);

        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(requestBody.Name, response.Name);
    }

    [Fact(DisplayName = "Find by id with existing id should returns professional")]
    public async Task FindById_With_ExistingId_Should_returns_Professional()
    {
        var professional = new Professional("Coy Smith");

        _professionalRepository.Setup(r => r.FindByExternalId(professional.ExternalId, CancellationToken.None))
            .ReturnsAsync(professional);

        var response = await _professionalService.FindById(professional.ExternalId);

        Assert.NotNull(response);
        Assert.Equal(professional.ExternalId, response.Id);
        Assert.Equal(professional.Name, response.Name);
    }

    [Fact(DisplayName = "Find by id with not existing id should throws not found exception")]
    public async Task FindById_With_NotExistingId_Should_throws_NotFoundException()
    {
        var exception =
            await Assert.ThrowsAsync<NotFoundException>(async () => await _professionalService.FindById(Guid.Empty));

        Assert.NotNull(exception);
        Assert.Equal("Professional not found", exception.Message);
    }

    [Fact(DisplayName = "Find all with valid pagination parameters should returns paginated list")]
    public async Task FindAll_With_Valid_Pagination_Parameters_Should_returns_PaginatedList()
    {
        const int pageIndex = 1;
        const int pageSize = 25;

        var professionals = new List<Professional>()
        {
            new("Coy Smith"),
            new("Marshall Fenders"),
        };

        _professionalRepository.Setup(r => r.FindAll(pageIndex, pageSize, CancellationToken.None))
            .ReturnsAsync(professionals);
        _professionalRepository.Setup(r => r.Count(CancellationToken.None)).ReturnsAsync(professionals.Count);

        var response = await _professionalService.FindAll(pageIndex, pageSize);

        _professionalRepository.Verify(r => r.FindAll(pageIndex, pageSize, CancellationToken.None), Times.Once);
        _professionalRepository.Verify(r => r.Count(CancellationToken.None), Times.Once);

        Assert.NotNull(response);
        Assert.Equal(pageIndex, response.PageIndex);
        Assert.Equal(pageSize, response.PageSize);
        Assert.Equal(professionals.Count, response.TotalCount);
        Assert.Equal(1, response.TotalPages);
        Assert.False(response.HasNextPage);
        Assert.False(response.HasPreviousPage);
        Assert.NotNull(response.Data);

        Assert.Collection(response.Data,
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
    }

    [Fact(DisplayName = "Find all with invalid pagination parameters should throws pagination exception")]
    public async Task FindAll_With_Invalid_Pagination_Parameters_Should_throws_PaginationException()
    {
        var exception =
            await Assert.ThrowsAsync<PaginationException>(async () => await _professionalService.FindAll(0, 25));

        Assert.NotNull(exception);
        Assert.Equal("Invalid pagination parameters", exception.Message);
    }

    [Fact(DisplayName = "Search with valid pagination parameters should returns paginated list")]
    public async Task Search_With_Valid_Pagination_Parameters_Should_returns_PaginatedList()
    {
        const int pageIndex = 1;
        const int pageSize = 25;
        const string q = "D";

        var professionals = new List<Professional>()
        {
            new("Marshall D Fenders"),
            new("Marshall D Teach"),
        };

        _professionalRepository.Setup(r => r.Search(q, pageIndex, pageSize, CancellationToken.None))
            .ReturnsAsync(professionals);
        _professionalRepository.Setup(r => r.SearchCount(q, CancellationToken.None)).ReturnsAsync(professionals.Count);

        var response = await _professionalService.Search(q, pageIndex, pageSize);

        _professionalRepository.Verify(r => r.Search(q, pageIndex, pageSize, CancellationToken.None), Times.Once);
        _professionalRepository.Verify(r => r.SearchCount(q, CancellationToken.None), Times.Once);

        Assert.NotNull(response);
        Assert.Equal(pageIndex, response.PageIndex);
        Assert.Equal(pageSize, response.PageSize);
        Assert.Equal(professionals.Count, response.TotalCount);
        Assert.Equal(1, response.TotalPages);
        Assert.False(response.HasNextPage);
        Assert.False(response.HasPreviousPage);
        Assert.NotNull(response.Data);

        Assert.Collection(response.Data,
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
    }

    [Fact(DisplayName = "Search with invalid pagination parameters should throws pagination exception")]
    public async Task Search_With_Invalid_Pagination_Parameters_Should_throws_PaginationException()
    {
        var exception = await Assert.ThrowsAsync<PaginationException>(async () =>
            await _professionalService.Search(string.Empty, 0, 25));

        Assert.NotNull(exception);
        Assert.Equal("Invalid pagination parameters", exception.Message);
    }
}