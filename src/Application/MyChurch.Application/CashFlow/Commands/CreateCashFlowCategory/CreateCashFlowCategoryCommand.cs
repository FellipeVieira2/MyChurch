using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.CashFlow.Commands.CreateCashFlowCategory
{
    public class CreateCashFlowCategoryCommand : JwtMemberDto, IRequest<int>
    {
        /// <summary>Nome da categoria</summary>
        public string Name { get; set; } = null!;
        /// <summary>Descrição da categoria</summary>
        public string? Description { get; set; }
    }
    public class CreateCashFlowCategoryCommandHandler : IRequestHandler<CreateCashFlowCategoryCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCashFlowCategoryCommandHandler> _logger;

        public CreateCashFlowCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateCashFlowCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(CreateCashFlowCategoryCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "Este membro não existe.");

            int churchId = loggedMember.ChurchId;

            // Verifica se já existe categoria com o mesmo nome na igreja
            var exists = await _unitOfWork.CashFlowCategories.Query()
                .AnyAsync(c => c.Name == request.Name && c.ChurchId == churchId, cancellationToken);

            if (exists)
                ValidationException.ThrowException("Categoria", "Já existe uma categoria com este nome para esta igreja.");

            // Cria a categoria
            var category = new CashFlowCategory
            {
                Name = request.Name,
                Description = request.Description,
                ChurchId = churchId
            };

            _unitOfWork.CashFlowCategories.Create(category);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Categoria de fluxo de caixa criada com ID: {CategoryId}", category.Id);

            return category.Id;
        }
    }
}
