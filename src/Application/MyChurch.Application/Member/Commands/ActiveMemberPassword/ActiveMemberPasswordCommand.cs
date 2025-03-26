using System.Text.Json.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MyChurch.Domain.Contracts;
using MyChurch.Infrastructure.Utils.Extensions;
using MyChurch.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Application.Member.Commands.ActiveMemberPassword
{
    public class ActiveMemberPasswordCommand : IRequest<Unit>
    {
        [JsonIgnore]
        public string? Hash { get; set; }
        public string Password { get; set; }
    }

    public class ActiveMemberPasswordCommandHandler : IRequestHandler<ActiveMemberPasswordCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ActiveMemberPasswordCommandHandler> _logger;

        public ActiveMemberPasswordCommandHandler(IUnitOfWork unitOfWork, ILogger<ActiveMemberPasswordCommandHandler> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(ActiveMemberPasswordCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.PasswordHash == request.Hash);
            if (member is null)
            {
                _logger.LogWarning("Invalid hash: {Hash}", request.Hash);
                ValidationException.ThrowException("Password", $"Invalid hash: {request.Hash}");
            }

            // Criptografar a nova senha usando o hash e o appHash
            var encryptedPassword = request.Password.Encrypt(request.Hash.ToString());
            member.Password = encryptedPassword;

            _unitOfWork.Members.Update(member);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Password updated for member with ID: {MemberId}", member.Id);

            return Unit.Value;
        }
    }
}

