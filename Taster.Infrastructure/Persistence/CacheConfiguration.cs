using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.DependencyInjection;

namespace Taster.Infrastructure.Persistence
{
    public static class CacheConfiguration
    {
        public static IServiceCollection AddDatabaseCache(this IServiceCollection services)
        {
            services.AddEFSecondLevelCache(options =>
                options.UseMemoryCacheProvider(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(5)).DisableLogging(false).UseCacheKeyPrefix("EF_")
            );

            return services;
        }
    }
}
