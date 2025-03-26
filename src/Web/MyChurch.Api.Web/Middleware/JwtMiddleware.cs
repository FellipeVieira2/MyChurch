using MyChurch.Application.Dtos;
using System.IdentityModel.Tokens.Jwt;


namespace MyChurch.Api.Web.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                AttachUserToContext(context, token);
            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var userId = jwtToken.Claims.First(x => x.Type == "nameid").Value;
            var email = jwtToken.Claims.First(x => x.Type == "email").Value;
            var role = jwtToken.Claims.First(x => x.Type == "role").Value;

            context.Items["User"] = new JwtMemberDto
            {
                UserId = int.Parse(userId),
                Email = email,
                Role = role
            };
        }
    }
}
