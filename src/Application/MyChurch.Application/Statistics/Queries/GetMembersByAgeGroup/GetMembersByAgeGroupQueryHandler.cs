using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Statistics.Queries.GetMembersByAgeGroup
{
    public class GetMembersByAgeGroupQueryHandler : IRequestHandler<GetMembersByAgeGroupQuery, List<MembersByAgeGroupDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMembersByAgeGroupQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MembersByAgeGroupDto>> Handle(GetMembersByAgeGroupQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Membro não encontrado.");

            int churchId = request.ChurchId ?? member.ChurchId;

            var members = await _unitOfWork.Members.Query()
                .Where(m => m.ChurchId == churchId && m.IsActive)
                .Select(m => m.BirthDate)
                .ToListAsync(cancellationToken);

            var now = DateTime.UtcNow;
            var totalMembers = members.Count;

            var ageGroups = new List<MembersByAgeGroupDto>
            {
                new() { AgeGroup = "0-12 anos (Crianças)", Count = 0 },
                new() { AgeGroup = "13-17 anos (Adolescentes)", Count = 0 },
                new() { AgeGroup = "18-25 anos (Jovens)", Count = 0 },
                new() { AgeGroup = "26-35 anos (Jovens Adultos)", Count = 0 },
                new() { AgeGroup = "36-50 anos (Adultos)", Count = 0 },
                new() { AgeGroup = "51-65 anos (Meia-idade)", Count = 0 },
                new() { AgeGroup = "65+ anos (Terceira Idade)", Count = 0 }
            };

            foreach (var birthDate in members)
            {
                var age = now.Year - birthDate.Year;
                if (birthDate.Date > now.AddYears(-age)) age--;

                if (age <= 12) ageGroups[0].Count++;
                else if (age <= 17) ageGroups[1].Count++;
                else if (age <= 25) ageGroups[2].Count++;
                else if (age <= 35) ageGroups[3].Count++;
                else if (age <= 50) ageGroups[4].Count++;
                else if (age <= 65) ageGroups[5].Count++;
                else ageGroups[6].Count++;
            }

            foreach (var group in ageGroups)
            {
                group.Percentage = totalMembers > 0 
                    ? Math.Round(((decimal)group.Count / totalMembers) * 100, 2) 
                    : 0;
            }

            return ageGroups.Where(g => g.Count > 0).ToList();
        }
    }
}
