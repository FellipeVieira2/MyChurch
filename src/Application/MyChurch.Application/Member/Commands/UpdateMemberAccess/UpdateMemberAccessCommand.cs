using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Member.Commands.UpdateMemberAccess
{
    public class UpdateMemberAccessCommand : JwtMemberDto, IRequest<MemberDto>
    {
        [JsonIgnore]
        public int MemberId { get; set; }

        public bool? IsActive { get; set; }

        public UserRole? Role { get; set; }
    }

    public class UpdateMemberAccessCommandHandler : IRequestHandler<UpdateMemberAccessCommand, MemberDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateMemberAccessCommandHandler> _logger;

        public UpdateMemberAccessCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateMemberAccessCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<MemberDto> Handle(UpdateMemberAccessCommand request, CancellationToken cancellationToken)
        {
            var admin = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (admin == null)
                ValidationException.ThrowException("Member", "Authenticated member does not exist.");

            if (admin.Role != UserRole.Admin)
                ValidationException.ThrowException("Member", "Only admins can update member access.");

            var member = await _unitOfWork.Members.Query()
                .Include(m => m.Documents)
                .Include(m => m.Address)
                .FirstOrDefaultAsync(m => m.Id == request.MemberId && m.ChurchId == admin.ChurchId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Member not found or does not belong to your church.");

            // Admin não pode desativar a própria conta via endpoint
            if (member.Id == admin.Id && request.IsActive.HasValue && request.IsActive.Value == false)
                ValidationException.ThrowException("Member", "You cannot deactivate your own account.");

            if (request.IsActive.HasValue)
                member.IsActive = request.IsActive.Value;

            if (request.Role.HasValue)
            {
                // Evitar rebaixar o último admin da igreja
                if (member.Role == UserRole.Admin && request.Role.Value != UserRole.Admin)
                {
                    var otherAdminsCount = await _unitOfWork.Members.Query()
                        .CountAsync(m => m.ChurchId == admin.ChurchId && m.Role == UserRole.Admin && m.Id != member.Id, cancellationToken);

                    if (otherAdminsCount == 0)
                        ValidationException.ThrowException("Member", "You cannot remove admin role from the last admin of the church.");
                }

                member.Role = request.Role.Value;
            }

            member.Updated = DateTime.UtcNow;

            _unitOfWork.Members.Update(member);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Member access updated. MemberId: {MemberId}, IsActive: {IsActive}, Role: {Role}", member.Id, member.IsActive, member.Role);

            return MemberDto.New(member);
        }
    }
}
