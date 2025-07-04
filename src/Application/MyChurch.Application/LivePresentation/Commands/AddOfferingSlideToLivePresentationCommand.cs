using MediatR;

namespace MyChurch.Application.LivePresentation.Commands
{
    public class AddOfferingSlideToLivePresentationCommand : IRequest
    {
        public int PresentationId { get; set; }
        public decimal? Amount { get; set; }
        public string Description { get; set; }
        public bool IsPixMethod { get; set; }
    }
}