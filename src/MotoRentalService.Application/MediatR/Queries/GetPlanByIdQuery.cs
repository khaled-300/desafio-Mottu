using MediatR;
using MotoRentalService.Application.Dtos;

namespace MotoRentalService.Application.MediatR.Queries
{
    public class GetPlanByIdQuery : IRequest<PlanDto>
    {
        public int Id { get; set; }
    }
}
