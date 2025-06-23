using MyChurch.Domain.Entities;

namespace MyChurch.Domain.Contracts
{
    public interface IPreLaunchInterestRepository : IGenericRepository<PreLaunchInterest>
    {
        Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken = default);
        Task<PreLaunchInterest> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<PreLaunchInterest> GetByConfirmationTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}