using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Infrastructure;

namespace MyChurch.Application.Ministries.Queries.GetMinistryById
{
    public class GetMinistryByIdQueryHandler : IRequestHandler<GetMinistryByIdQuery, MinistryDto?>
    {
        private readonly MyChurchDbContext _context;

        public GetMinistryByIdQueryHandler(MyChurchDbContext context)
        {
            _context = context;
        }

        public async Task<MinistryDto?> Handle(GetMinistryByIdQuery request, CancellationToken cancellationToken)
        {
            var ministry = await _context.Set<Domain.Entities.Ministry>()
                .Include(m => m.Leader)
                .Include(m => m.MinistryMembers)
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            return ministry == null ? null : MinistryDto.FromEntity(ministry);
        }
    }
}
