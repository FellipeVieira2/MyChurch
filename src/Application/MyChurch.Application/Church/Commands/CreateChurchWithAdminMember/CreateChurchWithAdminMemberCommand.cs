using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Infrastructure.Utils.Extensions;
using MyChurch.Infrastructure.Utils.S3;


namespace MyChurch.Application.Church.Commands.CreateChurchWithAdminMember
{
    public class CreateChurchWithAdminMemberCommand : IRequest<int>
    {
        // Dados da Igreja
        public string Name { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public int PlanId { get; set; }
        public string? Logo { get; set; }
        public AddressChurchWithAdminCreate Address { get; set; }

        // Dados do Admin
        public string AdminName { get; set; }
        public string? AdminEmail { get; set; }
        public string AdminDocument { get; set; }
        public string? AdminPhoto { get; set; }
        public string AdminPhone { get; set; }
        public DateTime AdminBirthDate { get; set; }
        public bool AdminIsBaptized { get; set; }
        public DateTime? AdminBaptizedDate { get; set; }
        public bool AdminIsTither { get; set; }
        public string AdminPassword { get; set; } // NOVO: senha do admin

        public class AddressChurchWithAdminCreate
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
            public string Country { get; set; }
            public string Neighborhood { get; set; }
        }
    }

    public class CreateChurchWithAdminMemberCommandHandler : IRequestHandler<CreateChurchWithAdminMemberCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateChurchWithAdminMemberCommandHandler> _logger;
        private readonly IS3Helper _s3Helper;

        public CreateChurchWithAdminMemberCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateChurchWithAdminMemberCommandHandler> logger,
            IS3Helper s3Helper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _s3Helper = s3Helper;
        }

        public async Task<int> Handle(CreateChurchWithAdminMemberCommand request, CancellationToken cancellationToken)
        {
            // 1. Cria a entidade Address
            var address = new Address(
                street: request.Address.Street,
                city: request.Address.City,
                state: request.Address.State,
                zipCode: request.Address.ZipCode,
                country: request.Address.Country,
                neighborhood: request.Address.Neighborhood);

            // 2. Cria a entidade Church
            var church = new Domain.Entities.Church(
                name: request.Name,
                address: address,
                phone: request.Phone,
                description: request.Description);

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
                Document = request.AdminDocument,
                Phone = request.AdminPhone,
                BirthDate = request.AdminBirthDate,
                IsBaptized = request.AdminIsBaptized,
                BaptizedDate = request.AdminBaptizedDate,
                IsTither = request.AdminIsTither,
                ChurchId = church.Id,
                Role = UserRole.Admin
            };

            // 6. Faz upload da foto do admin se necessário
            if (!string.IsNullOrEmpty(request.AdminPhoto))
            {
                adminMember.Photo = await UploadPhotoAsync(request.AdminPhoto, cancellationToken);
            }

            // 7. Criptografa e salva a senha do admin
            if (!string.IsNullOrWhiteSpace(request.AdminPassword))
            {
                // Gera um hash único para o admin (pode ser um guid, por exemplo)
                var hash = Guid.NewGuid().ToString("N");
                adminMember.PasswordHash = hash;
                // Usa o método de extensão Encrypt igual ao ActiveMemberPasswordCommand
                adminMember.Password = request.AdminPassword.Encrypt(hash);
            }

            // 8. Adiciona o membro admin ao repositório
            _unitOfWork.Members.Create(adminMember);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Igreja criada com ID: {ChurchId} e admin com ID: {AdminId}", church.Id, adminMember.Id);

            return church.Id;
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
    }
}
