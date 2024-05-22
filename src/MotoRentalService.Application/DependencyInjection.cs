using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MotoRentalService.Application.MediatR.Commands.Motorcycle;
using MotoRentalService.Application.MediatR.Validations.Moto;

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

            services.AddScoped<IValidator<CreateMotoCommand>, CreateMotoCommandValidator>();

            return services;
        }
    }
}
