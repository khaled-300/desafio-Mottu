using AutoMapper;
using Moq;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Application.MediatR.QueryHandlers;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.Tests.Handlers.QueryHandlers
{
    public class GetPlanByIdQueryHandlerTests
    {
        private readonly Mock<IPlansRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetPlanByIdQueryHandler _handler;

        public GetPlanByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IPlansRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetPlanByIdQueryHandler(_mapperMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_GetPlanById_ReturnsPlanDto()
        {
            // Arrange
            var query = new GetPlanByIdQuery { Id = 1 };
            var plan = new RentalPlans
            {
                Id = 1,
                Name = "Basic Plan",
                DurationInDays = 30,
                DailyRate = 9.99m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
            var planDto = new PlanDto
            {
                Id = 1,
                Name = "Basic Plan",
                DurationInDays = 30,
                DailyRate = 9.99m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _repositoryMock.Setup(repo => repo.GetPlanByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(plan);
            _mapperMock.Setup(mapper => mapper.Map<PlanDto?>(plan))
                       .Returns(planDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(planDto, result);
        }

        [Fact]
        public async Task Handle_GetPlanById_ReturnsNull_WhenPlanNotFound()
        {
            // Arrange
            var query = new GetPlanByIdQuery { Id = 1 };

            _repositoryMock.Setup(repo => repo.GetPlanByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((RentalPlans)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_ThrowsArgumentNullException_WhenRequestIsNull()
        {
            // Arrange
            GetPlanByIdQuery? query = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(query!, CancellationToken.None));
        }
    }
}
