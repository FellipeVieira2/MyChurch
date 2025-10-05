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
using System.Text.Json.Serialization;

namespace MyChurch.Application.Member.Commands.UpdateMember
{
    public class UpdateMemberCommand : JwtMemberDto, IRequest<MemberDto>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? IsBaptized { get; set; }
        public DateTime? BaptizedDate { get; set; }
        public bool? IsTither { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public DateTime? MemberSince { get; set; }
        public Ministry? Ministry { get; set; }
        public bool? IsActive { get; set; }
        public string? Notes { get; set; }
        public string? Photo { get; set; }
        public string? BirthCity { get; set; } 
        public string? BirthState { get; set; }
        public AddressMemberUpdate? Address { get; set; }


        /// <summary>Documentos do membro</summary>
        public List<MemberDocumentDtoUpdate>? Documents { get; set; } = new();

        public class MemberDocumentDtoUpdate
        {
            /// <summary>Tipo do documento</summary>
            /// <example>CPF</example>
            public MemberDocumentType Type { get; set; }
            /// <summary>Número do documento</summary>
            /// <example>12345678901</example>
            public string Number { get; set; }
        }
        public class AddressMemberUpdate
        {
            public string? Street { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? ZipCode { get; set; }
            public string? Country { get; set; }
            public string? Neighborhood { get; set; }
            public string? Number { get; set; }
        }
    }

    public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, MemberDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateMemberCommandHandler> _logger;
        private readonly IS3Helper _s3Helper;
        private readonly IDocumentValidator _documentValidator;

        public UpdateMemberCommandHandler(
            IUnitOfWork unitOfWork, 
            ILogger<UpdateMemberCommandHandler> logger, 
            IS3Helper s3Helper,
            IDocumentValidator documentValidator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _s3Helper = s3Helper;
            _documentValidator = documentValidator;
        }

        public async Task<MemberDto> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando atualização do membro. MemberId: {MemberId}, UserId: {UserId}", request.Id, request.UserId);

            // Busca o membro autenticado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
            {
                _logger.LogWarning("Membro autenticado não encontrado. UserId: {UserId}", request.UserId);
                ValidationException.ThrowException("Member", "Authenticated member does not exist.");
            }

            int churchId = loggedMember.ChurchId;

            // Busca o membro a ser atualizado e valida se pertence à mesma igreja
            var member = await _unitOfWork.Members.Query()
                .Include(m => m.Documents)
                .FirstOrDefaultAsync(m => m.Id == request.Id && m.ChurchId == churchId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Membro não encontrado ou não pertence à igreja. MemberId: {MemberId}, ChurchId: {ChurchId}", request.Id, churchId);
                ValidationException.ThrowException("Member", "Member not found or does not belong to your church.");
            }

            // Verifica se há foto para upload
            string? photoUrl = null;
            if (!string.IsNullOrEmpty(request.Photo) && request.Photo != member.Photo)
            {
                photoUrl = await UploadPhotoAsync(request.Photo, cancellationToken);
            }

            // Atualização centralizada via domínio
            member.Update(
                request.Name,
                request.Email,
                request.Phone,
                request.BirthDate,
                request.IsBaptized,
                request.BaptizedDate,
                request.IsTither,
                request.MaritalStatus,
                request.MemberSince,
                request.Ministry?.ToString(),
                request.IsActive,
                request.Notes,
                photoUrl ?? request.Photo, // Usa a URL da foto uploada ou mantém a atual
                request.BirthCity,
                request.BirthState
            );

            // Atualização do endereço (apenas campos enviados)
            if (request.Address != null)
            {
                if (member.Address == null)
                {
                    member.Address = new Address(
                        request.Address.Street ?? string.Empty,
                        request.Address.City ?? string.Empty,
                        request.Address.State ?? string.Empty,
                        request.Address.ZipCode ?? string.Empty,
                        request.Address.Country ?? string.Empty,
                        request.Address.Neighborhood ?? string.Empty
                    )
                    {
                        Number = request.Address.Number
                    };
                }
                else
                {
                    member.Address.Update(
                        request.Address.Street,
                        request.Address.City,
                        request.Address.ZipCode,
                        request.Address.Country,
                        request.Address.Neighborhood,
                        request.Address.State,
                        request.Address.Number,
                        member.Address.Complement
                    );
                }
            }

            // Atualização dos documentos com normalização
            if (request.Documents != null && request.Documents.Any())
            {
                member.Documents.Clear();
                foreach (var doc in request.Documents)
                {
                    member.Documents.Add(new MemberDocument
                    {
                        Type = doc.Type,
                        Number = _documentValidator.RemoveFormatting(doc.Number)
                    });
                }
            }

            _unitOfWork.Members.Update(member);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Membro atualizado com sucesso. MemberId: {MemberId}", member.Id);

            return MemberDto.New(member);
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