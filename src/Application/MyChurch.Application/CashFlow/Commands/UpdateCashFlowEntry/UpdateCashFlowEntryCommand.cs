using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.CashFlow.Commands.UpdateCashFlowEntry
{
    public class UpdateCashFlowEntryCommand : JwtMemberDto, IRequest<CashFlowEntryDto>
    {
        [JsonIgnore]
        /// <summary>ID do lançamento</summary>
        public int Id { get; set; }
        /// <summary>Valor do lançamento</summary>
        public decimal? Amount { get; set; }
        /// <summary>Data do lançamento</summary>
        public DateTime? Date { get; set; }
        /// <summary>Descrição</summary>
        public string? Description { get; set; }
        /// <summary>Tipo: Entrada ou Saída</summary>
        public CashFlowType? Type { get; set; }
        /// <summary>ID da Categoria</summary>
        public int? CategoryId { get; set; }

    }
    public class UpdateCashFlowEntryCommandHandler : IRequestHandler<UpdateCashFlowEntryCommand, CashFlowEntryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCashFlowEntryCommandHandler> _logger;

        public UpdateCashFlowEntryCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateCashFlowEntryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CashFlowEntryDto> Handle(UpdateCashFlowEntryCommand request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember is null)
                ValidationException.ThrowException("Member", "Este membro não existe.");

            var entry = await _unitOfWork.CashFlowEntries.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (entry is null)
                ValidationException.ThrowException("CashFlowEntry", "Lançamento de fluxo de caixa não encontrado.");

            // Atualização parcial
            if (request.Amount.HasValue)
                entry.Amount = request.Amount.Value;
            if (request.Date.HasValue)
                entry.Date = request.Date.Value;
            if (request.Description != null)
                entry.Description = request.Description;
            if (request.Type.HasValue)
                entry.Type = request.Type.Value;
            if (request.CategoryId.HasValue)
            {
                // Valida se a categoria existe e pertence à igreja
                var category = await _unitOfWork.CashFlowCategories.Query()
                    .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.ChurchId == loggedMember.ChurchId, cancellationToken);

                if (category is null)
                    ValidationException.ThrowException("Categoria", "Categoria de fluxo de caixa não encontrada para esta igreja.");

                entry.CategoryId = request.CategoryId.Value;
            }

            entry.Updated = DateTime.UtcNow;

            _unitOfWork.CashFlowEntries.Update(entry);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("CashFlowEntry atualizado com ID: {EntryId}", entry.Id);

            return CashFlowEntryDto.New(entry);
        }
    }
}
