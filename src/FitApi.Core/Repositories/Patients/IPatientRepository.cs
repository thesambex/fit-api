using FitApi.Core.Domain.Patients.Models;

namespace FitApi.Core.Repositories.Patients;

public interface IPatientRepository
{
    Task Add(Patient patient, CancellationToken cancellationToken = default);
    Task<Patient?> FindByExternalId(Guid id, CancellationToken cancellationToken = default);
    Task DeleteById(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Patient>> FindAll(int pageIndex, int pageSize, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Patient>> Search(
        string q,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    Task<long> Count(CancellationToken cancellationToken = default);
    Task<long> SearchCount(string q, CancellationToken cancellationToken = default);
}