using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Commands.UpdateBankingInfo
{
    public class UpdateBankingInfoV2Command : JwtMemberDto, IRequest<BankingInfoDto>
    {
        [JsonIgnore]
        public int BankingInfoId { get; set; }

        public string? Nickname { get; set; }
        public string? BankName { get; set; }
        public string? BankCode { get; set; }
        public string? Agency { get; set; }
        public string? Account { get; set; }
        public string? AccountDigit { get; set; }
        public string? AccountType { get; set; }
        public string? HolderName { get; set; }
        public string? HolderDocument { get; set; }
        public string? PixKey { get; set; }
        public string? PixKeyType { get; set; }
    }

    public class UpdateBankingInfoV2CommandHandler : IRequestHandler<UpdateBankingInfoV2Command, BankingInfoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateBankingInfoV2CommandHandler> _logger;

        public UpdateBankingInfoV2CommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateBankingInfoV2CommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BankingInfoDto> Handle(UpdateBankingInfoV2Command request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (member.Role != UserRole.Admin)
                ValidationException.ThrowException("Permissão", "Apenas administradores podem alterar contas bancárias.");

            var entity = await _unitOfWork.BankingInfos.Query()
                .FirstOrDefaultAsync(b => b.Id == request.BankingInfoId && b.ChurchId == member.ChurchId, cancellationToken);

            if (entity == null)
                ValidationException.ThrowException("BankingInfo", "Conta bancária não encontrada.");

            if (request.Nickname is not null) entity.Nickname = request.Nickname;
            if (request.BankName is not null) entity.BankName = request.BankName;
            if (request.BankCode is not null) entity.BankCode = request.BankCode;
            if (request.Agency is not null) entity.Agency = request.Agency;
            if (request.Account is not null) entity.Account = request.Account;
            if (request.AccountDigit is not null) entity.AccountDigit = request.AccountDigit;
            if (request.AccountType is not null) entity.AccountType = request.AccountType;
            if (request.HolderName is not null) entity.HolderName = request.HolderName;
            if (request.HolderDocument is not null) entity.HolderDocument = request.HolderDocument;
            if (request.PixKey is not null) entity.PixKey = request.PixKey;
            if (request.PixKeyType is not null) entity.PixKeyType = request.PixKeyType;

            entity.Updated = DateTime.UtcNow;

            _unitOfWork.BankingInfos.Update(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Conta bancária atualizada. BankingInfoId: {BankingInfoId}", entity.Id);

            return BankingInfoDto.New(entity);
        }
    }
}
