using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoRentalService.Domain.Interfaces.repository;
using MotoRentalService.Infrastructure.Repositories.Impl;
using Serilog.Extensions.Logging;

namespace MotoRentalService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var efLoggerFactory = new SerilogLoggerFactory();
            // Register your DbContext
            services.AddDbContext<AppDbContext>(options =>
                 options
                     .UseNpgsql(configuration.GetConnectionString("RENTAL_DSN"))
                     .UseLoggerFactory(efLoggerFactory)
             );

            // Register your repository with DbContext
            services.AddTransient<IMotorcycleRepository, MotorcycleRepository>();
            services.AddTransient<IRentalRepository, RentalRepository>();
            services.AddTransient<IDeliveryUserRepository, DeliveryUserRepository>();
            services.AddTransient<IPlansRepository, PlansRepository>();
            services.AddTransient<IRentalStatusHistory, RentalStatusHistoryRepository>();
            services.AddTransient<IUserAuthenticationRepository, UserAuthenticationRepository>();
            return services;
        }
    }
}
