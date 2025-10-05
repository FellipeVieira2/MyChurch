using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Church.Commands.UpdateChurchSocialMedia
{
    /// <summary>
    /// Comando para atualizar redes sociais e contatos da igreja
    /// </summary>
    public class UpdateChurchSocialMediaCommand : JwtMemberDto, IRequest<Unit>
    {
        public string? Website { get; set; }
        public string? Email { get; set; }
        public string? InstagramUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public string? YoutubeUrl { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? TwitterUrl { get; set; }
        public string? TikTokUrl { get; set; }
    }

    public class UpdateChurchSocialMediaCommandHandler : IRequestHandler<UpdateChurchSocialMediaCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateChurchSocialMediaCommandHandler> _logger;

        public UpdateChurchSocialMediaCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateChurchSocialMediaCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateChurchSocialMediaCommand request, CancellationToken cancellationToken)
        {
            // ?? SEGURANÇA: Busca o membro logado
            var currentMember = _unitOfWork.Members.Query()
                .FirstOrDefault(m => m.Id == request.UserId);

            if (currentMember == null)
            {
                ValidationException.ThrowException("Member", "Usuário não encontrado");
            }

            // Apenas admin pode atualizar
            if (currentMember.Role != Domain.Enum.UserRole.Admin)
            {
                ValidationException.ThrowException("Permission", "Apenas administradores podem atualizar as redes sociais");
            }

            // Busca a igreja do membro
            var church = _unitOfWork.Churchs.Query()
                .FirstOrDefault(c => c.Id == currentMember.ChurchId);

            if (church == null)
            {
                ValidationException.ThrowException("Church", "Igreja não encontrada");
            }

            // Validar URLs (opcional - pode criar um validator separado)
            ValidateUrls(request);

            // Atualizar redes sociais
            church.UpdateSocialMedia(
                website: request.Website,
                email: request.Email,
                instagramUrl: request.InstagramUrl,
                facebookUrl: request.FacebookUrl,
                youtubeUrl: request.YoutubeUrl,
                whatsAppNumber: request.WhatsAppNumber,
                twitterUrl: request.TwitterUrl,
                tiktokUrl: request.TikTokUrl
            );

            _unitOfWork.Churchs.Update(church);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Redes sociais atualizadas para igreja {ChurchId} por {MemberId}", 
                church.Id, currentMember.Id);

            return Unit.Value;
        }

        private void ValidateUrls(UpdateChurchSocialMediaCommand request)
        {
            if (!string.IsNullOrEmpty(request.Website) && !IsValidUrl(request.Website))
            {
                ValidationException.ThrowException("Website", "URL inválida");
            }

            if (!string.IsNullOrEmpty(request.Email) && !IsValidEmail(request.Email))
            {
                ValidationException.ThrowException("Email", "Email inválido");
            }

            if (!string.IsNullOrEmpty(request.InstagramUrl) && !IsValidUrl(request.InstagramUrl))
            {
                ValidationException.ThrowException("InstagramUrl", "URL inválida");
            }

            if (!string.IsNullOrEmpty(request.FacebookUrl) && !IsValidUrl(request.FacebookUrl))
            {
                ValidationException.ThrowException("FacebookUrl", "URL inválida");
            }

            if (!string.IsNullOrEmpty(request.YoutubeUrl) && !IsValidUrl(request.YoutubeUrl))
            {
                ValidationException.ThrowException("YoutubeUrl", "URL inválida");
            }

            if (!string.IsNullOrEmpty(request.TwitterUrl) && !IsValidUrl(request.TwitterUrl))
            {
                ValidationException.ThrowException("TwitterUrl", "URL inválida");
            }

            if (!string.IsNullOrEmpty(request.TikTokUrl) && !IsValidUrl(request.TikTokUrl))
            {
                ValidationException.ThrowException("TikTokUrl", "URL inválida");
            }
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
