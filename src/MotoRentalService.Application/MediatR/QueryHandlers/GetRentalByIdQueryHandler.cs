using AutoMapper;
using MediatR;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.MediatR.QueryHandlers
{
    public class GetRentalByIdQueryHandler : IRequestHandler<GetRentalByIdQuery, RentalDto?>
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IMapper _mapper;

        public GetRentalByIdQueryHandler(IMapper mapper, IRentalRepository rentalRepository)
        {
            _rentalRepository = rentalRepository;
            _mapper = mapper;
        }

        public async Task<RentalDto?> Handle(GetRentalByIdQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            var rental = await _rentalRepository.GetByIdAsync(request.RentalId, cancellationToken);
            return _mapper.Map<RentalDto>(rental);
        }
    }
}
