using MediatR;

namespace MyChurch.Application.WorshipActivity.Commands
{
    public class ChangeCurrentActivityCommand : IRequest<bool>
    {
        public int WorshipServiceId { get; set; }
        public int ActivityId { get; set; }
    }
}