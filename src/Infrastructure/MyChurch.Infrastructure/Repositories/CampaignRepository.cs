using MyChurch.Domain.Entities;
using MyChurch.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Infrastructure.Repositories
{
    public class CampaignRepository : GenericRepository<Campaign>, ICampaignRepository
    {
        public CampaignRepository(MyChurchDbContext context) : base(context) { }
    }
}
