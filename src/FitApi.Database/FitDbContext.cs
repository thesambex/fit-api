using System.Reflection;
using FitApi.Core.Domain.Patients.Enums;
using FitApi.Core.Domain.Patients.Models;
using Microsoft.EntityFrameworkCore;

namespace FitApi.Database;

public class FitDbContext(DbContextOptions<FitDbContext> options) : DbContext(options)
{
    #region Entities
    public DbSet<Patient>  Patients => Set<Patient>();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.UseIdentityAlwaysColumns();

        modelBuilder.HasPostgresEnum<BirthGenres>("patients", "birth_genres");
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    
}