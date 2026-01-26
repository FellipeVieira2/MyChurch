using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Application.Plans.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Services;

namespace MyChurch.Application.Church.Commands.CreateBranch
{
    public class CreateBranchCommand : JwtMemberDto, IRequest<int>
    {
        public int ParentChurchId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Document { get; set; }

        public AddressDto Address { get; set; } = null!;

        public string AdminName { get; set; } = string.Empty;
        public string? AdminEmail { get; set; }
        public string AdminPhone { get; set; } = string.Empty;
        public DateTime AdminBirthDate { get; set; }
        public bool AdminIsBaptized { get; set; }
        public DateTime? AdminBaptizedDate { get; set; }
        public bool AdminIsTither { get; set; }
        public string AdminPassword { get; set; } = string.Empty;
        public string? AdminBirthCity { get; set; }
        public string? AdminBirthState { get; set; }
        public string? Ministry { get; set; }
        public DateTime MemberSince { get; set; }
        public string? Notes { get; set; }
        public AddressDto AdminAddress { get; set; } = null!;
        public MaritalStatus? MaritalStatus { get; set; }

        public class AddressDto
        {
            public string Street { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string State { get; set; } = string.Empty;
            public string ZipCode { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
            public string Neighborhood { get; set; } = string.Empty;
            public string Number { get; set; } = string.Empty;
        }
    }

    public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<CreateBranchCommandHandler> _logger;
        private readonly IPlanLimitService _planLimits;

        public CreateBranchCommandHandler(
            IUnitOfWork uow,
            IPasswordHasher passwordHasher,
            ILogger<CreateBranchCommandHandler> logger,
            IPlanLimitService planLimits)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _planLimits = planLimits;
        }

        public async Task<int> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
        {
            var actor = await _uow.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (actor == null)
                ValidationException.ThrowException("Member", "Authenticated member does not exist.");

            if (actor.Role != UserRole.Admin)
                ValidationException.ThrowException("Member", "Only admins can create branches.");

            if (actor.ChurchId != request.ParentChurchId)
                ValidationException.ThrowException("Church", "You can only create branches for your own church.");

            var parent = await _uow.Churchs.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.ParentChurchId, cancellationToken);

            if (parent == null)
                ValidationException.ThrowException("Church", "Parent church not found.");

            if (parent.ParentChurchId.HasValue)
                ValidationException.ThrowException("Church", "Branches cannot create sub-branches.");

            await _planLimits.EnsureMaxBranchesAllowedAsync(parentChurchId: parent.Id, additionalBranchesToAdd: 1, cancellationToken);

            await _uow.BeginTransactionAsync();
            try
            {
                var churchAddress = new Address(
                    street: request.Address.Street,
                    city: request.Address.City,
                    state: request.Address.State,
                    zipCode: request.Address.ZipCode,
                    country: request.Address.Country,
                    neighborhood: request.Address.Neighborhood)
                {
                    Number = request.Address.Number,
                    Complement = "N/A"
                };

                var branch = new Domain.Entities.Church(
                    name: request.Name,
                    phone: request.Phone,
                    address: churchAddress,
                    description: request.Description ?? string.Empty)
                {
                    ParentChurchId = parent.Id,
                    Document = string.IsNullOrWhiteSpace(request.Document) ? null : new string(request.Document.Where(char.IsDigit).ToArray()),
                    Created = DateTime.UtcNow
                };

                await _uow.Churchs.Create(branch);
                await _uow.CommitAsync();

                // Cria assinatura Free para a filial (mantém consistência com regras atuais)
                var freePlan = await _uow.Plans.Query()
                    .AsNoTracking()
                    .Where(p => p.Price == 0)
                    .OrderBy(p => p.Id)
                    .FirstOrDefaultAsync(cancellationToken)
                    ?? await _uow.Plans.Query().AsNoTracking().OrderBy(p => p.Price).FirstOrDefaultAsync(cancellationToken);

                if (freePlan == null)
                    throw new InvalidOperationException("Nenhum plano disponível no sistema.");

                var branchSubscription = new Domain.Entities.Subscription(
                    planId: freePlan.Id,
                    startDate: DateTime.UtcNow,
                    endDate: DateTime.UtcNow.AddMonths(1))
                {
                    ChurchId = branch.Id,
                    Created = DateTime.UtcNow,
                    ExternalReference = null
                };

                await _uow.Subscriptions.Create(branchSubscription);
                await _uow.CommitAsync();

                var adminMember = new Domain.Entities.Member
                {
                    Name = request.AdminName,
                    Email = request.AdminEmail,
                    Phone = request.AdminPhone,
                    BirthDate = request.AdminBirthDate,
                    IsBaptized = request.AdminIsBaptized,
                    BaptizedDate = request.AdminBaptizedDate,
                    IsTither = request.AdminIsTither,
                    ChurchId = branch.Id,
                    Role = UserRole.Admin,
                    Created = DateTime.UtcNow,
                    BirthCity = request.AdminBirthCity,
                    BirthState = request.AdminBirthState,
                    Ministry = request.Ministry,
                    MaritalStatus = request.MaritalStatus,
                    Notes = request.Notes,
                    MemberSince = request.MemberSince,
                    Address = new Address(request.AdminAddress.Street, request.AdminAddress.City, request.AdminAddress.State, request.AdminAddress.ZipCode, request.AdminAddress.Country, request.AdminAddress.Neighborhood)
                    {
                        Number = request.AdminAddress.Number,
                        Complement = "N/A"
                    }
                };

                if (string.IsNullOrWhiteSpace(request.AdminPassword) || request.AdminPassword.Length < 6)
                    ValidationException.ThrowException("Password", "Password must be at least 6 characters.");

                adminMember.PasswordHash = _passwordHasher.HashPassword(request.AdminPassword);

                await _uow.Members.Create(adminMember);
                await _uow.CommitAsync();

                await _uow.CommitTransactionAsync();

                _logger.LogInformation("Branch created with ID: {BranchId} for parent {ParentChurchId}", branch.Id, parent.Id);
                return branch.Id;
            }
            catch
            {
                await _uow.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
