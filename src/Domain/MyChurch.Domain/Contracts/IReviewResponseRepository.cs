using MyChurch.Domain.Entities;

namespace MyChurch.Domain.Contracts
{
    public interface IReviewResponseRepository : IGenericRepository<ReviewResponse>
    {
        /// <summary>
        /// Busca resposta de uma review específica
        /// </summary>
        Task<ReviewResponse?> GetByReviewIdAsync(int reviewId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Verifica se uma review já tem resposta
        /// </summary>
        Task<bool> HasResponseAsync(int reviewId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Busca todas as respostas de uma igreja
        /// </summary>
        Task<List<ReviewResponse>> GetChurchResponsesAsync(int churchId, CancellationToken cancellationToken = default);
    }
}
