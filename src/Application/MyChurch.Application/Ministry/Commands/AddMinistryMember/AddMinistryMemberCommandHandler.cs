using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Infrastructure;

namespace MyChurch.Application.Ministries.Commands.AddMinistryMember
{
    public class AddMinistryMemberCommandHandler : IRequestHandler<AddMinistryMemberCommand, MinistryMemberDto>
    {
        private readonly MyChurchDbContext _context;

        public AddMinistryMemberCommandHandler(MyChurchDbContext context)
        {
            _context = context;
        }

        public async Task<MinistryMemberDto> Handle(AddMinistryMemberCommand request, CancellationToken cancellationToken)
        {
            // Validar se o ministério existe
            var ministry = await _context.Set<Domain.Entities.Ministry>()
                .FirstOrDefaultAsync(m => m.Id == request.MinistryId, cancellationToken);
            
            if (ministry == null)
                throw new ArgumentException("Ministério não encontrado");

            // Validar se o membro existe e pertence à mesma igreja
            var member = await _context.Set<Domain.Entities.Member>()
                .FirstOrDefaultAsync(m => m.Id == request.MemberId && m.ChurchId == ministry.ChurchId, cancellationToken);
            
            if (member == null)
                throw new ArgumentException("Membro não encontrado ou não pertence à igreja");

            // Verificar se o membro já está no ministério
            var existingMembership = await _context.Set<Domain.Entities.MinistryMember>()
                .FirstOrDefaultAsync(mm => mm.MinistryId == request.MinistryId && mm.MemberId == request.MemberId, cancellationToken);

            if (existingMembership != null)
            {
                // Se já existe mas está inativo, reativar
                if (!existingMembership.IsActive)
                {
                    existingMembership.IsActive = true;
                    existingMembership.Role = request.Role;
                    existingMembership.Notes = request.Notes;
                    existingMembership.JoinedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync(cancellationToken);
                    
                    await _context.Entry(existingMembership)
                        .Reference(mm => mm.Ministry)
                        .LoadAsync(cancellationToken);
                    await _context.Entry(existingMembership)
                        .Reference(mm => mm.Member)
                        .LoadAsync(cancellationToken);
                    
                    return MinistryMemberDto.FromEntity(existingMembership);
                }
                else
                {
                    throw new ArgumentException("Membro já está neste ministério");
                }
            }

            var ministryMember = new Domain.Entities.MinistryMember
            {
                MinistryId = request.MinistryId,
                MemberId = request.MemberId,
                Role = request.Role,
                Notes = request.Notes,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Set<Domain.Entities.MinistryMember>().Add(ministryMember);
            await _context.SaveChangesAsync(cancellationToken);

            // Carregar as navegações
            await _context.Entry(ministryMember)
                .Reference(mm => mm.Ministry)
                .LoadAsync(cancellationToken);
            await _context.Entry(ministryMember)
                .Reference(mm => mm.Member)
                .LoadAsync(cancellationToken);

            return MinistryMemberDto.FromEntity(ministryMember);
        }
    }
}
