using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.CommandHandlers.PlansHandlers;
using MotoRentalService.Application.MediatR.Commands.Plans;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.Tests.Handlers.PlansHandlers
{
    public class CreatePlanCommandHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPlansRepository> _plansRepositoryMock;
        private readonly CreatePlanCommandHandler _handler;
        private readonly Mock<IValidator<CreatePlanCommand>> _validator;

        public CreatePlanCommandHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _plansRepositoryMock = new Mock<IPlansRepository>();
            _validator = new Mock<IValidator<CreatePlanCommand>>();
            _handler = new CreatePlanCommandHandler(_mapperMock.Object, _plansRepositoryMock.Object, _validator.Object);
        }

        [Fact]
        public async Task Handle_PlanCreatedSuccessfully_ReturnsSuccessResult()
        {
            // Arrange
            var command = new CreatePlanCommand { Name = "Test Plan", DailyRate = 100, DurationInDays = 30 };
            var rentalPlan = new RentalPlans { Id = 1, Name = "Test Plan", DailyRate = 100, DurationInDays = 30 };
            var planDto = new PlanDto { Id = 1, Name = "Test Plan", DailyRate = 100, DurationInDays = 30 };

            _mapperMock.Setup(m => m.Map<RentalPlans>(command)).Returns(rentalPlan);
            _plansRepositoryMock.Setup(repo => repo.AddPlanAsync(rentalPlan, It.IsAny<CancellationToken>())).ReturnsAsync(rentalPlan);
            _mapperMock.Setup(m => m.Map<PlanDto>(rentalPlan)).Returns(planDto);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Plan was created successfully", result.Message);
            Assert.Equal(planDto, result.Plan);
        }

        [Fact]
        public async Task Handle_PlanCreationFailed_ThrowsException()
        {
            // Arrange
            var command = new CreatePlanCommand { Name = "Test Plan", DailyRate = 100, DurationInDays = 30 };
            var rentalPlan = new RentalPlans { Id = 1, Name = "Test Plan", DailyRate = 100, DurationInDays = 30 };

            _mapperMock.Setup(m => m.Map<RentalPlans>(command)).Returns(rentalPlan);
            _plansRepositoryMock.Setup(repo => repo.AddPlanAsync(rentalPlan, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Plan creation failed"));
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act & Assert
            var result = await _handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(1, result.Errors.Count());
        }
    }
}
