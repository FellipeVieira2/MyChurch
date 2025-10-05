using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Infrastructure;

namespace MyChurch.Application.Ministries.Commands.CreateMinistry
{
    public class CreateMinistryCommandHandler : IRequestHandler<CreateMinistryCommand, MinistryDto>
    {
        private readonly MyChurchDbContext _context;

        public CreateMinistryCommandHandler(MyChurchDbContext context)
        {
            _context = context;
        }

        public async Task<MinistryDto> Handle(CreateMinistryCommand request, CancellationToken cancellationToken)
        {
            // Validar se a igreja existe
            var churchExists = await _context.Set<Domain.Entities.Church>()
                .AnyAsync(c => c.Id == request.ChurchId, cancellationToken);
            
            if (!churchExists)
                throw new ArgumentException("Igreja não encontrada");
            
            // Validar se o líder existe e pertence à igreja (se fornecido)
            if (request.LeaderId.HasValue)
            {
                var leaderExists = await _context.Set<Domain.Entities.Member>()
                    .AnyAsync(m => m.Id == request.LeaderId.Value && m.ChurchId == request.ChurchId, cancellationToken);
                
                if (!leaderExists)
                    throw new ArgumentException("Líder não encontrado ou não pertence à igreja");
            }
            
            var ministry = new Domain.Entities.Ministry
            {
                Name = request.Name,
                Description = request.Description,
                LeaderId = request.LeaderId,
                ChurchId = request.ChurchId,
                Photo = request.Photo,
                Color = request.Color,
                MeetingDay = request.MeetingDay,
                MeetingTime = request.MeetingTime,
                MeetingLocation = request.MeetingLocation,
                IsActive = true,
                Created = DateTime.UtcNow
            };

            _context.Set<Domain.Entities.Ministry>().Add(ministry);
            await _context.SaveChangesAsync(cancellationToken);

            // Carregar o líder para retornar no DTO
            await _context.Entry(ministry)
                .Reference(m => m.Leader)
                .LoadAsync(cancellationToken);

            return MinistryDto.FromEntity(ministry);
        }
    }
}
