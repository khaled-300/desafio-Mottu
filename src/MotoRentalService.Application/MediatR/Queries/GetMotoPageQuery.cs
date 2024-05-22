using MediatR;
using MotoRentalService.Application.Dtos;

namespace MotoRentalService.Application.MediatR.Queries
{
    public class GetMotoPageQuery : IRequest<IEnumerable<MotorcycleDto>>
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public GetMotoPageQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber > 0 ? pageNumber : 1;
            PageSize = pageSize > 0 ? pageSize : 10;
        }
    }
}
