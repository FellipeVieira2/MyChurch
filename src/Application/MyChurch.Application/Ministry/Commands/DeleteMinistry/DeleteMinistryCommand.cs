using MediatR;

namespace MyChurch.Application.Ministries.Commands.DeleteMinistry
{
    public class DeleteMinistryCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
