using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Application.Presentation.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.LivePresentation.Commands
{
    public class AddHymnStanzaToLivePresentationCommand : JwtMemberDto, IRequest<Unit>
    {
        public int PresentationId { get; set; }
        public int HymnNumber { get; set; }
        public int StanzaNumber { get; set; }
    }

    public class AddHymnStanzaToLivePresentationCommandHandler : IRequestHandler<AddHymnStanzaToLivePresentationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ContentGenerationService _contentGenerationService;

        public AddHymnStanzaToLivePresentationCommandHandler(IUnitOfWork unitOfWork, ContentGenerationService contentGenerationService)
        {
            _unitOfWork = unitOfWork;
            _contentGenerationService = contentGenerationService;
        }

        public async Task<Unit> Handle(AddHymnStanzaToLivePresentationCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null)
                throw new System.Exception("Usuário não encontrado para obter ChurchId.");

            var contentRef = new
            {
                HymnNumber = request.HymnNumber,
                StanzaNumber = request.StanzaNumber
            };
            var contentReferenceJson = System.Text.Json.JsonSerializer.Serialize(contentRef);
            var tuple = await _contentGenerationService.GenerateSlideContent(SlideContentType.HymnStanza, contentReferenceJson, member.ChurchId);

            var slide = new Domain.Entities.Slide
            {
                PresentationId = request.PresentationId,
                ContentType = SlideContentType.HymnStanza,
                ContentReferenceJson = contentReferenceJson,
                CachedDisplayText = tuple.Item1,
                CachedMediaUrl = tuple.Item2,
                OrderIndex = await _unitOfWork.Slides.Query().CountAsync(s => s.PresentationId == request.PresentationId, cancellationToken)
            };
            _unitOfWork.Slides.Create(slide);
            await _unitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}
