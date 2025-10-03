using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Reviews.Commands.RespondToReview
{
    /// <summary>
    /// Comando para igreja responder a uma review
    /// </summary>
    public class RespondToReviewCommand : JwtMemberDto, IRequest<RespondToReviewResult>
    {
        public int ReviewId { get; set; }
        public string Response { get; set; } = string.Empty;
    }

    public class RespondToReviewResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? ResponseId { get; set; }
        public DateTime? RespondedAt { get; set; }
    }

    public class RespondToReviewCommandHandler : IRequestHandler<RespondToReviewCommand, RespondToReviewResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RespondToReviewCommandHandler> _logger;

        public RespondToReviewCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<RespondToReviewCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RespondToReviewResult> Handle(RespondToReviewCommand request, CancellationToken cancellationToken)
        {
            // Valida conteúdo da resposta
            if (string.IsNullOrWhiteSpace(request.Response) || request.Response.Length < 10)
            {
                return new RespondToReviewResult 
                { 
                    Success = false, 
                    Message = "A resposta deve ter pelo menos 10 caracteres." 
                };
            }

            if (request.Response.Length > 1000)
            {
                return new RespondToReviewResult 
                { 
                    Success = false, 
                    Message = "A resposta não pode ter mais de 1000 caracteres." 
                };
            }

            // Busca o membro respondente (deve ser Admin ou Leader)
            var responder = await _unitOfWork.Members.Query()
                .Include(m => m.Church)
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (responder == null)
            {
                return new RespondToReviewResult 
                { 
                    Success = false, 
                    Message = "Usuário não encontrado." 
                };
            }

            // Verifica permissão (apenas Admin pode responder)
            if (responder.Role != Domain.Enum.UserRole.Admin)
            {
                _logger.LogWarning("Member {MemberId} attempted to respond to review without permission", request.UserId);
                return new RespondToReviewResult 
                { 
                    Success = false, 
                    Message = "Apenas administradores podem responder a avaliações." 
                };
            }

            // Busca a review
            var review = await _unitOfWork.Reviews.Query()
                .Include(r => r.OfficialResponse)
                .Include(r => r.Reviewer)
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);

            if (review == null)
            {
                return new RespondToReviewResult 
                { 
                    Success = false, 
                    Message = "Avaliação não encontrada." 
                };
            }

            // Valida que a review pertence à igreja do respondente
            if (review.EntityId != responder.ChurchId || review.EntityType != "Church")
            {
                _logger.LogWarning("Member {MemberId} from church {ChurchId} tried to respond to review {ReviewId} of church {ReviewChurchId}", 
                    request.UserId, responder.ChurchId, request.ReviewId, review.EntityId);
                return new RespondToReviewResult 
                { 
                    Success = false, 
                    Message = "Você só pode responder a avaliações da sua igreja." 
                };
            }

            // Verifica se já existe resposta
            if (review.OfficialResponse != null)
            {
                return new RespondToReviewResult 
                { 
                    Success = false, 
                    Message = "Esta avaliação já possui uma resposta. Use o endpoint de edição para atualizar." 
                };
            }

            // Cria a resposta
            var response = new ReviewResponse
            {
                ReviewId = request.ReviewId,
                ResponderId = request.UserId,
                Response = request.Response,
                RespondedAt = DateTime.UtcNow
            };

            _unitOfWork.ReviewResponses.Create(response);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Church {ChurchId} responded to review {ReviewId} via member {MemberId}", 
                responder.ChurchId, request.ReviewId, request.UserId);

            // TODO: Enviar notificação ao avaliador
            // await _notificationService.NotifyReviewerOfResponseAsync(review.ReviewerId, review.Id);

            return new RespondToReviewResult
            {
                Success = true,
                Message = "Resposta enviada com sucesso!",
                ResponseId = response.Id,
                RespondedAt = response.RespondedAt
            };
        }
    }
}
