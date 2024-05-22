using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MotoRentalService.Application.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void AddApplication_RegistersMediatR()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplication();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var mediatr = serviceProvider.GetService<IMediator>();
            Assert.NotNull(mediatr);
        }

        [Fact]
        public void AddApplication_RegistersAutoMapper()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplication();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var mapper = serviceProvider.GetService<IMapper>();
            Assert.NotNull(mapper);
        }

        [Fact]
        public void AddApplication_RegistersAllValidatorsFromAssembly()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = typeof(DependencyInjection).Assembly;

            // Act
            services.AddApplication();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var validators = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)))
                .Select(t => serviceProvider.GetService(t));

            Assert.All(validators, validator => Assert.NotNull(validator));
        }
    }
}
