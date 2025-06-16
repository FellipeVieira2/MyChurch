using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure.Utils.S3;
using System.Text.Json.Serialization;

namespace MyChurch.Application.WorshipActivity.Commands
{
    public class SendAdminNoticeCommand : JwtMemberDto, IRequest<int>
    {
        [JsonIgnore]
        public int ChurchId { get; set; }
        public string Message { get; set; }
        public string? ImageBase64 { get; set; } // base64 da imagem opcional
    }

    public class SendAdminNoticeCommandHandler : IRequestHandler<SendAdminNoticeCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IS3Helper _s3Helper;

        public SendAdminNoticeCommandHandler(IUnitOfWork unitOfWork, IS3Helper s3Helper)
        {
            _unitOfWork = unitOfWork;
            _s3Helper = s3Helper;
        }

        public async Task<int> Handle(SendAdminNoticeCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Membro não encontrado.");

            // Valida igreja
            var church = await _unitOfWork.Churchs.Query().FirstOrDefaultAsync(c => c.Id == request.ChurchId, cancellationToken);
            if (church == null)
                ValidationException.ThrowException("Church", "Igreja não encontrada.");

            // Valida membro se enviado

            string? imageUrl = null;
            if (!string.IsNullOrWhiteSpace(request.ImageBase64))
            {
                var base64 = request.ImageBase64;
                if (base64.Contains(','))
                    base64 = base64.Split(',')[1];
                var bytes = Convert.FromBase64String(base64);
                using var stream = new MemoryStream(bytes);
                imageUrl = await _s3Helper.UploadFileAsync(stream, $"{Guid.NewGuid()}_adminnotice.jpg", "image/jpeg", cancellationToken);
            }

            var notice = new AdminNotice
            {
                ChurchId = request.ChurchId,
                MemberId = request.UserId,
                Message = request.Message,
                ImageUrl = imageUrl,
                Created = DateTime.UtcNow
            };

            _unitOfWork.AdminNotices.Create(notice);

            await _unitOfWork.CommitAsync();
            return notice.Id;
        }
    }
}
