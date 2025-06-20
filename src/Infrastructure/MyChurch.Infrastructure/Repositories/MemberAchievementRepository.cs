using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class MemberAchievementRepository : GenericRepository<MemberAchievement>, IMemberAchievementRepository
    {
        public MemberAchievementRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
