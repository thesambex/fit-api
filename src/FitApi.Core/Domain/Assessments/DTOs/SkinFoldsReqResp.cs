namespace FitApi.Core.Domain.Assessments.DTOs;

public record SkinFoldsReqResp(
    decimal Triceps,
    decimal Biceps,
    decimal Subscapular,
    decimal Suprailiac,
    decimal MedianAxillary,
    decimal Abdomen,
    decimal Thoracic,
    decimal Supraspinal,
    decimal Thigh,
    decimal Calf
)
{
    public decimal Sum() => Triceps + Biceps + Subscapular + Suprailiac + MedianAxillary + Thoracic + Abdomen +
                            Supraspinal + Thigh + Calf;
};