using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class FamilyInvitationRepository : GenericRepository<FamilyInvitation>, IFamilyInvitationRepository
    {
        public FamilyInvitationRepository(MyChurchDbContext context) : base(context) { }
    }
}
