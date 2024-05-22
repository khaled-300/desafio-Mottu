using MediatR;
using MotoRentalService.Application.MediatR.Response.Rental;

namespace MotoRentalService.Application.MediatR.Commands.Rental
{
    public class CreateRentalCommand : IRequest<RentalCommandResult>
    {
        public int MotorcycleId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RentalPlanId { get; set; }
    }
}
