using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
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
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            var worshipService = await _unitOfWork.WorshipServices.Query()
                .Include(ws => ws.Activities)
                    .ThenInclude(a => a.Bibles)
                .Include(ws => ws.Activities)
                    .ThenInclude(a => a.Hymns)
                .Include(ws => ws.Presences)
                .Include(ws => ws.Schedule)
                .FirstOrDefaultAsync(ws => ws.Id == request.Id && ws.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (worshipService == null)
                ValidationException.ThrowException("WorshipService", "Culto não encontrado ou não pertence à sua igreja.");

            return WorshipServiceDto.New(worshipService);
        }
    }
}
