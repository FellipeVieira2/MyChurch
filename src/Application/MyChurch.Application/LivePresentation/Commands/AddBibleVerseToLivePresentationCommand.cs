using MediatR;
using MyChurch.Application.Presentation.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.LivePresentation.Commands
{
    public class AddBibleVerseToLivePresentationCommand : JwtMemberDto, IRequest<Unit>
    {
        public int PresentationId { get; set; }
        public string Book { get; set; }
        public int Chapter { get; set; }
        public int StartVerse { get; set; }
        public int EndVerse { get; set; }
    }

    public class AddBibleVerseToLivePresentationCommandHandler : IRequestHandler<AddBibleVerseToLivePresentationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ContentGenerationService _contentGenerationService;

        public AddBibleVerseToLivePresentationCommandHandler(IUnitOfWork unitOfWork, ContentGenerationService contentGenerationService)
        {
            _unitOfWork = unitOfWork;
            _contentGenerationService = contentGenerationService;
        }

        public async Task<Unit> Handle(AddBibleVerseToLivePresentationCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null)
                throw new System.Exception("Usuário não encontrado para obter ChurchId.");

            var contentRef = new
            {
                Book = request.Book,
                Chapter = request.Chapter,
                StartVerse = request.StartVerse,
                EndVerse = request.EndVerse
            };
            var contentReferenceJson = System.Text.Json.JsonSerializer.Serialize(contentRef);

            // Verifica se já existe slide igual
            var existingSlide = await _unitOfWork.Slides.Query()
                .FirstOrDefaultAsync(s => s.PresentationId == request.PresentationId
                    && s.ContentType == SlideContentType.BibleVerse
                    && s.ContentReferenceJson == contentReferenceJson, cancellationToken);
            if (existingSlide != null)
            {
                // Já existe, não cria novamente
                return Unit.Value;
            }

            var tuple = await _contentGenerationService.GenerateSlideContent(SlideContentType.BibleVerse, contentReferenceJson, member.ChurchId);

            var slide = new Domain.Entities.Slide
            {
                PresentationId = request.PresentationId,
                ContentType = SlideContentType.BibleVerse,
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