using FluentValidation;
using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Services;
using ValidationException = MyChurch.Domain.Exceptions.ValidationException;

namespace MyChurch.Application.Onboarding.RegisterForApproval
{
    public class RegisterForApprovalCommand : IRequest
    {
        public int ChurchId { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Cpf { get; set; }
        public DateTime BirthDate { get; set; }
        public string? MaritalStatus { get; set; }
        public string Password { get; set; } // ✅ Senha definida no registro
        public AddressRegisterForApproval Address { get; set; }

        public class AddressRegisterForApproval
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
            public string Country { get; set; }
            public string Neighborhood { get; set; }
        }
        
        public class Validator : AbstractValidator<RegisterForApprovalCommand>
        {
            public Validator()
            {
                RuleFor(x => x.ChurchId).NotEmpty();
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Cpf).NotEmpty();
                RuleFor(x => x.BirthDate).NotEmpty();
                RuleFor(x => x.Password).NotEmpty().MinimumLength(6)
                    .WithMessage("A senha deve ter no mínimo 6 caracteres");
                RuleFor(x => x.Address).NotNull();
            }
        }

        public class Handler : IRequestHandler<RegisterForApprovalCommand>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IPasswordHasher _passwordHasher;
            private readonly IDocumentValidator _documentValidator;

            public Handler(
                IUnitOfWork unitOfWork, 
                IPasswordHasher passwordHasher,
                IDocumentValidator documentValidator)
            {
                _unitOfWork = unitOfWork;
                _passwordHasher = passwordHasher;
                _documentValidator = documentValidator;
            }

            public async Task Handle(RegisterForApprovalCommand request, CancellationToken cancellationToken)
            {
                // 📄 Normalizar CPF (remove pontos, traços, espaços)
                var normalizedCpf = _documentValidator.RemoveFormatting(request.Cpf);
                
                // Validar se já existe membro com este CPF
                var exists = _unitOfWork.Members.Query()
                    .Any(m => m.Documents.Any(d => d.Number == normalizedCpf));
                
                if (exists)
                {
                    ValidationException.ThrowException("User","Já existe um membro com este CPF.");
                }
                
                MaritalStatus? maritalStatus = null;
                if (!string.IsNullOrEmpty(request.MaritalStatus) && Enum.TryParse<MaritalStatus>(request.MaritalStatus, out var ms))
                    maritalStatus = ms;

                var member = new MyChurch.Domain.Entities.Member
                {
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.PhoneNumber,
                    BirthDate = request.BirthDate,
                    MaritalStatus = maritalStatus,
                    ChurchId = request.ChurchId,
                    IsActive = false, // ❌ Conta inativa até admin aprovar
                    Address = new MyChurch.Domain.Entities.Address(
                        request.Address.Street,
                        request.Address.City,
                        request.Address.State,
                        request.Address.ZipCode,
                        request.Address.Country,
                        request.Address.Neighborhood
                    ),
                    Role = UserRole.Member // Padrão é Member
                };

                // 🔐 SEGURANÇA: Salvar hash BCrypt da senha imediatamente
                member.PasswordHash = _passwordHasher.HashPassword(request.Password);
                
                // Salvar CPF normalizado (somente dígitos)
                member.Documents.Add(new MyChurch.Domain.Entities.MemberDocument 
                { 
                    Number = normalizedCpf // ✅ CPF sem formatação
                });
                member.PendingApproval = true; // ⏳ Aguardando aprovação do admin

                _unitOfWork.Members.Create(member);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}