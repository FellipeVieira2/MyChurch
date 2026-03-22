using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.WorshipService.Commands.ManageScale
{
    public class AddWorshipScaleMemberCommand : JwtMemberDto, IRequest<int>
    {
        public int WorshipServiceId { get; set; }
        public int MemberId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int Order { get; set; }
        public string? Notes { get; set; }
    }

    public class AddWorshipScaleMemberCommandHandler : IRequestHandler<AddWorshipScaleMemberCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddWorshipScaleMemberCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(AddWorshipScaleMemberCommand request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            var worshipService = await _unitOfWork.WorshipServices.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(ws => ws.Id == request.WorshipServiceId && ws.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (worshipService == null)
                ValidationException.ThrowException("WorshipService", "Culto não encontrado ou não pertence à sua igreja.");

            var targetMember = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.MemberId && m.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (targetMember == null)
                ValidationException.ThrowException("Member", "Membro não encontrado para esta igreja.");

            var normalizedRoleName = request.RoleName?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                ValidationException.ThrowException("RoleName", "A função na escala é obrigatória.");

            var alreadyExists = await _unitOfWork.WorshipScaleMembers.Query()
                .AsNoTracking()
                .AnyAsync(x => x.WorshipServiceId == request.WorshipServiceId
                    && x.MemberId == request.MemberId
                    && x.RoleName.ToLower() == normalizedRoleName.ToLower(), cancellationToken);

            if (alreadyExists)
                ValidationException.ThrowException("WorshipScale", "Este membro já está escalado com esta função neste culto.");

            var scaleMember = new WorshipScaleMember
            {
                WorshipServiceId = request.WorshipServiceId,
                MemberId = request.MemberId,
                RoleName = normalizedRoleName,
                Order = request.Order,
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
            };

            await _unitOfWork.WorshipScaleMembers.Create(scaleMember);
            await _unitOfWork.CommitAsync();

            return scaleMember.Id;
        }
    }
}
