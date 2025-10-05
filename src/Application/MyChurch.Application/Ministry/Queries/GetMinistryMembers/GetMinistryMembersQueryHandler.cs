using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Infrastructure;

namespace MyChurch.Application.Ministries.Queries.GetMinistryMembers
{
    public class GetMinistryMembersQueryHandler : IRequestHandler<GetMinistryMembersQuery, List<MinistryMemberDto>>
    {
        private readonly MyChurchDbContext _context;

        public GetMinistryMembersQueryHandler(MyChurchDbContext context)
        {
            _context = context;
        }

        public async Task<List<MinistryMemberDto>> Handle(GetMinistryMembersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Set<Domain.Entities.MinistryMember>()
                .Include(mm => mm.Ministry)
                .Include(mm => mm.Member)
                .Where(mm => mm.MinistryId == request.MinistryId);

            if (request.IsActive.HasValue)
            {
                query = query.Where(mm => mm.IsActive == request.IsActive.Value);
            }

            var ministryMembers = await query
                .OrderBy(mm => mm.Member.Name)
                .ToListAsync(cancellationToken);

            return ministryMembers.Select(MinistryMemberDto.FromEntity).ToList();
        }
    }
}
