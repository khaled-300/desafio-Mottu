using Microsoft.AspNetCore.Http;

namespace MotoRentalService.Services.Interfaces
{
    public interface IStorageService
    {
        Task<string> SaveFileAsync(int personId, string fileName, byte[] fileContent, string contentType, CancellationToken cancellationToken);
        bool IsValidFileType(string contentType);
        string GenerateUniqueFileName(string fileName, string contentType, int deliveryPersonId);
        Task<byte[]> GetFileBytes(IFormFile file);
    }
}
