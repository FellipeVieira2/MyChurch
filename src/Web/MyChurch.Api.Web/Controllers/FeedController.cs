using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Application.Event.Commands.CreateEvent;
using MyChurch.Application.Event.Commands.DeleteEvent;
using MyChurch.Application.Feed.Like.Commands.AddFeedLike;
using MyChurch.Application.Feed.Like.Commands.RemoveFeedLike;
using MyChurch.Application.Feed.Post.Commands.CreateFeedPost;
using MyChurch.Application.Feed.Post.Commands.DeleteFeedPost;
using MyChurch.Application.Feed.Post.Commands.UpdateFeedPost;
using MyChurch.Application.Feed.Post.Queries.GetAllFeedPosts;
using MyChurch.Application.Feed.Post.Queries.GetFeedPostById;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedController : BaseController
    {
        /// <summary>
        /// Cria um novo Post
        /// </summary>
        /// <response code="200">Sucesso: Post criado</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> CreateFeedPost([FromBody] CreateFeedPostCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
        
        /// <summary>
        /// Atualiza um Post
        /// </summary>
        /// <response code="200">Sucesso: Post Atualizado</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeedPostDto))]
        public async Task<IActionResult> UpdateFeedPost([FromRoute] int id,[FromBody] UpdateFeedPostCommand command)
        {
            command.PostId = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Remove um Post
        /// </summary>
        /// <response code="204">Sucesso: Post removido</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteFeedPost([FromRoute] int id)
        {
            var command = AuthorizationRequestCreate<DeleteFeedPostCommand>();
            command.PostId = id;
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Atualiza um Post
        /// </summary>
        /// <response code="200">Sucesso: Post Atualizado</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [Authorize()]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResultDto<FeedPostDto>))]
        public async Task<IActionResult> GetAllFeedPosts()
        {
            var command = AuthorizationRequestCreate<GetAllFeedPostsQuery>();
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Retorna um Post pelo Id
        /// </summary>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeedPostDto))]
        public async Task<IActionResult> GetFeedPostById([FromRoute] int id)
        {
            var query = AuthorizationRequestCreate<GetFeedPostByIdQuery>();
            query.PostId = id;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Adiciona um Like ao Post
        /// </summary>
        [Authorize]
        [HttpPost("{id}/like")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> AddLike([FromRoute] int id)
        {
            var command = AuthorizationRequestCreate<AddFeedLikeCommand>();
            command.PostId = id;
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Remove o Like do Post
        /// </summary>
        [Authorize]
        [HttpDelete("{id}/like")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveLike([FromRoute] int id)
        {
            var command = AuthorizationRequestCreate<RemoveFeedLikeCommand>();
            command.PostId = id;
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
