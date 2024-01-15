using Microsoft.Extensions.DependencyInjection;
using Taster.Application.Interfaces;
using Taster.Application.Services;

namespace Taster.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
            return services;
        }
    }
}
