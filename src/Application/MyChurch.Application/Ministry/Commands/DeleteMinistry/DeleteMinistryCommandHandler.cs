using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Infrastructure;

namespace MyChurch.Application.Ministries.Commands.DeleteMinistry
{
    public class DeleteMinistryCommandHandler : IRequestHandler<DeleteMinistryCommand, bool>
    {
        private readonly MyChurchDbContext _context;

        public DeleteMinistryCommandHandler(MyChurchDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteMinistryCommand request, CancellationToken cancellationToken)
        {
            var ministry = await _context.Set<Domain.Entities.Ministry>()
                .Include(m => m.MinistryMembers)
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (ministry == null)
                return false;

            // Remover todos os membros do ministério
            _context.Set<Domain.Entities.MinistryMember>().RemoveRange(ministry.MinistryMembers);
            
            // Remover o ministério
            _context.Set<Domain.Entities.Ministry>().Remove(ministry);
            
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
