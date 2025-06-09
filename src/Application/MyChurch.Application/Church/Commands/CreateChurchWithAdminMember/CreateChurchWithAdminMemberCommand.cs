using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mychurch.Common.WebClients.Asaas;
using Mychurch.Common.WebClients.Asaas.Models.Requests;
using MyChurch.Application.Subscription.Commands.CreateSubscription;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure.Utils.Extensions;
using MyChurch.Infrastructure.Utils.S3;
using System.Collections.Generic;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Church.Commands.CreateChurchWithAdminMember
{
    public class CreateChurchWithAdminMemberCommand : IRequest<CreateChurchWithAdminResultDto>
    {
        // Dados da Igreja
        public string Name { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public int PlanId { get; set; }
        public string BillingType { get; set; } // Ex:"PIX", "CREDIT_CARD"
        public string? Logo { get; set; }
        public AddressChurchWithAdminCreate Address { get; set; }

        // Dados do Admin
        public string AdminName { get; set; }
        public string? AdminEmail { get; set; }
        public List<MemberDocumentDto> AdminDocuments { get; set; } = new();
        public string? AdminPhoto { get; set; }
        public string Document { get; set; }
        public string AdminPhone { get; set; }
        public DateTime AdminBirthDate { get; set; }
        public bool AdminIsBaptized { get; set; }
        public DateTime? AdminBaptizedDate { get; set; }
        public bool AdminIsTither { get; set; }
        public string AdminPassword { get; set; }

        // Cartão de crédito
        public CreditCardDto? CreditCard { get; set; }
        public CreditCardHolderInfoDto? CreditCardHolderInfo { get; set; }
        public int? CreditCardInfoId { get; set; } // Novo: permite usar cartão já cadastrado

        public class AddressChurchWithAdminCreate
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
            public string Country { get; set; }
            public string Neighborhood { get; set; }
            public string Number { get; set; }
        }
    }

    public class CreateChurchWithAdminMemberCommandHandler : IRequestHandler<CreateChurchWithAdminMemberCommand, CreateChurchWithAdminResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateChurchWithAdminMemberCommandHandler> _logger;
        private readonly IS3Helper _s3Helper;
        private readonly IAsaasWebClient _asaasWebClient;
        private readonly ISender _sender;

        public CreateChurchWithAdminMemberCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateChurchWithAdminMemberCommandHandler> logger,
            IS3Helper s3Helper,
            IAsaasWebClient asaasWebClient,
            ISender sender)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _s3Helper = s3Helper;
            _asaasWebClient = asaasWebClient;
            _sender = sender;
        }

        public async Task<CreateChurchWithAdminResultDto> Handle(CreateChurchWithAdminMemberCommand request, CancellationToken cancellationToken)
        {
            // Validação de duplicidade de email, telefone e documentos do admin
            var exists = await _unitOfWork.Members.Query()
                .AnyAsync(m =>
                    (!string.IsNullOrEmpty(request.AdminEmail) && m.Email == request.AdminEmail) ||
                    m.Phone == request.AdminPhone ||
                    m.Documents.Any(x => request.AdminDocuments.Any(d => d.Number == x.Number)),
                    cancellationToken);

            if (exists)
                ValidationException.ThrowException("Member", "Já existe um membro cadastrado com este e-mail, telefone ou documento.");

            // 1. Cria a entidade Address
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

            // 2. Cria a entidade Church
            var church = new Domain.Entities.Church(
                name: request.Name,
                phone: request.Phone,
                address: address,
                description: request.Description
            )
            {
                Document = request.Document,
            };

            // 3. Faz upload do logo se necessário
            if (!string.IsNullOrEmpty(request.Logo))
            {
                church.LogoFileName = await UploadLogoAsync(request.Logo, cancellationToken);
            }

            // 4. Adiciona a igreja ao repositório
            _unitOfWork.Churchs.Create(church);
            await _unitOfWork.CommitAsync();

            // 5. Cria o membro admin
            var adminMember = new Domain.Entities.Member
            {
                Name = request.AdminName,
                Email = request.AdminEmail,
                Phone = request.AdminPhone,
                BirthDate = request.AdminBirthDate,
                IsBaptized = request.AdminIsBaptized,
                BaptizedDate = request.AdminBaptizedDate,
                IsTither = request.AdminIsTither,
                ChurchId = church.Id,
                Role = UserRole.Admin,
                Created = DateTime.Now,
                Documents = request.AdminDocuments?.Select(d => new MemberDocument
                {
                    Type = d.Type,
                    Number = d.Number
                }).ToList() ?? new List<MemberDocument>()
            };

            // 6. Faz upload da foto do admin se necessário
            if (!string.IsNullOrEmpty(request.AdminPhoto))
            {
                adminMember.Photo = await UploadPhotoAsync(request.AdminPhoto, cancellationToken);
            }

            // 7. Criptografa e salva a senha do admin
            if (!string.IsNullOrWhiteSpace(request.AdminPassword))
            {
                var hash = Guid.NewGuid().ToString("N");
                adminMember.PasswordHash = hash;
                adminMember.Password = request.AdminPassword.Encrypt(hash);
            }

            // 8. Adiciona o membro admin ao repositório
            _unitOfWork.Members.Create(adminMember);
            await _unitOfWork.CommitAsync();

            // 9. Cria o cliente no Asaas e salva o AsaasCustomerId
            church.AsaasCustomerId = await CreateAsaasCustomerAsync(church, request.AdminEmail, cancellationToken);
            _unitOfWork.Churchs.Update(church);
            await _unitOfWork.CommitAsync();

            // 10. Monta o comando de assinatura
            var subscriptionCommand = new CreateSubscriptionCommand
            {
                UserId = adminMember.Id,
                PlanId = request.PlanId,
                BillingType = request.BillingType,
                FirstPaymentDate = DateTime.UtcNow,
                CreditCard = request.CreditCard,
                CreditCardHolderInfo = request.CreditCardHolderInfo,
                CreditCardInfoId = request.CreditCardInfoId
            };

            // 11. Chama o fluxo de assinatura
            var checkoutResult = await _sender.Send(subscriptionCommand, cancellationToken);

            _logger.LogInformation("Igreja criada com ID: {ChurchId} e admin com ID: {AdminId}", church.Id, adminMember.Id);

            return new CreateChurchWithAdminResultDto
            {
                ChurchId = church.Id,
                CheckoutUrl = checkoutResult?.CheckoutUrl,
                PixQrCode = checkoutResult?.PixQrCode,
                Payload = checkoutResult?.Payload
            };
        }

        private async Task<string> UploadLogoAsync(string logoBase64, CancellationToken cancellationToken)
        {
            if (logoBase64.Contains(','))
                logoBase64 = logoBase64.Split(',')[1];

            var logoBytes = Convert.FromBase64String(logoBase64);
            using var logoStream = new MemoryStream(logoBytes);
            var logoUrl = await _s3Helper.UploadFileAsync(logoStream, $"{Guid.NewGuid()}logo.jpg", "image/jpeg", cancellationToken);

            return logoUrl;
        }

        private async Task<string> UploadPhotoAsync(string photoBase64, CancellationToken cancellationToken)
        {
            if (photoBase64.Contains(','))
                photoBase64 = photoBase64.Split(',')[1];

            var photoBytes = Convert.FromBase64String(photoBase64);
            using var photoStream = new MemoryStream(photoBytes);
            var photoUrl = await _s3Helper.UploadFileAsync(photoStream, $"{Guid.NewGuid()}photo.jpg", "image/jpeg", cancellationToken);

            return photoUrl;
        }
        private async Task<string> CreateAsaasCustomerAsync(Domain.Entities.Church church, string emailAdmin, CancellationToken cancellationToken)
        {
            var customer = new AsaasCustomerRequestDto
            {
                Name = church.Name,
                Email = emailAdmin,
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