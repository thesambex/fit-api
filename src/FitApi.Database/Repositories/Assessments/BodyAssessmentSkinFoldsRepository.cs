using FitApi.Core.Domain.Assessments.Models;
using FitApi.Core.Repositories.Assessments;

namespace FitApi.Database.Repositories.Assessments;

public class BodyAssessmentSkinFoldsRepository(FitDbContext dbContext) : IBodyAssessmentSkinFoldsRepository
{
    public async Task Add(BodyAssessmentSkinFolds assessment, CancellationToken cancellationToken) =>
        await dbContext.BodyAssessmentSkinFolds.AddAsync(assessment, cancellationToken);
}