using MediatR;
using MotoRentalService.Application.Dtos;

namespace MotoRentalService.Application.MediatR.Queries
{
    public class GetMotoByIdQuery : IRequest<MotorcycleDto>
    {
        public int MotorcycleId { get; }

        public GetMotoByIdQuery(int motorcycleId)
        {
            MotorcycleId = motorcycleId;
        }
    }
}
