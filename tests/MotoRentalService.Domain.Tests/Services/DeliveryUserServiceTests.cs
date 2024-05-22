using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Domain.Interfaces.repository;
using MotoRentalService.Domain.Services;
using MotoRentalService.Domain.ValueObjects;
using MotoRentalService.Services.Interfaces;

namespace MotoRentalService.Domain.Tests.Services
{
    public class DeliveryUserServiceTests
    {
        private readonly Mock<IDeliveryUserRepository> _deliveryUserRepositoryMock;
        private readonly Mock<IStorageService> _storageServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly IDeliveryUserService _deliveryUserService;

        public DeliveryUserServiceTests()
        {
            _deliveryUserRepositoryMock = new Mock<IDeliveryUserRepository>();
            _storageServiceMock = new Mock<IStorageService>();
            _configurationMock = new Mock<IConfiguration>();
            _deliveryUserService = new DeliveryUserService(
                _deliveryUserRepositoryMock.Object,
                _storageServiceMock.Object,
                _configurationMock.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var cancellationToken = CancellationToken.None;
            var deliveryUser = new DeliveryUser { Id = userId, Name = "John Doe" };
            _deliveryUserRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, cancellationToken))
                .ReturnsAsync(deliveryUser);

            // Act
            var result = await _deliveryUserService.GetUserByIdAsync(userId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(deliveryUser);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var cancellationToken = CancellationToken.None;
            _deliveryUserRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, cancellationToken))
                .ReturnsAsync((DeliveryUser)null);

            // Act
            var result = await _deliveryUserService.GetUserByIdAsync(userId, cancellationToken);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByUserIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var cancellationToken = CancellationToken.None;
            var deliveryUser = new DeliveryUser { Id = 1, Name = "John Doe", UserId = userId };
            _deliveryUserRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId, cancellationToken))
                .ReturnsAsync(deliveryUser);

            // Act
            var result = await _deliveryUserService.GetUserByUserIdAsync(userId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(deliveryUser);
        }

        [Fact]
        public async Task GetUserByUserIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var cancellationToken = CancellationToken.None;
            _deliveryUserRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId, cancellationToken))
                .ReturnsAsync((DeliveryUser)null);

            // Act
            var result = await _deliveryUserService.GetUserByUserIdAsync(userId, cancellationToken);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRegisterUserWithoutImage_WhenImageIsNull()
        {
            // Arrange
            var user = new DeliveryUser { Id = 1, CNPJ = "12.345.678/0001-99", Status = UserStatus.Pending, UserId = 1, Name = "User Test", LicenseType = LicenseType.AB, LicenseNumber = "123123123123", DateOfBirth = DateTime.Today.AddDays(-30) };
            var cancellationToken = CancellationToken.None;

            _deliveryUserRepositoryMock.Setup(repo => repo.AddAsync(user, cancellationToken))
                .ReturnsAsync(user);

            // Act
            var result = await _deliveryUserService.RegisterUserAsync(user, null, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(UserStatus.Pending);
            _deliveryUserRepositoryMock.Verify(repo => repo.AddAsync(user, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenImageIsInvalid()
        {
            // Arrange
            var user = new DeliveryUser { Id = 1, CNPJ = "12.345.678/0001-99", Status = UserStatus.Pending, UserId = 1, Name = "User Test", LicenseType = LicenseType.AB, LicenseNumber = "123123123123", DateOfBirth = DateTime.Today.AddDays(-30) };
            var image = new Mock<IFormFile>();
            var imageName = "image.jpeg";
            var imageContentType = "image/jpeg";
            var imageData = new byte[] { 0x20 };
            var cancellationToken = CancellationToken.None;

            image.Setup(f => f.Length).Returns(1);
            image.Setup(f => f.ContentType).Returns(imageContentType);
            image.Setup(f => f.FileName).Returns(imageName);

            _storageServiceMock.Setup(service => service.IsValidFileType(image.Object.ContentType))
                .Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _deliveryUserService.RegisterUserAsync(user, image.Object, cancellationToken));
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRegisterUserWithImage_WhenImageIsValid()
        {
            // Arrange
            var user = new DeliveryUser { Id = 1, CNPJ = "12.345.678/0001-99", Status = UserStatus.Pending, UserId = 1, Name = "User Test", LicenseType = LicenseType.AB, LicenseNumber = "123123123123", DateOfBirth = DateTime.Today.AddDays(-30) };
            var savedUser = new DeliveryUser { Id = 1, CNPJ = "12.345.678/0001-99", Status = UserStatus.Approved, UserId = 1, Name = "User Test", LicenseType = LicenseType.AB, LicenseNumber = "123123123123", DateOfBirth = DateTime.Today.AddDays(-30) };
            var image = new Mock<IFormFile>();
            var imageName = "image.png";
            var imageContentType = "image/png";
            var imageData = new byte[] { 0x20 };
            var cancellationToken = CancellationToken.None;

            image.Setup(f => f.Length).Returns(1);
            image.Setup(f => f.ContentType).Returns(imageContentType);
            image.Setup(f => f.FileName).Returns(imageName);

            _storageServiceMock.Setup(service => service.IsValidFileType(imageContentType))
                .Returns(true);
            _deliveryUserRepositoryMock.Setup(repo => repo.AddAsync(user, cancellationToken))
                .ReturnsAsync(savedUser);
            _storageServiceMock.Setup(service => service.GetFileBytes(image.Object))
                .ReturnsAsync(imageData);
            _storageServiceMock.Setup(service => service.GenerateUniqueFileName(imageName, imageContentType, user.Id))
                .Returns(imageName);
            _storageServiceMock.Setup(service => service.SaveFileAsync(savedUser.Id, imageName, imageData, imageContentType, cancellationToken))
                .ReturnsAsync("path/to/image.png");

            // Act
            var result = await _deliveryUserService.RegisterUserAsync(user, image.Object, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(UserStatus.Approved);
            result.LicenseImageURL.Should().Be("path/to/image.png");
            result.LicenseImageFullName.Should().Be(imageName);
            _deliveryUserRepositoryMock.Verify(repo => repo.UpdateAsync(user, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task UpdateUserLicenseImageAsync_ShouldThrowException_WhenImageIsNull()
        {
            // Arrange
            var user = new DeliveryUser { Id = 1, CNPJ = "12.345.678/0001-99", Status = UserStatus.Pending, UserId = 1, Name = "User Test", LicenseType = LicenseType.AB, LicenseNumber = "123123123123", DateOfBirth = DateTime.Today.AddDays(-30) };
            IFormFile? image = null;
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _deliveryUserService.UpdateUserLicenseImageAsync(user, image, cancellationToken));
            exception.Message.Should().Be("No image data provided.");
        }

        [Fact]
        public async Task UpdateUserLicenseImageAsync_ShouldThrowException_WhenImageIsInvalid()
        {
            // Arrange
            var user = new DeliveryUser { Id = 1, CNPJ = "12.345.678/0001-99", Status = UserStatus.Pending, UserId = 1, Name = "User Test", LicenseType = LicenseType.AB, LicenseNumber = "123123123123", DateOfBirth = DateTime.Today.AddDays(-30) };
            var image = new Mock<IFormFile>();
            image.Setup(f => f.Length).Returns(1);
            image.Setup(f => f.ContentType).Returns("image/jpeg");
            var cancellationToken = CancellationToken.None;

            _storageServiceMock.Setup(service => service.IsValidFileType(image.Object.ContentType))
                .Returns(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _deliveryUserService.UpdateUserLicenseImageAsync(user, image.Object, cancellationToken));
            exception.Message.Should().Be("Invalid image format.");
        }

        [Fact]
        public async Task UpdateUserLicenseImageAsync_ShouldUpdateUser_WhenImageIsValidAndFileNameIsGenerated()
        {
            // Arrange
            var user = new DeliveryUser { Id = 1, CNPJ = "12.345.678/0001-99", Status = UserStatus.Pending, UserId = 1, Name = "User Test", LicenseType = LicenseType.AB, LicenseNumber = "123123123123", DateOfBirth = DateTime.Today.AddDays(-30) };
            var image = new Mock<IFormFile>();
            var imageName = "image.png";
            var imageContentType = "image/png";
            var imageData = new byte[] { 0x20 };
            var cancellationToken = CancellationToken.None;

            var expectedImageName = "fc1ea6bea5f14246af7443800b930664.png";
            var expectedImagePath = "images/1/fc1ea6bea5f14246af7443800b930664.png";

            image.Setup(f => f.Length).Returns(1);
            image.Setup(f => f.ContentType).Returns(imageContentType);
            image.Setup(f => f.FileName).Returns(imageName);

            _storageServiceMock.Setup(service => service.IsValidFileType(imageContentType))
                .Returns(true);
            _storageServiceMock.Setup(service => service.GetFileBytes(image.Object))
                .ReturnsAsync(imageData);
            _storageServiceMock.Setup(service => service.GenerateUniqueFileName(imageName, imageContentType, user.Id))
                .Returns(expectedImageName);
            _storageServiceMock.Setup(service => service.SaveFileAsync(user.Id, expectedImageName, imageData, imageContentType, cancellationToken))
                .ReturnsAsync(expectedImagePath);
            _deliveryUserRepositoryMock.Setup(repo => repo.UpdateAsync(user, cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _deliveryUserService.UpdateUserLicenseImageAsync(user, image.Object, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.LicenseImageURL.Should().Be(expectedImagePath);
            result.LicenseImageFullName.Should().Be(expectedImageName);
            _deliveryUserRepositoryMock.Verify(repo => repo.UpdateAsync(user, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task UpdateUserLicenseImageAsync_ShouldUpdateUser_WhenImageIsValidAndFileNameIsNotGenerated()
        {
            // Arrange
            var user = new DeliveryUser { Id = 1, CNPJ = "12.345.678/0001-99", Status = UserStatus.Pending, UserId = 1, Name = "User Test", LicenseType = LicenseType.AB, LicenseNumber = "123123123123", DateOfBirth = DateTime.Today.AddDays(-30), LicenseImageFullName = "fc1ea6bea5f14246af7443800b930664.png" };
            var image = new Mock<IFormFile>();
            var imageName = "image.png";
            var imageContentType = "image/png";
            var imageData = new byte[] { 0x20 };
            var cancellationToken = CancellationToken.None;

            image.Setup(f => f.Length).Returns(1);
            image.Setup(f => f.ContentType).Returns(imageContentType);
            image.Setup(f => f.FileName).Returns(imageName);

            var expectedImageName = "fc1ea6bea5f14246af7443800b930664.png";
            var expectedImagePath = "images/1/fc1ea6bea5f14246af7443800b930664.png";

            _storageServiceMock.Setup(service => service.IsValidFileType(imageContentType))
                .Returns(true);
            _storageServiceMock.Setup(service => service.GetFileBytes(image.Object))
                .ReturnsAsync(imageData);
            _storageServiceMock.Setup(service => service.SaveFileAsync(user.Id, user.LicenseImageFullName, imageData, imageContentType, cancellationToken))
                .ReturnsAsync(expectedImagePath);
            _deliveryUserRepositoryMock.Setup(repo => repo.UpdateAsync(user, cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _deliveryUserService.UpdateUserLicenseImageAsync(user, image.Object, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.LicenseImageURL.Should().Be(expectedImagePath);
            result.LicenseImageFullName.Should().Be(expectedImageName);
            _deliveryUserRepositoryMock.Verify(repo => repo.UpdateAsync(user, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUserFromRepository()
        {
            // Arrange
            var userId = 1;
            var cancellationToken = CancellationToken.None;

            var rootImageDirectory = "ImageRootDirectory";
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), rootImageDirectory, userId.ToString());

            _deliveryUserRepositoryMock.Setup(repo => repo.DeleteByIdAsync(userId, cancellationToken))
                .Returns(Task.CompletedTask);

            Environment.SetEnvironmentVariable("IMAGE_ROOT_DIRECTORY", rootImageDirectory);

            _deliveryUserRepositoryMock.Setup(repo => repo.DeleteByIdAsync(userId, cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _deliveryUserService.DeleteUserAsync(userId, cancellationToken);

            // Assert
            _deliveryUserRepositoryMock.Verify(repo => repo.DeleteByIdAsync(userId, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUserDirectory_WhenDirectoryExists()
        {
            // Arrange
            var userId = 1;
            var cancellationToken = CancellationToken.None;
            var rootImageDirectory = "ImageRootDirectory";
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), rootImageDirectory, userId.ToString());

            _deliveryUserRepositoryMock.Setup(repo => repo.DeleteByIdAsync(userId, cancellationToken))
                .Returns(Task.CompletedTask);

            Environment.SetEnvironmentVariable("IMAGE_ROOT_DIRECTORY", rootImageDirectory);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Act
            await _deliveryUserService.DeleteUserAsync(userId, cancellationToken);

            // Assert
            Assert.False(Directory.Exists(directoryPath));
            _deliveryUserRepositoryMock.Verify(repo => repo.DeleteByIdAsync(userId, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldNotThrow_WhenDirectoryDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var cancellationToken = CancellationToken.None;
            var rootImageDirectory = "ImageRootDirectory";
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), rootImageDirectory, userId.ToString());

            _deliveryUserRepositoryMock.Setup(repo => repo.DeleteByIdAsync(userId, cancellationToken))
                .Returns(Task.CompletedTask);

            Environment.SetEnvironmentVariable("IMAGE_ROOT_DIRECTORY", rootImageDirectory);

            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }

            // Act
            await _deliveryUserService.DeleteUserAsync(userId, cancellationToken);

            // Assert
            Assert.False(Directory.Exists(directoryPath));
            _deliveryUserRepositoryMock.Verify(repo => repo.DeleteByIdAsync(userId, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldUseConfigurationPath_WhenEnvironmentVariableIsNotSet()
        {
            // Arrange
            var userId = 1;
            var cancellationToken = CancellationToken.None;
            var rootImageDirectory = "ImageRootDirectory";
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), rootImageDirectory, userId.ToString());

            _deliveryUserRepositoryMock.Setup(repo => repo.DeleteByIdAsync(userId, cancellationToken))
                .Returns(Task.CompletedTask);

            _configurationMock.Setup(config => config.GetSection("ImageStoragePath").Value)
                .Returns(rootImageDirectory);

            Environment.SetEnvironmentVariable("IMAGE_ROOT_DIRECTORY", null);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Act
            await _deliveryUserService.DeleteUserAsync(userId, cancellationToken);

            // Assert
            Assert.False(Directory.Exists(directoryPath));
            _deliveryUserRepositoryMock.Verify(repo => repo.DeleteByIdAsync(userId, cancellationToken), Times.Once);
            _configurationMock.Verify(config => config.GetSection("ImageStoragePath").Value, Times.Once);
        }

    }
}
