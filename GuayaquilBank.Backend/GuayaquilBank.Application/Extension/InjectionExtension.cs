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
            services.AddScoped<IProductAppService, ProductAppService>();
            services.AddScoped<ISalesAppService, SalesAppService>();
            services.AddScoped<IUserAppService, UserAppService>();
            services.AddScoped<ICustomerAppService, CustomerAppService>();
            services.AddScoped<IProfileAppService, ProfileAppService>();
            return services;
        }
    }
}
