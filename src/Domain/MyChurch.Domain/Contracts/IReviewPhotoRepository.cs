using MyChurch.Domain.Entities;

namespace MyChurch.Domain.Contracts
{
    public interface IReviewPhotoRepository : IGenericRepository<ReviewPhoto>
    {
        /// <summary>
        /// Busca fotos de uma review específica
        /// </summary>
        Task<List<ReviewPhoto>> GetPhotosByReviewIdAsync(int reviewId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Busca todas as fotos de reviews de uma entidade (ex: todas fotos de reviews de uma igreja)
        /// </summary>
        Task<List<ReviewPhoto>> GetPhotosByEntityAsync(int entityId, string entityType, int limit = 100, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Remove todas as fotos de uma review
        /// </summary>
        Task DeletePhotosByReviewIdAsync(int reviewId, CancellationToken cancellationToken = default);
    }
}
