using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.Plan.Commands.UpdatePlan
{
    public class UpdatePlanCommand : IRequest<PlanDto>
    {
        [JsonIgnore]
        public int Id { get; set; }
        
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

    public class UpdatePlanCommandHandler : IRequestHandler<UpdatePlanCommand, PlanDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdatePlanCommandHandler> _logger;

        public UpdatePlanCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdatePlanCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PlanDto> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating plan with ID {PlanId}", request.Id);

            var plan = await _unitOfWork.Plans.Query()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (plan == null)
                ValidationException.ThrowException("Plan", "Plano não encontrado.");

            plan.Name = request.Name;
            plan.Price = request.Price;
            plan.MaxMembers = request.MaxMembers;
            plan.MaxEvents = request.MaxEvents;
            plan.MaxStorageGB = request.MaxStorageGB;
            plan.Branches = request.Branches;
            plan.Updated = DateTime.UtcNow;

            _unitOfWork.Plans.Update(plan);
            await _unitOfWork.CommitAsync();

            return PlanDto.New(plan);
        }
    }
}