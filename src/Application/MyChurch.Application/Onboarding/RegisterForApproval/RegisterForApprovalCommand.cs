using MediatR;
using FluentValidation;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
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
        public AddressDto Address { get; set; }

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
                    throw new ValidationException("Já existe um membro com este CPF.");
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
                member.Documents.Add(new MyChurch.Domain.Entities.MemberDocument { Number = request.Cpf });
                // Seta o PendingApproval via reflexão já que o set é privado
                typeof(MyChurch.Domain.Entities.Member).GetProperty("PendingApproval")?.SetValue(member, true);
                _unitOfWork.Members.Create(member);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}