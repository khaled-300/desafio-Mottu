using MediatR;
using MotoRentalService.Application.MediatR.Response.Plans;
using MotoRentalService.Domain.Aggregates;

namespace MotoRentalService.Application.MediatR.Commands.Plans
{
    public class UpdatePlanCommand : IRequest<PlansCommandResult>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DurationDays { get; set; }
        public decimal DailyRate { get; set; }
        public bool IsActive { get; set; }
    }
}
