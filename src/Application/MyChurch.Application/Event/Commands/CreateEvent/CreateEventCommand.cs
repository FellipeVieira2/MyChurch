using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Event.Commands.CreateEvent
{
    public class CreateEventCommand : JwtMemberDto, IRequest<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime FinishDate { get; set; }
        public string Location { get; set; }
        public bool RequiresParticipantList { get; set; }

        // Recorrência
        public EventRecurrenceType? RecurrenceType { get; set; }
        public int? Frequency { get; set; }
    }

    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateEventCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "This Member does not exist.");

            int churchId = loggedMember.ChurchId;

            var entity = new Domain.Entities.Event
            {
                Title = request.Title,
                Description = request.Description,
                Date = request.Date,
                FinishDate = request.FinishDate,
                Location = request.Location,
                ChurchId = churchId,
                RequiresParticipantList = request.RequiresParticipantList
            };

            _unitOfWork.Events.Create(entity);

            // Se houver recorrência, cria o registro de recorrência
            if (request.RecurrenceType.HasValue && request.RecurrenceType != EventRecurrenceType.None && request.Frequency.HasValue)
            {
                var recurrence = new EventRecurrence
                {
                    Event = entity,
                    RecurrenceType = request.RecurrenceType.Value,
                    Frequency = request.Frequency.Value
                };
                _unitOfWork.EventRecurrences.Create(recurrence);
            }

            await _unitOfWork.CommitAsync();
            return entity.Id;
        }
    }
}
