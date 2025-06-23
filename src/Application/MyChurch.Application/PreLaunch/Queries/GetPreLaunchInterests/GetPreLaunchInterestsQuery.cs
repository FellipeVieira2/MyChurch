using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.PreLaunch.Queries.GetPreLaunchInterests
{
    public class GetPreLaunchInterestsQuery : IRequest<List<PreLaunchInterestDto>>
    {
        public bool? OnlyConfirmed { get; set; }
    }

    public class GetPreLaunchInterestsQueryHandler : IRequestHandler<GetPreLaunchInterestsQuery, List<PreLaunchInterestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetPreLaunchInterestsQueryHandler> _logger;

        public GetPreLaunchInterestsQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetPreLaunchInterestsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<PreLaunchInterestDto>> Handle(GetPreLaunchInterestsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting pre-launch interests. OnlyConfirmed: {OnlyConfirmed}", request.OnlyConfirmed);

            var query = _unitOfWork.PreLaunchInterests.Query();

            if (request.OnlyConfirmed.HasValue)
            {
                query = query.Where(i => i.IsEmailConfirmed == request.OnlyConfirmed.Value);
            }

            var interests = await query
                .OrderByDescending(i => i.RegisterDate)
                .ToListAsync(cancellationToken);

            return interests.Select(i => new PreLaunchInterestDto
            {
                Id = i.Id,
                Name = i.Name,
                Email = i.Email,
                Phone = i.Phone,
                ChurchName = i.ChurchName,
                ChurchRole = i.ChurchRole,
                Comments = i.Comments,
                RegisterDate = i.RegisterDate,
                IsEmailConfirmed = i.IsEmailConfirmed
            }).ToList();
        }
    }
}