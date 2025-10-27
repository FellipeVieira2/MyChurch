using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyChurch.Application.Dtos;
using MyChurch.Application.Member.Commands.Login;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace MyChurch.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IMediator mediator, IUnitOfWork unitOfWork, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var dto = await _mediator.Send(command);
            return Ok(new { Token = dto.Token, Role = dto.Role, Member = dto.Member });
        }

        public class GoogleLoginRequest
        {
            public string IdToken { get; set; } = string.Empty;
            public int? ChurchId { get; set; }
        }

        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest body)
        {
            if (string.IsNullOrWhiteSpace(body?.IdToken))
                return BadRequest(new { error = "missing_id_token" });

            var audiences = _configuration.GetSection("GoogleAuth:ClientIds").Get<string[]>()
                            ?? (_configuration["GoogleAuth:ClientId"] is string single ? new[] { single } : Array.Empty<string>());
            if (audiences.Length == 0)
            {
                return StatusCode(500, new { error = "google_not_configured" });
            }

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

            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(m => m.Email == email);

            if (member == null)
            {
                if (!body.ChurchId.HasValue || body.ChurchId.Value <= 0)
                {
                    var visitor = await _unitOfWork.Visitors.Query().FirstOrDefaultAsync(v => v.Email == email);
                    if (visitor == null)
                    {
                        visitor = new MyChurch.Domain.Entities.Visitor
                        {
                            Name = string.IsNullOrWhiteSpace(name) ? email : name,
                            Email = email,
                            CreatedAt = DateTime.UtcNow,
                            Status = VisitorStatus.New,
                            Score = 0,
                            NeedsFollowUp = false
                        };
                        _unitOfWork.Visitors.Create(visitor);
                        await _unitOfWork.CommitAsync();
                    }

                    // Emitir JWT de visitante com claims básicas
                    var token = GenerateVisitorJwtToken(visitor);
                    return Ok(new
                    {
                        Token = token,
                        Role = "Visitor",
                        Visitor = new { visitor.Id, visitor.Name, visitor.Email }
                    });
                }

                var churchExists = await _unitOfWork.Churchs.Query().AnyAsync(c => c.Id == body.ChurchId.Value);
                if (!churchExists)
                {
                    return NotFound(new { error = "church_not_found", churchId = body.ChurchId });
                }

                member = new MyChurch.Domain.Entities.Member
                {
                    Name = string.IsNullOrWhiteSpace(name) ? email : name,
                    Email = email,
                    Photo = picture,
                    ChurchId = body.ChurchId.Value,
                    Role = UserRole.Member,
                    Created = DateTime.UtcNow,
                    IsActive = true,
                    PendingApproval = false,
                    BirthDate = DateTime.UtcNow.Date
                };

                _unitOfWork.Members.Create(member);
                await _unitOfWork.CommitAsync();
            }

            if (member.PendingApproval)
            {
                return Unauthorized(new { error = "pending_approval" });
            }
            if (!member.IsActive)
            {
                return Unauthorized(new { error = "account_inactive" });
            }

            if (string.IsNullOrEmpty(member.Photo) && !string.IsNullOrEmpty(picture))
            {
                member.Photo = picture;
                await _unitOfWork.CommitAsync();
            }

            var memberToken = GenerateJwtToken(member);
            var dto = new LoginDto
            {
                Token = memberToken,
                Role = member.Role.ToString(),
                Member = MemberDto.New(member)
            };
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
                ValidateAudience = true,
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

        private string GenerateVisitorJwtToken(MyChurch.Domain.Entities.Visitor visitor)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var claims = new List<Claim>
            {
                new Claim("visitor_id", visitor.Id.ToString()),
                new Claim(ClaimTypes.Email, visitor.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, "Visitor")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // visitante pode durar mais tempo
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateJwtToken(MyChurch.Domain.Entities.Member member)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, member.Id.ToString()),
                    new Claim(ClaimTypes.Email, member.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, member.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}