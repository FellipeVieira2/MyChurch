using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Commands.CreateBankingInfo
{
    public class CreateBankingInfoCommand : JwtMemberDto, IRequest<BankingInfoDto>
    {
        public string BankName { get; set; }
        public string? BankCode { get; set; }
        public string Agency { get; set; }
        public string Account { get; set; }
        public string AccountDigit { get; set; }
        public string AccountType { get; set; }
        public string HolderName { get; set; }
        public string HolderDocument { get; set; }
        public string? PixKey { get; set; }
        public string? PixKeyType { get; set; }
    }

    public class CreateBankingInfoCommandHandler : IRequestHandler<CreateBankingInfoCommand, BankingInfoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateBankingInfoCommandHandler> _logger;

        public CreateBankingInfoCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateBankingInfoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BankingInfoDto> Handle(CreateBankingInfoCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (member.Role != UserRole.Admin)
                ValidationException.ThrowException("Permissão", "Apenas administradores podem cadastrar contas bancárias.");

            var entity = new BankingInfo
            {
                ChurchId = member.ChurchId,
                BankName = request.BankName,
                BankCode = request.BankCode,
                Agency = request.Agency,
                Account = request.Account,
                AccountDigit = request.AccountDigit,
                AccountType = request.AccountType,
                HolderName = request.HolderName,
                HolderDocument = request.HolderDocument,
                PixKey = request.PixKey,
                PixKeyType = request.PixKeyType,
                Created = DateTime.UtcNow
            };

            _unitOfWork.BankingInfos.Create(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Conta bancária criada. ChurchId: {ChurchId}, BankingInfoId: {BankingInfoId}", entity.ChurchId, entity.Id);

            return BankingInfoDto.New(entity);
        }
    }
}
