using FitApi.Core.Domain.Patients.Enums;
using FitApi.Core.Domain.Patients.Models;

namespace FitApi.UnitTests.Models;

public class PatientTest
{
    [Theory(DisplayName = "Patient get age should returns age")]
    [InlineData(1998, 12, 1, 26)]
    [InlineData(1997, 12, 1, 27)]
    [InlineData(2000, 12, 1, 24)]
    public void Patient_GetAge_Should_returns_Age(int bYear, int bMonth, int bDay, int expectedAge)
    {
        var patient = new Patient("Patient", new DateOnly(bYear, bMonth, bDay), BirthGenres.Male);
        
        Assert.Equal(expectedAge, patient.GetAge(new DateOnly(2025, 10, 1)));
    }
}