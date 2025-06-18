using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.Group.Commands
{
    public class RegisterGroupAttendanceCommand : JwtMemberDto, IRequest<bool>
    {
        [JsonIgnore]
        public int GroupId { get; set; }
        public int MeetingEventId { get; set; }
        public List<MemberAttendanceDto> Attendances { get; set; }
    }
    public class MemberAttendanceDto
    {
        public int MemberId { get; set; }
        public string Status { get; set; } // "Presente", "Ausente"
        public string? Justification { get; set; }
    }
    public class RegisterGroupAttendanceCommandHandler : IRequestHandler<RegisterGroupAttendanceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RegisterGroupAttendanceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(RegisterGroupAttendanceCommand request, CancellationToken cancellationToken)
        {
            var leader = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (leader == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
            var group = await _unitOfWork.Groups.Query().FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);
            if (group == null)
                ValidationException.ThrowException("Group", "Grupo não encontrado.");
            if (group.LeaderId != leader.Id)
                ValidationException.ThrowException("Group", "Apenas o líder do grupo pode registrar presença.");
            // Aqui você criaria e salvaria os registros de presença na tabela TB_GROUP_ATTENDANCES
            // Exemplo:
            // foreach (var att in request.Attendances) { ... }
            // _unitOfWork.GroupAttendances.AddAsync(...);
            // await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
