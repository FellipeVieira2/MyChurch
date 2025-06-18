using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Group.Queries
{
    public class ListGroupResourcesQuery : JwtMemberDto, IRequest<List<GroupResourceDto>>
    {
        public int GroupId { get; set; }
    }
    public class ListGroupResourcesQueryHandler : IRequestHandler<ListGroupResourcesQuery, List<GroupResourceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ListGroupResourcesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<GroupResourceDto>> Handle(ListGroupResourcesQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (member == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
            var group = await _unitOfWork.Groups.Query().FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);
            if (group == null)
                ValidationException.ThrowException("Group", "Grupo não encontrado.");
            var resources = await _unitOfWork.GroupResources.Query().Where(x => x.GroupId == group.Id).ToListAsync(cancellationToken);
            return resources.Select(r => new GroupResourceDto
            {
                Id = r.Id,
                GroupId = r.GroupId,
                Title = r.Title,
                Description = r.Description,
                FileUrl = r.FileUrl,
                UploadedByMemberId = r.UploadedByMemberId,
                UploadedAt = r.UploadedAt
            }).ToList();
        }
    }
}
