using FitApi.Core.Domain.Assessments.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitApi.Database.Mapping.Assessments;

internal class BodyAssessmentSkinFoldsMapping : IEntityTypeConfiguration<BodyAssessmentSkinFolds>
{
    public void Configure(EntityTypeBuilder<BodyAssessmentSkinFolds> builder)
    {
        builder.ToTable("body_assessment_skin_folds", "assessments");
        builder.Property(e => e.Id).HasColumnName("id").IsRequired();
        builder.Property(e => e.Biceps).HasColumnName("biceps").HasColumnType("DECIMAL(5,2)");
        builder.Property(e => e.Triceps).HasColumnName("triceps").HasColumnType("DECIMAL(5,2)");
        builder.Property(e => e.Subscapular).HasColumnName("subscapular").HasColumnType("DECIMAL(5,2)");
        builder.Property(e => e.Suprailiac).HasColumnName("suprailiac").HasColumnType("DECIMAL(5,2)");
        builder.Property(e => e.Supraspinal).HasColumnName("supraspinal").HasColumnType("DECIMAL(5,2)");
        builder.Property(e => e.MedianAxillary).HasColumnName("median_axillary").HasColumnType("DECIMAL(5,2)");
        builder.Property(e => e.Thigh).HasColumnName("thigh").HasColumnType("DECIMAL(5,2)");
        builder.Property(e => e.Thoracic).HasColumnName("thoracic").HasColumnType("DECIMAL(5,2)");
        builder.Property(e => e.Calf).HasColumnName("calf").HasColumnType("DECIMAL(5,2)");
        builder.Property(e => e.Abdomen).HasColumnName("abdomen").HasColumnType("DECIMAL(5,2)");
        builder.Property(e => e.AssessmentId).HasColumnName("assessment_id").IsRequired();

        builder.HasOne(e => e.Assessment)
            .WithOne(e => e.AssessmentSkinFolds)
            .HasForeignKey<BodyAssessmentSkinFolds>(e => e.AssessmentId)
            .HasConstraintName("FK_body_assessment_skin_folds")
            .OnDelete(DeleteBehavior.Cascade);
    }
}