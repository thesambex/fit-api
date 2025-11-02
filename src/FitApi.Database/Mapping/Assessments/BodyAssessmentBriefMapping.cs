using FitApi.Core.Domain.Assessments.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitApi.Database.Mapping.Assessments;

internal class BodyAssessmentBriefMapping : IEntityTypeConfiguration<BodyAssessmentBrief>
{
    public void Configure(EntityTypeBuilder<BodyAssessmentBrief> builder)
    {
        builder.ToView("vw_assessments_brief", "assessments");
        builder.Property(e => e.Date).HasColumnName("date").IsRequired();
        builder.Property(e => e.PatientExternalId).HasColumnName("patient_external_id").IsRequired();
        builder.Property(e => e.PatientName).HasColumnName("patient_name").IsRequired();
        builder.Property(e => e.Id).HasColumnName("id").IsRequired();
        builder.Property(e => e.ProfessionalExternalId).HasColumnName("professional_external_id").IsRequired();
        builder.Property(e => e.ProfessionalName).HasColumnName("professional_name").IsRequired();
        builder.Property(e => e.Weight).HasColumnName("weight").IsRequired();
        builder.HasNoKey();
    }
}