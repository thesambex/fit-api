using FitApi.Core.Domain.Assessments.Models;
using FitApi.Core.Repositories.Assessments;
using Microsoft.EntityFrameworkCore;

namespace FitApi.Database.Repositories.Assessments;

public class BodyAssessmentRepository(FitDbContext dbContext) : IBodyAssessmentRepository
{
    public async Task Add(BodyAssessment assessment, CancellationToken cancellationToken) =>
        await dbContext.BodyAssessments.AddAsync(assessment, cancellationToken);

    public async Task<BodyAssessment?> FindByExternalId(Guid id, CancellationToken cancellationToken) =>
        await dbContext.BodyAssessments
            .Include(e => e.AssessmentSkinFolds)
            .Include(e => e.Patient)
            .Include(e => e.Professional)
            .Where(e => e.ExternalId == id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task DeleteById(long id, CancellationToken cancellationToken) => await dbContext.BodyAssessments
        .Where(e => e.Id == id).ExecuteDeleteAsync(cancellationToken);

    public async Task<IReadOnlyList<BodyAssessmentBrief>> FindAllByPatientId(
        Guid patientId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken
    ) => await dbContext.BodyAssessmentsBrief.AsNoTracking()
        .OrderBy(e => e.Date)
        .Where(e => e.PatientExternalId == patientId)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    public async Task<long> CountByPatientId(Guid patientId, CancellationToken cancellationToken) => await dbContext
        .BodyAssessmentsBrief.Where(e => e.PatientExternalId == patientId).LongCountAsync(cancellationToken);
}