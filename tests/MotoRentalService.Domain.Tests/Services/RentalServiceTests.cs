using FluentValidation;
using FluentValidation.Results;
using Moq;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;
using MotoRentalService.Domain.Services;
using MotoRentalService.Domain.ValueObjects;
using System.Numerics;

namespace MotoRentalService.Domain.Tests.Services
{
    public class RentalServiceTests
    {
        private readonly Mock<IDeliveryUserRepository> _mockDeliveryUserRepository;
        private readonly Mock<IMotorcycleRepository> _mockMotorcycleRepository;
        private readonly Mock<IRentalRepository> _mockRentalRepository;
        private readonly Mock<IPlansRepository> _mockPlansRepository;
        private readonly Mock<IRentalStatusHistory> _mockRentalStatusHistory;
        private readonly RentalService _rentalService;

        public RentalServiceTests()
        {
            _mockDeliveryUserRepository = new Mock<IDeliveryUserRepository>();
            _mockMotorcycleRepository = new Mock<IMotorcycleRepository>();
            _mockRentalRepository = new Mock<IRentalRepository>();
            _mockPlansRepository = new Mock<IPlansRepository>();
            _mockRentalStatusHistory = new Mock<IRentalStatusHistory>();

            _rentalService = new RentalService(
                _mockDeliveryUserRepository.Object,
                _mockMotorcycleRepository.Object,
                _mockRentalRepository.Object,
                _mockPlansRepository.Object,
                _mockRentalStatusHistory.Object);
        }

        [Fact]
        public async Task RentMotorcycleAsync_ShouldThrowArgumentNullException_WhenRentalIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _rentalService.RentMotorcycleAsync(null, CancellationToken.None));
        }

        [Fact]
        public async Task RentMotorcycleAsync_ShouldThrowKeyNotFoundException_WhenDeliveryPersonNotFound()
        {
            // Arrange
            _mockDeliveryUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((DeliveryUser)null);

            var rental = new Rental
            {
                MotorcycleId = 1,
                UserId = 1,
                RentalPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _rentalService.RentMotorcycleAsync(rental, CancellationToken.None));
        }

        [Fact]
        public async Task RentMotorcycleAsync_ShouldThrowInvalidOperationException_WhenDeliveryPersonHasInvalidLicense()
        {
            // Arrange
            var deliveryUser = new DeliveryUser
            {
                Id = 1,
                LicenseType = LicenseType.B, // Invalid license type
            };

            _mockDeliveryUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(deliveryUser);

            var rental = new Rental
            {
                MotorcycleId = 1,
                UserId = deliveryUser.Id,
                RentalPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _rentalService.RentMotorcycleAsync(rental, CancellationToken.None));
        }

        [Fact]
        public async Task RentMotorcycleAsync_ShouldThrowKeyNotFoundException_WhenMotorcycleNotFound()
        {
            // Arrange
            _mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Motorcycle)null);

            var rental = new Rental
            {
                MotorcycleId = 1,
                UserId = 1,
                RentalPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _rentalService.RentMotorcycleAsync(rental, CancellationToken.None));
        }

        [Fact]
        public async Task RentMotorcycleAsync_ShouldThrowInvalidOperationException_WhenMotorcycleIsAlreadyRented()
        {
            // Arrange
            var motorcycle = new Motorcycle
            {
                Id = 1,
                IsRented = true
            };
            var rental = new Rental
            {
                MotorcycleId = motorcycle.Id,
                UserId = 1,
                RentalPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };

            var user = new DeliveryUser
            {
                Id = 1,
                UserId = 1,
            };

            _mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(motorcycle);
            _mockDeliveryUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _rentalService.RentMotorcycleAsync(rental, CancellationToken.None));
        }

        [Fact]
        public async Task RentMotorcycleAsync_ShouldThrowKeyNotFoundException_WhenRentalPlanNotFound()
        {
            // Arrange
            var deliveryUser = new DeliveryUser
            {
                Id = 1,
                LicenseType = LicenseType.A,
            };

            var motorcycle = new Motorcycle
            {
                Id = 1,
                IsRented = false
            };

            _mockDeliveryUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(deliveryUser);

            _mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(motorcycle);

            _mockPlansRepository.Setup(repo => repo.GetPlanByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RentalPlans)null);

            var rental = new Rental
            {
                MotorcycleId = motorcycle.Id,
                UserId = deliveryUser.Id,
                RentalPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _rentalService.RentMotorcycleAsync(rental, CancellationToken.None));
        }

        [Fact]
        public async Task RentMotorcycleAsync_ShouldRentMotorcycle()
        {
            // Arrange
            var deliveryUser = new DeliveryUser
            {
                Id = 1,
                LicenseType = LicenseType.A,
                Status = UserStatus.Approved,
                CNPJ = null,
                CreatedAt = DateTime.Today.AddDays(-10),
                DateOfBirth = DateTime.Today.AddDays(-30),
                LicenseImageFullName = "CNH.png",
                LicenseImageURL = "path/to/CNH.png",
                LicenseNumber = "123123123123",
                Name = "Test",
                UpdatedAt = DateTime.UtcNow,
                UserId = 1,
            };
            var motorcycle = new Motorcycle
            {
                Id = 1,
                IsRented = false
            };

            var plan = new RentalPlans
            {
                Id = 1,
                DurationInDays = 7,
                DailyRate = 100m
            };
            var rental = new Rental
            {
                Id = 1,
                MotorcycleId = motorcycle.Id,
                UserId = deliveryUser.UserId,
                RentalPlanId = plan.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7),
                ExpectedEndDate = DateTime.UtcNow.AddDays(7),
                DailyRate = 100m,
                TotalPrice = 700m,
                Status = RentalStatus.Pending
            };

            var history = new RentalStatusHistory
            {
                CreatedAt = DateTime.UtcNow,
                Id = 1,
                Rental = rental,
                RentalId = rental.Id,
                Status = rental.Status
            };

            _mockDeliveryUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(deliveryUser);

            _mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(motorcycle);

            _mockPlansRepository.Setup(repo => repo.GetPlanByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(plan);

            _mockRentalRepository.Setup(repo => repo.AddRentAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(rental);

            _mockMotorcycleRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Motorcycle>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockRentalStatusHistory.Setup(repo => repo.AddRentalStatusHistoryAsync(It.IsAny<RentalStatusHistory>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(history);

            // Act
            var result = await _rentalService.RentMotorcycleAsync(rental, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(RentalStatus.Pending, result.Status);
            _mockRentalRepository.Verify(repo => repo.AddRentAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockMotorcycleRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Motorcycle>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RentMotorcycleAsync_ShouldThrowException_WhenMotorcycleIsAlreadyRented()
        {
            // Arrange
            var motorcycle = new Motorcycle { Id = 1, IsRented = true };
            var deliveryUser = new DeliveryUser { Id = 1, Name = "test", Status = UserStatus.Approved, UserId = 1, LicenseType = LicenseType.A, CNPJ = "123123123123", DateOfBirth = DateTime.Today.AddDays(-30), CreatedAt = DateTime.UtcNow, UpdatedAt = null, LicenseNumber = "123123123123", LicenseImageFullName = "test.png", LicenseImageURL = "path/to/test.png" };
            var rental = new Rental
            {
                Id = 1,
                MotorcycleId = 1,
                UserId = 1,
                RentalPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7),
                ExpectedEndDate = DateTime.UtcNow.AddDays(7),
                DailyRate = 100m,
                TotalPrice = 700m,
                Status = RentalStatus.Pending
            };
            _mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(motorcycle);
            _mockDeliveryUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(deliveryUser);


            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _rentalService.RentMotorcycleAsync(rental, CancellationToken.None));
        }

        [Fact]
        public async Task RentMotorcycleAsync_ShouldThrowException_WhenUserIsInvalid()
        {
            // Arrange
            var rental = new Rental
            {
                Id = 1,
                MotorcycleId = 1,
                UserId = 1,
                RentalPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7),
                ExpectedEndDate = DateTime.UtcNow.AddDays(7),
                DailyRate = 100m,
                TotalPrice = 700m,
                Status = RentalStatus.Pending
            };
            _mockDeliveryUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((DeliveryUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _rentalService.RentMotorcycleAsync(rental, CancellationToken.None));
        }

        [Fact]
        public async Task RentMotorcycleAsync_ShouldThrowException_WhenPlanIsInvalid()
        {
            // Arrange
            var deliveryUser = new DeliveryUser { Id = 1, Name = "test", Status = UserStatus.Approved, UserId = 1, LicenseType = LicenseType.A, CNPJ = "123123123123", DateOfBirth = DateTime.Today.AddDays(-30), CreatedAt = DateTime.UtcNow, UpdatedAt = null, LicenseNumber = "123123123123", LicenseImageFullName = "test.png", LicenseImageURL = "path/to/test.png" };
            var motorcycle = new Motorcycle { Id = 1, IsRented = false, LicensePlate = "123123123", UpdatedAt = null, CreatedAt = DateTime.UtcNow, Model = "1231sdaa", Year = 2000 };
            var rental = new Rental
            {
                Id = 1,
                MotorcycleId = motorcycle.Id,
                UserId = deliveryUser.UserId,
                RentalPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7),
                ExpectedEndDate = DateTime.UtcNow.AddDays(7),
                DailyRate = 100m,
                TotalPrice = 700m,
                Status = RentalStatus.Pending
            };
            _mockDeliveryUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(deliveryUser);

            _mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(motorcycle);

            _mockPlansRepository.Setup(repo => repo.GetPlanByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RentalPlans)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _rentalService.RentMotorcycleAsync(rental, CancellationToken.None));
        }

        [Fact]
        public async Task MarkRentalAsCompletedAsync_ShouldUpdateRentalStatus()
        {
            // Arrange
            var rental = new Rental { Id = 1, Status = RentalStatus.Pending };
            var history = new RentalStatusHistory
            {
                CreatedAt = DateTime.UtcNow,
                Id = 1,
                Rental = rental,
                RentalId = rental.Id,
                Status = rental.Status
            };

            _mockRentalRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(rental);

            _mockRentalRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockRentalStatusHistory.Setup(repo => repo.AddRentalStatusHistoryAsync(It.IsAny<RentalStatusHistory>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(history);

            // Act
            var result = await _rentalService.MarkRentalAsCompletedAsync(1, CancellationToken.None);

            // Assert
            Assert.Equal(RentalStatus.Completed, result.Status);
            _mockRentalRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockRentalStatusHistory.Verify(repo => repo.AddRentalStatusHistoryAsync(It.IsAny<RentalStatusHistory>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CalculateFinalPriceAsync_ShouldReturnCorrectPrice()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1); // Use a fixed date for consistency
            var endDate = startDate.AddDays(7);
            var returnDate = startDate.AddDays(6);

            var rental = new Rental
            {
                MotorcycleId = 1,
                RentalPlanId = 1,
                StartDate = startDate,
                EndDate = endDate,
                ExpectedEndDate = endDate,
                DailyRate = 100m
            };

            var motorcycle = new Motorcycle { Id = 1 };
            var plan = new RentalPlans { Id = 1, DurationInDays = 7, DailyRate = 100m };

            _mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(motorcycle);

            _mockPlansRepository.Setup(repo => repo.GetPlanByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(plan);

            // Act
            var finalPrice = await _rentalService.CalculateFinalPriceAsync(rental, returnDate, CancellationToken.None);

            // Assert
            decimal expectedPrice = 700m - 100m + (0.2m * 100m); // baseCost - unpaidDaysCost + fineAmount
            Assert.Equal(expectedPrice, finalPrice);
        }

        [Fact]
        public async Task CalculateFinalPriceAsync_ShouldReturnCorrectPrice_WhenReturnedOnExpectedEndDate()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = startDate.AddDays(7);
            var returnDate = endDate;

            var rental = new Rental
            {
                MotorcycleId = 1,
                RentalPlanId = 1,
                StartDate = startDate,
                EndDate = endDate,
                ExpectedEndDate = endDate,
                DailyRate = 100m
            };

            var motorcycle = new Motorcycle { Id = 1 };
            var plan = new RentalPlans { Id = 1, DurationInDays = 7, DailyRate = 100m };

            _mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(motorcycle);

            _mockPlansRepository.Setup(repo => repo.GetPlanByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(plan);

            // Act
            var finalPrice = await _rentalService.CalculateFinalPriceAsync(rental, returnDate, CancellationToken.None);

            // Assert
            Assert.Equal(700m, finalPrice); // No change, since it's on the expected end date
        }

        [Fact]
        public async Task CalculateFinalPriceAsync_ShouldReturnCorrectPrice_WhenReturnedAfterExpectedEndDate()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = startDate.AddDays(7);
            var returnDate = endDate.AddDays(1);

            var rental = new Rental
            {
                MotorcycleId = 1,
                RentalPlanId = 1,
                StartDate = startDate,
                EndDate = endDate,
                ExpectedEndDate = endDate,
                DailyRate = 100m
            };

            var motorcycle = new Motorcycle { Id = 1 };
            var plan = new RentalPlans { Id = 1, DurationInDays = 7, DailyRate = 100m };

            _mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(motorcycle);

            _mockPlansRepository.Setup(repo => repo.GetPlanByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(plan);

            // Act
            var finalPrice = await _rentalService.CalculateFinalPriceAsync(rental, returnDate, CancellationToken.None);

            // Assert
            decimal expectedPrice = 700m + 50m; // baseCost + 1 day late charge
            Assert.Equal(expectedPrice, finalPrice);
        }

        [Fact]
        public async Task CalculateFinalPriceAsync_ShouldThrowException_WhenReturnedBeforeStartDate()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = startDate.AddDays(7);
            var returnDate = startDate.AddDays(-1);

            var rental = new Rental
            {
                MotorcycleId = 1,
                RentalPlanId = 1,
                StartDate = startDate,
                EndDate = endDate,
                ExpectedEndDate = endDate,
                DailyRate = 100m
            };

            var motorcycle = new Motorcycle { Id = 1, IsRented = true };

            _mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(motorcycle);

            _mockPlansRepository.Setup(repo => repo.GetPlanByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RentalPlans)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _rentalService.CalculateFinalPriceAsync(rental, returnDate, CancellationToken.None));
        }
    }
}
