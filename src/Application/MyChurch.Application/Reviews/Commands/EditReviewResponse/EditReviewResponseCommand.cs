using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Reviews.Commands.EditReviewResponse
{
    /// <summary>
    /// Comando para editar resposta da igreja a uma review
    /// </summary>
    public class EditReviewResponseCommand : JwtMemberDto, IRequest<EditReviewResponseResult>
    {
        public int ResponseId { get; set; }
        public string NewResponse { get; set; } = string.Empty;
    }

    public class EditReviewResponseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }
    }

    public class EditReviewResponseCommandHandler : IRequestHandler<EditReviewResponseCommand, EditReviewResponseResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EditReviewResponseCommandHandler> _logger;

        public EditReviewResponseCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<EditReviewResponseCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<EditReviewResponseResult> Handle(EditReviewResponseCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.NewResponse) || request.NewResponse.Length < 10)
            {
                return new EditReviewResponseResult 
                { 
                    Success = false, 
                    Message = "A resposta deve ter pelo menos 10 caracteres." 
                };
            }

            // Busca o membro
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null || member.Role != Domain.Enum.UserRole.Admin)
            {
                return new EditReviewResponseResult 
                { 
                    Success = false, 
                    Message = "Apenas administradores podem editar respostas." 
                };
            }

            // Busca a resposta
            var response = await _unitOfWork.ReviewResponses.Query()
                .Include(rr => rr.Review)
                .FirstOrDefaultAsync(rr => rr.Id == request.ResponseId, cancellationToken);

            if (response == null)
            {
                return new EditReviewResponseResult 
                { 
                    Success = false, 
                    Message = "Resposta não encontrada." 
                };
            }

            // Valida que a resposta pertence à igreja do membro
            if (response.Review.EntityId != member.ChurchId)
            {
                return new EditReviewResponseResult 
                { 
                    Success = false, 
                    Message = "Você só pode editar respostas da sua igreja." 
                };
            }

            // Edita a resposta
            response.Edit(request.NewResponse);
            
            _unitOfWork.ReviewResponses.Update(response);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Review response {ResponseId} edited by member {MemberId}", 
                request.ResponseId, request.UserId);

            return new EditReviewResponseResult
            {
                Success = true,
                Message = "Resposta atualizada com sucesso!",
                IsEdited = response.IsEdited,
                EditedAt = response.EditedAt
            };
        }
    }
}
