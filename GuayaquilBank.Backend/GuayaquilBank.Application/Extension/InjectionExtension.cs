using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuayaquilBank.Application.Extension
{
    public static class InjectionExtension
    {
        public static IServiceCollection AddInjectionApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthenticationAppService, AuthenticationAppService>();
            return services;
        }
    }
}
