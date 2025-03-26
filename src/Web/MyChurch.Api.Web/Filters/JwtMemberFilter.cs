using Microsoft.AspNetCore.Mvc.Filters;
using MyChurch.Application.Dtos;

namespace MyChurch.Api.Web.Filters
{
    public class JwtMemberFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Items["User"] is JwtMemberDto user)
            {
                foreach (var argument in context.ActionArguments.Values)
                {
                    if (argument is JwtMemberDto jwtUserDto)
                    {
                        jwtUserDto.UserId = user.UserId;
                        jwtUserDto.Email = user.Email;
                        jwtUserDto.Role = user.Role;
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing
        }
    }
}