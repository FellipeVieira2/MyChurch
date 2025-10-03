using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyChurch.Application.Common.Models;
using MyChurch.Application.Dtos;
using MyChurch.Application.Reviews.Commands.SubmitReview;
using MyChurch.Application.Reviews.Commands.VoteReview;
using MyChurch.Application.Reviews.Commands.RespondToReview;
using MyChurch.Application.Reviews.Commands.EditReviewResponse;
using MyChurch.Application.Reviews.Queries.GetReviews;
using MyChurch.Application.Reviews.Queries.CanReviewChurch;
using MyChurch.Application.Reviews.Queries.GetChurchPhotoGallery;
using System.Threading.Tasks;
using System.Security.Claims;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : BaseController
    {
        /// <summary>
        /// Envia uma avaliação para uma igreja ou entidade.
        /// Requer verificação de presença para igrejas.
        /// Aceita até 5 fotos por review.
        /// Limite: 3 reviews a cada 5 minutos.
        /// </summary>
        /// <param name="command">Dados da avaliação com fotos opcionais</param>
        /// <returns>Resultado da submissão</returns>
        [HttpPost]
        [Authorize]
        [EnableRateLimiting("review-limiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubmitReviewResult))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> SubmitReview([FromBody] SubmitReviewCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Lista avaliações de uma entidade (ex: igreja) com fotos e respostas.
        /// Se autenticado, mostra se o usuário já votou em cada review.
        /// Suporta filtros, paginação e ordenação.
        /// </summary>
        /// <param name="query">Filtros de busca, paginação e ordenação</param>
        /// <returns>Lista paginada de avaliações com fotos, respostas e status de voto</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<ReviewWithVoteDto>))]
        public async Task<IActionResult> GetReviews([FromQuery] GetReviewsQuery query)
        {
            // Se o usuário estiver autenticado, passa o MemberId para verificar votos
            if (User.Identity?.IsAuthenticated == true)
            {
                var memberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(memberIdClaim, out var memberId))
                {
                    query.CurrentMemberId = memberId;
                }
            }
            
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Igreja responde a uma avaliação (apenas Admin).
        /// Notifica automaticamente o avaliador.
        /// </summary>
        /// <param name="reviewId">ID da review</param>
        /// <param name="request">Conteúdo da resposta</param>
        /// <returns>Resultado da resposta</returns>
        [HttpPost("{reviewId}/respond")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespondToReviewResult))]
        public async Task<IActionResult> RespondToReview([FromRoute] int reviewId, [FromBody] RespondToReviewRequest request)
        {
            var command = AuthorizationRequestCreate<RespondToReviewCommand>();
            command.ReviewId = reviewId;
            command.Response = request.Response;
            
            var result = await Mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Edita resposta da igreja a uma avaliação (apenas Admin).
        /// </summary>
        /// <param name="responseId">ID da resposta</param>
        /// <param name="request">Novo conteúdo da resposta</param>
        /// <returns>Resultado da edição</returns>
        [HttpPut("response/{responseId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EditReviewResponseResult))]
        public async Task<IActionResult> EditReviewResponse([FromRoute] int responseId, [FromBody] EditReviewResponseRequest request)
        {
            var command = AuthorizationRequestCreate<EditReviewResponseCommand>();
            command.ResponseId = responseId;
            command.NewResponse = request.NewResponse;
            
            var result = await Mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Busca galeria de fotos de reviews de uma igreja.
        /// </summary>
        /// <param name="churchId">ID da igreja</param>
        /// <param name="limit">Limite de fotos (padrão: 50)</param>
        /// <returns>Galeria de fotos com informações dos revisores</returns>
        [HttpGet("church/{churchId}/photos")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChurchPhotoGalleryResult))]
        public async Task<IActionResult> GetChurchPhotoGallery([FromRoute] int churchId, [FromQuery] int limit = 50)
        {
            var query = new GetChurchPhotoGalleryQuery 
            { 
                ChurchId = churchId,
                Limit = limit
            };
            
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Verifica se o usuário logado pode avaliar uma igreja.
        /// Valida presença, período e reviews existentes.
        /// </summary>
        /// <param name="churchId">ID da igreja</param>
        /// <returns>Resultado da verificação</returns>
        [HttpGet("can-review/{churchId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CanReviewChurchResult))]
        public async Task<IActionResult> CanReviewChurch([FromRoute] int churchId)
        {
            var query = AuthorizationRequestCreate<CanReviewChurchQuery>();
            query.ChurchId = churchId;
            
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Vota em uma review como útil ou não útil (upvote/downvote).
        /// Limite: 10 votos por minuto.
        /// </summary>
        /// <param name="reviewId">ID da review</param>
        /// <param name="request">Tipo de voto</param>
        /// <returns>Resultado da votação com contadores atualizados</returns>
        [HttpPost("{reviewId}/vote")]
        [Authorize]
        [EnableRateLimiting("vote-limiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VoteReviewResult))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> VoteReview([FromRoute] int reviewId, [FromBody] VoteReviewRequest request)
        {
            var command = AuthorizationRequestCreate<VoteReviewCommand>();
            command.ReviewId = reviewId;
            command.IsHelpful = request.IsHelpful;
            
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Marca uma review como útil (atalho para upvote).
        /// Limite: 10 votos por minuto.
        /// </summary>
        [HttpPost("{reviewId}/helpful")]
        [Authorize]
        [EnableRateLimiting("vote-limiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VoteReviewResult))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> MarkAsHelpful([FromRoute] int reviewId)
        {
            var command = AuthorizationRequestCreate<VoteReviewCommand>();
            command.ReviewId = reviewId;
            command.IsHelpful = true;
            
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Marca uma review como não útil (atalho para downvote).
        /// Limite: 10 votos por minuto.
        /// </summary>
        [HttpPost("{reviewId}/not-helpful")]
        [Authorize]
        [EnableRateLimiting("vote-limiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VoteReviewResult))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> MarkAsNotHelpful([FromRoute] int reviewId)
        {
            var command = AuthorizationRequestCreate<VoteReviewCommand>();
            command.ReviewId = reviewId;
            command.IsHelpful = false;
            
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }

    public class VoteReviewRequest
    {
        public bool IsHelpful { get; set; }
    }

    public class RespondToReviewRequest
    {
        public string Response { get; set; } = string.Empty;
    }

    public class EditReviewResponseRequest
    {
        public string NewResponse { get; set; } = string.Empty;
    }
}
