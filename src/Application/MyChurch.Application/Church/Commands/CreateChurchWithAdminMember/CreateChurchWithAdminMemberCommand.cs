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

        /// <summary>Documento do Administrador</summary>
        /// <example>12345678900</example>
        public string AdminDocument { get; set; }

        /// <summary>Foto do Administrador (Base64)</summary>
        /// <example>Base64</example>
        public string? AdminPhoto { get; set; }

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
                Role = UserRole.Admin,
                Created = DateTime.Now
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
