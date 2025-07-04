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
    public class GetPresentationByIdQuery : JwtMemberDto, IRequest<PresentationDto>
    {
        public int Id { get; set; }
    }

    public class GetPresentationByIdQueryHandler : IRequestHandler<GetPresentationByIdQuery, PresentationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPresentationByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PresentationDto> Handle(GetPresentationByIdQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null)
                throw new System.Exception("Usuário não encontrado para obter ChurchId.");

            var presentation = await _unitOfWork.Presentations.Query()
                .Include(p => p.Slides)
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.ChurchId == member.ChurchId, cancellationToken);

            if (presentation == null)
            {
                return null;
            }

            var presentationDto = new PresentationDto
            {
                Id = presentation.Id,
                ChurchId = presentation.ChurchId,
                AdminUserId = presentation.AdminUserId,
                Name = presentation.Name,
                Description = presentation.Description,
                CurrentSlideIndex = presentation.CurrentSlideIndex,
                IsLive = presentation.IsLive,
                Slides = presentation.Slides?.OrderBy(s => s.OrderIndex).Select(s => new SlideDto
                {
                    Id = s.Id,
                    OrderIndex = s.OrderIndex,
                    ContentType = s.ContentType,
                    ContentReferenceJson = s.ContentReferenceJson,
                    CachedDisplayText = s.CachedDisplayText,
                    CachedMediaUrl = s.CachedMediaUrl
                }).ToList() ?? new List<SlideDto>()
            };

            return presentationDto;
        }
    }
}