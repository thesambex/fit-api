using FitApi.Core.Domain.Professionals.Models;
using FitApi.Core.Repositories.Professionals;
using Microsoft.EntityFrameworkCore;

namespace FitApi.Database.Repositories.Professionals;

public class ProfessionalRepository(FitDbContext dbContext) : IProfessionalRepository
{
    public async Task Add(Professional professional, CancellationToken cancellationToken) =>
        await dbContext.Professionals.AddAsync(professional, cancellationToken);

    public async Task<Professional?> FindByExternalId(Guid id, CancellationToken cancellationToken) =>
        await dbContext.Professionals.Where(e => e.ExternalId == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<Professional>> FindAll(int pageIndex, int pageSize,
        CancellationToken cancellationToken) => await dbContext.Professionals.AsNoTracking()
        .OrderBy(e => e.Name)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Professional>> Search(
        string q,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken
    ) => await dbContext.Professionals.AsNoTracking()
        .Where(e => EF.Functions.Like(e.Name, $"%{q}%"))
        .OrderBy(e => e.Name)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    public async Task<long> SearchCount(string q, CancellationToken cancellationToken) => await dbContext.Professionals
        .AsNoTracking().Where(e => EF.Functions.Like(e.Name, $"%{q}%")).LongCountAsync(cancellationToken);

    public async Task<long> Count(CancellationToken cancellationToken) =>
        await dbContext.Professionals.LongCountAsync(cancellationToken);
}