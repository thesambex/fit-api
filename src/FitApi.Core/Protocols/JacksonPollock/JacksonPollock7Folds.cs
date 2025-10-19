using FitApi.Core.Domain.Assessments.Models;
using FitApi.Core.Domain.Patients.Enums;

namespace FitApi.Core.Protocols.JacksonPollock;

public sealed class JacksonPollock7Folds : AbstractProtocol
{
    public JacksonPollock7Folds(BodyAssessment assessment)
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
            return 1.112m - 0.00043499m * (FoldsSum) + 0.00000055m * (FoldsSum * FoldsSum) - 0.00028826m * Age;
        }

        return 1.097m - 0.00046971m * (FoldsSum) + 0.00000056m * (FoldsSum * FoldsSum) - 0.00012828m * Age;
    }
}