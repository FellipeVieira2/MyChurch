using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Mychurch.Common.WebClients.Asaas;
using Mychurch.Common.WebClients.Models.Requests;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Infrastructure.Utils.S3;
using System.Text;

namespace MyChurch.Application.Church.Commands.CreateChurchCommand
{
    public class CreateChurchCommand : IRequest<int>
    {
        /// <summary>Name</summary>
        /// <example>Igreja Pentecostal</example>
        public string Name { get; set; }
        /// <summary>Description</summary>
        /// <example>Igreja Pentecostal</example>
        public string Description { get; set; }
        /// <summary>Phone</summary>
        /// <example>19987250777</example>
        public string Phone { get; set; }
        /// <summary>PlanId</summary>
        /// <example>1</example>
        public int PlanId { get; set; }
        /// <summary>Document</summary>
        public string Document { get; set; }
        /// <summary>Logo</summary>
        /// <example>Base64</example>
        public string? Logo { get; set; }

        public AddressChurchCreate Address { get; set; }

        public class AddressChurchCreate
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
            /// <summary>Number</summary>
            public string Number { get; set; }

        }
    }

    public class CreateChurchCommandHandler : IRequestHandler<CreateChurchCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateChurchCommandHandler> _logger;
        private readonly IS3Helper _s3Helper;
        private readonly IAsaasWebClient _asaasWebClient;
        public CreateChurchCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateChurchCommandHandler> logger, IS3Helper s3Helper, IAsaasWebClient asaasWebClient)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _s3Helper = s3Helper;
            _asaasWebClient = asaasWebClient;
        }

        public async Task<int> Handle(CreateChurchCommand request, CancellationToken cancellationToken)
        {
            var church = MapToChurchEntity(request);

            if (!string.IsNullOrEmpty(request.Logo))
            {
                church.LogoFileName = await UploadLogoAsync(request.Logo, cancellationToken);
            }

            _unitOfWork.Churchs.Create(church);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Church created with ID: {ChurchId}", church.Id);

            return church.Id;
        }

        private static Domain.Entities.Church MapToChurchEntity(CreateChurchCommand request)
        {
            var address = new Address(
                street: request.Address.Street,
                city: request.Address.City,
                state: request.Address.State,
                zipCode: request.Address.ZipCode,
                country: request.Address.Country,
                neighborhood: request.Address.Neighborhood
            )
            {
                Number = request.Address.Number
            };

            var church = new Domain.Entities.Church(
                name: request.Name,
                phone: request.Phone,
                address: address,
                description: request.Description
            )
            {
                Document = request.Document,
            };

            return church;
        }

        private async Task<string> UploadLogoAsync(string logoBase64, CancellationToken cancellationToken)
        {
            if (logoBase64.Contains(','))
            {
                logoBase64 = logoBase64.Split(',')[1];
            }

            var logoBytes = Convert.FromBase64String(logoBase64);
            using var logoStream = new MemoryStream(logoBytes);
            var logoUrl = await _s3Helper.UploadFileAsync(logoStream, $"{Guid.NewGuid()}logo.jpg", "image/jpeg", cancellationToken);

            return logoUrl;
        }

        private async Task<string> CreateAsaasCustomerAsync(Domain.Entities.Church church, CancellationToken cancellationToken)
        {
            var customer = new AsaasCustomerRequestDto
            {
                Name = church.Name,
                MobilePhone = church.Phone,
                AddressNumber = church.Address.Number,
                CpfCnpj = church.Document,
                ExternalReference = church.Id.ToString(),
                PostalCode = church.Address.ZipCode,

            };
            var asaasCustomer = await _asaasWebClient.CriarClienteAsync(customer);
            return asaasCustomer.Id;
        }
    }
}

