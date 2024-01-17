using Taster.Application.Interfaces;
using Taster.Infrastructure.Auth;

namespace Taster.Api.Application.Auth
{
    public static class JwtAuthenticationDataProviderConfiguration
    {
        public static IServiceCollection AddJwtAuthenticationDataProvider(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CookieSettings>(configuration.GetSection("CookieSettings"));
            services.AddScoped<IAuthenticationDataProvider, JwtAuthenticationDataProvider>();

            return services;
        }
    }
}
