using FitApi.Core.Domain.Professionals.Models;

namespace FitApi.Core.Repositories.Professionals;

public interface IProfessionalRepository
{
    Task Add(Professional professional, CancellationToken cancellationToken = default);
    Task<Professional?> FindByExternalId(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Professional>> FindAll(int pageIndex, int pageSize,
        CancellationToken cancellationToken = default);

    Task DeleteById(long id, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Professional>> Search(
        string q,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    Task<long> SearchCount(string q, CancellationToken cancellationToken = default);

    Task<long> Count(CancellationToken cancellationToken = default);
}