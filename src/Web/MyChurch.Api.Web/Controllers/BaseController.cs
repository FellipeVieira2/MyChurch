using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Dtos;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        private ISender _mediator = null!;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
        protected T AuthorizationRequestCreate<T>() where T : JwtMemberDto, new()
        {

            if (HttpContext.Items["User"] is JwtMemberDto user)
            {
                return new T { UserId = user.UserId, Email = user.Email, Role = user.Role };
            }
            else
            {
                return new T(); ;
            }
        }

    }
}
