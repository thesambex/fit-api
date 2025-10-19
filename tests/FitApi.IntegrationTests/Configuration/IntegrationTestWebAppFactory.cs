using FitApi.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FitApi.IntegrationTests.Configuration;

public class IntegrationTestWebAppFactory(PgContainerFixture pgContainerFixture) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<FitDbContext>));
            
            if (descriptor != null)
                services.Remove(descriptor);

            if (pgContainerFixture.DataSource != null)
            {
                services.AddDbContext<FitDbContext>(options =>
                {
                    options.UseNpgsql(pgContainerFixture.DataSource);
                });
                
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<FitDbContext>();
                
                if (db.Database.GetPendingMigrations().Any())
                {
                    db.Database.Migrate();
                }
            }
        });
    }
}