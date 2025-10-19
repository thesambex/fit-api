using DotNet.Testcontainers.Containers;
using FitApi.Core.Domain.Patients.Enums;
using FitApi.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace FitApi.IntegrationTests.Configuration;

public class PgContainerFixture : IAsyncLifetime
{
    private IDatabaseContainer? _dbContainer;

    public string ConnectionString { get; private set; } = string.Empty;
    public NpgsqlDataSource? DataSource { get; private set; }

    public async Task InitializeAsync()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:17.4-alpine3.21")
            .WithDatabase("db_fit")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();

        await _dbContainer.StartAsync();

        ConnectionString = _dbContainer.GetConnectionString();

        if (DataSource == null)
        {
            var npgsqlDataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString);
            npgsqlDataSourceBuilder.MapEnum<BirthGenres>("patients.birth_genres");
            DataSource = npgsqlDataSourceBuilder.Build();
        }
    }

    public async Task DisposeAsync()
    {
        if (_dbContainer != null)
        {
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();
        }
    }

    public FitDbContext CreateContext()
    {
        if (DataSource == null)
        {
            throw new InvalidOperationException("PgContainerFixture.CreateContext(): DataSource is null");
        }
        
        var options = new DbContextOptionsBuilder<FitDbContext>()
            .UseNpgsql(DataSource)
            .Options;

        var dbContext = new FitDbContext(options);
        if (dbContext.Database.GetMigrations().Any())
        {
            dbContext.Database.Migrate();
        }

        return dbContext;
    }
    
    public async Task ClearDatabaseAsync()
    {
        await using var dbContext = CreateContext();

        await dbContext.Patients.ExecuteDeleteAsync();
        await dbContext.SaveChangesAsync();
    }
}