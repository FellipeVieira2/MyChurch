using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Plan.Queries.GetPlanById
{
    public class GetPlanByIdQuery : IRequest<PlanDto>
    {
        public int Id { get; set; }
    }

    public class GetPlanByIdQueryHandler : IRequestHandler<GetPlanByIdQuery, PlanDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetPlanByIdQueryHandler> _logger;

        public GetPlanByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetPlanByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PlanDto> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting plan with ID {PlanId}", request.Id);

            var plan = await _unitOfWork.Plans.Query()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (plan == null)
                ValidationException.ThrowException("Plan", "Plano não encontrado.");

            return PlanDto.New(plan);
        }
    }
}