using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Application.Hymn.Queries.GetHymnByNumber;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MyChurch.Application.WorshipActivity.Commands
{
    public class PresentHymnCommand : JwtMemberDto, IRequest<object>
    {
        [JsonIgnore]
        public int WorshipServiceId { get; set; }
        [JsonIgnore]

        public int HymnNumber { get; set; }
        [JsonIgnore]

        public int VerseNumber { get; set; }
    }

    public class PresentHymnCommandHandler : IRequestHandler<PresentHymnCommand, object>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISender _sender;

        public PresentHymnCommandHandler(IUnitOfWork unitOfWork, ISender sender)
        {
            _unitOfWork = unitOfWork;
            _sender = sender;
        }

        public async Task<object> Handle(PresentHymnCommand request, CancellationToken cancellationToken)
        {
            var hymnQuery = new GetHymnByNumberQuery { Number = request.HymnNumber };
            var hymnDto = await _sender.Send(hymnQuery, cancellationToken);

            if (hymnDto == null)
            {
                ValidationException.ThrowException("Hymn", "Hino não encontrado.");
            }

            var worshipService = await _unitOfWork.WorshipServices.Query()
                .FirstOrDefaultAsync(ws => ws.Id == request.WorshipServiceId, cancellationToken);

            if (worshipService == null)
            {
                ValidationException.ThrowException("WorshipService", "Culto não encontrado.");
            }

            var activity = await _unitOfWork.WorshipActivities.Query()
                .Include(a => a.Bibles)
                .Include(a => a.Hymns)
                .FirstOrDefaultAsync(a => a.WorshipServiceId == request.WorshipServiceId && a.IsCurrent, cancellationToken);

            if (activity == null)
            {
                activity = new Domain.Entities.WorshipActivity
                {
                    WorshipServiceId = request.WorshipServiceId,
                    Name = "Louvor",
                    Order = 1,
                    IsCurrent = true,
                    Bibles = [],
                    Hymns = []
                };

                _unitOfWork.WorshipActivities.Create(activity);
                await _unitOfWork.CommitAsync();
            }
            else
            {
                activity.Name = "Louvor";
                activity.Bibles.Clear();
                activity.Hymns.Clear();
            }
            activity.Hymns.Add(new WorshipActivityHymn
            {
                HymnId = hymnDto.Id,
                HymnNumber = hymnDto.Number.ToString(),
                HymnTitle = hymnDto.Title,
                VerseNumber = request.VerseNumber
            });

            _unitOfWork.WorshipActivities.Update(activity);

            await _unitOfWork.CommitAsync();

            // Buscar apresentação do culto
            var presentation = await _unitOfWork.Presentations.Query().FirstOrDefaultAsync(p => p.Name == $"Culto_{request.WorshipServiceId}", cancellationToken);
            int? presentationId = presentation?.Id;
            int slideIndex = 0;
            if (presentationId != null)
            {
                var contentReferenceJson = System.Text.Json.JsonSerializer.Serialize(new { HymnNumber = request.HymnNumber, StanzaNumber = request.VerseNumber });
                var existingSlide = await _unitOfWork.Slides.Query()
                    .FirstOrDefaultAsync(s => s.PresentationId == presentationId.Value
                        && s.ContentType == MyChurch.Domain.Enum.SlideContentType.HymnStanza
                        && s.ContentReferenceJson == contentReferenceJson, cancellationToken);
                if (existingSlide != null)
                {
                    slideIndex = existingSlide.OrderIndex;
                }
                else
                {
                    var addSlide = new MyChurch.Application.Slide.Commands.AddSlideCommand
                    {
                        Email = request.Email,
                        Role = request.Role,
                        UserId = request.UserId,
                        PresentationId = presentationId.Value,
                        ContentType = MyChurch.Domain.Enum.SlideContentType.HymnStanza,
                        ContentReferenceJson = contentReferenceJson,
                        OrderIndex = await _unitOfWork.Slides.Query().CountAsync(s => s.PresentationId == presentationId.Value, cancellationToken)
                    };
                    // Não há contexto de usuário aqui, então Email/Role/UserId ficam nulos
                    var slideDto = await _sender.Send(addSlide, cancellationToken);
                    slideIndex = slideDto.OrderIndex;
                }
            }

            return new { hymnDto, VerseFocus = request.VerseNumber, PresentationId = presentationId, SlideIndex = slideIndex };
        }
    }
}