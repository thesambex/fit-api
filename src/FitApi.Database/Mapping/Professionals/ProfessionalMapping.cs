using FitApi.Core.Domain.Professionals.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitApi.Database.Mapping.Professionals;

internal class ProfessionalMapping : IEntityTypeConfiguration<Professional>
{
    public void Configure(EntityTypeBuilder<Professional> builder)
    {
        builder.ToTable("professionals", "professionals");
        builder.Property(e => e.Id).HasColumnName("id").IsRequired();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(60).IsRequired();
        builder.Property(e => e.ExternalId).HasColumnName("external_id").IsRequired();
    }
}