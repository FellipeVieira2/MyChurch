using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Journey.Queries
{
    public class GetPastoralAlertsQuery : JwtMemberDto, IRequest<List<PastoralAlertDto>>
    {
    }

    public class GetPastoralAlertsQueryHandler : IRequestHandler<GetPastoralAlertsQuery, List<PastoralAlertDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPastoralAlertsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PastoralAlertDto>> Handle(GetPastoralAlertsQuery request, CancellationToken cancellationToken)
        {
            var leader = _unitOfWork.Members.Query().FirstOrDefault(x => x.Id == request.UserId);
            var alerts = await _unitOfWork.PastoralAlerts.Query()
                .Where(a => a.ChurchId == leader.ChurchId && (a.LeaderId == null || a.LeaderId == request.UserId))
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new PastoralAlertDto
                {
                    Id = a.Id,
                    MemberId = a.MemberId,
                    Message = a.Message,
                    Source = a.Source,
                    IsRead = a.IsRead,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return alerts;
        }
    }
}
