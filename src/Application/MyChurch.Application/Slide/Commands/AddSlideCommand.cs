using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Application.Presentation.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Slide.Commands
{
    public class AddSlideCommand : JwtMemberDto, IRequest<SlideDto>
    {
        public int PresentationId { get; set; }
        public SlideContentType ContentType { get; set; }
        public string ContentReferenceJson { get; set; }
        public int OrderIndex { get; set; }
    }

    public class AddSlideCommandHandler : IRequestHandler<AddSlideCommand, SlideDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ContentGenerationService _contentGenerationService;

        public AddSlideCommandHandler(IUnitOfWork unitOfWork, ContentGenerationService contentGenerationService)
        {
            _unitOfWork = unitOfWork;
            _contentGenerationService = contentGenerationService;
        }

        public async Task<SlideDto> Handle(AddSlideCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro autenticado para obter ChurchId se necessário
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null)
                throw new System.Exception("Usuário não encontrado para obter ChurchId.");

            var tuple = await _contentGenerationService.GenerateSlideContent(request.ContentType, request.ContentReferenceJson, member.ChurchId);

            var slide = new Domain.Entities.Slide
            {
                PresentationId = request.PresentationId,
                ContentType = request.ContentType,
                ContentReferenceJson = request.ContentReferenceJson,
                CachedDisplayText = tuple.Item1,
                CachedMediaUrl = tuple.Item2,
                OrderIndex = request.OrderIndex
            };

            _unitOfWork.Slides.Create(slide);
            await _unitOfWork.CommitAsync();

            return new SlideDto
            {
                Id = slide.Id,
                OrderIndex = slide.OrderIndex,
                ContentType = slide.ContentType,
                ContentReferenceJson = slide.ContentReferenceJson,
                CachedDisplayText = slide.CachedDisplayText,
                CachedMediaUrl = slide.CachedMediaUrl
            };
        }
    }
}