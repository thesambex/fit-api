using FitApi.Core.Domain.Patients.Models;
using FitApi.Core.Repositories.Patients;
using Microsoft.EntityFrameworkCore;

namespace FitApi.Database.Repositories.Patients;

public class PatientRepository(FitDbContext dbContext) : IPatientRepository
{
    public async Task Add(Patient patient, CancellationToken cancellationToken) =>
        await dbContext.Patients.AddAsync(patient, cancellationToken);

    public async Task<Patient?> FindByExternalId(Guid id, CancellationToken cancellationToken) =>
        await dbContext.Patients.Where(e => e.ExternalId == id).FirstOrDefaultAsync(cancellationToken);

    public async Task DeleteById(long id, CancellationToken cancellationToken) =>
        await dbContext.Patients.Where(e => e.Id == id).ExecuteDeleteAsync(cancellationToken);

    public async Task<IReadOnlyList<Patient>> FindAll(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken
    ) => await dbContext.Patients.AsNoTracking()
        .OrderBy(e => e.Name)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Patient>> Search(
        string q,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken
    ) => await dbContext.Patients.AsNoTracking()
        .Where(e => EF.Functions.Like(e.Name, $"%{q}%"))
        .OrderBy(e => e.Name)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    public async Task<long> Count(CancellationToken cancellationToken) =>
        await dbContext.Patients.LongCountAsync(cancellationToken);

    public async Task<long> SearchCount(string q, CancellationToken cancellationToken) =>
        await dbContext.Patients.Where(e => EF.Functions.Like(e.Name, $"%{q}%")).LongCountAsync(cancellationToken);
}