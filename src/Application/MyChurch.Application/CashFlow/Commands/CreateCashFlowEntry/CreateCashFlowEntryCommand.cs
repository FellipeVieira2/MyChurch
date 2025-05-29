using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
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
        /// <summary>ID do Membro (opcional)</summary>
    }
    public class CreateCashFlowEntryCommandHandler : IRequestHandler<CreateCashFlowEntryCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCashFlowEntryCommandHandler> _logger;

        public CreateCashFlowEntryCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateCashFlowEntryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(CreateCashFlowEntryCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId, se necessário
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "Este membro não existe.");

            int churchId = loggedMember.ChurchId;

            // Valida se a categoria existe e pertence à igreja
            var category = await _unitOfWork.CashFlowCategories.Query()
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.ChurchId == churchId, cancellationToken);

            if (category == null)
                ValidationException.ThrowException("Categoria", "Categoria de fluxo de caixa não encontrada para esta igreja.");

            // Cria o lançamento
            var entry = new CashFlowEntry
            {
                Amount = request.Amount,
                Date = request.Date,
                Description = request.Description,
                Type = request.Type,
                CategoryId = request.CategoryId,
                ChurchId = churchId,
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
