using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Church.Commands.CreateChurchCommand;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;

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
        /// <example>14/01/2000</example>
        public DateTime? BirthDate { get; set; }
        /// <summary>IsBaptized</summary>
        /// <example>true</example>
        public bool IsBaptized { get; set; }
        /// <summary>BaptizedDate</summary>
        /// <example>14/01/2000</example>
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
    public class CreateChurchCommandHandler : IRequestHandler<CreateChurchCommand, int>
    {
        private readonly IUnitOfWork _repository;
        private readonly ILogger<CreateChurchCommandHandler> _logger;
        public CreateChurchCommandHandler(IUnitOfWork repository, ILogger<CreateChurchCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<int> Handle(CreateChurchCommand request, CancellationToken cancellationToken)
        {
            return 1;
        }
    }
}
