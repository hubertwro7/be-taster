using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Taster.Application.Interfaces;

namespace Taster.Infrastructure.Persistance
{
    public static class SqlDatabaseConfiguration
    {
        public static IServiceCollection AddSqlDatabase(this IServiceCollection services, string connectionString)
        {
            Action<IServiceProvider, DbContextOptionsBuilder> sqlOptions = (serviceProvider, options) => options.UseSqlServer(connectionString,
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));

            services.AddDbContext<IApplicationDbContext, MainDbContext>(sqlOptions);

            return services;
        }
    }
}
