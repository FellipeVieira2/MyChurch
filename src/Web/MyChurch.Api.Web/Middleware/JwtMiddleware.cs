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

            // 🔥 DEBUG LOG
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

                var userId = jwtToken.Claims.First(x => x.Type == "nameid").Value;
                var email = jwtToken.Claims.First(x => x.Type == "email").Value;
                var role = jwtToken.Claims.First(x => x.Type == "role").Value;

                // 🔥 DEBUG LOG
                _logger.LogInformation("✅ JWT Claims extraídos: UserId={UserId}, Email={Email}, Role={Role}", userId, email, role);

                context.Items["User"] = new JwtMemberDto
                {
                    UserId = int.Parse(userId),
                    Email = email,
                    Role = role
                };
            }
            catch (Exception ex)
            {
                // 🔥 LOG DE ERRO
                _logger.LogError(ex, "❌ Erro ao processar token JWT");
            }
        }
    }
}
