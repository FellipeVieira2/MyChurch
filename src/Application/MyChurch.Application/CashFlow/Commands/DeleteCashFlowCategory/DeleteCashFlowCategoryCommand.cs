using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.CashFlow.Commands.UpdateCashFlowCategory;
using MyChurch.Application.CashFlow.Commands.UpdateCashFlowEntry;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.CashFlow.Commands.DeleteCashFlowCategory
{
    public class DeleteCashFlowCategoryCommand : JwtMemberDto, IRequest<Unit>
    {
        [JsonIgnore]
        /// <summary>ID do lançamento</summary>
        public int Id { get; set; }

    }
    public class DeleteCashFlowCategoryCommandHandler : IRequestHandler<DeleteCashFlowCategoryCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCashFlowCategoryCommandHandler> _logger;

        public DeleteCashFlowCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteCashFlowCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteCashFlowCategoryCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "Este membro não existe.");

            if (!UserRoleAccess.CanManageFinancialModule(loggedMember.Role))
                ValidationException.ThrowException("Permissão", "Este perfil não possui gestão financeira.");

            // Busca o lançamento
            var category = await _unitOfWork.CashFlowCategories.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (category == null)
                ValidationException.ThrowException("CashFlowEntry", "Lançamento de fluxo de caixa não encontrado.");

            _unitOfWork.CashFlowCategories.Delete(category);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
