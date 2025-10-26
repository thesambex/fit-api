using FitApi.Core.Domain.Assessments.Models;

namespace FitApi.Core.Domain.Professionals.Models;

public sealed class Professional
{
    public long Id { get; init; }
    public string Name { get; private set; } = string.Empty;
    public Guid ExternalId { get; init; } = Guid.NewGuid();

    public ICollection<BodyAssessment>? BodyAssessments { get; set; }
    
    public Professional()
    {
    }

    public Professional(string name)
    {
        Name = name;
    }

    public void SetName(string name) => Name = name;
}