using MediatR;

namespace MyChurch.Application.LivePresentation.Commands
{
    public class AddPrayerRequestToLivePresentationCommand : IRequest
    {
        public int PresentationId { get; set; }
        public int PrayerRequestId { get; set; }
        public bool DisplayAnonymous { get; set; }
    }
}