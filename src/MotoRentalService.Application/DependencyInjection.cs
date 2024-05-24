using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace MotoRentalService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
            services.AddValidatorsFromAssembly(assembly);
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            return services;
        }
    }
}
