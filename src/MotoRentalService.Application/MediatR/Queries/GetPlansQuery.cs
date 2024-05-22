using MediatR;
using MotoRentalService.Application.Dtos;

namespace MotoRentalService.Application.MediatR.Queries
{
    public class GetPlansQuery : IRequest<IEnumerable<PlanDto>>
    {
    }
}
