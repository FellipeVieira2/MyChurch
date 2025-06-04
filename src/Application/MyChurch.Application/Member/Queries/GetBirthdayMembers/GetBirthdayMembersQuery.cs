using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
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

            var members = query
                .OrderBy(m => m.BirthDate.Month)
                .ThenBy(m => m.BirthDate.Day)
                .Select(MemberDto.New)
                .ToList();

            return members;
        }
    }
}