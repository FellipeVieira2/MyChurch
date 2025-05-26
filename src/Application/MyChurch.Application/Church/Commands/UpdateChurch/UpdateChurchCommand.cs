using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Commands.UpdateChurch
{
    public class UpdateChurchCommand : JwtMemberDto, IRequest<ChurchDto>
    {
        [JsonIgnore]
        public int Id { get; set; }
        /// <summary>Name</summary>
        /// <example>Igreja Pentecostal</example>
        public string? Name { get; set; }
        /// <summary>Phone</summary>
        /// <example>19987250777</example>
        public string? Phone { get; set; }
        public AddressChurch? Address { get; set; }

        public class AddressChurch
        {
            /// <summary>Street</summary>
            /// <example>Piracicaba</example>
            public string? Street { get; set; }
            /// <summary>City</summary>
            /// <example>Araras</example>
            public string? City { get; set; }
            /// <summary>State</summary>
            /// <example>SP</example>
            public string? State { get; set; }
            /// <summary>ZipCode</summary>
            /// <example>13609090</example>
            public string? ZipCode { get; set; }
            /// <summary>Country</summary>
            /// <example>Brasil</example>
            public string? Country { get; set; }
            /// <summary>Neighborhood</summary>
            /// <example>São João</example>
            public string? Neighborhood { get; set; }
        }

    }
    public class UpdateChurchCommandHandler : IRequestHandler<UpdateChurchCommand, ChurchDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateChurchCommandHandler> _logger;

        public UpdateChurchCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateChurchCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ChurchDto> Handle(UpdateChurchCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null || member.ChurchId != request.Id)
            {
                _logger.LogWarning("User does not have permission to update this church.");
                ValidationException.ThrowException("Update","You do not have permission to update this church.");
            }

            var church = await _unitOfWork.Churchs.Query().FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (church == null)
            {
                _logger.LogWarning("Church not found with ID: {Id}", request.Id);
                ValidationException.ThrowException("Update", "Church not found.");
            }

            church.Update(request.Name, request.Phone);
            if (request.Address != null)
            {
                church.Address.Update(request.Address.Street, request.Address.City, request.Address.ZipCode, request.Address.Country, request.Address.Neighborhood, request.Address.State);
            }

            _unitOfWork.Churchs.Update(church);
            await _unitOfWork.CommitAsync();

            return ChurchDto.New(church);
        }
    }
}
