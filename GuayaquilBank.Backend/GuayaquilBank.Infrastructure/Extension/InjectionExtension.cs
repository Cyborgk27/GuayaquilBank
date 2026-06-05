using GuayaquilBank.Domain.Interfaces;
using GuayaquilBank.Infrastructure.Authentication;
using GuayaquilBank.Infrastructure.Common.Settings;
using GuayaquilBank.Infrastructure.Identity;
using GuayaquilBank.Infrastructure.Interceptor;
using GuayaquilBank.Infrastructure.Persistence.Context;
using GuayaquilBank.Infrastructure.Persistence.Seeder;
using GuayaquilBank.Infrastructure.Services;
using GuayaquilBank.Infrastructure.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuayaquilBank.Infrastructure.Extension
{
    public static class InjectionExtension
    {
        public static IServiceCollection AddInjectionInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var infraSettings = configuration.GetSection("Infrastructure").Get<InfrastructureSettings>();
            if (infraSettings == null)
            {
                throw new InvalidOperationException("No se pudo cargar la configuración de 'Infrastructure' desde el appsettings.json");
            }
            services.AddSingleton(infraSettings);

            services.AddScoped<AuditEntitiesInterceptor>();

            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var auditInterceptor = serviceProvider.GetRequiredService<AuditEntitiesInterceptor>();
                options.AddInterceptors(auditInterceptor);

                if (infraSettings.DatabaseProvider?.Equals("Sqlite", StringComparison.OrdinalIgnoreCase) == true)
                {
                    options.UseSqlite(
                        configuration.GetConnectionString("SqliteConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                }
                else
                {
                    throw new InvalidOperationException($"El proveedor '{infraSettings.DatabaseProvider}' no está soportado en Guayaquil Bank.");
                }
            });

            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<ContextSeed>();

            services.AddCors(options =>
            {
                options.AddPolicy("GuayaquilBankCorsPolicy", policy =>
                {
                    var cors = infraSettings.Cors;
                    if (cors != null && cors.AllowedOrigins != null)
                    {
                        policy.WithOrigins(cors.AllowedOrigins)
                              .WithMethods(cors.AllowedMethods ?? new[] { "GET", "POST", "PUT", "DELETE" })
                              .WithHeaders(cors.AllowedHeaders ?? new[] { "*" });
                    }
                    else
                    {
                        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                });
            });

            services.AddHttpContextAccessor();

            services.AddScoped<ICurrentUser, CurrentUser>();

            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<IJwtProvider, JwtProvider>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}