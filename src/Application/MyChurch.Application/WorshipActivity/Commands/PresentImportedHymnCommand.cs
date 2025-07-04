using MyChurch.Application.Dtos;
using MediatR;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Presentation.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.WorshipActivity.Commands
{
    public class PresentImportedHymnCommand : JwtMemberDto, IRequest<object>
    {
        [JsonIgnore]
        public int WorshipServiceId { get; set; }
        [JsonIgnore]
        public int ImportedHymnId { get; set; }
        [JsonIgnore]
        public int StanzaOrder { get; set; } // Ordem da estrofe a ser apresentada

        public class Handler : IRequestHandler<PresentImportedHymnCommand, object>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ContentGenerationService _contentGenerationService;

            public Handler(IUnitOfWork unitOfWork, ContentGenerationService contentGenerationService)
            {
                _unitOfWork = unitOfWork;
                _contentGenerationService = contentGenerationService;
            }

            public async Task<object> Handle(PresentImportedHymnCommand request, CancellationToken cancellationToken)
            {
                var importedHymn = await _unitOfWork.ImportedHymns.GetByIdWithStanzasAsync(request.ImportedHymnId);
                if (importedHymn == null)
                    throw new System.Exception("Imported hymn not found.");

                var stanza = importedHymn.Stanzas.OrderBy(s => s.Order).FirstOrDefault(s => s.Order == request.StanzaOrder);
                if (stanza == null)
                    throw new System.Exception("Stanza not found.");

                var worshipService = await _unitOfWork.WorshipServices.Query()
                    .FirstOrDefaultAsync(ws => ws.Id == request.WorshipServiceId, cancellationToken);
                if (worshipService == null)
                    throw new System.Exception("Worship service not found.");

                // Buscar apresentação do culto
                var presentation = await _unitOfWork.Presentations.Query().FirstOrDefaultAsync(p => p.Name == $"Culto_{request.WorshipServiceId}", cancellationToken);
                int? presentationId = presentation?.Id;
                if (presentationId == null)
                    throw new System.Exception("Presentation for worship service not found.");

                var contentReferenceJson = System.Text.Json.JsonSerializer.Serialize(new { ImportedHymnId = importedHymn.Id, StanzaId = stanza.Id });
                var existingSlide = await _unitOfWork.Slides.Query()
                    .FirstOrDefaultAsync(s => s.PresentationId == presentationId.Value
                        && s.ContentType == SlideContentType.HymnStanza
                        && s.ContentReferenceJson == contentReferenceJson, cancellationToken);
                Domain.Entities.Slide slideResult;
                if (existingSlide != null)
                {
                    slideResult = existingSlide;
                }
                else
                {
                    // Gera slide
                    var tuple = await _contentGenerationService.GenerateSlideContent(SlideContentType.HymnStanza, contentReferenceJson, worshipService.ChurchId);
                    var slide = new MyChurch.Domain.Entities.Slide
                    {
                        PresentationId = presentationId.Value,
                        ContentType = SlideContentType.HymnStanza,
                        ContentReferenceJson = contentReferenceJson,
                        CachedDisplayText = tuple.Item1 ?? stanza.Text,
                        CachedMediaUrl = tuple.Item2,
                        OrderIndex = await _unitOfWork.Slides.Query().CountAsync(s => s.PresentationId == presentationId.Value, cancellationToken)
                    };
                    _unitOfWork.Slides.Create(slide);
                    await _unitOfWork.CommitAsync();
                    slideResult = slide;
                }

                return new { ImportedHymn = importedHymn, Stanza = stanza, PresentationId = presentationId, SlideIndex = slideResult.OrderIndex };
            }
        }
    }
}
