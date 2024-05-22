using AutoMapper;
using MediatR;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.MediatR.QueryHandlers
{
    public class GetMotoPageQueryHandler : IRequestHandler<GetMotoPageQuery, IEnumerable<MotorcycleDto>>,
        IRequestHandler<GetMotoByIdQuery, MotorcycleDto?>,
        IRequestHandler<GetMotoByLicensePlateQuery, MotorcycleDto?>
    {
        private readonly IMotorcycleRepository _repository;
        private readonly IMapper _mapper;

        public GetMotoPageQueryHandler(IMapper mapper, IMotorcycleRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<IEnumerable<MotorcycleDto>> Handle(GetMotoPageQuery request, CancellationToken cancellationToken)
        {
            // Ensure positive page number and page size
            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request), "Page number and page size must be positive.");
            }

            var motorcycles = await _repository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);
            return _mapper.Map<IEnumerable<MotorcycleDto>>(motorcycles);
        }

        public async Task<MotorcycleDto?> Handle(GetMotoByIdQuery request, CancellationToken cancellationToken)
        {
            var motorcycle = await _repository.GetByIdAsync(request.MotorcycleId, cancellationToken);
            return _mapper.Map<MotorcycleDto?>(motorcycle);
        }

        public async Task<MotorcycleDto?> Handle(GetMotoByLicensePlateQuery request, CancellationToken cancellationToken)
        {
            var motorcycle = await _repository.GetByLicensePlateAsync(request.LicensePlate, cancellationToken);
            return _mapper.Map<MotorcycleDto?>(motorcycle);
        }
    }
}