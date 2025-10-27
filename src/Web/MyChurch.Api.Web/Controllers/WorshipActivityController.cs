using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Api.Web.Middleware;
using MyChurch.Application.WorshipActivity.Commands;
using MyChurch.Application.WorshipService.Commands.ManageSchedule;
using MyChurch.Application.WorshipService.Commands.PrayerRequest;
using MyChurch.Application.WorshipService.Queries.PrayerRequest;
using MyChurch.Domain.Contracts;
using MyChurch.Application.WorshipService.Commands.Presence; // added

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorshipActivityController : BaseController
    {
        private readonly IHubContext<WorshipServiceHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;

        public WorshipActivityController(IHubContext<WorshipServiceHub> hubContext, IUnitOfWork unitOfWork)
        {
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Faz check-in do membro no culto (geolocalização obrigatória). Retorna 0 se já presente.
        /// </summary>
        [Authorize]
        [HttpPost("{worshipServiceId}/presence/check-in")]
        public async Task<IActionResult> RegisterPresence(int worshipServiceId, [FromBody] RegisterPresenceRequest body)
        {
            var command = AuthorizationRequestCreate<MyChurch.Application.WorshipService.Commands.Presence.RegisterWorshipPresenceCommand>();
            command.WorshipServiceId = worshipServiceId;
            command.Latitude = body.Latitude;
            command.Longitude = body.Longitude;
            command.MaxDistanceMeters = body.MaxDistanceMeters ?? 150;
            var id = await Mediator.Send(command);
            if (id > 0)
            {
                await _hubContext.Clients.Group($"worship_{worshipServiceId}")
                    .SendAsync("VisitorJoined", new { worshipServiceId, memberId = command.UserId });
            }
            return Ok(new { presenceId = id });
        }

        public class RegisterPresenceRequest
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double? MaxDistanceMeters { get; set; }
        }

        /// <summary>
        /// Destaca a leitura bíblica para todos os membros do culto
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{worshipServiceId}/bible-reading/highlight")]
        public async Task<IActionResult> HighlightBibleReading(int worshipServiceId, [FromQuery] int versionId, [FromQuery] int bookId, [FromQuery] int chapterId, [FromQuery] int? verseId)
        {
            var command = AuthorizationRequestCreate<HighlightBibleReadingCommand>();
            command.WorshipServiceId = worshipServiceId;
            command.VerseId = verseId;
            command.BookId = bookId;
            command.ChapterId = chapterId;
            command.VersionId = versionId;

            var result = await Mediator.Send(command);
            await _hubContext.Clients.Group($"worship_{worshipServiceId}")
                .SendAsync("BibleReadingHighlighted", new { ActivityId = result.ActivityId, VersionId = versionId, BookId = bookId, ChapterId = chapterId, VerseId = verseId, PresentationId = result.PresentationId, SlideIndex = result.SlideIndex });
            return Ok(new { ActivityId = result.ActivityId, VersionId = versionId, BookId = bookId, ChapterId = chapterId, VerseId = verseId, PresentationId = result.PresentationId, SlideIndex = result.SlideIndex });
        }

        /// <summary>
        /// Finaliza a leitura bíblica atual pelo id da atividade
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{worshipServiceId}/bible-reading/{activityId}/finish")]
        public async Task<IActionResult> FinishBibleReading(int worshipServiceId, int activityId)
        {
            var command = new HighlightBibleReadingCommand
            {
                WorshipServiceId = worshipServiceId,
                ActivityId = activityId,
                Finish = true
            };
            await Mediator.Send(command);
            await _hubContext.Clients.Group($"worship_{worshipServiceId}")
                .SendAsync("BibleReadingFinished", new { ActivityId = activityId });
            return Ok(new { ActivityId = activityId });
        }

        /// <summary>
        /// Apresenta um hino para todos os membros do culto
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{worshipServiceId}/hymn/{number}/present/{verseNumber}")]
        public async Task<IActionResult> PresentHymn(int worshipServiceId, int number, int verseNumber)
        {
            var command = AuthorizationRequestCreate<PresentHymnCommand>();
            command.HymnNumber = number;
            command.WorshipServiceId = worshipServiceId;
            command.VerseNumber = verseNumber;

            var returned = await Mediator.Send(command);
            await _hubContext.Clients.Group($"worship_{worshipServiceId}")
                .SendAsync("HymnPresented", returned );
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{worshipServiceId}/imported-hymn/{importedHymnId}/present/{stanzaOrder}")]
        public async Task<IActionResult> PresentImportedHymn(int worshipServiceId, int importedHymnId, int stanzaOrder)
        {
            var command = AuthorizationRequestCreate<PresentImportedHymnCommand>();
            command.WorshipServiceId = worshipServiceId;
            command.ImportedHymnId = importedHymnId;
            command.StanzaOrder = stanzaOrder;

            var result = await Mediator.Send(command);
            await _hubContext.Clients.Group($"worship_{worshipServiceId}")
                .SendAsync("ImportedHymnPresented", result);
            return Ok(result);
        }

        /// <summary>
        /// Apresenta o momento da oferta para todos os membros do culto
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{worshipServiceId}/offering/present")]
        public async Task<IActionResult> PresentOffering(int worshipServiceId)
        {
            var command = new PresentOfferingCommand
            {
                WorshipServiceId = worshipServiceId
            };
            var activityId = await Mediator.Send(command);
            await _hubContext.Clients.Group($"worship_{worshipServiceId}")
                .SendAsync("OfferingPresented", new { activityId });
            return Ok(new { activityId });
        }

        /// <summary>
        /// Finaliza o momento da oferta pelo id da atividade
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{worshipServiceId}/offering/{activityId}/finish")]
        public async Task<IActionResult> FinishOffering(int worshipServiceId, int activityId)
        {
            var command = new PresentOfferingCommand
            {
                WorshipServiceId = worshipServiceId,
                ActivityId = activityId,
                Finish = true
            };
            var finishedId = await Mediator.Send(command);
            await _hubContext.Clients.Group($"worship_{worshipServiceId}")
                .SendAsync("OfferingFinished", new { activityId });
            return Ok(new { activityId });
        }

        /// <summary>
        /// Finaliza o culto e notifica todos os membros
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{worshipServiceId}/finalize")]
        public async Task<IActionResult> FinalizeWorship(int worshipServiceId)
        {
            var command = new FinalizeWorshipCommand
            {
                WorshipServiceId = worshipServiceId
            };
            await Mediator.Send(command);
            await _hubContext.Clients.Group($"worship_{worshipServiceId}")
                .SendAsync("WorshipFinalized", new { worshipServiceId, message = "Culto finalizado!" });
            return Ok();
        }

        /// <summary>
        /// Inicia o culto e notifica todos os membros
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{worshipServiceId}/start")]
        public async Task<IActionResult> StartWorship(int worshipServiceId)
        {
            var command = new StartWorshipCommand
            {
                WorshipServiceId = worshipServiceId
            };
            await Mediator.Send(command);
            await _hubContext.Clients.Group($"worship_{worshipServiceId}")
                .SendAsync("WorshipStarted", new { worshipServiceId, message = "Culto iniciado!" });
            return Ok();
        }

        /// <summary>
        /// Retorna as atividades ativas do culto em andamento e registra a presença do usuário (registro simples sem geolocalização)
        /// </summary>
        [Authorize]
        [HttpGet("{worshipServiceId}/active-activities")]
        public async Task<IActionResult> GetActiveActivities(int worshipServiceId)
        {
            // Usa comando fully qualified para evitar ambiguidade
            var command = AuthorizationRequestCreate<MyChurch.Application.WorshipService.Commands.Presence.RegisterWorshipPresenceCommand>();
            command.WorshipServiceId = worshipServiceId;
            // Não seta lat/long aqui – poderia ser extendido se o cliente enviar
            await Mediator.Send(command);

            var activities = await Mediator.Send(new GetActiveWorshipActivitiesQuery { WorshipServiceId = worshipServiceId });
            return Ok(activities);
        }

        /// <summary>
        /// Adiciona um item ao cronograma do culto
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{worshipServiceId}/schedule/add")]
        public async Task<IActionResult> AddScheduleItem(int worshipServiceId, [FromBody] AddWorshipScheduleItemCommand command)
        {
            command.WorshipServiceId = worshipServiceId;
            var id = await Mediator.Send(command);
            return Ok(new { id });
        }

        /// <summary>
        /// Atualiza um item do cronograma do culto
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{worshipServiceId}/schedule/update/{id}")]
        public async Task<IActionResult> UpdateScheduleItem(int worshipServiceId, int id, [FromBody] UpdateWorshipScheduleItemCommand command)
        {
            command.WorshipServiceId = worshipServiceId;
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Remove um item do cronograma do culto
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{worshipServiceId}/schedule/remove/{id}")]
        public async Task<IActionResult> RemoveScheduleItem(int worshipServiceId, int id)
        {
            var command = AuthorizationRequestCreate<RemoveWorshipScheduleItemCommand>();
            command.Id = id;
            command.WorshipServiceId = worshipServiceId;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Envia um pedido de oração para o culto
        /// </summary>
        [Authorize]
        [HttpPost("{worshipServiceId}/prayer-request")]
        public async Task<IActionResult> CreatePrayerRequest(int worshipServiceId, [FromBody] CreatePrayerRequestCommand command)
        {
            command.WorshipServiceId = worshipServiceId;
            var id = await Mediator.Send(command);
            await _hubContext.Clients.Group($"worship_{worshipServiceId}")
                .SendAsync("PrayerRequestReceived", new { worshipServiceId, prayerRequestId = id });
            return Ok(id);
        }

        /// <summary>
        /// Lista os pedidos de oração do culto
        /// </summary>
        [Authorize]
        [HttpGet("{worshipServiceId}/prayer-requests/list")]
        public async Task<IActionResult> GetPrayerRequests(int worshipServiceId)
        {
            var query = AuthorizationRequestCreate<GetPrayerRequestsQuery>();

            query.WorshipServiceId = worshipServiceId;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Envia um aviso administrativo para todos do culto
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{worshipServiceId}/admin-notice")]
        public async Task<IActionResult> SendAdminNotice(int worshipServiceId, [FromBody] SendAdminNoticeCommand command)
        {
            // Busca o culto para obter o ChurchId
            var worshipService = await _unitOfWork.WorshipServices.Query().FirstOrDefaultAsync(ws => ws.Id == worshipServiceId);
            if (worshipService == null)
                return NotFound("Culto não encontrado.");

            command.ChurchId = worshipService.ChurchId;
            var noticeId = await Mediator.Send(command);
            await _hubContext.Clients.Group($"worship_{worshipServiceId}")
                .SendAsync("AdminNoticeReceived", new { noticeId });
            return Ok(new { noticeId });
        }
    }
}