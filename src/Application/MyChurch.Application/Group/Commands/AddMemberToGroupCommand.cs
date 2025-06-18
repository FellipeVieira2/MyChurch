using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.Group.Commands
{
    public class AddMemberToGroupCommand : JwtMemberDto, IRequest<bool>
    {
        [JsonIgnore]
        public int GroupId { get; set; }
        public int MemberId { get; set; }
        public string RoleInGroup { get; set; } = "Membro";
    }

    public class AddMemberToGroupCommandHandler : IRequestHandler<AddMemberToGroupCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddMemberToGroupCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(AddMemberToGroupCommand request, CancellationToken cancellationToken)
        {
            var leader = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (leader == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
            var group = await _unitOfWork.Groups.Query().FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);
            if (group == null)
                ValidationException.ThrowException("Group", "Grupo não encontrado.");
            if (group.LeaderId != leader.Id)
                ValidationException.ThrowException("Group", "Apenas o líder do grupo pode adicionar membros.");
            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.MemberId, cancellationToken);
            if (member == null)
                ValidationException.ThrowException("Member", "Membro a ser adicionado não encontrado.");
            var exists = await _unitOfWork.GroupMembers.Query().AnyAsync(x => x.GroupId == group.Id && x.MemberId == member.Id, cancellationToken);
            if (exists)
                ValidationException.ThrowException("GroupMember", "Membro já faz parte do grupo.");
            var groupMember = new GroupMember(group.Id, member.Id, request.RoleInGroup);
            _unitOfWork.GroupMembers.Create(groupMember);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
