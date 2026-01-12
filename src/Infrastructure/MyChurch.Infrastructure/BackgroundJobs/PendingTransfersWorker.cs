using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mychurch.Common.WebClients.Asaas;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;

namespace MyChurch.Infrastructure.BackgroundJobs
{
    public class PendingTransfersWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PendingTransfersWorker> _logger;

        public PendingTransfersWorker(IServiceProvider serviceProvider, ILogger<PendingTransfersWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingTransfersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar retiradas pendentes.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcessPendingTransfersAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var asaas = scope.ServiceProvider.GetRequiredService<IAsaasWebClient>();

            var now = DateTime.UtcNow;

            var pending = await uow.TransferHistories.Query()
                .Where(t => t.Status == "Pending" && t.ScheduledFor != null && t.ScheduledFor <= now)
                .OrderBy(t => t.ScheduledFor)
                .Take(20)
                .ToListAsync(cancellationToken);

            foreach (var transfer in pending)
            {
                try
                {
                    if (!transfer.BankingInfoId.HasValue)
                        throw new InvalidOperationException("Transferência pendente sem BankingInfoId.");

                    var bankingInfo = await uow.BankingInfos.Query()
                        .FirstOrDefaultAsync(b => b.Id == transfer.BankingInfoId.Value && b.ChurchId == transfer.ChurchId, cancellationToken);

                    if (bankingInfo == null)
                        throw new InvalidOperationException("Conta bancária não encontrada para transferência pendente.");

                    // Se Amount foi salvo como 0, significa "transferir tudo" no agendamento
                    var donations = await uow.Donations.Query()
                        .Include(d => d.Member)
                        .Include(d => d.Payments)
                        .Where(d => d.Member.ChurchId == transfer.ChurchId && d.IsTransferred == false &&
                                    d.Payments.Any(p => p.PaymentStatus == PaymentStatus.Received.ToString() || p.PaymentStatus == "Received"))
                        .OrderBy(d => d.Date)
                        .ToListAsync(cancellationToken);

                    var totalAvailable = donations.Sum(d => d.Amount);
                    if (totalAvailable <= 0)
                        throw new InvalidOperationException("Não há valor disponível para repasse.");

                    var amountToTransfer = transfer.Amount <= 0 ? totalAvailable : transfer.Amount;
                    if (amountToTransfer > totalAvailable)
                        throw new InvalidOperationException("Valor agendado maior que o disponível.");

                    object transferenciaRequest;
                    if (!string.IsNullOrWhiteSpace(bankingInfo.PixKey))
                    {
                        transferenciaRequest = new
                        {
                            value = amountToTransfer,
                            pixAddressKey = bankingInfo.PixKey,
                            pixAddressKeyType = bankingInfo.PixKeyType,
                            description = transfer.Notes
                        };
                    }
                    else
                    {
                        transferenciaRequest = new
                        {
                            value = amountToTransfer,
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
                            description = transfer.Notes
                        };
                    }

                    await asaas.CriarTransferenciaAsync(transferenciaRequest);

                    decimal remaining = amountToTransfer;
                    foreach (var donation in donations)
                    {
                        if (remaining <= 0) break;

                        if (donation.Amount <= remaining)
                        {
                            donation.IsTransferred = true;
                            donation.TransferredAt = DateTime.UtcNow;
                            remaining -= donation.Amount;
                            uow.Donations.Update(donation);
                        }
                        else
                        {
                            break;
                        }
                    }

                    transfer.Amount = amountToTransfer;
                    transfer.Status = "Completed";
                    transfer.CompletedAt = DateTime.UtcNow;
                    transfer.FailureReason = null;

                    uow.TransferHistories.Update(transfer);
                    await uow.CommitAsync();
                }
                catch (Exception ex)
                {
                    transfer.Status = "Failed";
                    transfer.FailureReason = ex.Message;
                    uow.TransferHistories.Update(transfer);
                    await uow.CommitAsync();

                    _logger.LogError(ex, "Falha ao executar retirada agendada. TransferId: {TransferId}", transfer.Id);
                }
            }
        }
    }
}
