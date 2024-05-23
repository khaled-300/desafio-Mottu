using MediatR;
using MotoRentalService.Application.MediatR.Response.Moto;
using MotoRentalService.Domain.Aggregates;

namespace MotoRentalService.Application.MediatR.Commands.Motorcycle
{
    public class UpdateMotoCommand : IRequest<MotorcycleCommandResult>
    {
        [SwaggerExclude]
        public int Id { get; set; }
        public string LicensePlate { get; set; }
    }
}
