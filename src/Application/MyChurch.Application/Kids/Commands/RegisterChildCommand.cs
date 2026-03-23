using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Kids.Commands
{
    public class RegisterChildCommand : JwtMemberDto, IRequest<int>
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public List<ChildPickupAuthorizationInputDto> AuthorizedPickups { get; set; } = [];
    }

    public class RegisterChildCommandHandler : IRequestHandler<RegisterChildCommand, int>
    {
        private readonly IUnitOfWork _uow;

        public RegisterChildCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> Handle(RegisterChildCommand request, CancellationToken cancellationToken)
        {
            var actor = await _uow.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (actor == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");

            if (!actor.FamilyId.HasValue)
                ValidationException.ThrowException("Family", "Você precisa estar vinculado a uma família para cadastrar uma criança.");

            var family = await _uow.Families.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == actor.FamilyId.Value && x.ChurchId == actor.ChurchId, cancellationToken);

            if (family == null)
                ValidationException.ThrowException("Family", "Família não encontrada para a sua igreja.");

            var childName = request.FullName?.Trim();
            if (string.IsNullOrWhiteSpace(childName))
                ValidationException.ThrowException("FullName", "O nome da criança é obrigatório.");

            if (request.AuthorizedPickups == null || request.AuthorizedPickups.Count == 0)
                ValidationException.ThrowException("AuthorizedPickups", "Informe ao menos uma pessoa autorizada para retirada.");

            var invalidPickup = request.AuthorizedPickups.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.FullName) || string.IsNullOrWhiteSpace(x.Relationship));
            if (invalidPickup != null)
                ValidationException.ThrowException("AuthorizedPickups", "Todas as pessoas autorizadas precisam informar nome e vínculo.");

            var createdAt = DateTime.UtcNow;
            var child = new Child
            {
                FamilyId = family.Id,
                FullName = childName,
                BirthDate = request.BirthDate.Date,
                Gender = request.Gender,
                IsActive = true,
                PickupAuthorizations = request.AuthorizedPickups
                    .Select(x => new ChildPickupAuthorization
                    {
                        FullName = x.FullName.Trim(),
                        Relationship = x.Relationship.Trim(),
                        DocumentNumber = string.IsNullOrWhiteSpace(x.DocumentNumber) ? null : x.DocumentNumber.Trim(),
                        PhoneNumber = string.IsNullOrWhiteSpace(x.PhoneNumber) ? null : x.PhoneNumber.Trim(),
                        IsActive = true,
                        CreatedAt = createdAt,
                        CreatedByMemberId = actor.Id
                    })
                    .ToList()
            };

            await _uow.Children.Create(child);
            await _uow.CommitAsync();
            return child.Id;
        }
    }
}
