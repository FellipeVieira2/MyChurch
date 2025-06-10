using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Commands.UpdateBankingInfo
{
    public class UpdateBankingInfoCommand : JwtMemberDto, IRequest<BankingInfoDto>
    {
        [JsonIgnore]
        public int ChurchId { get; set; }

        /// <summary>Banco</summary>
        public string BankName { get; set; }
        /// <summary>Agência</summary>
        public string Agency { get; set; }
        /// <summary>Conta</summary>
        public string Account { get; set; }
        /// <summary>Dígito da Conta</summary>
        public string AccountDigit { get; set; }
        /// <summary>Tipo de Conta</summary>
        public string AccountType { get; set; }
        /// <summary>Titular</summary>
        public string HolderName { get; set; }
        /// <summary>Documento do Titular</summary>
        public string HolderDocument { get; set; }
        /// <summary>Chave PIX</summary>
        public string PixKey { get; set; }
        /// <summary>Tipo da Chave PIX</summary>
        public string PixKeyType { get; set; }
    }

    public class UpdateBankingInfoCommandHandler : IRequestHandler<UpdateBankingInfoCommand, BankingInfoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateBankingInfoCommandHandler> _logger;

        public UpdateBankingInfoCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateBankingInfoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BankingInfoDto> Handle(UpdateBankingInfoCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Usuário não encontrado.");
                ValidationException.ThrowException("Member", "Usuário não encontrado.");
            }

            // Garante que só o admin pode alterar
            if (member.Role != Domain.Enum.UserRole.Admin)
            {
                _logger.LogWarning("Usuário não tem permissão para alterar dados bancários.");
                ValidationException.ThrowException("Permissão", "Apenas administradores podem alterar os dados bancários.");
            }

            var churchId = member.ChurchId;

            // Busca ou cria o registro de dados bancários
            var bankingInfo = await _unitOfWork.BankingInfos.Query()
                .FirstOrDefaultAsync(b => b.ChurchId == churchId, cancellationToken);

            if (bankingInfo == null)
            {
                bankingInfo = new BankingInfo
                {
                    ChurchId = churchId
                };
                _unitOfWork.BankingInfos.Create(bankingInfo);
            }

            bankingInfo.BankName = request.BankName;
            bankingInfo.Agency = request.Agency;
            bankingInfo.Account = request.Account;
            bankingInfo.AccountType = request.AccountType;
            bankingInfo.AccountDigit = request.AccountDigit;
            bankingInfo.HolderName = request.HolderName;
            bankingInfo.HolderDocument = request.HolderDocument;
            bankingInfo.PixKey = request.PixKey;
            bankingInfo.PixKeyType = request.PixKeyType;

            _unitOfWork.BankingInfos.Update(bankingInfo);
            await _unitOfWork.CommitAsync();

            return BankingInfoDto.New(bankingInfo);
        }
    }
}