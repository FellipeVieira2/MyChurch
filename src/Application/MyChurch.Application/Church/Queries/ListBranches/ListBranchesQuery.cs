using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Queries.ListBranches
{
    public class ListBranchesQuery : JwtMemberDto, IRequest<List<BranchListItemDto>>
    {
        public int ParentChurchId { get; set; }
    }

    public class BranchListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Phone { get; set; } = string.Empty;
        public DateTime Created { get; set; }
    }

    public class ListBranchesQueryHandler : IRequestHandler<ListBranchesQuery, List<BranchListItemDto>>
    {
        private readonly IUnitOfWork _uow;

        public ListBranchesQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<BranchListItemDto>> Handle(ListBranchesQuery request, CancellationToken cancellationToken)
        {
            var member = await _uow.Members.Query().AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null)
                ValidationException.ThrowException("Member", "Authenticated member does not exist.");

            if (member.Role != UserRole.Admin)
                ValidationException.ThrowException("Member", "Only admins can list branches.");

            if (member.ChurchId != request.ParentChurchId)
                ValidationException.ThrowException("Church", "You can only list branches for your own church.");

            var list = await _uow.Churchs.Query()
                .AsNoTracking()
                .Where(c => c.ParentChurchId == request.ParentChurchId)
                .OrderByDescending(c => c.Created)
                .Select(c => new BranchListItemDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Phone = c.Phone,
                    Created = c.Created
                })
                .ToListAsync(cancellationToken);

            return list;
        }
    }
}
