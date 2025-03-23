using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Commands.UpdateChurch
{
    public class UpdateChurchCommand : IRequest<ChurchDto>
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
        private readonly IUnitOfWork _repository;
        private readonly ILogger<UpdateChurchCommandHandler> _logger;
        public UpdateChurchCommandHandler(IUnitOfWork repository, ILogger<UpdateChurchCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ChurchDto> Handle(UpdateChurchCommand request, CancellationToken cancellationToken)
        {
            var churchToUpdate = await _repository.Churchs.Query().Include(church => church.Address).FirstOrDefaultAsync(church => church.Id == request.Id);

            if (churchToUpdate is null)
            {
                ValidationException.ThrowException("Church", "This Church Id Does not Exist!");
            }

            churchToUpdate.Update(name: request.Name, phone: request.Phone);
            if (request.Address is not null)
            {
                churchToUpdate.Address.Update(
                    street: request.Address.Street ?? null,
                    city: request.Address.City ?? null,
                    zipCode: request.Address.ZipCode ?? null,
                    country: request.Address.Country ?? null,
                    neighborhood: request.Address.Neighborhood ?? null,
                    state: request.Address.State ?? null);
            }

            _repository.Churchs.Update(churchToUpdate);
            await _repository.CommitAsync();

            return ChurchDto.New(churchToUpdate);
        }
    }
}
