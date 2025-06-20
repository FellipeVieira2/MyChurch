using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Plan.Queries.GetAllPlans
{
    public class GetAllPlansQuery : IRequest<List<PlanDto>>
    {
        // No properties needed as we're just retrieving all plans
    }

    public class GetAllPlansQueryHandler : IRequestHandler<GetAllPlansQuery, List<PlanDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllPlansQueryHandler> _logger;

        public GetAllPlansQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllPlansQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<PlanDto>> Handle(GetAllPlansQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all available plans");

            var plans = await _unitOfWork.Plans.Query()
                .OrderBy(p => p.Price)
                .ToListAsync(cancellationToken);

            return plans.Select(PlanDto.New).ToList();
        }
    }
}