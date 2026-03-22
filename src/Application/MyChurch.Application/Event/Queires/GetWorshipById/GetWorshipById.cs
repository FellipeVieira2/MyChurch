using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using System.Threading;

namespace MyChurch.Application.Event.Queires.GetWorshipById
{
    public class GetWorshipByIdQuery : JwtMemberDto, IRequest<WorshipServiceDto>
    {
        public int Id { get; set; }
    }

    public class GetWorshipByIdQueryHandler : IRequestHandler<GetWorshipByIdQuery, WorshipServiceDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorshipByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorshipServiceDto> Handle(GetWorshipByIdQuery request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            var worshipQuery = _unitOfWork.WorshipServices.Query()
                .AsNoTracking()
                .Include(ws => ws.Department)
                .Include(ws => ws.Activities)
                    .ThenInclude(a => a.Bibles)
                .Include(ws => ws.Activities)
                    .ThenInclude(a => a.Hymns)
                .Include(ws => ws.Presences)
                .Include(ws => ws.Schedule)
                .Include(ws => ws.ScaleMembers)
                    .ThenInclude(sm => sm.Member)
                .Where(ws => ws.Id == request.Id && ws.ChurchId == loggedMember.ChurchId);

            if (loggedMember.Role != UserRole.Admin)
            {
                var allowedDepartmentIds = await _unitOfWork.DepartmentMembers.Query()
                    .AsNoTracking()
                    .Where(dm => dm.MemberId == loggedMember.Id && dm.IsActive)
                    .Select(dm => dm.DepartmentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                worshipQuery = worshipQuery.Where(ws => ws.DepartmentId == null || (ws.DepartmentId.HasValue && allowedDepartmentIds.Contains(ws.DepartmentId.Value)));
            }

            var worshipService = await worshipQuery.FirstOrDefaultAsync(cancellationToken);

            if (worshipService == null)
                ValidationException.ThrowException("WorshipService", "Culto não encontrado ou não pertence à sua igreja.");

            return WorshipServiceDto.New(worshipService);
        }
    }
}
