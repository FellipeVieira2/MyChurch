using MyChurch.Application.Dtos;
using System.IdentityModel.Tokens.Jwt;


namespace MyChurch.Api.Web.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtMiddleware> _logger; // 🔥 NOVO

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger) // 🔥 NOVO
        {
            _next = next;
            _logger = logger; // 🔥 NOVO
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("⚠️ Token JWT não encontrado no header Authorization");
            }
            else
            {
                _logger.LogInformation("✅ Token JWT recebido: {Token}", token.Substring(0, Math.Min(20, token.Length)) + "...");
            }

            if (token != null)
            {
                AttachUserToContext(context, token);
            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var role = jwtToken.Claims.FirstOrDefault(x => x.Type == "role")?.Value
                           ?? jwtToken.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

                // Platform user token
                var platformUserIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "platform_user_id")?.Value;
                if (!string.IsNullOrWhiteSpace(platformUserIdClaim))
                {
                    var email = jwtToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value
                              ?? jwtToken.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

                    _logger.LogInformation("✅ JWT PlatformUser Claims: PlatformUserId={PlatformUserId}, Email={Email}, Role={Role}", platformUserIdClaim, email, role);

                    context.Items["PlatformUser"] = new PlatformUserJwtDto
                    {
                        PlatformUserId = int.Parse(platformUserIdClaim),
                        Email = email,
                        Role = role ?? "PlatformAdmin"
                    };
                    return;
                }

                // Visitor token
                if (string.Equals(role, "Visitor", StringComparison.OrdinalIgnoreCase))
                {
                    var visitorId = jwtToken.Claims.First(x => x.Type == "visitor_id").Value;
                    var email = jwtToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value
                              ?? jwtToken.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

                    _logger.LogInformation("✅ JWT Visitor Claims: VisitorId={VisitorId}, Email={Email}", visitorId, email);

                    context.Items["Visitor"] = new VisitorJwtDto
                    {
                        VisitorId = int.Parse(visitorId),
                        Email = email,
                        Role = "Visitor"
                    };
                    return;
                }

                // Member token
                var userId = jwtToken.Claims.First(x => x.Type == "nameid").Value;
                var memberEmail = jwtToken.Claims.First(x => x.Type == "email").Value;
                var memberRole = jwtToken.Claims.First(x => x.Type == "role").Value;

                _logger.LogInformation("✅ JWT Claims extraídos: UserId={UserId}, Email={Email}, Role={Role}", userId, memberEmail, memberRole);

                context.Items["User"] = new JwtMemberDto
                {
                    UserId = int.Parse(userId),
                    Email = memberEmail,
                    Role = memberRole
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao processar token JWT");
            }
        }
    }
}
