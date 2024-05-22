using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.CommandHandlers.PlansHandlers;
using MotoRentalService.Application.MediatR.Commands.Plans;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;
using System;

namespace MotoRentalService.Application.Tests.Handlers.PlansHandlers
{
    public class UpdatePlanCommandHandlerTests
    {
        private readonly Mock<IPlansRepository> _plansRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UpdatePLanCommandHandler _handler;
        private readonly Mock<IValidator<UpdatePlanCommand>> _validator;

        public UpdatePlanCommandHandlerTests()
        {
            _plansRepositoryMock = new Mock<IPlansRepository>();
            _mapperMock = new Mock<IMapper>();
            _validator = new Mock<IValidator<UpdatePlanCommand>>();
            _handler = new UpdatePLanCommandHandler(_mapperMock.Object, _plansRepositoryMock.Object, _validator.Object);
        }

        [Fact]
        public async Task Handle_PlanUpdatedSuccessfully_ReturnsSuccessResult()
        {
            // Arrange
            var command = new UpdatePlanCommand
            {
                Id = 1,
                DurationDays = 30,
                DailyRate = 10,
                IsActive = true,
                Name = "Updated Plan"
            };

            var rentalPlan = new RentalPlans { Id = 1 };
            var updatedRentalPlan = new RentalPlans { Id = 1, DurationInDays = 30, DailyRate = 10, IsActive = true, Name = "Updated Plan" };
            var planDto = new PlanDto { Id = 1, DurationInDays = 30, DailyRate = 10, IsActive = true, Name = "Updated Plan" };

            _mapperMock.Setup(m => m.Map<RentalPlans>(command)).Returns(rentalPlan);
            _plansRepositoryMock.Setup(repo => repo.GetPlanByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync(rentalPlan);
            _plansRepositoryMock.Setup(repo => repo.UpdatePlanAsync(It.IsAny<RentalPlans>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedRentalPlan);
            _mapperMock.Setup(m => m.Map<PlanDto>(updatedRentalPlan)).Returns(planDto);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Plan was updated successfully", result.Message);
            Assert.Equal(planDto, result.Plan);
        }

        [Fact]
        public async Task Handle_PlanNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var command = new UpdatePlanCommand { Id = 1 };

            _mapperMock.Setup(m => m.Map<RentalPlans>(command)).Returns(new RentalPlans { Id = command.Id });
            _plansRepositoryMock.Setup(repo => repo.GetPlanByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync((RentalPlans)null);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act & Assert
            var result = await _handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(1, result.Errors.Count());
        }
    }
}
