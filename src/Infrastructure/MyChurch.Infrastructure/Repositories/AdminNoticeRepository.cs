using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Infrastructure.Repositories
{
    public class AdminNoticeRepository : GenericRepository<AdminNotice>, IAdminNoticeRepository
    {
        public AdminNoticeRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
