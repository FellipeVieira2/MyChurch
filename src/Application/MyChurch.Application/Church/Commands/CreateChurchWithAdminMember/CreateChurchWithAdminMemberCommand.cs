using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mychurch.Common.WebClients.Asaas;
using Mychurch.Common.WebClients.Asaas.Models.Requests;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Services;
using MyChurch.Infrastructure.Utils.S3;
using System.Collections.Generic;
using MyChurch.Application.Dtos;
using MyChurch.Infrastructure;
using QRCoder;

namespace MyChurch.Application.Church.Commands.CreateChurchWithAdminMember
{
    public class CreateChurchWithAdminMemberCommand : IRequest<CreateChurchWithAdminResultDto>
    {
        // Dados da Igreja
        public string Name { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public string? Logo { get; set; }
        public AddressChurchWithAdminCreate Address { get; set; }

        // Dados do Admin
        public string AdminName { get; set; }
        public string? AdminEmail { get; set; }
        public List<MemberDocumentDtoCreateChurch> AdminDocuments { get; set; } = new();
        public string? AdminPhoto { get; set; }
        public string Document { get; set; }
        public string AdminPhone { get; set; }
        public DateTime AdminBirthDate { get; set; }
        public bool AdminIsBaptized { get; set; }
        public DateTime? AdminBaptizedDate { get; set; }
        public bool AdminIsTither { get; set; }
        public string AdminPassword { get; set; }
        public string? AdminBirthCity { get; set; } // Novo: cidade de nascimento do admin
        public string? AdminBirthState { get; set; } // Novo: estado de nascimento do admin
        public string? Ministry { get; set; }
        public DateTime MemberSince { get; set; }
        public string? Notes { get; set; }
        public AddressChurchWithAdminCreate AdminAddress { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        
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

        public class MemberDocumentDtoCreateChurch
        {
            public MemberDocumentType Type { get; set; } // Ex: "CPF", "RG", "Título de Eleitor"
            public string Number { get; set; }
        }
    }

    public class CreateChurchWithAdminMemberCommandHandler : IRequestHandler<CreateChurchWithAdminMemberCommand, CreateChurchWithAdminResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateChurchWithAdminMemberCommandHandler> _logger;
        private readonly IS3Helper _s3Helper;
        private readonly IAsaasWebClient _asaasWebClient;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDocumentValidator _documentValidator;

        public CreateChurchWithAdminMemberCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateChurchWithAdminMemberCommandHandler> logger,
            IS3Helper s3Helper,
            IAsaasWebClient asaasWebClient,
            IPasswordHasher passwordHasher,
            IDocumentValidator documentValidator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _s3Helper = s3Helper;
            _asaasWebClient = asaasWebClient;
            _passwordHasher = passwordHasher;
            _documentValidator = documentValidator;
        }

        public async Task<CreateChurchWithAdminResultDto> Handle(CreateChurchWithAdminMemberCommand request, CancellationToken cancellationToken)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    ValidateAdminDocuments(request, cancellationToken);
                    ValidateChurchDocument(request);

                    var address = CreateAddress(request.Address);
                    var church = await CreateChurchAsync(request, address, cancellationToken);
                    await SeedDefaultJourneysAsync(church.Id);
                    var adminMember = await CreateAdminMemberAsync(request, church.Id, cancellationToken);
                    await SaveChurchAndAdminAsync(church, adminMember, cancellationToken);
                    await UpdateChurchWithAsaasCustomerIdAsync(church, request.AdminEmail, cancellationToken);
                    
                    // Gera o QRCode de onboarding
                    var onboardingUrl = $"https://www.mychurchlab.net/onboarding?church={church.Id}";
                    church.OnboardingQrCode = GenerateQrCodeBase64(onboardingUrl);
                    _unitOfWork.Churchs.Update(church);
                    await _unitOfWork.CommitAsync();

                    // 🆓 Criar assinatura gratuita automaticamente (plano Free - ID 1)
                    await CreateFreeSubscriptionAsync(church.Id, cancellationToken);

                    await _unitOfWork.CommitTransactionAsync();

                    _logger.LogInformation("Igreja criada com ID: {ChurchId} e admin com ID: {AdminId} com plano gratuito", church.Id, adminMember.Id);

                    return new CreateChurchWithAdminResultDto
                    {
                        ChurchId = church.Id,
                        CheckoutUrl = null,
                        PixQrCode = null,
                        Payload = null
                    };
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
        }

        private static string OnlyDigits(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return new string(input.Where(char.IsDigit).ToArray());
        }

        private void ValidateAdminDocuments(CreateChurchWithAdminMemberCommand request, CancellationToken cancellationToken)
        {
            // Usar o DocumentValidator para normalizar os documentos
            var adminDocumentNumbers = request.AdminDocuments?
                .Select(d => _documentValidator.RemoveFormatting(d.Number))
                .ToList() ?? new List<string>();
                
            var existingDocuments = _unitOfWork.MemberDocuments
                .Query()
                .Where(x => adminDocumentNumbers.Contains(x.Number))
                .Select(x => x.Number)
                .ToList();
                
            if (existingDocuments.Any())
            {
                ValidationException.ThrowException("Church", "Document Already Used");
            }
        }

        private void ValidateChurchDocument(CreateChurchWithAdminMemberCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.Document) || _unitOfWork.Churchs.Query().Any(x => x.Document == request.Document))
            {
                ValidationException.ThrowException("Church", "Church Document error");
            }
        }

        private Address CreateAddress(CreateChurchWithAdminMemberCommand.AddressChurchWithAdminCreate addressDto)
        {
            var address = new Address(
                street: addressDto.Street,
                city: addressDto.City,
                state: addressDto.State,
                zipCode: addressDto.ZipCode,
                country: addressDto.Country,
                neighborhood: addressDto.Neighborhood
            )
            {
                Number = addressDto.Number,
                Complement = "N/A"
            };
            return address;
        }

        private async Task<Domain.Entities.Church> CreateChurchAsync(CreateChurchWithAdminMemberCommand request, Address address, CancellationToken cancellationToken)
        {
            var church = new Domain.Entities.Church(
                name: request.Name,
                phone: request.Phone,
                address: address,
                description: request.Description
            )
            {
                Document = OnlyDigits(request.Document),
            };
            if (!string.IsNullOrEmpty(request.Logo))
            {
                church.LogoFileName = await UploadLogoAsync(request.Logo, cancellationToken);
            }
            await _unitOfWork.Churchs.Create(church);
            await _unitOfWork.CommitAsync();
            return church;
        }

        private async Task SeedDefaultJourneysAsync(int churchId)
        {
            var defaultJourneys = JourneySeedData.GetDefaultJourneys(churchId);
            foreach (var journey in defaultJourneys)
            {
                await _unitOfWork.Journeys.Create(journey);
            }
        }

        private async Task<Domain.Entities.Member> CreateAdminMemberAsync(CreateChurchWithAdminMemberCommand request, int churchId, CancellationToken cancellationToken)
        {
            var adminMember = new Domain.Entities.Member
            {
                Name = request.AdminName,
                Email = request.AdminEmail,
                Phone = request.AdminPhone,
                BirthDate = request.AdminBirthDate,
                IsBaptized = request.AdminIsBaptized,
                BaptizedDate = request.AdminBaptizedDate,
                IsTither = request.AdminIsTither,
                ChurchId = churchId,
                Role = UserRole.Admin,
                Created = DateTime.Now,
                BirthCity = request.AdminBirthCity,
                BirthState = request.AdminBirthState,
                Ministry = request.Ministry,
                MaritalStatus = request.MaritalStatus,
                Notes = request.Notes,
                MemberSince = request.MemberSince,
                Address = CreateAddress(request.AdminAddress),
                // Normalizar documentos do admin
                Documents = request.AdminDocuments?.Select(d => new MemberDocument
                {
                    Type = d.Type,
                    Number = _documentValidator.RemoveFormatting(d.Number)
                }).ToList() ?? []
            };
            
            if (!string.IsNullOrEmpty(request.AdminPhoto))
            {
                adminMember.Photo = await UploadPhotoAsync(request.AdminPhoto, cancellationToken);
            }
            
            // 🔐 SEGURANÇA: Gerar hash BCrypt da senha usando IPasswordHasher
            if (!string.IsNullOrWhiteSpace(request.AdminPassword))
            {
                adminMember.PasswordHash = _passwordHasher.HashPassword(request.AdminPassword);
            }
            
            return adminMember;
        }

        private async Task SaveChurchAndAdminAsync(Domain.Entities.Church church, Domain.Entities.Member adminMember, CancellationToken cancellationToken)
        {
            await _unitOfWork.Members.Create(adminMember);
            await _unitOfWork.CommitAsync();
        }

        private async Task UpdateChurchWithAsaasCustomerIdAsync(Domain.Entities.Church church, string? adminEmail, CancellationToken cancellationToken)
        {
            church.AsaasCustomerId = await CreateAsaasCustomerAsync(church, adminEmail, cancellationToken);
            _unitOfWork.Churchs.Update(church);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Cria uma assinatura gratuita de 30 dias para a igreja
        /// </summary>
        private async Task CreateFreeSubscriptionAsync(int churchId, CancellationToken cancellationToken)
        {
            // Busca o plano gratuito (assumindo que ID = 1 é o plano Free)
            var freePlan = await _unitOfWork.Plans.Query()
                .FirstOrDefaultAsync(p => p.Price == 0, cancellationToken);

            if (freePlan == null)
            {
                _logger.LogWarning("Plano gratuito não encontrado. Criando assinatura com período de teste.");
                // Se não houver plano gratuito, pega qualquer plano e cria período de teste
                freePlan = await _unitOfWork.Plans.Query()
                    .OrderBy(p => p.Price)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (freePlan == null)
            {
                throw new InvalidOperationException("Nenhum plano disponível no sistema.");
            }

            var subscription = new Domain.Entities.Subscription(
                planId: freePlan.Id,
                startDate: DateTime.UtcNow,
                endDate: DateTime.UtcNow.AddMonths(1) // 30 dias de período gratuito
            )
            {
                ChurchId = churchId,
                Created = DateTime.UtcNow,
                ExternalReference = null // Não tem referência externa pois é gratuito
            };

            await _unitOfWork.Subscriptions.Create(subscription);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Assinatura gratuita criada para a igreja {ChurchId} no plano {PlanId} ({PlanName})", 
                churchId, freePlan.Id, freePlan.Name);
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

        private string GenerateQrCodeBase64(string url)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);
            return "data:image/png;base64," + Convert.ToBase64String(qrCodeBytes);
        }
    }
}