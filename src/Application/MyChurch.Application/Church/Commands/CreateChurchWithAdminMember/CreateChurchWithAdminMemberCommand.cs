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

        /// <summary>Nome da Igreja</summary>
        /// <example>Igreja Pentecostal</example>
        public string Name { get; set; }

        /// <summary>Descrição da Igreja</summary>
        /// <example>Igreja Pentecostal</example>
        public string Description { get; set; }

        /// <summary>Telefone da Igreja</summary>
        /// <example>19987250777</example>
        public string Phone { get; set; }

        /// <summary>ID do Plano</summary>
        /// <example>1</example>
        public int PlanId { get; set; }

        /// <summary>BillingType</summary>
        /// <example>PIX</example>
        public string BillingType { get; set; } // Ex:"PIX", "CREDIT_CARD"

        /// <summary>Logo da Igreja (Base64)</summary>
        /// <example>Base64</example>
        public string? Logo { get; set; }

        public AddressChurchWithAdminCreate Address { get; set; }

        // Dados do Admin

        /// <summary>Nome do Administrador</summary>
        /// <example>João da Silva</example>
        public string AdminName { get; set; }

        /// <summary>Email do Administrador</summary>
        /// <example>joao@email.com</example>
        public string? AdminEmail { get; set; }

        /// <summary>Documentos do Administrador</summary>
        public List<MemberDocumentDto> AdminDocuments { get; set; } = new();

        /// <summary>Foto do Administrador (Base64)</summary>
        /// <example>Base64</example>
        public string? AdminPhoto { get; set; }
        public string Document { get; set; }

        /// <summary>Telefone do Administrador</summary>
        /// <example>19999999999</example>
        public string AdminPhone { get; set; }

        /// <summary>Data de Nascimento do Administrador</summary>
        /// <example>1990-01-01</example>
        public DateTime AdminBirthDate { get; set; }

        /// <summary>Administrador é Batizado?</summary>
        /// <example>true</example>
        public bool AdminIsBaptized { get; set; }

        /// <summary>Data do Batismo do Administrador</summary>
        /// <example>2010-05-20</example>
        public DateTime? AdminBaptizedDate { get; set; }

        /// <summary>Administrador é Dizimista?</summary>
        /// <example>true</example>
        public bool AdminIsTither { get; set; }

        /// <summary>Senha do Administrador</summary>
        /// <example>SenhaForte123!</example>
        public string AdminPassword { get; set; }

        // Adicione os campos para cartão de crédito
        public CreditCardDto? CreditCard { get; set; }
        public CreditCardHolderInfoDto? CreditCardHolderInfo { get; set; }

        public class AddressChurchWithAdminCreate
        {
            /// <summary>Rua</summary>
            /// <example>Piracicaba</example>
            public string Street { get; set; }

            /// <summary>Cidade</summary>
            /// <example>Araras</example>
            public string City { get; set; }

            /// <summary>Estado</summary>
            /// <example>SP</example>
            public string State { get; set; }

            /// <summary>CEP</summary>
            /// <example>13609090</example>
            public string ZipCode { get; set; }

            /// <summary>País</summary>
            /// <example>Brasil</example>
            public string Country { get; set; }

            /// <summary>Bairro</summary>
            /// <example>São João</example>
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

            // 1. Cria a entidade Address com todos os campos
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

            // 2. Cria a entidade Church com todos os campos disponíveis
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
                // Novo: lista de documentos
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

            string? checkoutUrl = null;

            // Se o plano selecionado não for o Descubra (ID 1), já inicia o fluxo de contratação real
            var subscriptionCommand = new MyChurch.Application.Subscription.Commands.CreateSubscription.CreateSubscriptionCommand
            {
                UserId = adminMember.Id,
                PlanId = request.PlanId,
                BillingType = request.BillingType, // ou outro tipo conforme sua lógica
                FirstPaymentDate = DateTime.UtcNow
            };

            // Chama o fluxo de assinatura e obtém o link de checkout
            var checkoutResult = await _sender.Send(new CreateSubscriptionCommand()
            {
                BillingType = request.BillingType,
                Email = request.AdminEmail,
                FirstPaymentDate = DateTime.UtcNow,
                PlanId = request.PlanId,
                Role = "Admin",
                UserId = adminMember.Id,
                CreditCard = request.CreditCard,
                CreditCardHolderInfo = request.CreditCardHolderInfo
            }, cancellationToken);


            _logger.LogInformation("Igreja criada com ID: {ChurchId} e admin com ID: {AdminId}", church.Id, adminMember.Id);

            return new CreateChurchWithAdminResultDto
            {
                ChurchId = church.Id,
                CheckoutUrl = checkoutResult.CheckoutUrl,
                PixQrCode = checkoutResult.PixQrCode,
                Payload = checkoutResult.Payload

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