using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Church.Commands.UpdateChurch;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Enum;
using Amazon.Runtime.Telemetry;

namespace MyChurch.Application.Church.Queries.GetChurch
{
    public class GetChurchByIdQuery : JwtMemberDto, IRequest<ChurchDto>
    {
        [JsonIgnore]
        public int Id { get; set; }
    }
    public class GetChurchByIdQueryHandler : IRequestHandler<GetChurchByIdQuery, ChurchDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetChurchByIdQueryHandler> _logger;

        public GetChurchByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetChurchByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ChurchDto> Handle(GetChurchByIdQuery request, CancellationToken cancellationToken)
        {
            var church = await _unitOfWork.Churchs.Query().AsNoTrackingWithIdentityResolution()
                .Include(c => c.Address)
                .Include(c => c.Members)
                .Include(c => c.Subscription)
                    .ThenInclude(x => x.Plan)
                .Include(c => c.Branches)
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.Members.Any(x => x.Id == request.UserId), cancellationToken);
            if (church == null)
            {
                _logger.LogError("Church not found");
                ValidationException.ThrowException("Get", "Church not found");
            }

            var isAdmin = request.Role == UserRole.Admin.ToString();

            var dto = new ChurchDto
            {
                Id = church.Id,
                Name = church.Name,
                Description = church.Description,
                Phone = church.Phone,
                Logo = church.LogoFileName,
                Address = AddressDto.New(church.Address),
                Subscription = church.Subscription != null ? SubscriptionDto.New(church.Subscription) : null,
                OnboardingQrCode = isAdmin ? church.OnboardingQrCode : null,
                ParentChurchId = church.ParentChurchId,
                BranchesCount = church.Branches?.Count ?? 0,
                AllowedBranches = church.Subscription?.Plan?.Branches ?? 0,
                DefaultBankingInfoId = isAdmin ? church.DefaultBankingInfoId : null
            };

            if (!isAdmin)
            {
                dto.Members = null;
                dto.Subscription = null;
            }
            else
            {
                // If user is admin, include banking information
                var bankingInfos = await _unitOfWork.BankingInfos.Query()
                    .Where(b => b.ChurchId == church.Id)
                    .OrderByDescending(b => b.Created)
                    .ToListAsync(cancellationToken);

                dto.BankingInfos = bankingInfos.Select(BankingInfoDto.New).ToList();

                dto.DefaultBankingInfo = dto.DefaultBankingInfoId.HasValue
                    ? dto.BankingInfos.FirstOrDefault(b => b.Id == dto.DefaultBankingInfoId.Value)
                    : null;

                // Backward compatibility
                dto.BankingInfo = dto.DefaultBankingInfo ?? dto.BankingInfos.FirstOrDefault();
            }

            return dto;
        }
    }
}
