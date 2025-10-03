using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Reviews.Commands.SubmitReview;
using MyChurch.Application.Reviews.Commands.VoteReview;
using MyChurch.Application.Reviews.Queries.GetReviews;
using MyChurch.Application.Reviews.Queries.CanReviewChurch;
using MyChurch.Application.Reviews.Queries.GetChurchPhotoGallery;
using System.Threading.Tasks;

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
        /// </summary>
        /// <param name="command">Dados da avaliação com fotos opcionais</param>
        /// <returns>Resultado da submissão</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubmitReviewResult))]
        public async Task<IActionResult> SubmitReview([FromBody] SubmitReviewCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Lista avaliações de uma entidade (ex: igreja) com fotos.
        /// </summary>
        /// <param name="query">Filtros de busca</param>
        /// <returns>Lista paginada de avaliações com fotos</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviews([FromQuery] GetReviewsQuery query)
        {
            var result = await Mediator.Send(query);
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
        /// Vota em uma review como útil ou não útil (upvote/downvote)
        /// </summary>
        /// <param name="reviewId">ID da review</param>
        /// <param name="request">Tipo de voto</param>
        /// <returns>Resultado da votação com contadores atualizados</returns>
        [HttpPost("{reviewId}/vote")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VoteReviewResult))]
        public async Task<IActionResult> VoteReview([FromRoute] int reviewId, [FromBody] VoteReviewRequest request)
        {
            var command = AuthorizationRequestCreate<VoteReviewCommand>();
            command.ReviewId = reviewId;
            command.IsHelpful = request.IsHelpful;
            
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Marca uma review como útil (atalho para upvote)
        /// </summary>
        [HttpPost("{reviewId}/helpful")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VoteReviewResult))]
        public async Task<IActionResult> MarkAsHelpful([FromRoute] int reviewId)
        {
            var command = AuthorizationRequestCreate<VoteReviewCommand>();
            command.ReviewId = reviewId;
            command.IsHelpful = true;
            
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Marca uma review como não útil (atalho para downvote)
        /// </summary>
        [HttpPost("{reviewId}/not-helpful")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VoteReviewResult))]
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
}
