using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Member.Queries.GetBirthdayMembers
{
    public enum BirthdayFilterType
    {
        Day,
        Week,
        Month
    }

    public class GetBirthdayMembersQuery : JwtMemberDto, IRequest<List<MemberDto>>
    {
        public BirthdayFilterType FilterType { get; set; }
    }

    public class GetBirthdayMembersQueryHandler : IRequestHandler<GetBirthdayMembersQuery, List<MemberDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetBirthdayMembersQueryHandler> _logger;

        public GetBirthdayMembersQueryHandler(IUnitOfWork unitOfWork, ILogger<GetBirthdayMembersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<MemberDto>> Handle(GetBirthdayMembersQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Usuário não encontrado.");
                ValidationException.ThrowException("Member", "This Member does not exist.");
            }

            int churchId = member.ChurchId;
            var today = DateTime.Today;

            var query = _unitOfWork.Members.Query()
                .AsNoTracking()
                .Where(m => m.ChurchId == churchId && m.IsActive);

            // Não-admin (Leader incluído): restringe a membros que compartilham ao menos um departamento
            if (member.Role != UserRole.Admin)
            {
                var myDeptIds = await _unitOfWork.DepartmentMembers.Query()
                    .AsNoTracking()
                    .Where(dm => dm.MemberId == member.Id && dm.IsActive)
                    .Select(dm => dm.DepartmentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                query = query.Where(m => _unitOfWork.DepartmentMembers.Query()
                    .AsNoTracking()
                    .Any(dm => dm.MemberId == m.Id && dm.IsActive && myDeptIds.Contains(dm.DepartmentId)));
            }

            switch (request.FilterType)
            {
                case BirthdayFilterType.Day:
                    query = query.Where(m => m.BirthDate.Month == today.Month && m.BirthDate.Day == today.Day);
                    break;
                case BirthdayFilterType.Week:
                    var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                    var endOfWeek = startOfWeek.AddDays(6);
                    query = query.Where(m =>
                        (m.BirthDate.Month == startOfWeek.Month && m.BirthDate.Day >= startOfWeek.Day && m.BirthDate.Day <= endOfWeek.Day) ||
                        (m.BirthDate.Month == endOfWeek.Month && m.BirthDate.Day >= startOfWeek.Day && m.BirthDate.Day <= endOfWeek.Day)
                    );
                    break;
                case BirthdayFilterType.Month:
                    query = query.Where(m => m.BirthDate.Month == today.Month);
                    break;
            }

            return await query
                .OrderBy(m => m.BirthDate.Month)
                .ThenBy(m => m.BirthDate.Day)
                .Select(m => MemberDto.New(m))
                .ToListAsync(cancellationToken);
        }
    }
}