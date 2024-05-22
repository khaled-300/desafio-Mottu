using MediatR;
using MotoRentalService.Application.MediatR.Response.Rental;

namespace MotoRentalService.Application.MediatR.Commands.Rental
{
    public class DeleteRentalCommand : IRequest<RentalCommandResult>
    {
        public int Id { get; set; }
    }
}
