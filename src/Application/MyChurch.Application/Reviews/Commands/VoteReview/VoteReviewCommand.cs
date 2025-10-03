using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Reviews.Commands.VoteReview
{
    /// <summary>
    /// Adiciona ou atualiza um voto em uma review (útil/não útil)
    /// </summary>
    public class VoteReviewCommand : JwtMemberDto, IRequest<VoteReviewResult>
    {
        public int ReviewId { get; set; }
        public bool IsHelpful { get; set; } // true = útil, false = não útil
    }

    public class VoteReviewResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int HelpfulVotes { get; set; }
        public int NotHelpfulVotes { get; set; }
        public int HelpfulnessScore { get; set; }
    }

    public class VoteReviewCommandHandler : IRequestHandler<VoteReviewCommand, VoteReviewResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VoteReviewCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<VoteReviewResult> Handle(VoteReviewCommand request, CancellationToken cancellationToken)
        {
            // Verifica se a review existe
            var review = await _unitOfWork.Reviews.Query()
                .Include(r => r.Votes)
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);

            if (review == null)
                ValidationException.ThrowException("Review", "Avaliação não encontrada.");

            // Verifica se o membro já votou nesta review
            var existingVote = await _unitOfWork.ReviewVotes
                .GetVoteByMemberAndReviewAsync(request.UserId, request.ReviewId, cancellationToken);

            if (existingVote != null)
            {
                // Se já votou, atualiza o voto (toggle ou muda de útil para não útil)
                if (existingVote.IsHelpful == request.IsHelpful)
                {
                    // Se está votando no mesmo tipo novamente, remove o voto
                    _unitOfWork.ReviewVotes.Delete(existingVote);
                    await _unitOfWork.CommitAsync();

                    return new VoteReviewResult
                    {
                        Success = true,
                        Message = "Voto removido com sucesso",
                        HelpfulVotes = await _unitOfWork.ReviewVotes.CountHelpfulVotesAsync(request.ReviewId, cancellationToken),
                        NotHelpfulVotes = await _unitOfWork.ReviewVotes.CountNotHelpfulVotesAsync(request.ReviewId, cancellationToken),
                        HelpfulnessScore = review.GetHelpfulnessScore()
                    };
                }
                else
                {
                    // Muda de útil para não útil ou vice-versa
                    existingVote.ToggleVote();
                    _unitOfWork.ReviewVotes.Update(existingVote);
                    await _unitOfWork.CommitAsync();

                    return new VoteReviewResult
                    {
                        Success = true,
                        Message = "Voto atualizado com sucesso",
                        HelpfulVotes = await _unitOfWork.ReviewVotes.CountHelpfulVotesAsync(request.ReviewId, cancellationToken),
                        NotHelpfulVotes = await _unitOfWork.ReviewVotes.CountNotHelpfulVotesAsync(request.ReviewId, cancellationToken),
                        HelpfulnessScore = review.GetHelpfulnessScore()
                    };
                }
            }

            // Cria novo voto
            var newVote = new ReviewVote
            {
                ReviewId = request.ReviewId,
                MemberId = request.UserId,
                IsHelpful = request.IsHelpful,
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.ReviewVotes.Create(newVote);
            await _unitOfWork.CommitAsync();

            return new VoteReviewResult
            {
                Success = true,
                Message = request.IsHelpful ? "Marcado como útil" : "Marcado como não útil",
                HelpfulVotes = await _unitOfWork.ReviewVotes.CountHelpfulVotesAsync(request.ReviewId, cancellationToken),
                NotHelpfulVotes = await _unitOfWork.ReviewVotes.CountNotHelpfulVotesAsync(request.ReviewId, cancellationToken),
                HelpfulnessScore = review.GetHelpfulnessScore()
            };
        }
    }
}
