using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyChurch.Domain.Entities;

namespace MyChurch.Domain.Contracts
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<List<Review>> GetReviewsAsync(int entityId, string entityType, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<double> GetAverageScoreAsync(int entityId, string entityType, CancellationToken cancellationToken = default);
        Task<int> GetTotalReviewsAsync(int entityId, string entityType, CancellationToken cancellationToken = default);
    }
}
