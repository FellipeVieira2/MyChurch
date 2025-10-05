using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Infrastructure;

namespace MyChurch.Application.Ministries.Queries.GetMinistriesByChurch
{
    public class GetMinistriesByChurchQueryHandler : IRequestHandler<GetMinistriesByChurchQuery, List<MinistryDto>>
    {
        private readonly MyChurchDbContext _context;

        public GetMinistriesByChurchQueryHandler(MyChurchDbContext context)
        {
            _context = context;
        }

        public async Task<List<MinistryDto>> Handle(GetMinistriesByChurchQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Set<Domain.Entities.Ministry>()
                .Include(m => m.Leader)
                .Include(m => m.MinistryMembers)
                .Where(m => m.ChurchId == request.ChurchId);

            if (request.IsActive.HasValue)
            {
                query = query.Where(m => m.IsActive == request.IsActive.Value);
            }

            var ministries = await query
                .OrderBy(m => m.Name)
                .ToListAsync(cancellationToken);

            return ministries.Select(m => MinistryDto.FromEntity(m)).ToList();
        }
    }
}
