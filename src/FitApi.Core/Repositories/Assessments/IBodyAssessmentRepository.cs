using FitApi.Core.Domain.Assessments.Models;

namespace FitApi.Core.Repositories.Assessments;

public interface IBodyAssessmentRepository
{
    Task Add(BodyAssessment assessment, CancellationToken cancellationToken = default);
    Task<BodyAssessment?> FindByExternalId(Guid id, CancellationToken cancellationToken = default);
    Task DeleteById(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BodyAssessmentBrief>> FindAllByPatientId(
        Guid patientId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    Task<long> CountByPatientId(Guid patientId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BodyAssessmentBrief>> FindAllByProfessionalId(
        Guid professionalId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    Task<long> CountByProfessionalId(Guid professionalId, CancellationToken cancellationToken = default);
}