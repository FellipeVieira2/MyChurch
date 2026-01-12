using MediatR;
using MyChurch.Domain.Contracts;
using Mychurch.Common.WebClients.Asaas;
using Mychurch.Common.WebClients.Asaas.Models.Requests;

namespace MyChurch.Application.Visitors.Commands
{
    public class CreateAnonymousVisitorCommand : IRequest<int>
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }

    public class CreateAnonymousVisitorCommandHandler : IRequestHandler<CreateAnonymousVisitorCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAsaasWebClient _asaas;
        public CreateAnonymousVisitorCommandHandler(IUnitOfWork unitOfWork, IAsaasWebClient asaas)
        {
            _unitOfWork = unitOfWork;
            _asaas = asaas;
        }
        public async Task<int> Handle(CreateAnonymousVisitorCommand request, CancellationToken cancellationToken)
        {
            var visitor = new Domain.Entities.Visitor
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                CreatedAt = DateTime.UtcNow
            };

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                try
                {
                    var asaasResp = await _asaas.CriarClienteAsync(new AsaasCustomerRequestDto
                    {
                        Name = request.Name ?? request.Email,
                        Email = request.Email,
                        MobilePhone = request.Phone
                    });
                    visitor.AsaasCustomerId = asaasResp.Id;
                }
                catch
                {
                    // tolerante a falhas - segue sem AsaasCustomerId
                }
            }

            await _unitOfWork.Visitors.Create(visitor);
            await _unitOfWork.CommitAsync();
            return visitor.Id;
        }
    }
}
