using FitApi.Core.Domain.Patients.Enums;

namespace FitApi.Core.Domain.Assessments.Models;

public sealed class BodyAssessment
{
    public long Id { get; init; }
    public int Age { get; private set; }
    public BirthGenres BirthGenre { get; private set; }
    public decimal Height { get; private set; }
    public decimal Weight { get; private set; }
    public decimal FoldsSum { get; private set; }

    public BodyAssessment()
    {
    }

    public BodyAssessment(int age, BirthGenres birthGenre, decimal height, decimal weight, SkinFolds skinFolds)
    {
        Age = age;
        BirthGenre = birthGenre;
        Height = height;
        Weight = weight;
        FoldsSum = skinFolds.Sum();
    }

    public void SetAge(int age) => Age = age;
    
    public void SetBirthGenre(BirthGenres birthGenre) => BirthGenre = birthGenre;
    
    public void SetHeight(decimal height) => Height = height;
    
    public void SetWeight(decimal weight) => Weight = weight;
    
    public void SetFoldsSum(SkinFolds skinFolds) => FoldsSum = skinFolds.Sum();
}