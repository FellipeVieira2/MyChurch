using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.PlatformUsers.Commands.GoogleLoginPlatformUser
{
    public class GoogleLoginPlatformUserCommand : IRequest<GoogleLoginPlatformUserResultDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Picture { get; set; }
        public bool EmailVerified { get; set; }
    }

    public class GoogleLoginPlatformUserResultDto
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = UserRole.PlatformAdmin.ToString();
        public LoginPlatformUser.PlatformUserDto User { get; set; } = new();
        public bool Created { get; set; }
    }

    public class GoogleLoginPlatformUserCommandHandler : IRequestHandler<GoogleLoginPlatformUserCommand, GoogleLoginPlatformUserResultDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;

        public GoogleLoginPlatformUserCommandHandler(IUnitOfWork uow, IConfiguration configuration)
        {
            _uow = uow;
            _configuration = configuration;
        }

        public async Task<GoogleLoginPlatformUserResultDto> Handle(GoogleLoginPlatformUserCommand request, CancellationToken cancellationToken)
        {
            var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email) || !request.EmailVerified)
                throw new UnauthorizedAccessException("email_not_verified");

            var user = await _uow.PlatformUsers.Query().FirstOrDefaultAsync(u => u.Email.ToLower() == email, cancellationToken);
            var created = false;

            if (user == null)
            {
                user = new MyChurch.Domain.Entities.PlatformUser
                {
                    Name = string.IsNullOrWhiteSpace(request.Name) ? email : request.Name,
                    Email = email,
                    Photo = request.Picture,
                    Role = UserRole.PlatformAdmin,
                    IsActive = true,
                    Created = DateTime.UtcNow
                };

                await _uow.PlatformUsers.Create(user);
                await _uow.CommitAsync();
                created = true;
            }

            if (!user.IsActive)
                throw new UnauthorizedAccessException("account_inactive");

            if (string.IsNullOrWhiteSpace(user.Photo) && !string.IsNullOrWhiteSpace(request.Picture))
            {
                user.Photo = request.Picture;
                user.Updated = DateTime.UtcNow;
                _uow.PlatformUsers.Update(user);
                await _uow.CommitAsync();
            }

            return new GoogleLoginPlatformUserResultDto
            {
                Token = GenerateJwtToken(user),
                Role = user.Role.ToString(),
                User = LoginPlatformUser.PlatformUserDto.New(user),
                Created = created
            };
        }

        private string GenerateJwtToken(MyChurch.Domain.Entities.PlatformUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var claims = new List<Claim>
            {
                new("platform_user_id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
