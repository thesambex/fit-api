using FitApi.Core.Domain.Assessments.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitApi.Database.Mapping.Assessments;

internal class BodyAssessmentMapping : IEntityTypeConfiguration<BodyAssessment>
{
    public void Configure(EntityTypeBuilder<BodyAssessment> builder)
    {
        builder.ToTable("body_assessments", "assessments");
        builder.Property(e => e.Id).HasColumnName("id").IsRequired();;
        builder.Property(e => e.Height).HasColumnName("height").HasColumnType("decimal(3, 2)").IsRequired();
        builder.Property(e => e.Weight).HasColumnName("weight").HasColumnType("decimal(5, 2)").IsRequired();
        builder.Property(e => e.Age).HasColumnName("age").IsRequired();
        builder.Property(e => e.BirthGenre).HasColumnName("birth_genre").IsRequired();
        builder.Property(e => e.FoldsSum).HasColumnName("folds_sum").HasColumnType("decimal(10,9)").IsRequired();
        builder.Property(e => e.ExternalId).HasColumnName("external_id").IsRequired();
        builder.Property(e => e.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(e => e.ProfessionalId).HasColumnName("professional_id").IsRequired();
        
        builder.HasOne(e => e.Patient)
            .WithMany(e => e.BodyAssessments)
            .HasForeignKey(e => e.PatientId)
            .HasConstraintName("FK_body_assessments_patients")
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.Professional)
            .WithMany(e => e.BodyAssessments)
            .HasForeignKey(e => e.ProfessionalId)
            .HasConstraintName("FK_body_assessments_professionals")
            .OnDelete(DeleteBehavior.Cascade);
    }
}