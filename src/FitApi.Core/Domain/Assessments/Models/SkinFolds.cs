namespace FitApi.Core.Domain.Assessments.Models;

public struct SkinFolds
{
    public decimal Triceps;
    public decimal Biceps;
    public decimal Subscapular;
    public decimal Suprailiac;
    public decimal MedianAxillary;
    public decimal Abdomen;
    public decimal Thoracic;
    public decimal Supraspinal;
    public decimal Thigh;
    public decimal Calf;

    public decimal Sum() => Triceps + Biceps + Subscapular + Suprailiac + MedianAxillary + Thoracic + Abdomen +
                            Supraspinal + Thigh + Calf;
}