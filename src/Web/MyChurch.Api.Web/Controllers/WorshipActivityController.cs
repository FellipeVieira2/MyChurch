using Amazon.Runtime.Internal;
using Azure.Core;
using MediatR;
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
using static Amazon.S3.Util.S3EventNotification;

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
        /// Destaca a leitura bíblica para todos os membros do culto
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{worshipServiceId}/bible-reading/highlight")]
        public async Task<IActionResult> HighlightBibleReading(int worshipServiceId, [FromQuery] int versionId, [FromQuery] int bookId, [FromQuery] int chapterId, [FromQuery] int? verseId)
        {
            var command = new HighlightBibleReadingCommand
            {
                WorshipServiceId = worshipServiceId,
                VersionId = versionId,
                BookId = bookId,
                ChapterId = chapterId,
                VerseId = verseId
            };
            var activityId = await Mediator.Send(command);
            await _hubContext.Clients.Group($"worship_{worshipServiceId}")
                .SendAsync("BibleReadingHighlighted", new { ActivityId = activityId, VersionId = versionId, BookId = bookId, ChapterId = chapterId, VerseId = verseId });
            return Ok(new { ActivityId = activityId, VersionId = versionId, BookId = bookId, ChapterId = chapterId, VerseId = verseId });
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
            var command = new PresentHymnCommand
            {
                WorshipServiceId = worshipServiceId,
                HymnNumber = number,
                VerseNumber = verseNumber
            };
            var returned = await Mediator.Send(command);
            await _hubContext.Clients.Group($"worship_{worshipServiceId}")
                .SendAsync("HymnPresented", returned );
            return Ok();
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
        /// Retorna as atividades ativas do culto em andamento e registra a presença do usuário
        /// </summary>
        [Authorize]
        [HttpGet("{worshipServiceId}/active-activities")]
        public async Task<IActionResult> GetActiveActivities(int worshipServiceId)
        {
            var command = AuthorizationRequestCreate<RegisterWorshipPresenceCommand>();
            command.WorshipServiceId = worshipServiceId;
            await Mediator.Send(command);

            // Busca as atividades ativas do culto
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