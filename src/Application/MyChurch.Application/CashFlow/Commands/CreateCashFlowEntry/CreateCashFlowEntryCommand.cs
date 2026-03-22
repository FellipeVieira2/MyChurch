using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Departments.Services;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.CashFlow.Commands.CreateCashFlowEntry
{
    public class CreateCashFlowEntryCommand : JwtMemberDto, IRequest<int>
    {
        /// <summary>Valor do lançamento</summary>
        public decimal Amount { get; set; }
        /// <summary>Data do lançamento</summary>
        public DateTime Date { get; set; }
        /// <summary>Descrição</summary>
        public string? Description { get; set; }
        /// <summary>Tipo: Entrada ou Saída</summary>
        public CashFlowType Type { get; set; }
        /// <summary>ID da Categoria</summary>
        public int CategoryId { get; set; }
        /// <summary>ID do Departamento (opcional)</summary>
        public int? DepartmentId { get; set; }
    }
    public class CreateCashFlowEntryCommandHandler : IRequestHandler<CreateCashFlowEntryCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCashFlowEntryCommandHandler> _logger;
        private readonly IDepartmentAccessService _departmentAccess;

        public CreateCashFlowEntryCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateCashFlowEntryCommandHandler> logger,
            IDepartmentAccessService departmentAccess)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _departmentAccess = departmentAccess;
        }

        public async Task<int> Handle(CreateCashFlowEntryCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId, se necessário
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "Este membro não existe.");

            if (!UserRoleAccess.CanManageFinancialModule(loggedMember.Role))
                ValidationException.ThrowException("Permissão", "Este perfil não possui gestão financeira.");

            int churchId = loggedMember.ChurchId;

            // Valida se a categoria existe e pertence à igreja
            var category = await _unitOfWork.CashFlowCategories.Query()
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.ChurchId == churchId, cancellationToken);

            if (category == null)
                ValidationException.ThrowException("Categoria", "Categoria de fluxo de caixa não encontrada para esta igreja.");

            if (request.DepartmentId.HasValue)
            {
                var dept = await _unitOfWork.Departments.Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == request.DepartmentId.Value && d.ChurchId == churchId, cancellationToken);

                if (dept == null)
                    ValidationException.ThrowException("Department", "Departamento não encontrado para esta igreja.");

                var access = await _departmentAccess.CanAccessDepartmentFinancialAsync(loggedMember.Id, dept.Id, cancellationToken);
                if (!access.hasAccess || !access.canEdit)
                    ValidationException.ThrowException("Department", "Sem permissão para lançar no financeiro deste departamento.");
            }

            // Cria o lançamento
            var entry = new CashFlowEntry
            {
                Amount = request.Amount,
                Date = request.Date,
                Description = request.Description,
                Type = request.Type,
                CategoryId = request.CategoryId,
                ChurchId = churchId,
                DepartmentId = request.DepartmentId,
                MemberId = loggedMember.Id,
                Created = DateTime.UtcNow
            };

            _unitOfWork.CashFlowEntries.Create(entry);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("CashFlowEntry criado com ID: {EntryId}", entry.Id);

            return entry.Id;
        }
    }
}
