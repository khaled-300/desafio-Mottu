using MediatR;
using MotoRentalService.Application.MediatR.Response.DeliveryUser;

namespace MotoRentalService.Application.MediatR.Commands.User
{
    public class DeleteDeliveryUserCommand : IRequest<DeliveryUserCommandResult>
    {
        public int Id { get; set; }
    }
}
