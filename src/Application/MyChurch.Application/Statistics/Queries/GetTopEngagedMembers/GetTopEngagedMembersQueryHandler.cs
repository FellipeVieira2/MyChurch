using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure;

namespace MyChurch.Application.Statistics.Queries.GetTopEngagedMembers
{
    public class GetTopEngagedMembersQueryHandler : IRequestHandler<GetTopEngagedMembersQuery, List<TopEngagedMembersDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MyChurchDbContext _context;

        public GetTopEngagedMembersQueryHandler(IUnitOfWork unitOfWork, MyChurchDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<List<TopEngagedMembersDto>> Handle(GetTopEngagedMembersQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Membro não encontrado.");

            int churchId = request.ChurchId ?? member.ChurchId;

            // Buscar membros com estatísticas
            var members = await _unitOfWork.Members.Query()
                .Where(m => m.ChurchId == churchId && m.IsActive)
                .OrderByDescending(m => m.EngagementScore)
                .Take(request.Top)
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Photo,
                    m.EngagementScore
                })
                .ToListAsync(cancellationToken);

            var memberIds = members.Select(m => m.Id).ToList();

            // Contar presenças
            var attendanceCounts = await _context.Set<Domain.Entities.WorshipPresence>()
                .Where(wp => wp.MemberId.HasValue && memberIds.Contains(wp.MemberId.Value))
                .GroupBy(wp => wp.MemberId!.Value)
                .Select(g => new { MemberId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MemberId, x => x.Count, cancellationToken);

            // Contar doações
            var donationCounts = await _unitOfWork.Donations.Query()
                .Where(d => d.MemberId.HasValue && memberIds.Contains(d.MemberId.Value))
                .GroupBy(d => d.MemberId!.Value)
                .Select(g => new { MemberId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MemberId, x => x.Count, cancellationToken);

            // Contar pedidos de oração
            var prayerRequestCounts = await _unitOfWork.PrayerRequests.Query()
                .Where(pr => memberIds.Contains(pr.MemberId))
                .GroupBy(pr => pr.MemberId)
                .Select(g => new { MemberId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MemberId, x => x.Count, cancellationToken);

            var result = members.Select(m => new TopEngagedMembersDto
            {
                MemberId = m.Id,
                MemberName = m.Name,
                MemberPhoto = m.Photo,
                EngagementScore = m.EngagementScore,
                AttendanceCount = attendanceCounts.GetValueOrDefault(m.Id, 0),
                DonationCount = donationCounts.GetValueOrDefault(m.Id, 0),
                PrayerRequestCount = prayerRequestCounts.GetValueOrDefault(m.Id, 0)
            }).ToList();

            return result;
        }
    }
}
