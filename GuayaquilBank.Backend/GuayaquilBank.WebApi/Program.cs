using GuayaquilBank.Application.Extension;
using GuayaquilBank.Infrastructure.Extension;
using GuayaquilBank.Infrastructure.Persistence.Context;
using GuayaquilBank.Infrastructure.Persistence.Seeder;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build())
    .CreateLogger();

try
{
    Log.Information("Iniciando el servidor de Guayaquil Bank Web API...");

    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Guayaquil Bank ERP Web API",
            Version = "v1",
            Description = "API Modular Multi-Tenant para la gestión de ventas, identidad y auditoría core."
        });

        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Introduce el token JWT en este formato: Bearer {tu_token_aqui}"
        });

        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    builder.Services.AddInjectionInfrastructure(configuration);
    builder.Services.AddInjectionApplication(configuration);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            Log.Information("Ejecutando migraciones y siembra de datos de prueba...");
            var context = services.GetRequiredService<ApplicationDbContext>();
            var seeder = services.GetRequiredService<ContextSeed>();

            await context.Database.MigrateAsync();
            await seeder.SeedAsync();
            Log.Information("Base de datos sincronizada y poblada exitosamente.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Ocurrió un error irreversible mientras se poblaba la base de datos de desarrollo.");
        }
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseCors("GuayaquilBankCorsPolicy");

    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "El servidor de la API colapsó inesperadamente durante el arranque.");
}
finally
{
    Log.Information("Apagando el servidor y limpiando recursos de logging...");
    await Log.CloseAndFlushAsync();
}