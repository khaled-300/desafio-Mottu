using AutoMapper;
using MediatR;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.MediatR.QueryHandlers
{
    public class GetPlanByIdQueryHandler : IRequestHandler<GetPlanByIdQuery, PlanDto?>
    {
        private readonly IPlansRepository _plansRepository;
        private readonly IMapper _mapper;

        public GetPlanByIdQueryHandler(IMapper mapper, IPlansRepository plansRepository)
        {
            _plansRepository = plansRepository;
            _mapper = mapper;
        }

        public async Task<PlanDto?> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var plan = await _plansRepository.GetPlanByIdAsync(request.Id, cancellationToken);
            return _mapper.Map<PlanDto?>(plan);
        }
    }
}
