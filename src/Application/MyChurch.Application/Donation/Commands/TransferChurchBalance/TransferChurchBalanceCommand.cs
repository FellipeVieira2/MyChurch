using MediatR;
using Microsoft.EntityFrameworkCore;
using Mychurch.Common.WebClients.Asaas;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Donation.Commands.TransferChurchBalance
{
    public class TransferChurchBalanceCommand : JwtMemberDto, IRequest<bool>
    {
        public string? Notes { get; set; }
    }

    public class TransferChurchBalanceCommandHandler : IRequestHandler<TransferChurchBalanceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAsaasWebClient _asaasWebClient;

        public TransferChurchBalanceCommandHandler(IUnitOfWork unitOfWork, IAsaasWebClient asaasWebClient)
        {
            _unitOfWork = unitOfWork;
            _asaasWebClient = asaasWebClient;
        }

        public async Task<bool> Handle(TransferChurchBalanceCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (member.Role != Domain.Enum.UserRole.Admin)
                ValidationException.ThrowException("Member", "Apenas administradores podem efetuar retiradas.");

            var churchId = member.ChurchId;

            // Busca os dados bancários da igreja
            var bankingInfo = await _unitOfWork.BankingInfos.Query()
                .FirstOrDefaultAsync(b => b.ChurchId == churchId, cancellationToken);

            if (bankingInfo == null)
                ValidationException.ThrowException("Church" ,"Dados bancários da igreja não cadastrados.");

            // Busca as doações disponíveis para repasse
            var donations = await _unitOfWork.Donations.Query()
                .Where(d => d.Member.ChurchId == churchId && d.IsTransferred == false &&
                            d.Payments.Any(p => p.PaymentStatus == "Received"))
                .OrderBy(d => d.Date)
                .ToListAsync(cancellationToken);

            decimal totalAvailable = donations.Sum(d => d.Amount);

            if (totalAvailable <= 0)
                ValidationException.ThrowException("Church","Não há valor disponível para repasse.");

            // Preferencialmente transfere por PIX se houver chave cadastrada
            object transferenciaRequest;
            if (!string.IsNullOrWhiteSpace(bankingInfo.PixKey))
            {
                transferenciaRequest = new
                {
                    value = totalAvailable,
                    pixAddressKey = bankingInfo.PixKey,
                    pixAddressKeyType = bankingInfo.PixKeyType,
                    description = request.Notes
                };
            }
            else
            {
                transferenciaRequest = new
                {
                    value = totalAvailable,
                    bankAccount = new
                    {
                        bank = new { code = bankingInfo.BankCode },
                        ownerName = bankingInfo.HolderName,
                        cpfCnpj = bankingInfo.HolderDocument,
                        agency = bankingInfo.Agency,
                        account = bankingInfo.Account,
                        accountDigit = bankingInfo.AccountDigit,
                        bankAccountType = bankingInfo.AccountType
                    },
                    operationType = "TED",

                    description = request.Notes
                };
            }

            // Realiza a transferência via Asaas
            var asaasTransferResponse = await _asaasWebClient.CriarTransferenciaAsync(transferenciaRequest);

            decimal amountToTransfer = totalAvailable;
            foreach (var donation in donations)
            {
                if (amountToTransfer <= 0) break;
                if (donation.Amount <= amountToTransfer)
                {
                    donation.IsTransferred = true;
                    donation.TransferredAt = DateTime.UtcNow;
                    amountToTransfer -= donation.Amount;
                    _unitOfWork.Donations.Update(donation);
                }
                // Caso queira permitir repasse parcial, ajuste aqui
            }

            // Registra a retirada
            var transfer = new TransferHistory
            {
                ChurchId = churchId,
                Amount = totalAvailable,
                RequestedAt = DateTime.UtcNow,
                Status = "Completed",
                Notes = request.Notes
            };
            _unitOfWork.TransferHistories.Create(transfer);

            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}