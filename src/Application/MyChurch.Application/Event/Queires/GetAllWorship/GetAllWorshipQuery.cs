using MediatR;
using Microsoft.EntityFrameworkCore;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Event.Queires.GetAllWorship
{
    public class GetAllWorshipQuery : JwtMemberDto, IRequest<PagedResultDto<WorshipServiceDto>>
    {
        public string? Title { get; set; }
        public string? Theme { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? OnlyPast { get; set; } // true: só cultos passados, false: só futuros, null: todos
        public WorshipServiceStatus? Status { get; set; } // Filtro por status
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetAllWorshipQueryHandler : IRequestHandler<GetAllWorshipQuery, PagedResultDto<WorshipServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllWorshipQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<WorshipServiceDto>> Handle(GetAllWorshipQuery request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            int churchId = loggedMember.ChurchId;

            var query = _unitOfWork.WorshipServices.Query()
                .Where(ws => ws.ChurchId == churchId);

            if (!string.IsNullOrEmpty(request.Title))
                query = query.Where(ws => ws.Title.Contains(request.Title));

            if (!string.IsNullOrEmpty(request.Theme))
                query = query.Where(ws => ws.Theme != null && ws.Theme.Contains(request.Theme));

            if (request.StartTime.HasValue)
                query = query.Where(ws => ws.StartTime >= request.StartTime.Value);

            if (request.EndTime.HasValue)
                query = query.Where(ws => ws.EndTime <= request.EndTime.Value);

            if (request.Status.HasValue)
                query = query.Where(ws => ws.Status == request.Status.Value);

            if (request.OnlyPast.HasValue)
            {
                var now = DateTime.UtcNow;
                if (request.OnlyPast.Value)
                    query = query.Where(ws => ws.EndTime != null ? ws.EndTime < now : ws.StartTime < now);
                else
                    query = query.Where(ws => ws.EndTime != null ? ws.EndTime >= now : ws.StartTime >= now);
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(ws => ws.StartTime)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResultDto<WorshipServiceDto>
            {
                TotalCount = total,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                Items = items.Select(WorshipServiceDto.New).ToList()
            };
        }
    }
}