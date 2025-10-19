using FitApi.Core.Domain.Patients.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitApi.Database.Mapping.Patients;

internal class PatientMapping : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients", "patients");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(60).IsRequired();
        builder.Property(e => e.BirthDate).HasColumnName("birth_date").HasColumnType("date").IsRequired();
        builder.Property(e => e.BirthGenre).HasColumnName("birth_genre").IsRequired();
        builder.Property(e => e.ExternalId).HasColumnName("external_id").IsRequired();
    }
}