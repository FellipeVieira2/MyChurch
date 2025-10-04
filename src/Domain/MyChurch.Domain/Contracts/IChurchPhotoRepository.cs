using MyChurch.Domain.Entities;

namespace MyChurch.Domain.Contracts
{
    /// <summary>
    /// Repository para fotos da igreja
    /// </summary>
    public interface IChurchPhotoRepository : IGenericRepository<ChurchPhoto>
    {
        /// <summary>
        /// Busca fotos de uma igreja com filtros
        /// </summary>
        Task<List<ChurchPhoto>> GetChurchPhotosAsync(
            int churchId, 
            PhotoCategory? category = null, 
            bool onlyApproved = true,
            bool onlyFeatured = false,
            int page = 1, 
            int pageSize = 20, 
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Busca fotos pendentes de aprovação
        /// </summary>
        Task<List<ChurchPhoto>> GetPendingPhotosAsync(
            int churchId, 
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Conta total de fotos de uma igreja
        /// </summary>
        Task<int> GetTotalPhotosCountAsync(
            int churchId, 
            PhotoCategory? category = null,
            bool onlyApproved = true,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Busca foto por ID com relacionamentos
        /// </summary>
        Task<ChurchPhoto?> GetPhotoWithDetailsAsync(
            int photoId, 
            CancellationToken cancellationToken = default);
    }
    
    /// <summary>
    /// Repository para likes em fotos da igreja
    /// </summary>
    public interface IChurchPhotoLikeRepository : IGenericRepository<ChurchPhotoLike>
    {
        /// <summary>
        /// Verifica se um usuário já curtiu uma foto
        /// </summary>
        Task<bool> HasUserLikedPhotoAsync(
            int photoId, 
            int? memberId, 
            int? visitorId, 
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Busca like de um usuário em uma foto
        /// </summary>
        Task<ChurchPhotoLike?> GetUserLikeAsync(
            int photoId, 
            int? memberId, 
            int? visitorId, 
            CancellationToken cancellationToken = default);
    }
}
