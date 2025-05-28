using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure.Utils.Extensions;

namespace MyChurch.Application.Member.Commands.Login
{
    public class LoginCommand : IRequest<LoginDto>
    {
        public string Identifier { get; set; } // Pode ser email ou telefone
        public string Password { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(IUnitOfWork unitOfWork, ILogger<LoginCommandHandler> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<LoginDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(x => x.Email == request.Identifier || x.Phone == request.Identifier || x.Document == request.Identifier);

            if (member is null)
            {
                _logger.LogWarning("Invalid identifier: {Identifier}", request.Identifier);
                ValidationException.ThrowException("Login", "Invalid email/phone or password.");
            }

            if (string.IsNullOrEmpty(member.Password))
            {
                _logger.LogWarning("Account not activated: {Identifier}", request.Identifier);
                ValidationException.ThrowException("Login", "Account not activated. Please activate your account.");
            }

            var encryptedPassword = request.Password.Encrypt(member.PasswordHash);

            if (member.Password != encryptedPassword)
            {
                _logger.LogWarning("Invalid password for identifier: {Identifier}", request.Identifier);
                ValidationException.ThrowException("Login", "Invalid email/phone or password.");
            }

            var token = GenerateJwtToken(member);

            return new LoginDto
            {
                Token = token,
                Role = member.Role.ToString()
            };
        }

        private string GenerateJwtToken(Domain.Entities.Member member)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, member.Id.ToString()),
                    new Claim(ClaimTypes.Email, member.Email),
                    new Claim(ClaimTypes.Role, member.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}