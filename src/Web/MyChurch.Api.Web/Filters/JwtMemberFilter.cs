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
            JwtMemberDto? memberContext = null;

            if (context.HttpContext.Items["User"] is JwtMemberDto member)
            {
                memberContext = member;
            }
            else if (context.HttpContext.Items["PlatformUser"] is PlatformUserJwtDto platform)
            {
                // Adapta PlatformUserJwtDto para o formato esperado pelos commands/queries que herdam de JwtMemberDto
                memberContext = new JwtMemberDto
                {
                    UserId = platform.PlatformUserId,
                    Email = platform.Email,
                    Role = platform.Role
                };
            }

            if (memberContext != null)
            {
                _logger.LogInformation(
                    "✅ JwtMemberFilter: Preenchendo request com UserId={UserId}, Email={Email}, Role={Role}",
                    memberContext.UserId, memberContext.Email, memberContext.Role);

                foreach (var argument in context.ActionArguments.Values)
                {
                    if (argument is JwtMemberDto jwtUserDto)
                    {
                        jwtUserDto.UserId = memberContext.UserId;
                        jwtUserDto.Email = memberContext.Email;
                        jwtUserDto.Role = memberContext.Role;

                        _logger.LogInformation(
                            "✅ Request preenchido: {Type} com UserId={UserId}",
                            argument.GetType().Name, jwtUserDto.UserId);
                    }
                    else if (argument is PlatformUserJwtDto platformArg)
                    {
                        // Se algum endpoint usar PlatformUserJwtDto diretamente
                        platformArg.PlatformUserId = memberContext.UserId;
                        platformArg.Email = memberContext.Email;
                        platformArg.Role = memberContext.Role ?? platformArg.Role;

                        _logger.LogInformation(
                            "✅ Request preenchido: {Type} com PlatformUserId={PlatformUserId}",
                            argument.GetType().Name, platformArg.PlatformUserId);
                    }
                }
            }
            else
            {
                _logger.LogWarning("⚠️ JwtMemberFilter: Token não processado (nenhum User/PlatformUser no HttpContext.Items)");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing
        }
    }
}
