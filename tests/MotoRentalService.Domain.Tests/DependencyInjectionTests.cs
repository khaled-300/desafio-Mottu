using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Domain.Services;
using MotoRentalService.Infrastructure;
using MotoRentalService.Services.Implementations;
using MotoRentalService.Services.Interfaces;

namespace MotoRentalService.Domain.Tests
{
    public class DependencyInjectionTests
    {
        private readonly IServiceCollection _services;
        private readonly IConfiguration _configuration;

        public DependencyInjectionTests()
        {
            _services = new ServiceCollection();
            var inMemorySettings = new Dictionary<string, string>
            {
                {"ImageStoragePath", "images"},
                {"JwtConfig:SecretKey", "your_super_secret_key"},
                {"JwtConfig:Issuer", "https://yourdomain.com"},
                {"JwtConfig:Audience", "https://yourdomain.com"},
                {"JwtConfig:AccessTokenExpirationMinutes", "30"}
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            _services.AddInfrastructure(_configuration);
            _services.AddDomain(_configuration);
        }

        [Fact]
        public void AddDomain_ShouldRegisterIMotorcycleService_AsTransient()
        {
            var provider = _services.BuildServiceProvider();
            var service1 = provider.GetService<IMotorcycleService>();
            var service2 = provider.GetService<IMotorcycleService>();

            Assert.NotNull(service1);
            Assert.IsType<MotorcycleService>(service1);
            Assert.NotSame(service1, service2);
        }

        [Fact]
        public void AddDomain_ShouldRegisterITokenService_AsSingleton()
        {
            var provider = _services.BuildServiceProvider();
            var service1 = provider.GetService<IStorageService>();
            var service2 = provider.GetService<IStorageService>();

            Assert.NotNull(service1);
            Assert.IsType<LocalStorageService>(service1);
            Assert.Same(service1, service2);
        }
    }
}
