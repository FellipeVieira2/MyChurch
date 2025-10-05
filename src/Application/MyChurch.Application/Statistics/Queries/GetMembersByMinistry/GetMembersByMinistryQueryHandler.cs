using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure;

namespace MyChurch.Application.Statistics.Queries.GetMembersByMinistry
{
    public class GetMembersByMinistryQueryHandler : IRequestHandler<GetMembersByMinistryQuery, List<MembersByMinistryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MyChurchDbContext _context;

        public GetMembersByMinistryQueryHandler(IUnitOfWork unitOfWork, MyChurchDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<List<MembersByMinistryDto>> Handle(GetMembersByMinistryQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Membro não encontrado.");

            int churchId = request.ChurchId ?? member.ChurchId;

            // Buscar ministérios com contagem de membros ativos
            var ministries = await _context.Set<Domain.Entities.Ministry>()
                .Where(m => m.ChurchId == churchId && m.IsActive)
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    MemberCount = m.MinistryMembers.Count(mm => mm.IsActive)
                })
                .ToListAsync(cancellationToken);

            var totalMembers = ministries.Sum(m => m.MemberCount);

            var result = ministries
                .Where(m => m.MemberCount > 0)
                .Select(m => new MembersByMinistryDto
                {
                    MinistryName = m.Name,
                    MemberCount = m.MemberCount,
                    Percentage = totalMembers > 0 
                        ? Math.Round(((decimal)m.MemberCount / totalMembers) * 100, 2) 
                        : 0
                })
                .OrderByDescending(m => m.MemberCount)
                .ToList();

            // Adicionar membros sem ministério
            var membersWithoutMinistry = await _unitOfWork.Members.Query()
                .Where(m => m.ChurchId == churchId && 
                           m.IsActive && 
                           !_context.Set<Domain.Entities.MinistryMember>()
                               .Any(mm => mm.MemberId == m.Id && mm.IsActive))
                .CountAsync(cancellationToken);

            if (membersWithoutMinistry > 0)
            {
                var totalWithUnassigned = totalMembers + membersWithoutMinistry;
                result.Add(new MembersByMinistryDto
                {
                    MinistryName = "Sem Ministério",
                    MemberCount = membersWithoutMinistry,
                    Percentage = Math.Round(((decimal)membersWithoutMinistry / totalWithUnassigned) * 100, 2)
                });
            }

            return result;
        }
    }
}
