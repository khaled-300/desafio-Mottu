using MediatR;
using MotoRentalService.Application.Dtos;

namespace MotoRentalService.Application.MediatR.Queries
{
    public class GetUserByIdQuery : IRequest<DeliveryUserDto?>
    {
        public int Id { get; set; }
    }
}
