using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Visitors.Commands
{
    public class PromoteVisitorToMemberCommand : JwtMemberDto, IRequest<int>
    {
        public int VisitorId { get; set; }
        public UserRole Role { get; set; } = UserRole.Worker;
        public string? Notes { get; set; }
    }

    public class PromoteVisitorToMemberCommandHandler : IRequestHandler<PromoteVisitorToMemberCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public PromoteVisitorToMemberCommandHandler(IUnitOfWork uow){ _uow = uow; }
        public async Task<int> Handle(PromoteVisitorToMemberCommand request, CancellationToken cancellationToken)
        {
            var visitor = await _uow.Visitors.Query().FirstOrDefaultAsync(v => v.Id == request.VisitorId, cancellationToken);
            if (visitor == null)
                ValidationException.ThrowException("Visitor","Visitante não encontrado");

            // Descobrir Igreja a partir do membro logado
            var adminMember = await _uow.Members.Query().FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (adminMember == null)
                ValidationException.ThrowException("Auth","Membro autenticado inválido");

            // Evitar duplicidade por email
            if (!string.IsNullOrWhiteSpace(visitor.Email))
            {
                var existsMember = await _uow.Members.Query().AnyAsync(m => m.ChurchId == adminMember.ChurchId && m.Email == visitor.Email, cancellationToken);
                if (existsMember)
                    ValidationException.ThrowException("Visitor","Já existe membro com este email");
            }

            var member = new MyChurch.Domain.Entities.Member
            {
                Name = visitor.Name ?? "Visitante",
                Email = visitor.Email,
                Phone = visitor.Phone,
                BirthDate = DateTime.UtcNow.AddYears(-18), // default se não informado
                IsBaptized = false,
                IsTither = false,
                ChurchId = adminMember.ChurchId,
                Role = request.Role,
                Created = DateTime.UtcNow,
                Notes = request.Notes,
                IsActive = true,
                MemberSince = DateTime.UtcNow,
                TotalFaithPoints = visitor.Score
            };
            _uow.Members.Create(member);
            await _uow.CommitAsync();

            // Migrar doações
            var donations = await _uow.Donations.Query().Where(d => d.VisitorId == visitor.Id).ToListAsync(cancellationToken);
            foreach(var d in donations){ d.MemberId = member.Id; d.VisitorId = null; }
            if (donations.Any()) await _uow.CommitAsync();

            // Atualiza visitor status para Integrated e flag followup false
            visitor.Status = VisitorStatus.Integrated;
            visitor.NeedsFollowUp = false;
            _uow.Visitors.Update(visitor);
            _uow.VisitorStatusHistories.Create(new VisitorStatusHistory
            {
                VisitorId = visitor.Id,
                OldStatus = VisitorStatus.Engaging,
                NewStatus = VisitorStatus.Integrated,
                ChangedByMemberId = adminMember.Id,
                ChangedAt = DateTime.UtcNow,
                Note = "Convertido em membro"
            });
            await _uow.CommitAsync();
            return member.Id;
        }
    }
}
