using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Group.Commands
{
    public class CreateGroupCommand : JwtMemberDto, IRequest<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public GroupType Type { get; set; }
        public bool AcceptsNewMembers { get; set; }
        public string? CoverImageUrl { get; set; }
    }

    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateGroupCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var leader = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (leader == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
            var group = new MyChurch.Domain.Entities.Group(
                leader.ChurchId,
                request.Name,
                request.Description,
                request.Type,
                leader.Id,
                request.AcceptsNewMembers,
                request.CoverImageUrl
            );
            _unitOfWork.Groups.Create(group);
            var groupMember = new MyChurch.Domain.Entities.GroupMember(group.Id, leader.Id, "Líder");
            _unitOfWork.GroupMembers.Create(groupMember);
            await _unitOfWork.CommitAsync();
            return group.Id;
        }
    }
}
