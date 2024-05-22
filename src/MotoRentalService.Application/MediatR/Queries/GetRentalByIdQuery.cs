using MediatR;
using MotoRentalService.Application.Dtos;

namespace MotoRentalService.Application.MediatR.Queries
{
    public class GetRentalByIdQuery : IRequest<RentalDto?>
    {
        public int RentalId { get; set; }
    }
}
