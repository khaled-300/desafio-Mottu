using MediatR;
using MotoRentalService.Application.MediatR.Response.Rental;

namespace MotoRentalService.Application.MediatR.Commands.Rental
{
    public class SimulateTotalValueCommand : IRequest<SimulateTotalValueResult>
    {
        public int Id { get; set; }

        public DateTime ReturnDate { get; set; }
    }
}
