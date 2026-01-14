using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyChurch.Application.PlatformUsers.Commands.GoogleLoginPlatformUser;
using MyChurch.Application.PlatformUsers.Commands.LoginPlatformUser;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/platform/auth")]
    public class PlatformAuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public PlatformAuthController(IMediator mediator, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _mediator = mediator;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginPlatformUserCommand command)
        {
            var dto = await _mediator.Send(command);
            return Ok(dto);
        }

        public class PlatformGoogleLoginRequest
        {
            public string IdToken { get; set; } = string.Empty;
        }

        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<IActionResult> Google([FromBody] PlatformGoogleLoginRequest body)
        {
            if (string.IsNullOrWhiteSpace(body?.IdToken))
                return BadRequest(new { error = "missing_id_token" });

            var principal = await ValidateGoogleIdTokenAsync(body.IdToken);
            if (principal == null)
                return Unauthorized(new { error = "invalid_google_token" });

            var email = principal.FindFirst(ClaimTypes.Email)?.Value
                        ?? principal.FindFirst("email")?.Value;

            var emailVerified = string.Equals(
                principal.FindFirst("email_verified")?.Value,
                "true",
                StringComparison.OrdinalIgnoreCase);

            var name = principal.FindFirst("name")?.Value ?? string.Empty;
            var picture = principal.FindFirst("picture")?.Value;

            if (string.IsNullOrWhiteSpace(email) || !emailVerified)
                return Unauthorized(new { error = "email_not_verified" });

            var cmd = new GoogleLoginPlatformUserCommand
            {
                Email = email,
                EmailVerified = emailVerified,
                Name = name,
                Picture = picture
            };

            var dto = await _mediator.Send(cmd);
            return Ok(dto);
        }

        private async Task<ClaimsPrincipal?> ValidateGoogleIdTokenAsync(string idToken)
        {
            var audiences = _configuration.GetSection("GoogleAuth:ClientIds").Get<string[]>()
                            ?? (_configuration["GoogleAuth:ClientId"] is string single ? new[] { single } : Array.Empty<string>());

            var http = _httpClientFactory.CreateClient();
            var jwksJson = await http.GetStringAsync("https://www.googleapis.com/oauth2/v3/certs");
            var jwks = new JsonWebKeySet(jwksJson);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = new[] { "accounts.google.com", "https://accounts.google.com" },
                ValidateAudience = audiences.Length > 0,
                ValidAudiences = audiences.Length > 0 ? audiences : null,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = jwks.Keys,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var principal = handler.ValidateToken(idToken, tokenValidationParameters, out var securityToken);
                if (securityToken is JwtSecurityToken jwt &&
                    (jwt.Issuer == "accounts.google.com" || jwt.Issuer == "https://accounts.google.com"))
                {
                    return principal;
                }
            }
            catch
            {
                // ignore
            }

            return null;
        }
    }
}
