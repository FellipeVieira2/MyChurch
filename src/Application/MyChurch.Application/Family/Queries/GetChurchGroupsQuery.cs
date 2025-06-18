using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;
using MyChurch.Application.Dtos;
using System.Collections.Generic;
using MyChurch.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Application.Family.Queries
{
    public class GetChurchGroupsQuery : JwtMemberDto, IRequest<List<GroupListItemDto>>
    {
    }

    public class GetChurchGroupsQueryHandler : IRequestHandler<GetChurchGroupsQuery, List<GroupListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetChurchGroupsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<GroupListItemDto>> Handle(GetChurchGroupsQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (member == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
            var groups = await _unitOfWork.Groups.Query().Where(g => g.ChurchId == member.ChurchId && g.IsActive).ToListAsync(cancellationToken);
            return groups.Select(g => new GroupListItemDto
            {
                GroupId = g.Id,
                Name = g.Name,
                Description = g.Description,
                Type = g.Type.ToString(),
                LeaderId = g.LeaderId
            }).ToList();
        }
    }
}
