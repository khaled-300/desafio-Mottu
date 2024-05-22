using MediatR;
using MotoRentalService.Application.MediatR.Response.Moto;

namespace MotoRentalService.Application.MediatR.Commands.Motorcycle
{
    public class UpdateMotoCommand : IRequest<MotorcycleCommandResult>
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; }
    }
}
