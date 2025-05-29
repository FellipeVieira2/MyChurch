using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.CashFlow.Commands.DeleteCashFlowEntry
{
    public class DeleteCashFlowEntryCommand : JwtMemberDto, IRequest<Unit>
    {
        [JsonIgnore]
        public int Id { get; set; }
    }
    public class DeleteCashFlowEntryCommandHandler : IRequestHandler<DeleteCashFlowEntryCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCashFlowEntryCommandHandler> _logger;

        public DeleteCashFlowEntryCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteCashFlowEntryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteCashFlowEntryCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "Este membro não existe.");

            // Busca o lançamento
            var entry = await _unitOfWork.CashFlowEntries.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (entry == null)
                ValidationException.ThrowException("CashFlowEntry", "Lançamento de fluxo de caixa não encontrado.");

            _unitOfWork.CashFlowEntries.Delete(entry);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("CashFlowEntry excluído com ID: {EntryId}", entry.Id);

            return Unit.Value;
        }
    }
}
