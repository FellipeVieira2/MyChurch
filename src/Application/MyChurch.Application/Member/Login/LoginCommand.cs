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
using MyChurch.Domain.Services;

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
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDocumentValidator _documentValidator;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork, 
            ILogger<LoginCommandHandler> logger, 
            IConfiguration configuration,
            IPasswordHasher passwordHasher,
            IDocumentValidator documentValidator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _documentValidator = documentValidator;
        }

        public async Task<LoginDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Normalizar o identifier usando o DocumentValidator
            string normalizedIdentifier = _documentValidator.RemoveFormatting(request.Identifier);
            string originalIdentifier = request.Identifier;

            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(x => 
                    x.Email == originalIdentifier || 
                    x.Phone == originalIdentifier || 
                    x.Documents.Any(d => d.Number == normalizedIdentifier), cancellationToken);

            if (member is null)
            {
                _logger.LogWarning("Invalid identifier: {Identifier}", request.Identifier);
                ValidationException.ThrowException("invalid_credentials", "Invalid email/phone or password.");
            }

            // 🔐 VERIFICAÇÃO DE SEGURANÇA: Apenas PasswordHash deve existir
            if (string.IsNullOrEmpty(member.PasswordHash))
            {
                _logger.LogWarning("Account not activated: {Identifier}", request.Identifier);
                ValidationException.ThrowException("account_not_activated", "Account not activated. Please activate your account.");
            }

            if (member.PendingApproval)
            {
                _logger.LogWarning("Account pending approval: {Identifier}", request.Identifier);
                ValidationException.ThrowException("pending_approval", "Account pending approval. Please wait for admin approval.");
            }

            if (!member.IsActive)
            {
                _logger.LogWarning("Account inactive: {Identifier}", request.Identifier);
                ValidationException.ThrowException("account_inactive", "Account inactive. Contact your administrator.");
            }

            // 🔐 SEGURANÇA: Verificar senha usando BCrypt
            if (!_passwordHasher.VerifyPassword(request.Password, member.PasswordHash))
            {
                _logger.LogWarning("Invalid password for identifier: {Identifier}", request.Identifier);
                ValidationException.ThrowException("invalid_credentials", "Invalid email/phone or password.");
            }

            var token = GenerateJwtToken(member);

            return new LoginDto
            {
                Token = token,
                Role = member.Role.ToString(),
                Member = MemberDto.New(member)
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