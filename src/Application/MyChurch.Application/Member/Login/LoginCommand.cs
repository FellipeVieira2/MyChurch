using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
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
            // Verifica se o identifier está no formato de CPF e remove pontos e traços
            string normalizedIdentifier = NormalizeIdentifier(request.Identifier);
            string originalIdentifier = request.Identifier;

            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(x => 
                    x.Email == originalIdentifier || 
                    x.Phone == originalIdentifier || 
                    x.Documents.Any(d => d.Number == normalizedIdentifier));

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
                Role = member.Role.ToString(),
                Member = MemberDto.New(member)
            };
        }

        private string NormalizeIdentifier(string identifier)
        {
            // Se o formato parecer um CPF (com pontos e traços), remova esses caracteres
            if (Regex.IsMatch(identifier, @"^\d{3}\.?\d{3}\.?\d{3}\-?\d{2}$"))
            {
                return Regex.Replace(identifier, @"[^\d]", "");
            }
            
            // Para outros formatos (como e-mail), retorne o identificador original
            return identifier;
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
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}