using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Church.Commands.CreateChurchCommand
{
    public class CreateChurchCommand : IRequest<int>
    {
        /// <summary>Name</summary>
        /// <example>Igreja Pentecostal</example>
        public string Name { get; set; }
        /// <summary>Phone</summary>
        /// <example>19987250777</example>
        public string Phone { get; set; }
        /// <summary>PlanId</summary>
        /// <example>1</example>
        public int PlanId { get; set; }
        /// <summary>Logo</summary>
        /// <example>Base64</example>
        public int Logo { get; set; }
        public AddressChurch Address { get; set; }

        public class AddressChurch
        {
            /// <summary>Street</summary>
            /// <example>Piracicaba</example>
            public string Street { get; set; }
            /// <summary>City</summary>
            /// <example>Araras</example>
            public string City { get; set; }
            /// <summary>State</summary>
            /// <example>SP</example>
            public string State { get; set; }
            /// <summary>ZipCode</summary>
            /// <example>13609090</example>
            public string ZipCode { get; set; }
            /// <summary>Country</summary>
            /// <example>Brasil</example>
            public string Country { get; set; }
            /// <summary>Neighborhood</summary>
            /// <example>São João</example>
            public string Neighborhood { get; set; }
        }
    }
    public class CreateChurchCommandHandler : IRequestHandler<CreateChurchCommand, int>
    {
        private readonly IUnitOfWork _repository;
        private readonly ILogger<CreateChurchCommandHandler> _logger;
        public CreateChurchCommandHandler(IUnitOfWork repository, ILogger<CreateChurchCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<int> Handle(CreateChurchCommand request, CancellationToken cancellationToken)
        {
            var churchToSave = new Domain.Entities.Church(
                name: request.Name,
                address: new Address(street: request.Address.Street,
                                     city: request.Address.City,
                                     state: request.Address.State,
                                     zipCode: request.Address.ZipCode,
                                     country: request.Address.Country,
                                     neighborhood: request.Address.Neighborhood),
                phone: request.Phone,
                subscription: new Subscription(planId: request.PlanId,
                                               startDate: DateTime.Now,
                                               DateTime.Now.AddMonths(1)));

            _repository.Churchs.Create(churchToSave);
            await _repository.CommitAsync();

            return churchToSave.Id;
        }
    }
}
