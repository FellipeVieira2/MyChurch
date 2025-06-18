using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Onboarding.RegisterForApproval
{
    public class RegisterForApprovalCommandHandler : IRequestHandler<RegisterForApprovalCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterForApprovalCommandHandler(IUnitOfWork unitOfWork)
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
            member.MarkAsPendingApproval();
            member.Documents.Add(new MyChurch.Domain.Entities.MemberDocument { Number = request.Cpf });
            _unitOfWork.Members.Create(member);
            await _unitOfWork.CommitAsync();
        }
    }
}