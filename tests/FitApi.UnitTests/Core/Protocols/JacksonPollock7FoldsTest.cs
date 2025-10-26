using FitApi.Core.Domain.Assessments.Models;
using FitApi.Core.Domain.Patients.Enums;
using FitApi.Core.Protocols;
using FitApi.Core.Protocols.JacksonPollock;

namespace FitApi.UnitTests.Core.Protocols;

public class JacksonPollock7FoldsTest
{
    private readonly BodyAssessment _assessment;

    public JacksonPollock7FoldsTest()
    {
        var skinFolds = new SkinFolds
        {
            Triceps = 6.43m,
            MedianAxillary = 6.50m,
            Subscapular = 12.7m,
            Suprailiac = 9.2m,
            Thoracic = 4.50m,
            Abdomen = 13.66m,
            Thigh = 10.06m
        };

        _assessment = new BodyAssessment(-1, -1, 24, BirthGenres.Male, 1.68m, 67.5m);
        _assessment.SetFoldsSum(skinFolds.Sum());
    }

    [Fact(DisplayName = "Test BMI")]
    public void Test_Bmi()
    {
        AbstractProtocol protocol = new JacksonPollock7Folds(_assessment);

        var bmi = protocol.Bmi();

        Assert.Equal(23.92m, bmi);
    }

    [Fact(DisplayName = "Test body density")]
    public void Test_BodyDensity()
    {
        AbstractProtocol protocol = new JacksonPollock7Folds(_assessment);

        var bodyDensity = protocol.BodyDensity();

        Assert.Equal(1.08m, Math.Round(bodyDensity, 2));
    }

    [Fact(DisplayName = "Test body fat")]
    public void Test_BodyFat()
    {
        AbstractProtocol protocol = new JacksonPollock7Folds(_assessment);

        var bodyDensity = protocol.BodyDensity();
        var bodyFatPercent = AbstractProtocol.BodyFatPercent(bodyDensity);
        var bodyFatWeight = protocol.BodyFatWeight(bodyFatPercent);

        Assert.Equal(8.4m, bodyFatPercent);
        Assert.Equal(5.67m, bodyFatWeight);
    }

    [Fact(DisplayName = "Test lean mass")]
    public void Test_LeanMass()
    {
        AbstractProtocol protocol = new JacksonPollock7Folds(_assessment);

        var bodyDensity = protocol.BodyDensity();
        var bodyFatPercent = AbstractProtocol.BodyFatPercent(bodyDensity);
        var bodyFatWeight = protocol.BodyFatWeight(bodyFatPercent);
        var leanMass = protocol.LeanMass(bodyFatWeight);

        Assert.Equal(61.83m, leanMass);
    }
}