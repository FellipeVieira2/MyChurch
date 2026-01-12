using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Departments.Commands.AddDepartmentMember
{
    public class AddDepartmentMemberCommand : JwtMemberDto, IRequest<int>
    {
        public int DepartmentId { get; set; }
        public int MemberId { get; set; }
        public DepartmentMemberRole Role { get; set; } = DepartmentMemberRole.Member;
    }

    public class AddDepartmentMemberCommandHandler : IRequestHandler<AddDepartmentMemberCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddDepartmentMemberCommandHandler> _logger;

        public AddDepartmentMemberCommandHandler(IUnitOfWork unitOfWork, ILogger<AddDepartmentMemberCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(AddDepartmentMemberCommand request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (loggedMember.Role != UserRole.Admin)
                ValidationException.ThrowException("Department", "Apenas administradores podem gerenciar membros de departamentos.");

            var department = await _unitOfWork.Departments.Query()
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId && d.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (department == null)
                ValidationException.ThrowException("Department", "Departamento não encontrado.");

            var targetMember = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.MemberId && m.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (targetMember == null)
                ValidationException.ThrowException("Member", "Membro não encontrado para esta igreja.");

            var exists = await _unitOfWork.DepartmentMembers.Query()
                .AnyAsync(dm => dm.DepartmentId == request.DepartmentId && dm.MemberId == request.MemberId, cancellationToken);

            if (exists)
                ValidationException.ThrowException("DepartmentMember", "Este membro já faz parte do departamento.");

            var dm = new DepartmentMember
            {
                DepartmentId = request.DepartmentId,
                MemberId = request.MemberId,
                Role = request.Role,
                IsActive = true,
                JoinedAt = DateTime.UtcNow
            };

            _unitOfWork.DepartmentMembers.Create(dm);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Membro adicionado ao departamento. DepartmentId: {DepartmentId} MemberId: {MemberId}", request.DepartmentId, request.MemberId);

            return dm.Id;
        }
    }
}
