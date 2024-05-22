using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using MotoRentalService.Services.Implementations;

namespace MotoRentalService.Domain.Tests.Services
{
    public class LocalStorageServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly LocalStorageService _localStorageService;
        private readonly string _testUploadPath;

        public LocalStorageServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _testUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "TestUploads");
            _configurationMock.Setup(config => config.GetSection("ImageStoragePath").Value)
                .Returns("TestUploads");

            Environment.SetEnvironmentVariable("IMAGE_ROOT_DIRECTORY", null);

            _localStorageService = new LocalStorageService(_configurationMock.Object);
        }

        [Fact]
        public void IsValidFileType_ShouldReturnTrueForValidFileTypes()
        {
            // Arrange
            var validTypes = new[] { "image/png", "image/bmp" };

            // Act & Assert
            foreach (var type in validTypes)
            {
                Assert.True(_localStorageService.IsValidFileType(type));
            }
        }

        [Fact]
        public void IsValidFileType_ShouldReturnFalseForInvalidFileTypes()
        {
            // Arrange
            var invalidTypes = new[] { "image/jpeg", "text/plain", "application/pdf" };

            // Act & Assert
            foreach (var type in invalidTypes)
            {
                Assert.False(_localStorageService.IsValidFileType(type));
            }
        }

        [Fact]
        public async Task SaveFileAsync_ShouldSaveFileAndReturnFilePath()
        {
            // Arrange
            var personId = 1;
            var fileName = "test.png";
            var fileContent = new byte[] { 0x1, 0x2, 0x3 };
            var contentType = "image/png";
            var cancellationToken = CancellationToken.None;

            // Act
            var filePath = await _localStorageService.SaveFileAsync(personId, fileName, fileContent, contentType, cancellationToken);

            // Assert
            Assert.True(File.Exists(filePath));
            var savedContent = await File.ReadAllBytesAsync(filePath);
            Assert.Equal(fileContent, savedContent);
        }

        [Fact]
        public async Task SaveFileAsync_ShouldThrowNotSupportedExceptionForInvalidFileTypes()
        {
            // Arrange
            var personId = 1;
            var fileName = "test.pdf";
            var fileContent = new byte[] { 0x1, 0x2, 0x3 };
            var contentType = "application/pdf";
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            await Assert.ThrowsAsync<NotSupportedException>(() =>
                _localStorageService.SaveFileAsync(personId, fileName, fileContent, contentType, cancellationToken));
        }


        [Fact]
        public void GenerateUniqueFileName_ShouldGenerateUniqueFileName()
        {
            // Arrange
            var fileName = "test.png";
            var contentType = "image/png";
            var deliveryPersonId = 1;

            // Act
            var uniqueFileName = _localStorageService.GenerateUniqueFileName(fileName, contentType, deliveryPersonId);
            var imagePath = Path.Combine(_testUploadPath, deliveryPersonId.ToString());

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(uniqueFileName));
            Assert.EndsWith(".png", uniqueFileName);
            Assert.True(Directory.Exists(imagePath));
        }


        [Fact]
        public async Task GetFileBytes_ShouldReturnByteArrayOfFileContent()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "Hello World";
            var fileName = "test.txt";
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(content);
            writer.Flush();
            memoryStream.Position = 0;

            fileMock.Setup(_ => _.OpenReadStream()).Returns(memoryStream);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(memoryStream.Length);
            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream stream, System.Threading.CancellationToken token) => memoryStream.CopyToAsync(stream, 81920, token));
            // Act
            var fileBytes = await _localStorageService.GetFileBytes(fileMock.Object);

            // Assert
            var resultContent = System.Text.Encoding.UTF8.GetString(fileBytes);
            Assert.Equal(content, resultContent);
        }

    }
}
