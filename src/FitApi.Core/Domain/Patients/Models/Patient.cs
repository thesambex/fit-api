using FitApi.Core.Domain.Assessments.Models;
using FitApi.Core.Domain.Patients.Enums;

namespace FitApi.Core.Domain.Patients.Models;

public sealed class Patient
{
    public long Id { get; init; }
    public string Name { get; private set; } = string.Empty;
    public DateOnly BirthDate { get; private set; }
    public BirthGenres BirthGenre { get; private set; }
    public Guid ExternalId { get; } = Guid.NewGuid();

    public ICollection<BodyAssessment>? BodyAssessments { get; set; }
    
    public Patient()
    {
    }

    public Patient(string name, DateOnly birthDate, BirthGenres birthGenre)
    {
        Name = name;
        BirthDate = birthDate;
        BirthGenre = birthGenre;
    }

    public void SetName(string name) => Name = name;

    public void SetBirthDate(DateOnly birthDate) => BirthDate = birthDate;

    public void SetBirthGenre(BirthGenres birthGenre) => BirthGenre = birthGenre;

    public int GetAge(DateOnly? dtToday = null)
    {
        var today = dtToday ?? DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - BirthDate.Year;

        if (BirthDate > today.AddYears(-age)) age--;
        
        return age;
    }
}