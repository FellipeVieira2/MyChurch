using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Infrastructure;

namespace MyChurch.Application.Ministries.Commands.UpdateMinistry
{
    public class UpdateMinistryCommandHandler : IRequestHandler<UpdateMinistryCommand, MinistryDto>
    {
        private readonly MyChurchDbContext _context;

        public UpdateMinistryCommandHandler(MyChurchDbContext context)
        {
            _context = context;
        }

        public async Task<MinistryDto> Handle(UpdateMinistryCommand request, CancellationToken cancellationToken)
        {
            var ministry = await _context.Set<Domain.Entities.Ministry>()
                .Include(m => m.Leader)
                .Include(m => m.MinistryMembers)
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (ministry == null)
                throw new ArgumentException("Ministério não encontrado");

            // Validar se o novo líder existe e pertence à igreja (se fornecido)
            if (request.LeaderId.HasValue)
            {
                var leaderExists = await _context.Set<Domain.Entities.Member>()
                    .AnyAsync(m => m.Id == request.LeaderId.Value && m.ChurchId == ministry.ChurchId, cancellationToken);
                
                if (!leaderExists)
                    throw new ArgumentException("Líder não encontrado ou não pertence à igreja");
            }

            ministry.Update(
                name: request.Name,
                description: request.Description,
                leaderId: request.LeaderId,
                photo: request.Photo,
                color: request.Color,
                meetingDay: request.MeetingDay,
                meetingTime: request.MeetingTime,
                meetingLocation: request.MeetingLocation,
                isActive: request.IsActive
            );

            await _context.SaveChangesAsync(cancellationToken);

            // Recarregar o líder atualizado
            await _context.Entry(ministry)
                .Reference(m => m.Leader)
                .LoadAsync(cancellationToken);

            return MinistryDto.FromEntity(ministry);
        }
    }
}
