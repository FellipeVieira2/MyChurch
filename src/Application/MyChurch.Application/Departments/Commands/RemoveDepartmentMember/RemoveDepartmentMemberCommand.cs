using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Departments.Commands.RemoveDepartmentMember
{
    public class RemoveDepartmentMemberCommand : JwtMemberDto, IRequest<Unit>
    {
        public int DepartmentId { get; set; }
        public int MemberId { get; set; }
    }

    public class RemoveDepartmentMemberCommandHandler : IRequestHandler<RemoveDepartmentMemberCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RemoveDepartmentMemberCommandHandler> _logger;

        public RemoveDepartmentMemberCommandHandler(IUnitOfWork unitOfWork, ILogger<RemoveDepartmentMemberCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveDepartmentMemberCommand request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (loggedMember.Role != UserRole.Admin)
                ValidationException.ThrowException("Department", "Apenas administradores podem gerenciar membros de departamentos.");

            var department = await _unitOfWork.Departments.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId && d.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (department == null)
                ValidationException.ThrowException("Department", "Departamento não encontrado.");

            var dm = await _unitOfWork.DepartmentMembers.Query()
                .FirstOrDefaultAsync(x => x.DepartmentId == request.DepartmentId && x.MemberId == request.MemberId, cancellationToken);

            if (dm == null)
                ValidationException.ThrowException("DepartmentMember", "Membro não faz parte do departamento.");

            _unitOfWork.DepartmentMembers.Delete(dm);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Membro removido do departamento. DepartmentId: {DepartmentId} MemberId: {MemberId}", request.DepartmentId, request.MemberId);

            return Unit.Value;
        }
    }
}
