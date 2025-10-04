using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.ChurchPhoto.Queries
{
    /// <summary>
    /// Query para buscar galeria de fotos da igreja
    /// </summary>
    public class GetChurchPhotosQuery : IRequest<ChurchPhotoGalleryDto>
    {
        public int ChurchId { get; set; }
        public string? Category { get; set; }
        public bool OnlyApproved { get; set; } = true;
        public bool OnlyFeatured { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        
        // Para verificar se usuário curtiu
        public int? MemberId { get; set; }
        public int? VisitorId { get; set; }
    }
    
    public class GetChurchPhotosQueryHandler : IRequestHandler<GetChurchPhotosQuery, ChurchPhotoGalleryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetChurchPhotosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ChurchPhotoGalleryDto> Handle(GetChurchPhotosQuery request, CancellationToken cancellationToken)
        {
            // Buscar igreja
            var church = await _unitOfWork.Churchs.Query()
                .FirstOrDefaultAsync(c => c.Id == request.ChurchId, cancellationToken);
            
            if (church == null)
                ValidationException.ThrowException("Church", "Igreja não encontrada.");

            // Parse categoria
            PhotoCategory? category = null;
            if (!string.IsNullOrEmpty(request.Category) && Enum.TryParse<PhotoCategory>(request.Category, out var cat))
            {
                category = cat;
            }

            // Buscar fotos
            var photos = await _unitOfWork.ChurchPhotos.GetChurchPhotosAsync(
                request.ChurchId,
                category,
                request.OnlyApproved,
                request.OnlyFeatured,
                request.Page,
                request.PageSize,
                cancellationToken);

            // Buscar total
            var totalCount = await _unitOfWork.ChurchPhotos.GetTotalPhotosCountAsync(
                request.ChurchId,
                category,
                request.OnlyApproved,
                cancellationToken);

            // Verificar likes do usuário
            var photoIds = photos.Select(p => p.Id).ToList();
            var userLikes = new List<int>();
            
            if (request.MemberId.HasValue || request.VisitorId.HasValue)
            {
                userLikes = await _unitOfWork.ChurchPhotoLikes.Query()
                    .Where(l => photoIds.Contains(l.ChurchPhotoId) &&
                               ((request.MemberId.HasValue && l.MemberId == request.MemberId.Value) ||
                                (request.VisitorId.HasValue && l.VisitorId == request.VisitorId.Value)))
                    .Select(l => l.ChurchPhotoId)
                    .ToListAsync(cancellationToken);
            }

            // Mapear para DTOs
            var photoDtos = photos.Select(p => ChurchPhotoDto.New(p, userLikes.Contains(p.Id))).ToList();

            // Buscar contagem por categoria
            var categoriesCount = await GetCategoriesCountAsync(request.ChurchId, request.OnlyApproved, cancellationToken);

            // Calcular paginação
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new ChurchPhotoGalleryDto
            {
                ChurchId = request.ChurchId,
                ChurchName = church.Name,
                TotalPhotos = totalCount,
                Photos = photoDtos,
                CategoriesCount = categoriesCount,
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.Page < totalPages,
                HasPreviousPage = request.Page > 1
            };
        }

        private async Task<List<PhotoCategoryCountDto>> GetCategoriesCountAsync(
            int churchId, 
            bool onlyApproved, 
            CancellationToken cancellationToken)
        {
            var query = _unitOfWork.ChurchPhotos.Query()
                .Where(p => p.ChurchId == churchId);

            if (onlyApproved)
                query = query.Where(p => p.IsApproved && !p.IsRejected);

            var counts = await query
                .GroupBy(p => p.Category)
                .Select(g => new PhotoCategoryCountDto
                {
                    Category = g.Key.ToString(),
                    CategoryDisplay = GetCategoryDisplay(g.Key),
                    Count = g.Count()
                })
                .OrderByDescending(c => c.Count)
                .ToListAsync(cancellationToken);

            return counts;
        }

        private static string GetCategoryDisplay(PhotoCategory category)
        {
            return category switch
            {
                PhotoCategory.Exterior => "Fachada",
                PhotoCategory.Interior => "Interior",
                PhotoCategory.Worship => "Culto",
                PhotoCategory.Events => "Eventos",
                PhotoCategory.Community => "Comunidade",
                PhotoCategory.Facilities => "Instalações",
                PhotoCategory.ChildMinistry => "Ministério Infantil",
                PhotoCategory.YouthMinistry => "Ministério Jovem",
                PhotoCategory.Menu => "Programação",
                _ => "Outros"
            };
        }
    }
    
    /// <summary>
    /// Query para buscar fotos pendentes de moderação (apenas Admin)
    /// </summary>
    public class GetPendingPhotosQuery : JwtMemberDto, IRequest<List<ChurchPhotoDto>>
    {
        public int ChurchId { get; set; }
    }
    
    public class GetPendingPhotosQueryHandler : IRequestHandler<GetPendingPhotosQuery, List<ChurchPhotoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPendingPhotosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ChurchPhotoDto>> Handle(GetPendingPhotosQuery request, CancellationToken cancellationToken)
        {
            // Verificar se é admin
            var admin = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            
            if (admin == null || admin.Role != Domain.Enum.UserRole.Admin)
            {
                ValidationException.ThrowException("Authorization", "Apenas administradores podem ver fotos pendentes.");
            }

            // Buscar fotos pendentes
            var photos = await _unitOfWork.ChurchPhotos.GetPendingPhotosAsync(request.ChurchId, cancellationToken);

            return photos.Select(p => ChurchPhotoDto.New(p)).ToList();
        }
    }
}
