using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Services;

namespace MyChurch.Application.PlatformUsers.Commands.LoginPlatformUser
{
    public class LoginPlatformUserCommand : IRequest<LoginPlatformUserResultDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginPlatformUserResultDto
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = UserRole.PlatformAdmin.ToString();
        public PlatformUserDto User { get; set; } = new();
    }

    public class PlatformUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Photo { get; set; }
        public string Role { get; set; } = UserRole.PlatformAdmin.ToString();

        public static PlatformUserDto New(MyChurch.Domain.Entities.PlatformUser user)
            => new()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Photo = user.Photo,
                Role = user.Role.ToString()
            };
    }

    public class LoginPlatformUserCommandHandler : IRequestHandler<LoginPlatformUserCommand, LoginPlatformUserResultDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginPlatformUserCommandHandler> _logger;
        private readonly IPasswordHasher _passwordHasher;

        public LoginPlatformUserCommandHandler(IUnitOfWork uow, IConfiguration configuration, ILogger<LoginPlatformUserCommandHandler> logger, IPasswordHasher passwordHasher)
        {
            _uow = uow;
            _configuration = configuration;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginPlatformUserResultDto> Handle(LoginPlatformUserCommand request, CancellationToken cancellationToken)
        {
            var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
                ValidationException.ThrowException("invalid_credentials", "Invalid email or password.");

            var user = await _uow.PlatformUsers.Query().FirstOrDefaultAsync(u => u.Email.ToLower() == email, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Platform login failed (user not found): {Email}", email);
                ValidationException.ThrowException("invalid_credentials", "Invalid email or password.");
            }

            if (!user.IsActive)
                ValidationException.ThrowException("account_inactive", "Account inactive.");

            if (string.IsNullOrWhiteSpace(user.PasswordHash) || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Platform login failed (invalid password): {Email}", email);
                ValidationException.ThrowException("invalid_credentials", "Invalid email or password.");
            }

            return new LoginPlatformUserResultDto
            {
                Token = GenerateJwtToken(user),
                Role = user.Role.ToString(),
                User = PlatformUserDto.New(user)
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
