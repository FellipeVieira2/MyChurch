using MediatR;
using Microsoft.EntityFrameworkCore;
using Mychurch.Common.WebClients.Asaas;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Donation.Commands.TransferChurchBalance
{
    public class TransferChurchBalanceCommand : JwtMemberDto, IRequest<bool>
    {
        public int? BankingInfoId { get; set; }
        public int? DepartmentId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? ScheduledFor { get; set; }
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

            if (!UserRoleAccess.CanManageFinancialModule(member.Role))
                ValidationException.ThrowException("Member", "Apenas perfis com gestão financeira podem efetuar retiradas.");

            var churchId = member.ChurchId;

            // Resolve conta destino:
            // 1) BankingInfoId explícito
            // 2) Conta do departamento (se DepartmentId informado)
            // 3) Conta default da igreja
            int? resolvedBankingInfoId = request.BankingInfoId;

            if (!resolvedBankingInfoId.HasValue && request.DepartmentId.HasValue)
            {
                var dept = await _unitOfWork.Departments.Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == request.DepartmentId.Value && d.ChurchId == churchId, cancellationToken);

                if (dept == null)
                    ValidationException.ThrowException("Department", "Departamento não encontrado para esta igreja.");

                resolvedBankingInfoId = dept.BankingInfoId;
            }

            if (!resolvedBankingInfoId.HasValue)
            {
                var churchDefault = await _unitOfWork.Churchs.Query()
                    .AsNoTracking()
                    .Where(c => c.Id == churchId)
                    .Select(c => c.DefaultBankingInfoId)
                    .FirstOrDefaultAsync(cancellationToken);

                resolvedBankingInfoId = churchDefault;
            }

            if (!resolvedBankingInfoId.HasValue)
                ValidationException.ThrowException("Church", "Nenhuma conta destino definida. Informe BankingInfoId, DepartmentId com conta, ou defina uma conta principal da igreja.");

            var bankingInfo = await _unitOfWork.BankingInfos.Query()
                .FirstOrDefaultAsync(b => b.Id == resolvedBankingInfoId.Value && b.ChurchId == churchId, cancellationToken);

            if (bankingInfo == null)
                ValidationException.ThrowException("Church", "Conta bancária não encontrada para esta igreja.");

            // Agendamento: registra e deixa para o worker executar
            if (request.ScheduledFor.HasValue && request.ScheduledFor.Value.ToUniversalTime() > DateTime.UtcNow)
            {
                var scheduledTransfer = new TransferHistory
                {
                    ChurchId = churchId,
                    BankingInfoId = resolvedBankingInfoId.Value,
                    Amount = request.Amount ?? 0m,
                    RequestedAt = DateTime.UtcNow,
                    ScheduledFor = request.ScheduledFor.Value.ToUniversalTime(),
                    Status = "Pending",
                    Notes = request.Notes
                };

                _unitOfWork.TransferHistories.Create(scheduledTransfer);
                await _unitOfWork.CommitAsync();
                return true;
            }

            // Busca as doações disponíveis para repasse
            var donationsQuery = _unitOfWork.Donations.Query()
                .Include(d => d.Member)
                .Include(d => d.Payments)
                .Where(d => d.Member.ChurchId == churchId && d.IsTransferred == false &&
                            d.Payments.Any(p => p.PaymentStatus == PaymentStatus.Received.ToString() || p.PaymentStatus == "Received"));

            // Se for transferência por departamento, considerar apenas doações do departamento
            if (request.DepartmentId.HasValue)
                donationsQuery = donationsQuery.Where(d => d.DepartmentId == request.DepartmentId.Value);

            donationsQuery = donationsQuery.OrderBy(d => d.Date);

            var donations = await donationsQuery.ToListAsync(cancellationToken);

            decimal totalAvailable = donations.Sum(d => d.Amount);
            if (totalAvailable <= 0)
                ValidationException.ThrowException("Church", "Não há valor disponível para repasse.");

            var amountToTransferTotal = request.Amount.HasValue ? request.Amount.Value : totalAvailable;
            if (amountToTransferTotal <= 0)
                ValidationException.ThrowException("Church", "Valor inválido para repasse.");

            if (amountToTransferTotal > totalAvailable)
                ValidationException.ThrowException("Church", "Valor solicitado maior que o disponível para repasse.");

            var transferRecord = new TransferHistory
            {
                ChurchId = churchId,
                BankingInfoId = resolvedBankingInfoId.Value,
                Amount = amountToTransferTotal,
                RequestedAt = DateTime.UtcNow,
                Status = "Pending",
                Notes = request.Notes
            };
            _unitOfWork.TransferHistories.Create(transferRecord);
            await _unitOfWork.CommitAsync();

            try
            {
                object transferenciaRequest;
                if (!string.IsNullOrWhiteSpace(bankingInfo.PixKey))
                {
                    transferenciaRequest = new
                    {
                        value = amountToTransferTotal,
                        pixAddressKey = bankingInfo.PixKey,
                        pixAddressKeyType = bankingInfo.PixKeyType,
                        description = request.Notes
                    };
                }
                else
                {
                    transferenciaRequest = new
                    {
                        value = amountToTransferTotal,
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

                await _asaasWebClient.CriarTransferenciaAsync(transferenciaRequest);

                // Marca doações como transferidas até cobrir o valor
                decimal remaining = amountToTransferTotal;
                foreach (var donation in donations)
                {
                    if (remaining <= 0) break;

                    if (donation.Amount <= remaining)
                    {
                        donation.IsTransferred = true;
                        donation.TransferredAt = DateTime.UtcNow;
                        remaining -= donation.Amount;
                        _unitOfWork.Donations.Update(donation);
                    }
                    else
                    {
                        // Não suporta parcial por doação ainda; ficaria pendente restante
                        break;
                    }
                }

                transferRecord.Status = "Completed";
                transferRecord.CompletedAt = DateTime.UtcNow;
                transferRecord.FailureReason = null;
                _unitOfWork.TransferHistories.Update(transferRecord);

                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                transferRecord.Status = "Failed";
                transferRecord.FailureReason = ex.Message;
                _unitOfWork.TransferHistories.Update(transferRecord);
                await _unitOfWork.CommitAsync();
                throw;
            }
        }
    }
}