using MediatR;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Application.Family.Commands
{
    public class AssignChildToGroupCommand : JwtMemberDto, IRequest<bool>
    {
        public int ChildId { get; set; }
        public int GroupId { get; set; }
    }

    public class AssignChildToGroupCommandHandler : IRequestHandler<AssignChildToGroupCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AssignChildToGroupCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(AssignChildToGroupCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (member == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
            var child = await _unitOfWork.Children.Query().FirstOrDefaultAsync(x => x.Id == request.ChildId, cancellationToken);
            if (child == null)
                ValidationException.ThrowException("Child", "Filho não encontrado.");
            if (member.FamilyId == null || child.FamilyId != member.FamilyId)
                ValidationException.ThrowException("Family", "Você só pode associar filhos da sua família.");
            var group = await _unitOfWork.Groups.Query().FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);
            if (group == null)
                ValidationException.ThrowException("Group", "Grupo não encontrado.");
            var exists = await _unitOfWork.ChildGroupAssignments.Query().AnyAsync(x => x.ChildId == request.ChildId && x.GroupId == request.GroupId, cancellationToken);
            if (exists)
                ValidationException.ThrowException("ChildGroupAssignment", "Filho já está associado a este grupo.");
            var assignment = new ChildGroupAssignment
            {
                ChildId = request.ChildId,
                GroupId = request.GroupId
            };
            _unitOfWork.ChildGroupAssignments.Create(assignment);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
