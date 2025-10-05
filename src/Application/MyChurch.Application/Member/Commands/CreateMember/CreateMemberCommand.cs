using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Services;
using MyChurch.Infrastructure.Utils.S3;
using MyChurch.Infrastructure.Utils.SES;
using System.Collections.Generic;
using static MyChurch.Application.Church.Commands.CreateChurchWithAdminMember.CreateChurchWithAdminMemberCommand;

namespace MyChurch.Application.Member.Commands.CreateMember
{
    public class CreateMemberCommand : JwtMemberDto, IRequest<int>
    {
        /// <summary>Name</summary>
        /// <example>Fellipe</example>
        public string Name { get; set; }
        /// <summary>Email</summary>
        /// <example>fvsouza623@gmail.com</example>
        public string? Email { get; set; }
        /// <summary>Photo</summary>
        /// <example>base64</example>
        public string? Photo { get; set; }
        /// <summary>Phone</summary>
        /// <example>19987250777</example>
        public string Phone { get; set; }
        /// <summary>BirthDate</summary>
        /// <example>2023-10-14T00:00:00</example>
        public DateTime BirthDate { get; set; }
        /// <summary>IsBaptized</summary>
        /// <example>true</example>
        public bool IsBaptized { get; set; }
        /// <summary>BaptizedDate</summary>
        /// <example>2023-10-14T00:00:00</example>
        public DateTime? BaptizedDate { get; set; }
        /// <summary>IsTither</summary>
        /// <example>true</example>
        public bool IsTither { get; set; }
        /// <summary>Role</summary>
        /// <example>Worker</example>
        public UserRole RoleMember { get; set; }

        // Novos campos
        /// <summary>Estado civil</summary>
        /// <example>Solteiro</example>
        public string? MaritalStatus { get; set; }
        /// <summary>Membro desde</summary>
        /// <example>2020-01-01T00:00:00</example>
        public DateTime? MemberSince { get; set; }
        /// <summary>Ministério</summary>
        /// <example>Louvor</example>
        public string? Ministry { get; set; }
        /// <summary>Membro ativo</summary>
        /// <example>true</example>
        public bool IsActive { get; set; } = true;
        /// <summary>Observações</summary>
        /// <example>Participa do grupo de jovens</example>
        public string? Notes { get; set; }
        public string? BirthCity { get; set; } // Novo: cidade de nascimento do admin
        public string? BirthState { get; set; } // Novo: estado de nascimento do admin
        public AddressChurchWithAdminCreate Address { get; set; }

        /// <summary>Documentos do membro</summary>
        public List<MemberDocumentDtoCreate>? Documents { get; set; } = new();
        public class AddressMemberCreate
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
            public string Country { get; set; }
            public string Neighborhood { get; set; }
            public string Number { get; set; }
        }
        public class MemberDocumentDtoCreate
        {
            /// <summary>Tipo do documento</summary>
            /// <example>CPF</example>
            public MemberDocumentType Type { get; set; }
            /// <summary>Número do documento</summary>
            /// <example>12345678901</example>
            public string Number { get; set; }
        }
    }

    public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateMemberCommandHandler> _logger;
        private readonly IS3Helper _s3Helper;
        private readonly IDocumentValidator _documentValidator;

        public CreateMemberCommandHandler(
            IUnitOfWork unitOfWork, 
            ILogger<CreateMemberCommandHandler> logger, 
            IS3Helper s3Helper,
            IDocumentValidator documentValidator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _s3Helper = s3Helper;
            _documentValidator = documentValidator;
        }

        public async Task<int> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "This Member does not exist.");

            int churchId = loggedMember.ChurchId;

            // Validação de limite de membros do plano
            await ValidatePlanMemberLimitAsync(churchId, cancellationToken);

            // Normalizar números de documentos antes da validação de duplicidade
            var documentNumbers = request.Documents?
                .Select(d => _documentValidator.RemoveFormatting(d.Number))
                .ToList() ?? new List<string>();

            // Validação de duplicidade (documento/email)
            var exists = await _unitOfWork.Members.Query()
                .AnyAsync(m =>
                    m.ChurchId == churchId &&
                    (m.Documents.Any(x => documentNumbers.Contains(x.Number)) ||
                     (!string.IsNullOrEmpty(request.Email) && m.Email == request.Email) || (request.Name == m.Name)),
                    cancellationToken);
            
            if (request.Name == "Quercio Goes Santos ")
            {
                var teste = "x";
            }
            
            if (exists)
                ValidationException.ThrowException("Member", "This Member already exists.");

            var member = new Domain.Entities.Member
            {
                Name = request.Name,
                Email = request?.Email,
                Phone = request.Phone,
                BirthDate = request.BirthDate,
                IsBaptized = request.IsBaptized,
                BaptizedDate = request?.BaptizedDate,
                IsTither = request.IsTither,
                ChurchId = churchId,
                Role = request.RoleMember,
                Created = DateTime.UtcNow,
                MaritalStatus =request.MaritalStatus is not null ? Enum.Parse<MaritalStatus>(request.MaritalStatus) : null,
                MemberSince = request?.MemberSince,
                Ministry= string.IsNullOrWhiteSpace(request.Ministry) ? null : Enum.Parse<Ministry>(request?.Ministry).ToString(),
                IsActive = request.IsActive,
                Notes = request.Notes,
                BirthCity = request.BirthCity,
                BirthState = request.BirthState,
                Address =request.Address is not null ? new Address(request.Address.Street, request.Address.City, request.Address.State, request.Address.ZipCode, request.Address.Country, request.Address.Neighborhood) 
                {Number = request.Address.Number } : null,
                // Mapeamento dos documentos com normalização
                Documents = request.Documents?.Select(d => new MemberDocument
                {
                    Type = d.Type,
                    Number = _documentValidator.RemoveFormatting(d.Number)
                }).ToList() ?? new List<MemberDocument>()
            };

            var hash = Guid.NewGuid().ToString("N");
            member.PasswordHash = hash;

            if (!string.IsNullOrEmpty(request.Photo))
            {
                member.Photo = await UploadPhotoAsync(request.Photo, cancellationToken);
            }

            _unitOfWork.Members.Create(member);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Member created with ID: {MemberId}", member.Id);

            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Emails", "WelcomeMemberTemplate.html");

            var htmlTemplatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "AccountConfirmation.html");

            //var link = $"https://www.mychurchlab.net/conta/criar-senha?token={hash}";
            //var htmlContent = await File.ReadAllTextAsync(htmlTemplatePath, cancellationToken);
            //htmlContent = htmlContent.Replace("{{link}}", link);

            //await _emailService.EnviarEmailAsync(
            //    destinatario: member.Email,
            //    assunto: "Bem-vindo à nossa igreja!",
            //    corpoHtml: htmlContent
            //);

            return member.Id;
        }

        /// <summary>
        /// Valida se a igreja pode cadastrar mais membros de acordo com o plano atual.
        /// </summary>
        private async Task ValidatePlanMemberLimitAsync(int churchId, CancellationToken cancellationToken)
        {
            // Busca assinatura ativa da igreja (Subscription + Plan)
            var subscription = await _unitOfWork.Subscriptions.Query()
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.ChurchId == churchId && s.EndDate > DateTime.UtcNow, cancellationToken);

            if (subscription == null || subscription.Plan == null)
            {
                _logger.LogInformation("This Church does not has a Active Plan.");
                ValidationException.ThrowException("Plan", "This Church does not has a Active Plan.");
            }

            // Conta membros já cadastrados
            var currentMembersCount = await _unitOfWork.Members.Query()
                .CountAsync(m => m.ChurchId == churchId, cancellationToken);

            // Valida limite do plano
            if (currentMembersCount >= subscription.Plan.MaxMembers)
            {
                _logger.LogInformation("This Church does not has a Active Plan.");
                ValidationException.ThrowException("Plan", $"Member limit reached for the plan '{subscription.Plan.Name}'. To register more members, please upgrade your plan.");
            }
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