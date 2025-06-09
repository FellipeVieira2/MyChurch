using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Webhook.Commands
{
    public class UpdateSubscriptionStatusCommand : IRequest<bool>
    {
        public string SubscriptionId { get; set; }
        public AsaasSubscriptionStatus Status { get; set; }
    }

    public class UpdateSubscriptionStatusCommandHandler : IRequestHandler<UpdateSubscriptionStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateSubscriptionStatusCommandHandler> _logger;

        public UpdateSubscriptionStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateSubscriptionStatusCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateSubscriptionStatusCommand request, CancellationToken cancellationToken)
        {
            // Busca a assinatura pelo campo ExternalReference (que armazena o ID da assinatura no Asaas)
            var subscription = await _unitOfWork.Subscriptions.Query()
                .FirstOrDefaultAsync(s => s.ExternalReference == request.SubscriptionId, cancellationToken);

            if (subscription == null)
            {
                _logger.LogWarning("Assinatura não encontrada para o ID Asaas: {SubscriptionId}", request.SubscriptionId);
                return false;
            }

            // Atualiza o status da assinatura (você pode criar uma propriedade Status na entidade Subscription se ainda não existir)
            // Exemplo: subscription.Status = request.Status;
            // Caso não exista, pode salvar em um campo string ou outro campo apropriado

            // Exemplo usando um campo string:
            // subscription.Status = request.Status.ToString();

            // Atualiza a data de modificação
            subscription.Updated = DateTime.UtcNow;

            _unitOfWork.Subscriptions.Update(subscription);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Status da assinatura {SubscriptionId} atualizado para {Status}", request.SubscriptionId, request.Status);

            return true;
        }
    }
}