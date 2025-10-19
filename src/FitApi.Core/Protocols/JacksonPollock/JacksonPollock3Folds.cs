using FitApi.Core.Domain.Assessments.Models;
using FitApi.Core.Domain.Patients.Enums;

namespace FitApi.Core.Protocols.JacksonPollock;

public sealed class JacksonPollock3Folds : AbstractProtocol
{
    public JacksonPollock3Folds(BodyAssessment assessment)
    {
        Age = assessment.Age;
        Height = assessment.Height;
        Weight = assessment.Weight;
        BirthGenre = assessment.BirthGenre;
        FoldsSum = assessment.FoldsSum;
    }

    public override decimal BodyDensity()
    {
        if (BirthGenre == BirthGenres.Male)
        {
            return 1.10938m - 0.0008267m * (FoldsSum) + 0.0000016m * (FoldsSum * FoldsSum) - 0.0002574m * Age;
        }

        return 1.0994921m - 0.0009929m * (FoldsSum) + 0.0000023m * (FoldsSum * FoldsSum) - 0.0001392m * Age;
    }
}