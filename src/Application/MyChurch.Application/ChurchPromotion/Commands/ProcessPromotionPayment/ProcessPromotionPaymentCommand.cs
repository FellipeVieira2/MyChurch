using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.ChurchPromotion.Events;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.ChurchPromotion.Commands.ProcessPromotionPayment
{
    /// <summary>
    /// Processa confirmação de pagamento de promoção via webhook do Asaas
    /// </summary>
    public class ProcessPromotionPaymentCommand : IRequest<bool>
    {
        public string TransactionId { get; set; }
        public string PaymentStatus { get; set; } // "RECEIVED", "CONFIRMED", etc.
    }

    public class ProcessPromotionPaymentCommandHandler : IRequestHandler<ProcessPromotionPaymentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessPromotionPaymentCommandHandler> _logger;
        private readonly IMediator _mediator;

        public ProcessPromotionPaymentCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<ProcessPromotionPaymentCommandHandler> logger,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<bool> Handle(ProcessPromotionPaymentCommand request, CancellationToken cancellationToken)
        {
            // 1. Buscar o pagamento pelo transactionId
            var payment = await _unitOfWork.Payments.Query()
                .FirstOrDefaultAsync(p => p.TransactionId == request.TransactionId, cancellationToken);

            if (payment == null)
            {
                _logger.LogWarning("Pagamento não encontrado: {TransactionId}", request.TransactionId);
                return false;
            }

            // 2. Atualizar status do pagamento
            var oldStatus = payment.PaymentStatus;
            payment.PaymentStatus = MapAsaasStatusToPaymentStatus(request.PaymentStatus).ToString();

            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Pagamento {PaymentId} atualizado: {OldStatus} -> {NewStatus}",
                payment.Id, oldStatus, payment.PaymentStatus);

            // 3. Se o pagamento foi CONFIRMADO, ativar a promoção
            if (request.PaymentStatus == "RECEIVED" || request.PaymentStatus == "CONFIRMED" || request.PaymentStatus == "RECEIVED_IN_CASH")
            {
                // 3.1. Buscar promoção de igreja
                var churchPromotion = await _unitOfWork.ChurchPromotions.Query()
                    .Include(p => p.Church)
                    .FirstOrDefaultAsync(p => p.PaymentId == payment.Id, cancellationToken);

                if (churchPromotion != null)
                {
                    // ? ATIVAR A PROMOÇÃO
                    churchPromotion.Status = PromotionStatus.Active;
                    _unitOfWork.ChurchPromotions.Update(churchPromotion);
                    await _unitOfWork.CommitAsync();

                    _logger.LogInformation(
                        "? Promoção de Igreja {PromotionId} ATIVADA após confirmação de pagamento",
                        churchPromotion.Id);

                    // ?? DISPARAR EVENTO PARA ENVIAR EMAIL
                    var adminMember = await _unitOfWork.Members.Query()
                        .FirstOrDefaultAsync(m => m.ChurchId == churchPromotion.ChurchId && m.Role == UserRole.Admin, cancellationToken);

                    if (adminMember != null && !string.IsNullOrEmpty(adminMember.Email))
                    {
                        var duration = (churchPromotion.EndDate - churchPromotion.StartDate).Days;
                        var estimatedViews = CalculateEstimatedViews(churchPromotion.Type, duration);

                        await _mediator.Publish(new PromotionActivatedEvent
                        {
                            PromotionId = churchPromotion.Id,
                            ChurchId = churchPromotion.ChurchId,
                            ChurchName = churchPromotion.Church.Name,
                            AdminEmail = adminMember.Email,
                            AdminName = adminMember.Name,
                            PromotionType = churchPromotion.Type.ToString(),
                            StartDate = churchPromotion.StartDate,
                            EndDate = churchPromotion.EndDate,
                            AmountPaid = churchPromotion.AmountPaid,
                            EstimatedViews = estimatedViews
                        }, cancellationToken);
                    }

                    return true;
                }

                // 3.2. Buscar promoção de evento
                var eventPromotion = await _unitOfWork.EventPromotions.Query()
                    .Include(p => p.Church)
                    .FirstOrDefaultAsync(p => p.PaymentId == payment.Id, cancellationToken);

                if (eventPromotion != null)
                {
                    // ? ATIVAR A PROMOÇÃO
                    eventPromotion.Status = PromotionStatus.Active;
                    _unitOfWork.EventPromotions.Update(eventPromotion);
                    await _unitOfWork.CommitAsync();

                    _logger.LogInformation(
                        "? Promoção de Evento {PromotionId} ATIVADA após confirmação de pagamento",
                        eventPromotion.Id);

                    // TODO: Enviar email para promoção de evento também
                    return true;
                }
            }

            // 4. Se pagamento foi CANCELADO ou FALHOU, rejeitar a promoção
            if (request.PaymentStatus == "REFUNDED" || request.PaymentStatus == "CANCELLED")
            {
                var churchPromotion = await _unitOfWork.ChurchPromotions.Query()
                    .FirstOrDefaultAsync(p => p.PaymentId == payment.Id, cancellationToken);

                if (churchPromotion != null)
                {
                    churchPromotion.Status = PromotionStatus.Rejected;
                    _unitOfWork.ChurchPromotions.Update(churchPromotion);
                    await _unitOfWork.CommitAsync();

                    _logger.LogWarning(
                        "? Promoção de Igreja {PromotionId} REJEITADA - pagamento cancelado",
                        churchPromotion.Id);
                }

                var eventPromotion = await _unitOfWork.EventPromotions.Query()
                    .FirstOrDefaultAsync(p => p.PaymentId == payment.Id, cancellationToken);

                if (eventPromotion != null)
                {
                    eventPromotion.Status = PromotionStatus.Rejected;
                    _unitOfWork.EventPromotions.Update(eventPromotion);
                    await _unitOfWork.CommitAsync();

                    _logger.LogWarning(
                        "? Promoção de Evento {PromotionId} REJEITADA - pagamento cancelado",
                        eventPromotion.Id);
                }
            }

            return true;
        }

        private PaymentStatus MapAsaasStatusToPaymentStatus(string asaasStatus)
        {
            return asaasStatus switch
            {
                "PENDING" => PaymentStatus.Pending,
                "RECEIVED" => PaymentStatus.Completed,
                "CONFIRMED" => PaymentStatus.Completed,
                "RECEIVED_IN_CASH" => PaymentStatus.Completed,
                "OVERDUE" => PaymentStatus.Pending,
                "REFUNDED" => PaymentStatus.Cancelled,
                "CANCELLED" => PaymentStatus.Cancelled,
                _ => PaymentStatus.Pending
            };
        }

        private int CalculateEstimatedViews(PromotionType type, int durationInDays)
        {
            var viewsPerDay = type switch
            {
                PromotionType.FeaturedInSearch => 500,
                PromotionType.CarouselHome => 800,
                PromotionType.TopSearch => 600,
                PromotionType.SidebarBanner => 200,
                PromotionType.RegionalPromotion => 300,
                _ => 250
            };

            return viewsPerDay * durationInDays;
        }
    }
}
