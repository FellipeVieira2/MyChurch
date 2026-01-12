using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Journey.Commands
{
    public class CreateJourneyCommand : JwtMemberDto, IRequest<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public List<CreateJourneyStageDto> Stages { get; set; } = new List<CreateJourneyStageDto>();
    }

    public class CreateJourneyStageDto
    {
        public string Title { get; set; }
        public JourneyStageType Type { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public int FaithPointsAwarded { get; set; }
    }

    public class CreateJourneyCommandHandler : IRequestHandler<CreateJourneyCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateJourneyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateJourneyCommand request, CancellationToken cancellationToken)
        {
            var journey = new Domain.Entities.Journey
            {
                Title = request.Title,
                Description = request.Description,
                IconUrl = request.IconUrl,
                IsActive = request.IsActive,
                IsDefault = request.IsDefault
            };

            foreach (var stageDto in request.Stages)
            {
                journey.Stages.Add(new JourneyStage
                {
                    Title = stageDto.Title,
                    Type = stageDto.Type,
                    Content = stageDto.Content,
                    Order = stageDto.Order,
                    FaithPointsAwarded = stageDto.FaithPointsAwarded
                });
            }

            await _unitOfWork.Journeys.Create(journey);
            await _unitOfWork.CommitAsync();

            return journey.Id;
        }
    }
}
