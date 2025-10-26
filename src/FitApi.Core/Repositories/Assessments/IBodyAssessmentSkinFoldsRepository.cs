using FitApi.Core.Domain.Assessments.Models;

namespace FitApi.Core.Repositories.Assessments;

public interface IBodyAssessmentSkinFoldsRepository
{
    Task Add(BodyAssessmentSkinFolds assessment, CancellationToken cancellationToken = default);
}