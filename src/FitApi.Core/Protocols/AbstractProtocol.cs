using FitApi.Core.Domain.Patients.Enums;

namespace FitApi.Core.Protocols;

public abstract class AbstractProtocol
{
    protected decimal Weight { get; init; }
    protected decimal Height { get; init; }
    protected decimal FoldsSum { get; init; }
    protected BirthGenres BirthGenre { get; init; }
    protected int Age { get; init; }

    /// <summary>
    /// Calculate Body Density
    /// </summary>
    /// <returns></returns>
    public abstract decimal BodyDensity();

    /// <summary>
    /// Calculate BMI
    /// </summary>
    /// <returns></returns>
    public decimal Bmi()
    {
        var bmi = Weight / (Height * Height);
        return Math.Round(bmi, 2);
    }

    /// <summary>
    /// Returns body fat percent
    /// </summary>
    /// <param name="bodyDensity">Body Density</param>
    /// <returns></returns>
    public static decimal BodyFatPercent(decimal bodyDensity)
    {
        var bf = (495 / bodyDensity) - 450;
        return Math.Round(bf, 2);
    }

    /// <summary>
    /// Returns body fat weight
    /// </summary>
    /// <param name="bfPercent">Body fat percent</param>
    /// <returns></returns>
    public decimal BodyFatWeight(decimal bfPercent)
    {
        var weight = (bfPercent * Weight) / 100;
        return Math.Round(weight, 2);
    }

    /// <summary>
    /// Returns lean mass weight
    /// </summary>
    /// <param name="fatMassWeight">Fat mass weight</param>
    /// <returns></returns>
    public decimal LeanMass(decimal fatMassWeight)
    {
        return Weight - fatMassWeight;
    }
}