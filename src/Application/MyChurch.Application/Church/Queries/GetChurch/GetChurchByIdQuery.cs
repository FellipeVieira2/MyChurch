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
                .Include(c => c.Subscription)
                    .ThenInclude(x => x.Plan)
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.Members.Any(x => x.Id == request.UserId), cancellationToken);
            if (church == null)
            {
                _logger.LogError("Church not found");
                ValidationException.ThrowException("Get", "You do not have permission to update this church.");
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
                Subscription = church.Subscription != null ? SubscriptionDto.New(church.Subscription) : null
            };

            if (!isAdmin)
            {
                dto.Members = null;
                dto.Subscription = null;
            }
            else
            {
                // If user is admin, include banking information
                var bankingInfo = await _unitOfWork.BankingInfos.Query()
                    .FirstOrDefaultAsync(b => b.ChurchId == church.Id, cancellationToken);
                
                if (bankingInfo != null)
                {
                    dto.BankingInfo = BankingInfoDto.New(bankingInfo);
                }else
                {
                    dto.BankingInfo = null;
                }
            }

            return dto;
        }
    }
}
