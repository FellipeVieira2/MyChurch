using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Reviews.Commands.SubmitReview;
using MyChurch.Application.Reviews.Queries.GetReviews;
using System.Threading.Tasks;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : BaseController
    {
        /// <summary>
        /// Envia uma avaliação para uma igreja ou entidade.
        /// </summary>
        /// <param name="command">Dados da avaliação</param>
        /// <returns>Resultado da submissão</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitReview([FromBody] SubmitReviewCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Lista avaliações de uma entidade (ex: igreja).
        /// </summary>
        /// <param name="entityId">ID da entidade</param>
        /// <param name="entityType">Tipo da entidade (ex: Church)</param>
        /// <param name="page">Página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista paginada de avaliações</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviews([FromQuery] GetReviewsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
