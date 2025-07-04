using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Presentation.Queries
{
    public class GetAllPresentationsQuery : JwtMemberDto, IRequest<List<PresentationDto>>
    {
    }

    public class GetAllPresentationsQueryHandler : IRequestHandler<GetAllPresentationsQuery, List<PresentationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllPresentationsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PresentationDto>> Handle(GetAllPresentationsQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null)
                throw new System.Exception("Usuário não encontrado para obter ChurchId.");

            var presentations = await _unitOfWork.Presentations.Query()
                .Include(p => p.Slides)
                .Where(p => p.ChurchId == member.ChurchId)
                .ToListAsync(cancellationToken);

            var dtos = presentations.Select(p => new PresentationDto
            {
                Id = p.Id,
                ChurchId = p.ChurchId,
                AdminUserId = p.AdminUserId,
                Name = p.Name,
                Description = p.Description,
                CurrentSlideIndex = p.CurrentSlideIndex,
                IsLive = p.IsLive,
                Slides = p.Slides?.OrderBy(s => s.OrderIndex).Select(s => new SlideDto
                {
                    Id = s.Id,
                    OrderIndex = s.OrderIndex,
                    ContentType = s.ContentType,
                    ContentReferenceJson = s.ContentReferenceJson,
                    CachedDisplayText = s.CachedDisplayText,
                    CachedMediaUrl = s.CachedMediaUrl
                }).ToList() ?? new List<SlideDto>()
            }).ToList();
            return dtos;
        }
    }
}