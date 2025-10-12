using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;

namespace MyChurch.Api.Web.Filters
{
    public class JwtMemberFilter : IActionFilter
    {
        private readonly ILogger<JwtMemberFilter> _logger;

        public JwtMemberFilter(ILogger<JwtMemberFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Items["User"] is JwtMemberDto user)
            {
                _logger.LogInformation("✅ JwtMemberFilter: Preenchendo request com UserId={UserId}, Email={Email}, Role={Role}", 
                    user.UserId, user.Email, user.Role);

                foreach (var argument in context.ActionArguments.Values)
                {
                    if (argument is JwtMemberDto jwtUserDto)
                    {
                        jwtUserDto.UserId = user.UserId;
                        jwtUserDto.Email = user.Email;
                        jwtUserDto.Role = user.Role;

                        _logger.LogInformation("✅ Request preenchido: {Type} com UserId={UserId}", 
                            argument.GetType().Name, jwtUserDto.UserId);
                    }
                }
            }
            else
            {
                _logger.LogWarning("⚠️ HttpContext.Items['User'] não contém JwtMemberDto - Token não foi processado!");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing
        }
    }
}
