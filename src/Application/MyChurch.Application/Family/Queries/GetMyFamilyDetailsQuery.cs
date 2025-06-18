using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Application.Family.Queries
{
    public class GetMyFamilyDetailsQuery : JwtMemberDto, IRequest<FamilyDetailsDto>
    {
    }

    public class GetMyFamilyDetailsQueryHandler : IRequestHandler<GetMyFamilyDetailsQuery, FamilyDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetMyFamilyDetailsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<FamilyDetailsDto> Handle(GetMyFamilyDetailsQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (member == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");
            if (member.FamilyId == null)
                return null;
            var family = await _unitOfWork.Families.Query().FirstOrDefaultAsync(f => f.Id == member.FamilyId, cancellationToken);
            if (family == null)
                return null;
            var members = await _unitOfWork.Members.Query().Where(m => m.FamilyId == family.Id).ToListAsync(cancellationToken);
            var children = await _unitOfWork.Children.Query().Where(c => c.FamilyId == family.Id).ToListAsync(cancellationToken);
            return new FamilyDetailsDto
            {
                FamilyId = family.Id,
                FamilyName = family.FamilyName,
                Members = [.. members.Select(m => new FamilyMemberDto { MemberId = m.Id, Name = m.Name, Email = m.Email })],
                Children = [.. children.Select(c => new FamilyChildDto { ChildId = c.Id, FullName = c.FullName, BirthDate = c.BirthDate, Gender = c.Gender.ToString() })]
            };
        }
    }
}
