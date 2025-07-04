using FluentValidation;
using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Infrastructure.Utils.Extensions;
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
        public string Password { get; set; }
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
                RuleFor(x => x.Address).NotNull();
            }
        }

        public class Handler : IRequestHandler<RegisterForApprovalCommand>
        {
            private readonly IUnitOfWork _unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task Handle(RegisterForApprovalCommand request, CancellationToken cancellationToken)
            {
                var exists = _unitOfWork.Members.Query().Any(m => m.Documents.Any(d => d.Number == request.Cpf));
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
                    IsActive = false,
                    Address = new MyChurch.Domain.Entities.Address(
                        request.Address.Street,
                        request.Address.City,
                        request.Address.State,
                        request.Address.ZipCode,
                        request.Address.Country,
                        request.Address.Neighborhood
                    )
                };

                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    var hash = Guid.NewGuid().ToString("N");
                    member.PasswordHash = hash;
                    member.Password = request.Password.Encrypt(hash);
                }
                member.Documents.Add(new MyChurch.Domain.Entities.MemberDocument { Number = request.Cpf });
                member.PendingApproval = true;

                _unitOfWork.Members.Create(member);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}