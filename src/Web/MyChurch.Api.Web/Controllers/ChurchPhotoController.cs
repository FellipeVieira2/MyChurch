using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.ChurchPhoto.Commands.LikePhoto;
using MyChurch.Application.ChurchPhoto.Commands.ModeratePhoto;
using MyChurch.Application.ChurchPhoto.Commands.UploadPhoto;
using MyChurch.Application.ChurchPhoto.Queries;
using MyChurch.Application.Dtos;

namespace MyChurch.Api.Web.Controllers
{
    /// <summary>
    /// Controller para galeria de fotos da igreja
    /// Sistema similar ao TripAdvisor para fotos da comunidade
    /// </summary>
    [ApiController]
    [Route("api/church/{churchId}/photos")]
    public class ChurchPhotoController : BaseController
    {
        private readonly IMediator _mediator;

        public ChurchPhotoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// ?? Upload de foto da igreja (Membros autenticados)
        /// </summary>
        /// <param name="churchId">ID da igreja</param>
        /// <param name="command">Dados da foto</param>
        [HttpPost("upload")]
        [Authorize] // Membros autenticados
        public async Task<ActionResult<ChurchPhotoDto>> UploadPhoto(int churchId, [FromBody] UploadChurchPhotoCommand command)
        {
            // Não sobrescrever o body: apenas aplicar dados de auth (UserId, Role etc.)
            var auth = AuthorizationRequestCreate<UploadChurchPhotoCommand>();
            command.UserId = auth.UserId;
            command.Role = auth.Role;
            command.ChurchId = churchId;

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// ?? Upload de foto por visitante (sem autenticação)
        /// </summary>
        /// <param name="churchId">ID da igreja</param>
        /// <param name="visitorId">ID do visitante</param>
        /// <param name="command">Dados da foto</param>
        [HttpPost("upload/visitor/{visitorId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ChurchPhotoDto>> UploadPhotoAsVisitor(int churchId, int visitorId, [FromBody] UploadChurchPhotoCommand command)
        {
            command.ChurchId = churchId;
            command.VisitorId = visitorId;

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// ??? Buscar galeria de fotos da igreja
        /// </summary>
        /// <param name="churchId">ID da igreja</param>
        /// <param name="category">Filtrar por categoria (Exterior, Interior, Worship, etc)</param>
        /// <param name="onlyFeatured">Apenas fotos em destaque</param>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ChurchPhotoGalleryDto>> GetPhotos(
            int churchId,
            [FromQuery] string? category = null,
            [FromQuery] bool onlyFeatured = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = new GetChurchPhotosQuery
            {
                ChurchId = churchId,
                Category = category,
                OnlyApproved = true,
                OnlyFeatured = onlyFeatured,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// ?? Buscar fotos pendentes de moderação (Admin apenas)
        /// </summary>
        /// <param name="churchId">ID da igreja</param>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ChurchPhotoDto>>> GetPendingPhotos(int churchId)
        {
            var query = AuthorizationRequestCreate<GetPendingPhotosQuery>();
            query.ChurchId = churchId;

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// ? Aprovar foto (Admin apenas)
        /// </summary>
        /// <param name="churchId">ID da igreja</param>
        /// <param name="photoId">ID da foto</param>
        /// <param name="command">Dados da aprovação</param>
        [HttpPost("{photoId}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ChurchPhotoDto>> ApprovePhoto(int churchId, int photoId, [FromBody] ApproveChurchPhotoCommand command)
        {
            command = AuthorizationRequestCreate<ApproveChurchPhotoCommand>();
            command.PhotoId = photoId;

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// ? Rejeitar foto (Admin apenas)
        /// </summary>
        /// <param name="churchId">ID da igreja</param>
        /// <param name="photoId">ID da foto</param>
        /// <param name="command">Motivo da rejeição</param>
        [HttpPost("{photoId}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RejectPhoto(int churchId, int photoId, [FromBody] RejectChurchPhotoCommand command)
        {
            command = AuthorizationRequestCreate<RejectChurchPhotoCommand>();
            command.PhotoId = photoId;

            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// ?? Curtir/Descurtir foto
        /// </summary>
        /// <param name="churchId">ID da igreja</param>
        /// <param name="photoId">ID da foto</param>
        [HttpPost("{photoId}/like")]
        [Authorize]
        public async Task<ActionResult<PhotoLikeResultDto>> ToggleLike(int churchId, int photoId)
        {
            var command = AuthorizationRequestCreate<TogglePhotoLikeCommand>();
            command.PhotoId = photoId;

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// ?? Curtir foto como visitante
        /// </summary>
        /// <param name="churchId">ID da igreja</param>
        /// <param name="photoId">ID da foto</param>
        /// <param name="visitorId">ID do visitante</param>
        [HttpPost("{photoId}/like/visitor/{visitorId}")]
        [AllowAnonymous]
        public async Task<ActionResult<PhotoLikeResultDto>> ToggleLikeAsVisitor(int churchId, int photoId, int visitorId)
        {
            var command = new TogglePhotoLikeCommand
            {
                PhotoId = photoId,
                VisitorId = visitorId
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// ? Marcar foto como destaque (Admin apenas)
        /// </summary>
        /// <param name="churchId">ID da igreja</param>
        /// <param name="photoId">ID da foto</param>
        /// <param name="displayOrder">Ordem de exibição</param>
        [HttpPost("{photoId}/feature")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ChurchPhotoDto>> SetAsFeatured(int churchId, int photoId, [FromQuery] int displayOrder = 0)
        {
            var command = AuthorizationRequestCreate<ApproveChurchPhotoCommand>();
            command.PhotoId = photoId;
            command.SetAsFeatured = true;
            command.DisplayOrder = displayOrder;

            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
