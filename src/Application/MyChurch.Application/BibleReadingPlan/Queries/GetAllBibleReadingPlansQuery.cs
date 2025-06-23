using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.BibleReadingPlan.Queries
{
    public class GetAllBibleReadingPlansQuery : JwtMemberDto, IRequest<List<BibleReadingPlanDto>>
    {
        public int? ChurchId { get; set; }
        public bool IncludeStages { get; set; } = false;
    }

    public class GetAllBibleReadingPlansQueryHandler : IRequestHandler<GetAllBibleReadingPlansQuery, List<BibleReadingPlanDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllBibleReadingPlansQueryHandler> _logger;

        public GetAllBibleReadingPlansQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllBibleReadingPlansQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<BibleReadingPlanDto>> Handle(GetAllBibleReadingPlansQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all Bible reading plans for ChurchId: {ChurchId}", request.ChurchId);

            // Se o ChurchId não foi explicitamente definido, use o ChurchId do membro logado
            var churchId = request.ChurchId;
            if (!churchId.HasValue && request.UserId > 0)
            {
                var member = await _unitOfWork.Members.Query()
                    .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
                
                if (member != null)
                {
                    churchId = member.ChurchId;
                }
            }

            var plans = await _unitOfWork.BibleReadingPlans.GetAllDefaultAndPublicPlansAsync(churchId);

            var planDtos = plans.Select(p => new BibleReadingPlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                DurationInDays = p.DurationInDays,
                IsDefault = p.IsDefault,
                IsPublic = p.IsPublic,
                ChurchId = p.ChurchId,
                Created = p.Created,
                Stages = request.IncludeStages ? p.BibleReadingPlanStages.Select(s => new BibleReadingPlanStageDto
                {
                    Id = s.Id,
                    BibleReadingPlanId = s.BibleReadingPlanId,
                    Order = s.Order,
                    Description = s.Description,
                    VerseReferences = s.VerseReferences
                })
                .OrderBy(s => s.Order)
                .ToList() : new List<BibleReadingPlanStageDto>()
            }).ToList();

            return planDtos;
        }
    }
}