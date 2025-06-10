using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
    }

    public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, MemberDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateMemberCommandHandler> _logger;

        public UpdateMemberCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateMemberCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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

            // Atualização parcial dos campos
            if (!string.IsNullOrEmpty(request.Name))
                member.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Email))
                member.Email = request.Email;

            if (!string.IsNullOrEmpty(request.Phone))
                member.Phone = request.Phone;

            if (request.BirthDate.HasValue)
                member.BirthDate = request.BirthDate.Value;

            if (request.IsBaptized.HasValue)
                member.IsBaptized = request.IsBaptized.Value;

            if (request.BaptizedDate.HasValue)
                member.BaptizedDate = request.BaptizedDate;

            if (request.IsTither.HasValue)
                member.IsTither = request.IsTither.Value;

            if (request.MaritalStatus.HasValue)
                member.MaritalStatus = request.MaritalStatus;

            if (request.MemberSince.HasValue)
                member.MemberSince = request.MemberSince;

            if (request.Ministry.HasValue)
                member.Ministry = request.Ministry.ToString();

            if (request.IsActive.HasValue)
                member.IsActive = request.IsActive.Value;

            if (!string.IsNullOrEmpty(request.Notes))
                member.Notes = request.Notes;

            if (!string.IsNullOrEmpty(request.Photo))
                member.Photo = request.Photo;

            // Atualização dos documentos
            if (request.Documents != null && request.Documents.Any())
            {
                // Remove documentos antigos
                member.Documents.Clear();

                // Adiciona os novos documentos
                foreach (var doc in request.Documents)
                {
                    member.Documents.Add(new MemberDocument
                    {
                        Type = doc.Type,
                        Number = doc.Number
                    });
                }
            }

            _unitOfWork.Members.Update(member);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Membro atualizado com sucesso. MemberId: {MemberId}", member.Id);

            return MemberDto.New(member);
        }
    }
}