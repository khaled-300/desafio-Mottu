using AutoMapper;
using MediatR;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.MediatR.QueryHandlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, DeliveryUserDto?>
    {
        private readonly IDeliveryUserRepository _deliveryUserRepository;
        private readonly IMapper _mapper;
        public GetUserByIdQueryHandler(IMapper mapper, IDeliveryUserRepository deliveryUserRepository)
        {
            _deliveryUserRepository = deliveryUserRepository;
            _mapper = mapper;
        }

        public async Task<DeliveryUserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _deliveryUserRepository.GetByIdAsync(request.Id, cancellationToken);
            return _mapper.Map<DeliveryUserDto>(user);
        }
    }
}
