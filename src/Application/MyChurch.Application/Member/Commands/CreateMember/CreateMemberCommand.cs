using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Infrastructure.Utils.S3;
using System.Text;

namespace MyChurch.Application.Member.Commands.CreateMember
{
    public class CreateMemberCommand : IRequest<int>
    {
        /// <summary>Name</summary>
        /// <example>Fellipe</example>
        public string Name { get; set; }
        /// <summary>Email</summary>
        /// <example>fvsouza623@gmail.com</example>
        public string? Email { get; set; }
        /// <summary>Document</summary>
        /// <example>45570179836</example>
        public string Document { get; set; }
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
        public DateTime BaptizedDate { get; set; }
        /// <summary>IsTither</summary>
        /// <example>true</example>
        public bool IsTither { get; set; }
        /// <summary>ChurchId</summary>
        /// <example>1</example>
        public int ChurchId { get; set; }
        /// <summary>Role</summary>
        /// <example>Worker</example>
        public UserRole Role { get; set; }
    }

    public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateMemberCommandHandler> _logger;
        private readonly IS3Helper _s3Helper;

        public CreateMemberCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateMemberCommandHandler> logger, IS3Helper s3Helper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _s3Helper = s3Helper;
        }

        public async Task<int> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            var member = MapToMemberEntity(request);

            if (!string.IsNullOrEmpty(request.Photo))
            {
                member.Photo = await UploadPhotoAsync(request.Photo, cancellationToken);
            }

            _unitOfWork.Members.Create(member);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Member created with ID: {MemberId}", member.Id);

            return member.Id;
        }

        private Domain.Entities.Member MapToMemberEntity(CreateMemberCommand request)
        {
            return new Domain.Entities.Member
            {
                Name = request.Name,
                Email = request.Email,
                Document = request.Document,
                Phone = request.Phone,
                BirthDate = request.BirthDate,
                IsBaptized = request.IsBaptized,
                BaptizedDate = request.BaptizedDate,
                IsTither = request.IsTither,
                ChurchId = request.ChurchId,
                Role = request.Role
            };
        }

        private async Task<string> UploadPhotoAsync(string photoBase64, CancellationToken cancellationToken)
        {
            if (photoBase64.Contains(','))
            {
                photoBase64 = photoBase64.Split(',')[1];
            }

            var photoBytes = Convert.FromBase64String(photoBase64);
            using var photoStream = new MemoryStream(photoBytes);
            var photoUrl = await _s3Helper.UploadFileAsync(photoStream, $"{Guid.NewGuid()}photo.jpg", "image/jpeg", cancellationToken);

            return photoUrl;
        }
    }
}
