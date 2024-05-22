using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Domain.Interfaces.repository;
using MotoRentalService.Domain.Validations;
using MotoRentalService.Domain.ValueObjects;
using MotoRentalService.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MotoRentalService.Domain.Services
{
    public class DeliveryUserService : IDeliveryUserService
    {
        private readonly IDeliveryUserRepository _deliveryUserRepository;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;

        public DeliveryUserService(IDeliveryUserRepository deliveryUserRepository, IStorageService storageService, IConfiguration configuration)
        {
            _deliveryUserRepository = deliveryUserRepository;
            _storageService = storageService;
            _configuration = configuration;
        }

        public Task<DeliveryUser?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            return _deliveryUserRepository.GetByIdAsync(id, cancellationToken);
        }

        public Task<DeliveryUser?> GetUserByUserIdAsync(int id, CancellationToken cancellationToken)
        {
            return _deliveryUserRepository.GetByUserIdAsync(id, cancellationToken);
        }

        public async Task<DeliveryUser> RegisterUserAsync(DeliveryUser user, IFormFile? image, CancellationToken cancellationToken)
        {
            await ValidateDeliveryPersonAsync(user, cancellationToken);
            await CheckForExistingCNPJAndLicenseNumberAsync(user, cancellationToken);

            if (image != null && image.Length > 0)
            {
                if (!_storageService.IsValidFileType(image.ContentType))
                {
                    throw new Exception("Invalid image format.");
                }

                var savedPerson = await _deliveryUserRepository.AddAsync(user, cancellationToken);

                var imageData = await _storageService.GetFileBytes(image);
                var uniqueFileName = _storageService.GenerateUniqueFileName(image.FileName, image.ContentType, user.Id);
                var storedFilePath = await _storageService.SaveFileAsync(savedPerson.Id, uniqueFileName, imageData, image.ContentType, cancellationToken);
                user.LicenseImageURL = storedFilePath;
                user.LicenseImageFullName = uniqueFileName;
                user.Status = UserStatus.Approved;
                await _deliveryUserRepository.UpdateAsync(user, cancellationToken);
                return user;
            }
            else
            {
                user.Status = UserStatus.Pending;
            }
            user.CNPJ = Regex.Replace(user.CNPJ, @"[^\d]", "");
            return await _deliveryUserRepository.AddAsync(user, cancellationToken);
        }

        public async Task<DeliveryUser?> UpdateUserLicenseImageAsync(DeliveryUser user, IFormFile image, CancellationToken cancellationToken)
        {
            await ValidateDeliveryPersonAsync(user, cancellationToken);

            if (image == null || image.Length == 0)
                throw new Exception("No image data provided.");

            if (!_storageService.IsValidFileType(image.ContentType))
                throw new Exception("Invalid image format.");

            var imageData = await _storageService.GetFileBytes(image);
            string? uniqueFileName;
            if (string.IsNullOrEmpty(user.LicenseImageFullName))
                uniqueFileName = _storageService.GenerateUniqueFileName(image.FileName, image.ContentType, user.Id);
            else
                uniqueFileName = user.LicenseImageFullName;
            var storedFilePath = await _storageService.SaveFileAsync(user.Id, uniqueFileName, imageData, image.ContentType, cancellationToken);

            user.LicenseImageURL = storedFilePath;
            user.LicenseImageFullName = uniqueFileName;
            await _deliveryUserRepository.UpdateAsync(user, cancellationToken);
            return user;
        }
        public async Task DeleteUserAsync(int id, CancellationToken cancellationToken)
        {
            await _deliveryUserRepository.DeleteByIdAsync(id, cancellationToken);

            var rootImageDirectory = Environment.GetEnvironmentVariable("IMAGE_ROOT_DIRECTORY") ?? _configuration.GetSection("ImageStoragePath").Value;
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), rootImageDirectory, id.ToString());
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }

        public async Task ValidateDeliveryPersonAsync(DeliveryUser user, CancellationToken cancellationToken)
        {
            var validationResult = await new UserValidator().ValidateAsync(user, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errorMessage);
            }
        }

        public async Task CheckForExistingCNPJAndLicenseNumberAsync(DeliveryUser user, CancellationToken cancellationToken)
        {
            var digitsOnly = Regex.Replace(user.CNPJ, @"[^\d]", "");
            var existingCnpj = await _deliveryUserRepository.ExistsByCNPJAsync(digitsOnly, cancellationToken);
            if (existingCnpj)
            {
                throw new Exception("CNPJ already exists.");
            }

            var existingLicenseNumber = await _deliveryUserRepository.ExistsByLicenseNumberAsync(user.LicenseNumber, cancellationToken);
            if (existingLicenseNumber)
            {
                throw new Exception("Driver's License Number already exists.");
            }
        }

        
        
    }
}
