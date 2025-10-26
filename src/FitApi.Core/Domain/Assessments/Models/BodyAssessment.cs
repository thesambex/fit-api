using FitApi.Core.Domain.Patients.Enums;
using FitApi.Core.Domain.Patients.Models;
using FitApi.Core.Domain.Professionals.Models;

namespace FitApi.Core.Domain.Assessments.Models;

public sealed class BodyAssessment
{
    public long Id { get; init; }
    public int Age { get; private set; }
    public BirthGenres BirthGenre { get; private set; }
    public decimal Height { get; private set; }
    public decimal Weight { get; private set; }
    public decimal FoldsSum { get; private set; }
    public DateTimeOffset AssessmentDate { get; } = DateTimeOffset.UtcNow;
    public Guid ExternalId { get; } = Guid.NewGuid();
    public long PatientId { get; private init; }
    public long ProfessionalId { get; private init; }

    public BodyAssessmentSkinFolds? AssessmentSkinFolds { get; set; }
    public Patient? Patient { get; set; }
    public Professional? Professional { get; set; }

    public BodyAssessment()
    {
    }

    public BodyAssessment(
        long patientId,
        long professionalId,
        int age,
        BirthGenres birthGenre,
        decimal height,
        decimal weight
    )
    {
        PatientId = patientId;
        ProfessionalId = professionalId;
        Age = age;
        BirthGenre = birthGenre;
        Height = height;
        Weight = weight;
    }

    public BodyAssessment(
        int age,
        BirthGenres birthGenre,
        decimal height,
        decimal weight
    )
    {
        Age = age;
        BirthGenre = birthGenre;
        Height = height;
        Weight = weight;
    }

    public void SetAge(int age) => Age = age;

    public void SetBirthGenre(BirthGenres birthGenre) => BirthGenre = birthGenre;

    public void SetHeight(decimal height) => Height = height;

    public void SetWeight(decimal weight) => Weight = weight;

    public void SetFoldsSum(decimal sum) => FoldsSum = sum;

    public void UpdateFoldSumFromChild()
    {
        if (AssessmentSkinFolds != null)
        {
            var folds = AssessmentSkinFolds.GetFolds();
            FoldsSum = folds.Sum();
        }
    }
}