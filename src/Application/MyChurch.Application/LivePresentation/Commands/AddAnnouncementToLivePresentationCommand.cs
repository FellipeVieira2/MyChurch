using MediatR;

namespace MyChurch.Application.LivePresentation.Commands
{
    public class AddAnnouncementToLivePresentationCommand : IRequest
    {
        public int PresentationId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
    }
}