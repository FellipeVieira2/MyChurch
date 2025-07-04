using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Presentation.Commands
{
    public class CreatePresentationCommand : JwtMemberDto, IRequest<PresentationDto>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CreatePresentationCommandHandler : IRequestHandler<CreatePresentationCommand, PresentationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePresentationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PresentationDto> Handle(CreatePresentationCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null)
                throw new System.Exception("Usuário não encontrado para obter ChurchId.");

            var presentation = new Domain.Entities.Presentation
            {
                ChurchId = member.ChurchId,
                AdminUserId = request.UserId,
                Name = request.Name,
                Description = request.Description,
                IsLive = false,
                CurrentSlideIndex = 0
            };

            _unitOfWork.Presentations.Create(presentation);
            await _unitOfWork.CommitAsync();

            return new PresentationDto
            {
                Id = presentation.Id,
                ChurchId = presentation.ChurchId,
                AdminUserId = presentation.AdminUserId,
                Name = presentation.Name,
                Description = presentation.Description,
                CurrentSlideIndex = presentation.CurrentSlideIndex,
                IsLive = presentation.IsLive,
                Slides = new List<SlideDto>()
            };
        }
    }
}