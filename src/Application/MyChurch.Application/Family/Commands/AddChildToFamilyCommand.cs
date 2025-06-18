using MediatR;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Application.Family.Commands
{
    public class AddChildToFamilyCommand : JwtMemberDto, IRequest<int>
    {
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }

        public class Handler : IRequestHandler<AddChildToFamilyCommand, int>
        {
            private readonly IUnitOfWork _unitOfWork;
            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<int> Handle(AddChildToFamilyCommand request, CancellationToken cancellationToken)
            {
                var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
                if (member == null)
                    ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
                if (member.FamilyId == null)
                    ValidationException.ThrowException("Family", "Usuário não está vinculado a uma família.");
                var child = new Child
                {
                    FamilyId = member.FamilyId.Value,
                    FullName = request.FullName,
                    BirthDate = request.BirthDate,
                    Gender = request.Gender,
                    IsActive = true
                };
                _unitOfWork.Children.Create(child);
                await _unitOfWork.CommitAsync();
                return child.Id;
            }
        }
    }
}
