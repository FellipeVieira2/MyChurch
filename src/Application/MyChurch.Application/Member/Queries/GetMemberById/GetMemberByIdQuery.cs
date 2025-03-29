using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Church.Queries.GetChurch;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Member.Queries.GetMemberById
{
    public class GetMemberByIdQuery : JwtMemberDto, IRequest<MemberDto>
    {
        [JsonIgnore]
        public int Id { get; set; }
    }
    public class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, MemberDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetMemberByIdQuery> _logger;

        public GetMemberByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetMemberByIdQuery> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<MemberDto> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query().AsNoTrackingWithIdentityResolution()
                .Include(m => m.Church)
                .FirstOrDefaultAsync(m => m.Id == request.Id && m.Church.Members.Any(x => x.Id == request.UserId), cancellationToken);
            if (member == null)
            {
                _logger.LogError("Member not found");
                ValidationException.ThrowException("Get", "Member not found");
            }
            return MemberDto.New(member);
        }
    }

}
