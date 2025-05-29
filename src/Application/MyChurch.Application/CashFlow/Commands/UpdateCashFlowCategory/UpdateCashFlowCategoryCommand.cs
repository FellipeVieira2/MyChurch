using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.CashFlow.Commands.UpdateCashFlowEntry;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.CashFlow.Commands.UpdateCashFlowCategory
{
    public class UpdateCashFlowCategoryCommand : JwtMemberDto, IRequest<CashFlowCategoryDto>
    {
        [JsonIgnore]
        /// <summary>ID do lançamento</summary>
        public int Id { get; set; }
        /// <summary>Nome da categoria</summary>
        public string Name { get; set; } = null!;
        /// <summary>Descrição da categoria</summary>
        public string? Description { get; set; }
        /// <summary>ID da Igreja (opcional, pode ser resolvido pelo membro logado)</summary>

    }
    public class UpdateCashFlowCategoryCommandHandler : IRequestHandler<UpdateCashFlowCategoryCommand, CashFlowCategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCashFlowEntryCommandHandler> _logger;

        public UpdateCashFlowCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateCashFlowEntryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CashFlowCategoryDto> Handle(UpdateCashFlowCategoryCommand request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember is null)
                ValidationException.ThrowException("Member", "Este membro não existe.");

            var categories = await _unitOfWork.CashFlowCategories.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (categories is null)
                ValidationException.ThrowException("CashFlowEntry", "Lançamento de fluxo de caixa não encontrado.");

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                categories.Name = request.Name;
            }

            if (request.Description != null)
            {
                categories.Description = request.Description;
            }

            _unitOfWork.CashFlowCategories.Update(categories);
            await _unitOfWork.CommitAsync();

            return CashFlowCategoryDto.New(categories);
        }
    }
}
