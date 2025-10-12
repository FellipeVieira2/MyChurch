using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.Member.Commands.DeleteMember
{
    /// <summary>
    /// Comando para deletar permanentemente um membro (Hard Delete)
    /// </summary>
    public class DeleteMemberCommand : JwtMemberDto, IRequest<Unit>
    {
        [JsonIgnore]
        public int MemberId { get; set; }
        
        /// <summary>
        /// Motivo da exclusão (obrigatório para auditoria)
        /// </summary>
        public string Reason { get; set; }
    }

    public class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteMemberCommandHandler> _logger;

        public DeleteMemberCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteMemberCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
        {
            // Busca o admin logado
            var admin = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (admin == null || admin.Role != Domain.Enum.UserRole.Admin)
            {
                ValidationException.ThrowException("Authorization", "Apenas administradores podem deletar membros.");
            }

            // Busca o membro a ser deletado com suas dependências
            var member = await _unitOfWork.Members.Query()
                .Include(m => m.Documents)
                .Include(m => m.Address)
                .Include(m => m.CreditCardInfos)
                .Include(m => m.PrayerRequests)
                .FirstOrDefaultAsync(m => m.Id == request.MemberId && m.ChurchId == admin.ChurchId, cancellationToken);

            if (member == null)
            {
                ValidationException.ThrowException("Member", "Membro não encontrado ou não pertence a esta igreja.");
            }

            // Proteção: Não permite deletar o próprio usuário
            if (member.Id == admin.Id)
            {
                ValidationException.ThrowException("Member", "Você não pode deletar sua própria conta.");
            }

            // Verifica se tem dados críticos vinculados
            var hasDonations = await _unitOfWork.Donations.Query()
                .AnyAsync(d => d.MemberId == member.Id, cancellationToken);

            var hasCashFlowEntries = await _unitOfWork.CashFlowEntries.Query()
                .AnyAsync(c => c.MemberId == member.Id, cancellationToken);

            if (hasDonations || hasCashFlowEntries)
            {
                ValidationException.ThrowException(
                    "Member", 
                    "Este membro possui doações ou lançamentos financeiros vinculados. " +
                    "Por questões de auditoria, apenas a desativação (soft delete) é permitida. " +
                    "Use o endpoint de atualização para definir IsActive = false.");
            }

            _logger.LogWarning(
                "HARD DELETE iniciado - Admin: {AdminId} ({AdminName}), Membro: {MemberId} ({MemberName}), Motivo: {Reason}",
                admin.Id, admin.Name, member.Id, member.Name, request.Reason);

            // Remove dependências em ordem
            await DeleteMemberDependenciesAsync(member, cancellationToken);

            // Remove o membro
            _unitOfWork.Members.Delete(member);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Membro {MemberId} ({MemberName}) deletado permanentemente por {AdminName}",
                member.Id, member.Name, admin.Name);

            return Unit.Value;
        }

        /// <summary>
        /// Remove todas as dependências do membro antes de deletá-lo
        /// </summary>
        private async Task DeleteMemberDependenciesAsync(Domain.Entities.Member member, CancellationToken cancellationToken)
        {
            // 1. Remove documentos
            if (member.Documents != null && member.Documents.Any())
            {
                foreach (var doc in member.Documents.ToList())
                {
                    _unitOfWork.MemberDocuments.Delete(doc);
                }
            }

            // 2. Remove cartões de crédito
            if (member.CreditCardInfos != null && member.CreditCardInfos.Any())
            {
                foreach (var card in member.CreditCardInfos.ToList())
                {
                    _unitOfWork.CreditCardInfos.Delete(card);
                }
            }

            // 3. Remove pedidos de oração
            if (member.PrayerRequests != null && member.PrayerRequests.Any())
            {
                foreach (var prayer in member.PrayerRequests.ToList())
                {
                    _unitOfWork.PrayerRequests.Delete(prayer);
                }
            }

            // 4. Remove likes em posts do feed
            var feedLikes = await _unitOfWork.FeedLikes.Query()
                .Where(fl => fl.MemberId == member.Id)
                .ToListAsync(cancellationToken);
            
            foreach (var like in feedLikes)
            {
                _unitOfWork.FeedLikes.Delete(like);
            }

            // 5. Remove posts do feed (se houver)
            var feedPosts = await _unitOfWork.FeedPosts.Query()
                .Include(fp => fp.Images)
                .Include(fp => fp.Likes)
                .Where(fp => fp.MemberId == member.Id)
                .ToListAsync(cancellationToken);
            
            foreach (var post in feedPosts)
            {
                // Remove imagens do post
                if (post.Images != null)
                {
                    foreach (var image in post.Images.ToList())
                    {
                        _unitOfWork.FeedPostImages.Delete(image);
                    }
                }
                
                // Remove likes do post
                if (post.Likes != null)
                {
                    foreach (var like in post.Likes.ToList())
                    {
                        _unitOfWork.FeedLikes.Delete(like);
                    }
                }
                
                _unitOfWork.FeedPosts.Delete(post);
            }

            // 6. Remove progresso de jornadas
            var journeyProgress = await _unitOfWork.MemberJourneyProgresses.Query()
                .Where(jp => jp.MemberId == member.Id)
                .ToListAsync(cancellationToken);
            
            foreach (var progress in journeyProgress)
            {
                _unitOfWork.MemberJourneyProgresses.Delete(progress);
            }

            // 7. Remove atribuições de jornadas
            var journeyAssignments = await _unitOfWork.MemberJourneyAssignments.Query()
                .Where(ja => ja.MemberId == member.Id)
                .ToListAsync(cancellationToken);
            
            foreach (var assignment in journeyAssignments)
            {
                _unitOfWork.MemberJourneyAssignments.Delete(assignment);
            }

            // 8. Remove progresso de leitura bíblica
            var bibleProgress = await _unitOfWork.MemberBibleReadingProgresses.Query()
                .Where(bp => bp.MemberId == member.Id)
                .ToListAsync(cancellationToken);
            
            foreach (var progress in bibleProgress)
            {
                _unitOfWork.MemberBibleReadingProgresses.Delete(progress);
            }

            // 9. Remove configurações do membro
            var config = await _unitOfWork.MemberConfigurations.Query()
                .FirstOrDefaultAsync(mc => mc.MemberId == member.Id, cancellationToken);
            
            if (config != null)
            {
                _unitOfWork.MemberConfigurations.Delete(config);
            }

            // 10. Remove permissões customizadas
            var customPermissions = await _unitOfWork.MemberCustomPermissions.Query()
                .Where(mcp => mcp.MemberId == member.Id || mcp.GrantedByMemberId == member.Id)
                .ToListAsync(cancellationToken);
            
            foreach (var perm in customPermissions)
            {
                _unitOfWork.MemberCustomPermissions.Delete(perm);
            }

            // 11. Remove endereço se não for compartilhado
            if (member.Address != null && member.AddressId.HasValue)
            {
                var addressShared = await _unitOfWork.Members.Query()
                    .AnyAsync(m => m.AddressId == member.AddressId && m.Id != member.Id, cancellationToken);
                
                if (!addressShared)
                {
                    // Buscar o endereço diretamente pelo contexto
                    var churchContext = _unitOfWork.Members.Query().FirstOrDefault()?.Church;
                    // Remove referência do endereço antes de deletar
                    member.AddressId = null;
                    member.Address = null;
                }
            }

            _logger.LogInformation("Dependências do membro {MemberId} removidas", member.Id);
        }
    }
}
