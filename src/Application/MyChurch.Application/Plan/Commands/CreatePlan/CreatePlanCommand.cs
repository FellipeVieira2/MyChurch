using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Plan.Commands.CreatePlan
{
    public class CreatePlanCommand : IRequest<int>
    {
        /// <summary>Nome do plano</summary>
        /// <example>Premium</example>
        public string Name { get; set; }
        
        /// <summary>Preço do plano</summary>
        /// <example>99.90</example>
        public decimal Price { get; set; }
        
        /// <summary>Número máximo de membros permitidos</summary>
        /// <example>500</example>
        public int MaxMembers { get; set; }
        
        /// <summary>Número máximo de eventos permitidos</summary>
        /// <example>50</example>
        public int MaxEvents { get; set; }
        
        /// <summary>Espaço de armazenamento em GB</summary>
        /// <example>10</example>
        public int MaxStorageGB { get; set; }
        
        /// <summary>Número de filiais permitidas</summary>
        /// <example>3</example>
        public int Branches { get; set; }
    }

    public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreatePlanCommandHandler> _logger;

        public CreatePlanCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreatePlanCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new plan {PlanName}", request.Name);

            var plan = new Domain.Entities.Plan
            {
                Name = request.Name,
                Price = request.Price,
                MaxMembers = request.MaxMembers,
                MaxEvents = request.MaxEvents,
                MaxStorageGB = request.MaxStorageGB,
                Branches = request.Branches,
                Created = DateTime.UtcNow
            };

            _unitOfWork.Plans.Create(plan);
            await _unitOfWork.CommitAsync();

            return plan.Id;
        }
    }
}