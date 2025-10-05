using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Church.Queries.GetChurchPhotos
{
    /// <summary>
    /// Query para buscar fotos de uma igreja
    /// </summary>
    public class GetChurchPhotosQuery : IRequest<ChurchPhotoGalleryDto>
    {
        public int ChurchId { get; set; }
        public PhotoCategory? Category { get; set; }
        public bool OnlyApproved { get; set; } = true;
        public bool OnlyFeatured { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int? CurrentUserId { get; set; } // Para verificar likes
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
            var query = _unitOfWork.ChurchPhotos.Query()
                .Include(p => p.Church)
                .Include(p => p.UploadedBy)
                .Include(p => p.UploadedByVisitor)
                .Where(p => p.ChurchId == request.ChurchId);

            if (request.OnlyApproved)
            {
                query = query.Where(p => p.IsApproved && !p.IsRejected);
            }

            if (request.Category.HasValue)
            {
                query = query.Where(p => p.Category == request.Category.Value);
            }

            if (request.OnlyFeatured)
            {
                query = query.Where(p => p.IsFeatured);
            }

            // Ordenação: Featured primeiro, depois por likes, depois por data
            query = query
                .OrderByDescending(p => p.IsFeatured)
                .ThenBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.Likes)
                .ThenByDescending(p => p.UploadedAt);

            var totalPhotos = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalPhotos / (double)request.PageSize);

            var photos = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Busca quais fotos o usuário curtiu
            var userLikes = new HashSet<int>();
            if (request.CurrentUserId.HasValue)
            {
                var likedPhotoIds = await _unitOfWork.ChurchPhotoLikes.Query()
                    .Where(l => l.MemberId == request.CurrentUserId.Value && 
                                photos.Select(p => p.Id).Contains(l.ChurchPhotoId))
                    .Select(l => l.ChurchPhotoId)
                    .ToListAsync(cancellationToken);
                
                userLikes = likedPhotoIds.ToHashSet();
            }

            // Contagem por categoria
            var categoryCounts = await _unitOfWork.ChurchPhotos.Query()
                .Where(p => p.ChurchId == request.ChurchId && p.IsApproved && !p.IsRejected)
                .GroupBy(p => p.Category)
                .Select(g => new PhotoCategoryCountDto
                {
                    Category = g.Key.ToString(),
                    CategoryDisplay = GetCategoryDisplay(g.Key),
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            return new ChurchPhotoGalleryDto
            {
                ChurchId = request.ChurchId,
                ChurchName = photos.FirstOrDefault()?.Church?.Name ?? string.Empty,
                TotalPhotos = totalPhotos,
                Photos = photos.Select(p => ChurchPhotoDto.New(p, userLikes.Contains(p.Id))).ToList(),
                CategoriesCount = categoryCounts,
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.Page < totalPages,
                HasPreviousPage = request.Page > 1
            };
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
}
