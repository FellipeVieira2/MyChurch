using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Dtos;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        private readonly ISender? _injectedMediator;
        private ISender _mediator = null!;

        // Default constructor for production use
        public BaseController() { }

        // Constructor for test injection
        public BaseController(ISender mediator)
        {
            _injectedMediator = mediator;
        }

        protected ISender Mediator => _injectedMediator ?? (_mediator ??= HttpContext?.RequestServices?.GetService<ISender>() ?? throw new InvalidOperationException("ISender not available. Make sure HttpContext is set or inject ISender via constructor."));

        protected T AuthorizationRequestCreate<T>() where T : JwtMemberDto, new()
        {
            if (HttpContext?.Items["User"] is JwtMemberDto user)
            {
                return new T { UserId = user.UserId, Email = user.Email, Role = user.Role };
            }
            else
            {
                return new T();
            }
        }
    }
}
