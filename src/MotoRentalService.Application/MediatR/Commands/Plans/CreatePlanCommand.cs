using MediatR;
using MotoRentalService.Application.MediatR.Response.Plans;

namespace MotoRentalService.Application.MediatR.Commands.Plans
{
    public class CreatePlanCommand : IRequest<PlansCommandResult>
    {
        public string Name { get; set; }
        public int DurationInDays { get; set; }
        public decimal DailyRate { get; set; }
        public bool IsActive { get; set; }
    }
}
