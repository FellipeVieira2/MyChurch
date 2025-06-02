//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Mychurch.Common.WebClients.Asaas;
//using MyChurch.Domain.Contracts;
//using MyChurch.Domain.Enum;

//namespace MyChurch.Application.Webhook.Commands
//{
//    public class ConfirmPaymentCommand : IRequest<bool>
//    {
//        public string PaymentId { get; set; }
//        public AsaasPaymentStatus Status { get; set; }
//    }

//    public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, bool>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly ILogger<ConfirmPaymentCommandHandler> _logger;
//        private readonly IAsaasWebClient _asaasWebClient;

//        public ConfirmPaymentCommandHandler(
//            IUnitOfWork unitOfWork,
//            ILogger<ConfirmPaymentCommandHandler> logger,
//            IAsaasWebClient asaasWebClient)
//        {
//            _unitOfWork = unitOfWork;
//            _logger = logger;
//            _asaasWebClient = asaasWebClient;
//        }

//        public async Task<bool> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                // 1. Buscar o pagamento no Asaas para confirmar os detalhes
//                var asaasPayment = await _asaasWebClient.ConsultarCobrancaAsync(request.PaymentId);

//                // 2. Buscar o pagamento correspondente no banco de dados
//                var payment = await _unitOfWork.Payments.Query()
//                    .Include(p => p.Subscription)
//                    .ThenInclude(s => s.Church)
//                    .FirstOrDefault(p => p.TransactionId == request.PaymentId);

//                if (payment == null)
//                {
//                    _logger.LogWarning($"Pagamento não encontrado para o ID Asaas: {request.PaymentId}");
//                    return false;
//                }

//                // 3. Atualizar o status do pagamento
//                payment.Status = request.Status;
//                payment.Updated = DateTime.UtcNow;

//                if (request.Status == AsaasPaymentStatus.CONFIRMED)
//                {
//                    // 4. Se confirmado, atualizar dados da assinatura
//                    var subscription = payment.Subscription;
//                    subscription.Status = AsaasSubscriptionStatus.ACTIVE;

//                    // 5. Estender a data de validade da assinatura
//                    if (subscription.EndDate < DateTime.UtcNow)
//                    {
//                        subscription.StartDate = DateTime.UtcNow;
//                    }
//                    subscription.EndDate = DateTime.UtcNow.AddMonths(1);
//                    subscription.Updated = DateTime.UtcNow;

//                    _unitOfWork.Subscriptions.Update(subscription);
//                }

//                // 6. Salvar as alterações
//                _unitOfWork.Payments.Update(payment);
//                await _unitOfWork.CommitAsync();

//                _logger.LogInformation($"Pagamento {request.PaymentId} confirmado com sucesso. Status: {request.Status}");

//                return true;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Erro ao confirmar pagamento {request.PaymentId}");
//                throw;
//            }
//        }
//    }
//}