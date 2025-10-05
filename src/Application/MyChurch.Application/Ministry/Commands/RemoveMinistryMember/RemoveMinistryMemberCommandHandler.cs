using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Infrastructure;

namespace MyChurch.Application.Ministries.Commands.RemoveMinistryMember
{
    public class RemoveMinistryMemberCommandHandler : IRequestHandler<RemoveMinistryMemberCommand, bool>
    {
        private readonly MyChurchDbContext _context;

        public RemoveMinistryMemberCommandHandler(MyChurchDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(RemoveMinistryMemberCommand request, CancellationToken cancellationToken)
        {
            var ministryMember = await _context.Set<Domain.Entities.MinistryMember>()
                .FirstOrDefaultAsync(mm => mm.MinistryId == request.MinistryId && mm.MemberId == request.MemberId, cancellationToken);

            if (ministryMember == null)
                return false;

            // Marcar como inativo em vez de remover (para manter histórico)
            ministryMember.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
