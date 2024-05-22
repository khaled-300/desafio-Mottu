using AutoMapper;
using MediatR;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.MediatR.QueryHandlers
{
    public class GetPlansQueryHandler : IRequestHandler<GetPlansQuery, IEnumerable<PlanDto>>
    {
        private readonly IPlansRepository _plansRepository;
        private readonly IMapper _mapper;

        public GetPlansQueryHandler(IMapper mapper, IPlansRepository plansRepository)
        {
            _plansRepository = plansRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PlanDto>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
        {
            var plans = await _plansRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<PlanDto>>(plans);
        }
    }
}
