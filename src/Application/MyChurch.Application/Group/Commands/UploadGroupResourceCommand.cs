using MediatR;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using System;
using MyChurch.Infrastructure.Utils.S3;
using System.IO;

namespace MyChurch.Application.Group.Commands
{
    public class UploadGroupResourceCommand : IRequest<int>
    {
        public int GroupId { get; set; }
        public string FileName { get; set; }
        public string FileBase64 { get; set; } // Novo campo para receber o arquivo em base64
        public string? Description { get; set; }
        public int UploadedByMemberId { get; set; } // Para rastrear quem fez o upload
    }

    public class UploadGroupResourceCommandHandler : IRequestHandler<UploadGroupResourceCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IS3Helper _s3Helper;
        public UploadGroupResourceCommandHandler(IUnitOfWork unitOfWork, IS3Helper s3Helper)
        {
            _unitOfWork = unitOfWork;
            _s3Helper = s3Helper;
        }
        public async Task<int> Handle(UploadGroupResourceCommand request, CancellationToken cancellationToken)
        {
            var group = await _unitOfWork.Groups.Query().FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);
            if (group == null)
                ValidationException.ThrowException("Group", "Grupo não encontrado.");

            // Upload do arquivo para o S3
            string fileUrl = await UploadFileAsync(request.FileBase64, request.FileName, cancellationToken);

            var resource = new GroupResource(
                request.GroupId,
                request.FileName,
                request.Description ?? string.Empty,
                fileUrl,
                request.UploadedByMemberId
            );
            _unitOfWork.GroupResources.Create(resource);
            await _unitOfWork.CommitAsync();
            return resource.Id;
        }

        private async Task<string> UploadFileAsync(string fileBase64, string fileName, CancellationToken cancellationToken)
        {
            if (fileBase64.Contains(','))
                fileBase64 = fileBase64.Split(',')[1];
            var fileBytes = Convert.FromBase64String(fileBase64);
            using var fileStream = new MemoryStream(fileBytes);
            var fileUrl = await _s3Helper.UploadFileAsync(fileStream, fileName, "application/octet-stream", cancellationToken);
            return fileUrl;
        }
    }
}
