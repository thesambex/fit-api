using System.Reflection;
using System.Text.Json.Serialization;
using FitApi.Api.Middlewares;
using FitApi.Core.Domain.Patients.Enums;
using FitApi.Core.Domain.Patients.Validators;
using FitApi.Core.Repositories;
using FitApi.Core.Repositories.Patients;
using FitApi.Core.Repositories.Professionals;
using FitApi.Core.Services;
using FitApi.Database;
using FitApi.Database.Repositories;
using FitApi.Database.Repositories.Patients;
using FitApi.Database.Repositories.Professionals;
using FitApi.Infra.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using Serilog;

namespace FitApi.Api;

public static class DependencyInjection
{
    public static void InjectDependencies(this WebApplicationBuilder builder)
    {
        builder.InjectDatabase();
        builder.InjectServices();
        
        builder.Host.UseSerilog((context, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(context.Configuration)
        );

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "FitApi",
            });
            
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        
        builder.Services.AddValidatorsFromAssemblyContaining<CreatePatientValidator>();

        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandlingMiddleware>();
        builder.Services.AddControllers().AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    private static void InjectDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<FitDbContext>(opt =>
        {
            var connectionString = builder.Configuration.GetConnectionString("FitDb");

            var npgsqlDataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            npgsqlDataSourceBuilder.MapEnum<BirthGenres>("patients.birth_genres");

            var npgsqlDatasource = npgsqlDataSourceBuilder.Build();

            opt.UseNpgsql(npgsqlDatasource);
        });

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddTransient<IPatientRepository, PatientRepository>();
        builder.Services.AddTransient<IProfessionalRepository, ProfessionalRepository>();
    }

    private static void InjectServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPatientService, PatientService>();
        builder.Services.AddScoped<IProfessionalService, ProfessionalService>();
    }
}