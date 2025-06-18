using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.Group.Commands
{
    public class UpdateGroupCommand : JwtMemberDto, IRequest<bool>
    {
        [JsonIgnore]
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool AcceptsNewMembers { get; set; }
        public string? CoverImageUrl { get; set; }
    }

    public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateGroupCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var leader = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (leader == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
            var group = await _unitOfWork.Groups.Query().FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);
            if (group == null)
                ValidationException.ThrowException("Group", "Grupo não encontrado.");
            if (group.LeaderId != leader.Id)
                ValidationException.ThrowException("Group", "Apenas o líder do grupo pode atualizar o grupo.");
            group.UpdateDetails(request.Name, request.Description, request.AcceptsNewMembers, request.CoverImageUrl);
            _unitOfWork.Groups.Update(group);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
