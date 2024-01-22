using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Taster.Application.Interfaces;
using Taster.Application.Logic.Abstractions;
using Taster.Application.Services;
using Taster.Application.Validators;

namespace Taster.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining(typeof(BaseQueryHandler));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
