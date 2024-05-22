using MediatR;
using MotoRentalService.Application.MediatR.Response.Moto;

namespace MotoRentalService.Application.MediatR.Commands.Motorcycle
{
    public class DeleteMotoCommand : IRequest<MotorcycleCommandResult>
    {
        public int Id { get; set; }
    }
}
