using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Group.Queries
{
    public class GetGroupDetailsQuery : JwtMemberDto, IRequest<GroupDetailsDto>
    {
        public int GroupId { get; set; }
    }
    public class GetGroupDetailsQueryHandler : IRequestHandler<GetGroupDetailsQuery, GroupDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetGroupDetailsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GroupDetailsDto> Handle(GetGroupDetailsQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (member == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
            var group = await _unitOfWork.Groups.Query().FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);
            if (group == null)
                ValidationException.ThrowException("Group", "Grupo não encontrado.");
            // Montar DTO conforme necessidade
            return new GroupDetailsDto
            {
                GroupId = group.Id,
                Name = group.Name,
                Description = group.Description,
                Type = group.Type.ToString(),
                LeaderId = group.LeaderId,
                AcceptsNewMembers = group.AcceptsNewMembers,
                CoverImageUrl = group.CoverImageUrl
            };
        }
    }
}
