namespace FitApi.Core.Domain.Assessments.Models;

public sealed class BodyAssessmentBrief
{
    public Guid Id { get; init; }
    public decimal Weight { get; init; }
    public DateTimeOffset Date { get; init; }
    public Guid ProfessionalExternalId { get; init; }
    public string ProfessionalName { get; init; }
    public Guid PatientExternalId { get; init; }
    public string PatientName { get; init; }
    
    public BodyAssessmentBrief()
    {
    }
}