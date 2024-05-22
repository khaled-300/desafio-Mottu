using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MotoRentalService.Services.Interfaces;

namespace MotoRentalService.Services.Implementations
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _uploadPath;
        private readonly IConfiguration _configuration;

        public LocalStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            var rootImageDirectory = Environment.GetEnvironmentVariable("IMAGE_ROOT_DIRECTORY") ?? _configuration.GetSection("ImageStoragePath").Value;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), rootImageDirectory!);
        }

        public bool IsValidFileType(string contentType)
        {
            var allowedTypes = new string[] { "image/png", "image/bmp" };
            return allowedTypes.Contains(contentType);
        }

        public async Task<string> SaveFileAsync(int personId, string fileName, byte[] fileContent, string contentType, CancellationToken cancellationToken)
        {
            if (!IsValidFileType(contentType))
            {
                throw new NotSupportedException("Unsupported file type.");
            }

            lock (_uploadPath)
            {
                if (!Directory.Exists(_uploadPath))
                {
                    Directory.CreateDirectory(_uploadPath);
                }
            }

            var filePath = Path.Combine(_uploadPath, personId.ToString(), fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.WriteAsync(fileContent, 0, fileContent.Length, cancellationToken);
            }

            return filePath;
        }

        public string GenerateUniqueFileName(string fileName, string contentType, int deliveryPersonId)
        {
            var randomFileName = $"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";

            var imagePath = Path.Combine(_uploadPath, deliveryPersonId.ToString());

            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            return randomFileName;
        }

        public async Task<byte[]> GetFileBytes(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                return memoryStream.ToArray();
            }
        }
    }
}
