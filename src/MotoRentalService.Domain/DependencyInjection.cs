using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Domain.Services;
using MotoRentalService.Services.Implementations;
using MotoRentalService.Services.Interfaces;

namespace MotoRentalService.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IMotorcycleService, MotorcycleService>();
            services.AddTransient<IRentalService, RentalService>();
            services.AddTransient<IDeliveryUserService, DeliveryUserService>();
            services.AddTransient<IUserAuthenticationService, UserAuthenticationService>();
            services.AddTransient<IPasswordHasherService, PasswordHasherService>();
            services.AddSingleton<IStorageService, LocalStorageService>();
            services.AddSingleton<ITokenService,TokenService>();
            services.AddSingleton(configuration);
            return services;
        }
    }
}
