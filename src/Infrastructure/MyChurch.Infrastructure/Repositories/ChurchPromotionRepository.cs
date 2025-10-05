using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class ChurchPromotionRepository : GenericRepository<ChurchPromotion>, IChurchPromotionRepository
    {
        public ChurchPromotionRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
