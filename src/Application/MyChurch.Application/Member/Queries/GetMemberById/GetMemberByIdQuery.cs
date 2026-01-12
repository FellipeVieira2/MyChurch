using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Member.Queries.GetMemberById
{
    public class GetMemberByIdQuery : JwtMemberDto, IRequest<MemberDto>
    {
        [JsonIgnore]
        public int Id { get; set; }
    }

    public class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, MemberDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetMemberByIdQuery> _logger;

        public GetMemberByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetMemberByIdQuery> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<MemberDto> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember is null)
            {
                _logger.LogError("Authenticated member not found");
                ValidationException.ThrowException("Member", "Authenticated member not found");
            }

            int churchId = loggedMember.ChurchId;

            // Busca o membro solicitado, validando se pertence à mesma igreja
            var member = await _unitOfWork.Members.Query()
                .AsNoTrackingWithIdentityResolution()
                .Include(m => m.Church)
                    .ThenInclude(x => x.Address)
                .Include(x => x.Documents)
                .Include(x => x.Address)
                .FirstOrDefaultAsync(m => m.Id == request.Id && m.ChurchId == churchId, cancellationToken);

            if (member == null)
            {
                _logger.LogError("Member not found or does not belong to your church");
                ValidationException.ThrowException("Get", "Member not found or does not belong to your church");
            }

            // Regra: Admin vê qualquer membro da igreja.
            // Não-admin (Leader incluído) só pode ver a si mesmo ou membros que compartilham ao menos 1 departamento.
            if (loggedMember.Role != UserRole.Admin && loggedMember.Id != member.Id)
            {
                var myDeptIds = await _unitOfWork.DepartmentMembers.Query()
                    .AsNoTracking()
                    .Where(dm => dm.MemberId == loggedMember.Id && dm.IsActive)
                    .Select(dm => dm.DepartmentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var targetDeptIds = await _unitOfWork.DepartmentMembers.Query()
                    .AsNoTracking()
                    .Where(dm => dm.MemberId == member.Id && dm.IsActive)
                    .Select(dm => dm.DepartmentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                if (!myDeptIds.Intersect(targetDeptIds).Any())
                    ValidationException.ThrowException("Member", "Sem permissão para visualizar este membro.");
            }

            return MemberDto.New(member);
        }
    }
}
