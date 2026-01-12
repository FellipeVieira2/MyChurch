using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommand : JwtMemberDto, IRequest<int>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? BankingInfoId { get; set; }
    }

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateDepartmentCommandHandler> _logger;

        public CreateDepartmentCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateDepartmentCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (member.Role != UserRole.Admin)
                ValidationException.ThrowException("Department", "Apenas administradores podem criar departamentos.");

            if (string.IsNullOrWhiteSpace(request.Name))
                ValidationException.ThrowException("Department", "Nome é obrigatório.");

            var exists = await _unitOfWork.Departments.Query()
                .AnyAsync(d => d.ChurchId == member.ChurchId && d.Name.ToLower() == request.Name.ToLower(), cancellationToken);

            if (exists)
                ValidationException.ThrowException("Department", "Já existe um departamento com este nome.");

            if (request.BankingInfoId.HasValue)
            {
                var bank = await _unitOfWork.BankingInfos.Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == request.BankingInfoId.Value && b.ChurchId == member.ChurchId, cancellationToken);

                if (bank == null)
                    ValidationException.ThrowException("BankingInfo", "Conta bancária não encontrada para esta igreja.");
            }

            var dept = new Department
            {
                ChurchId = member.ChurchId,
                Name = request.Name.Trim(),
                Description = request.Description,
                BankingInfoId = request.BankingInfoId,
                Created = DateTime.UtcNow,
                IsActive = true
            };

            _unitOfWork.Departments.Create(dept);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Departamento criado. DepartmentId: {DepartmentId} ChurchId: {ChurchId}", dept.Id, member.ChurchId);

            return dept.Id;
        }
    }
}
