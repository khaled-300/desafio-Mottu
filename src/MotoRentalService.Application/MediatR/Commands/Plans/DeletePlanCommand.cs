using MediatR;
using MotoRentalService.Application.MediatR.Response.Plans;

namespace MotoRentalService.Application.MediatR.Commands.Plans
{
    public class DeletePlanCommand : IRequest<PlansCommandResult>
    {
        public int Id { get; set; }
    }
}
