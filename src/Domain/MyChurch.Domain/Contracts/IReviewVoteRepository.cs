using MyChurch.Domain.Entities;

namespace MyChurch.Domain.Contracts
{
    public interface IReviewVoteRepository : IGenericRepository<ReviewVote>
    {
        /// <summary>
        /// Busca um voto específico de um membro em uma review
        /// </summary>
        Task<ReviewVote?> GetVoteByMemberAndReviewAsync(int memberId, int reviewId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Conta quantos votos úteis uma review tem
        /// </summary>
        Task<int> CountHelpfulVotesAsync(int reviewId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Conta quantos votos não úteis uma review tem
        /// </summary>
        Task<int> CountNotHelpfulVotesAsync(int reviewId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Verifica se um membro já votou em uma review
        /// </summary>
        Task<bool> HasMemberVotedAsync(int memberId, int reviewId, CancellationToken cancellationToken = default);
    }
}
