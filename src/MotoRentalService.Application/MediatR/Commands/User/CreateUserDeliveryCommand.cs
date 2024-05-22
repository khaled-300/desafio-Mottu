using MediatR;
using Microsoft.AspNetCore.Http;
using MotoRentalService.Application.MediatR.Response.DeliveryUser;
using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Application.MediatR.Commands.User
{
    public class CreateUserDeliveryCommand : IRequest<DeliveryUserCommandResult>
    {
        public int AuthUserId { get; set; }
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string LicenseNumber { get; set; }
        public LicenseType LicenseType { get; set; } = LicenseType.None;
        public IFormFile? LicenseImage { get; set; }
    }
}

