using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Church.Queries.GetChurchSchedules
{
    /// <summary>
    /// Query para buscar horários de uma igreja
    /// </summary>
    public class GetChurchSchedulesQuery : IRequest<List<ChurchScheduleDto>>
    {
        public int ChurchId { get; set; }
        public bool? OnlyActive { get; set; } = true;
    }

    public class GetChurchSchedulesQueryHandler : IRequestHandler<GetChurchSchedulesQuery, List<ChurchScheduleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetChurchSchedulesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ChurchScheduleDto>> Handle(GetChurchSchedulesQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.ChurchSchedules.Query()
                .Where(s => s.ChurchId == request.ChurchId);

            if (request.OnlyActive == true)
            {
                query = query.Where(s => s.IsActive);
            }

            var schedules = await query
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync(cancellationToken);

            return schedules.Select(ChurchScheduleDto.New).ToList();
        }
    }
}
