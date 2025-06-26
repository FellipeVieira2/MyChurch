using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class EngagementEventRepository : GenericRepository<EngagementEvent>, IEngagementEventRepository
    {
        private readonly MyChurchDbContext _context;
        public EngagementEventRepository(MyChurchDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> CalculateScoreForMemberAsync(int memberId, int days)
        {
            var dateLimit = DateTime.UtcNow.AddDays(-days);
            return await _context.EngagementEvents
                .Where(e => e.MemberId == memberId && e.CreatedAt >= dateLimit)
                .SumAsync(e => e.Points);
        }
    }
}
