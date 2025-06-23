using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities.Bible;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChurch.Infrastructure.Repositories
{
    public class BibleReadingPlanRepository : GenericRepository<BibleReadingPlan>, IBibleReadingPlanRepository
    {
        public BibleReadingPlanRepository(MyChurchDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BibleReadingPlan>> GetAllDefaultAndPublicPlansAsync(int? churchId = null)
        {
            var query = _context.BibleReadingPlans
                .Include(p => p.BibleReadingPlanStages)
                .Where(p => p.IsDefault || p.IsPublic);

            if (churchId.HasValue)
            {
                // Adiciona os planos específicos da igreja aos planos padrão e públicos
                query = query.Union(_context.BibleReadingPlans
                    .Include(p => p.BibleReadingPlanStages)
                    .Where(p => p.ChurchId == churchId));
            }

            return await query
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<BibleReadingPlan> GetPlanWithStagesAsync(int planId)
        {
            return await _context.BibleReadingPlans
                .Include(p => p.BibleReadingPlanStages.OrderBy(s => s.Order))
                .FirstOrDefaultAsync(p => p.Id == planId);
        }
    }
}