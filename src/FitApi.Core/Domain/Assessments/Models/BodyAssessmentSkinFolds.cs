namespace FitApi.Core.Domain.Assessments.Models;

public sealed class BodyAssessmentSkinFolds
{
    public long Id { get; init; }
    public decimal Triceps { get; private set; }
    public decimal Biceps { get; private set; }
    public decimal Subscapular { get; private set; }
    public decimal Suprailiac { get; private set; }
    public decimal MedianAxillary { get; private set; }
    public decimal Thoracic { get; private set; }
    public decimal Supraspinal { get; private set; }
    public decimal Thigh { get; private set; }
    public decimal Abs { get; private set; }
    public decimal Calf { get; private set; }
    public long AssessmentId { get; init; }

    public BodyAssessment? Assessment { get; set; }

    public BodyAssessmentSkinFolds()
    {
    }

    public BodyAssessmentSkinFolds(long assessmentId, SkinFolds folds)
    {
        AssessmentId = assessmentId;
        Triceps = folds.Triceps;
        Biceps = folds.Biceps;
        Subscapular = folds.Subscapular;
        Suprailiac = folds.Suprailiac;
        MedianAxillary = folds.MedianAxillary;
        Thoracic = folds.Thoracic;
        Supraspinal = folds.Supraspinal;
        Thigh = folds.Thigh;
        Abs = folds.Abs;
        Calf = folds.Calf;
    }

    public void SetFolds(SkinFolds folds)
    {
        Triceps = folds.Triceps;
        Biceps = folds.Biceps;
        Subscapular = folds.Subscapular;
        Suprailiac = folds.Suprailiac;
        MedianAxillary = folds.MedianAxillary;
        Thoracic = folds.Thoracic;
        Supraspinal = folds.Supraspinal;
        Thigh = folds.Thigh;
        Abs = folds.Abs;
        Calf = folds.Calf;
    }

    public SkinFolds GetFolds()
    {
        return new SkinFolds
        {
            Triceps = Triceps,
            Biceps = Biceps,
            Subscapular = Subscapular,
            Suprailiac = Suprailiac,
            MedianAxillary = MedianAxillary,
            Thoracic = Thoracic,
            Supraspinal = Supraspinal,
            Thigh = Thigh,
            Abs = Abs,
            Calf = Calf,
        };
    }
}