using MyChurch.Domain.Contracts;

namespace MyChurch.Domain.Services
{
    /// <summary>
    /// Serviço para verificar se um membro visitou uma igreja
    /// </summary>
    public interface IReviewVerificationService
    {
        /// <summary>
        /// Verifica se um membro tem presença confirmada em uma igreja
        /// </summary>
        Task<bool> HasMemberVisitedChurchAsync(int memberId, int churchId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Verifica se um visitante anônimo tem presença confirmada em uma igreja
        /// </summary>
        Task<bool> HasVisitorVisitedChurchAsync(int visitorId, int churchId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Busca o ID da presença mais recente de um membro em uma igreja
        /// </summary>
        Task<int?> GetLatestPresenceIdAsync(int memberId, int churchId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Verifica se um membro pode avaliar uma igreja (tem presença nos últimos 6 meses)
        /// </summary>
        Task<(bool canReview, string? reason)> CanMemberReviewChurchAsync(int memberId, int churchId, CancellationToken cancellationToken = default);
    }
}
