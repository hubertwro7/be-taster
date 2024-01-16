using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Taster.Infrastructure.Auth
{
    public static class JwtAuthConfiguration
    {
        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtAuthenticationOptions>(configuration.GetSection("JwtAuthentication"));
            services.AddSingleton<JwtManager>();

            return services;
        }
    }
}
