using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MyChurch.Application.Presentation.Commands;
using MyChurch.Application.Slide.Commands;
using MyChurch.Domain.Enum;
using System.Text.Json;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.WorshipActivity.Commands
{
    public class HighlightBibleReadingCommand : JwtMemberDto, IRequest<HighlightBibleReadingResult>
    {
        public int WorshipServiceId { get; set; }
        public int VersionId { get; set; }
        public int BookId { get; set; }
        public int ChapterId { get; set; }
        public int? VerseId { get; set; }
        public bool? Finish { get; set; }
        public int? ActivityId { get; set; }
    }

    public class HighlightBibleReadingResult
    {
        public int ActivityId { get; set; }
        public int PresentationId { get; set; }
        public int SlideIndex { get; set; } = 0;
    }

    public class HighlightBibleReadingCommandHandler : IRequestHandler<HighlightBibleReadingCommand, HighlightBibleReadingResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public HighlightBibleReadingCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<HighlightBibleReadingResult> Handle(HighlightBibleReadingCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.WorshipActivity activity = null;
            if (request.Finish == true && request.ActivityId.HasValue)
            {
                activity = await _unitOfWork.WorshipActivities.Query()
                    .Include(a => a.Bibles)
                    .FirstOrDefaultAsync(a => a.Id == request.ActivityId.Value && a.WorshipServiceId == request.WorshipServiceId, cancellationToken);
                if (activity != null)
                {
                    activity.IsCurrent = false;
                    _unitOfWork.WorshipActivities.Update(activity);
                    await _unitOfWork.CommitAsync();
                    return new HighlightBibleReadingResult { ActivityId = activity.Id, PresentationId = 0 };
                }
                return new HighlightBibleReadingResult { ActivityId = 0, PresentationId = 0 };
            }

            activity = await _unitOfWork.WorshipActivities.Query()
                .Include(a => a.Bibles)
                .Include(a => a.Hymns)
                .FirstOrDefaultAsync(a => a.WorshipServiceId == request.WorshipServiceId && a.IsCurrent, cancellationToken);

            if (activity == null)
            {
                activity = new Domain.Entities.WorshipActivity
                {
                    WorshipServiceId = request.WorshipServiceId,
                    Name = "Leitura Bíblica",
                    Order = 1,
                    IsCurrent = true,
                    Bibles = []
                };

                _unitOfWork.WorshipActivities.Create(activity);
                await _unitOfWork.CommitAsync();
            }
            else
            {
                activity.Name = "Leitura Bíblica";
                activity.Bibles.Clear();
                activity.Hymns.Clear();
            }
            activity.Bibles.Add(new WorshipActivityBible
            {
                BibleVersionId = request.VersionId,
                BookId = request.BookId,
                ChapterId = request.ChapterId,
                VerseStart = request.VerseId ?? 1,
                VerseEnd = request.VerseId
            });
            activity.IsCurrent = true;
            _unitOfWork.WorshipActivities.Update(activity);
            await _unitOfWork.CommitAsync();

            var presentation = await _unitOfWork.Presentations.Query().FirstOrDefaultAsync(p => p.Name == $"Culto_{request.WorshipServiceId}", cancellationToken);
            int presentationId;
            if (presentation == null)
            {
                var createPresentation = new CreatePresentationCommand
                {
                    Email = request.Email,
                    Role = request.Role,
                    UserId = request.UserId,
                    Name = $"Culto_{request.WorshipServiceId}",
                    Description = "Apresentação automática do culto"
                };
                var created = await _mediator.Send(createPresentation, cancellationToken);
                presentationId = created.Id;
            }
            else
            {
                presentationId = presentation.Id;
            }

            var contentReferenceJson = JsonSerializer.Serialize(new { Book = request.BookId, Chapter = request.ChapterId, StartVerse = request.VerseId ?? 1, EndVerse = request.VerseId ?? 1 });

            // Busca slide existente
            var existingSlide = await _unitOfWork.Slides.Query()
                .FirstOrDefaultAsync(s => s.PresentationId == presentationId
                    && s.ContentType == SlideContentType.BibleVerse
                    && s.ContentReferenceJson == contentReferenceJson, cancellationToken);
            int slideIndex;
            if (existingSlide != null)
            {
                slideIndex = existingSlide.OrderIndex;
            }
            else
            {
                var addSlide = new AddSlideCommand
                {
                    Email = request.Email,
                    Role = request.Role,
                    UserId = request.UserId,
                    PresentationId = presentationId,
                    ContentType = SlideContentType.BibleVerse,
                    ContentReferenceJson = contentReferenceJson,
                    OrderIndex = await _unitOfWork.Slides.Query().CountAsync(s => s.PresentationId == presentationId, cancellationToken)
                };
                var slideDto = await _mediator.Send(addSlide, cancellationToken);
                slideIndex = slideDto.OrderIndex;
            }

            return new HighlightBibleReadingResult { ActivityId = activity.Id, PresentationId = presentationId, SlideIndex = slideIndex };
        }
    }
}