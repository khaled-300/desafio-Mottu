using MediatR;
using MotoRentalService.Application.MediatR.Response.Moto;

namespace MotoRentalService.Application.MediatR.Commands.Motorcycle
{
    public class CreateMotoCommand : IRequest<MotorcycleCommandResult>
    {
        public int Year { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
    }
}
