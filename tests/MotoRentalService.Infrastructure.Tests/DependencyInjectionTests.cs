using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoRentalService.Domain.Interfaces.repository;
using MotoRentalService.Infrastructure.Repositories.Impl;

namespace MotoRentalService.Infrastructure.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void AddInfrastructure_ShouldRegisterDbContext_WithInMemoryDatabase()
        {
            // Arrange
            var services = new ServiceCollection();

            // Adding In-Memory database
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            var configuration = new ConfigurationBuilder().Build();
            services.AddInfrastructure(configuration); 

            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<AppDbContext>();

            // Act & Assert
            Assert.NotNull(dbContext);
            Assert.IsType<AppDbContext>(dbContext);

            dbContext.Users.Add(new MotoRentalService.Domain.Entities.Users { Email = "test@example.com", Password = "testPassword" }); // Make sure to initialize the required properties
            dbContext.SaveChanges();

            var user = dbContext.Users.FirstOrDefault(u => u.Email == "test@example.com");
            Assert.NotNull(user);
        }

        [Theory]
        [InlineData(typeof(IMotorcycleRepository), typeof(MotorcycleRepository))]
        [InlineData(typeof(IRentalRepository), typeof(RentalRepository))]
        [InlineData(typeof(IDeliveryUserRepository), typeof(DeliveryUserRepository))]
        [InlineData(typeof(IPlansRepository), typeof(PlansRepository))]
        [InlineData(typeof(IRentalStatusHistory), typeof(RentalStatusHistoryRepository))]
        [InlineData(typeof(IUserAuthenticationRepository), typeof(UserAuthenticationRepository))]
        public void AddInfrastructure_ShouldRegisterRepositories_WithCorrectImplementations(Type serviceType, Type implementationType)
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();

            // Act
            services.AddInfrastructure(configuration);
            var serviceProvider = services.BuildServiceProvider();
            var serviceInstance = serviceProvider.GetRequiredService(serviceType);

            // Assert
            Assert.NotNull(serviceInstance);
            Assert.IsType(implementationType, serviceInstance);
        }
    }
}
