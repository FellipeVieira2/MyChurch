using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyChurch.Application.Dtos
{
   
    public class AsaasWebhookPaymentDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("customer")]
        public string Customer { get; set; }

        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("netValue")]
        public decimal NetValue { get; set; }

        [JsonPropertyName("billingType")]
        public string BillingType { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("subscription")]
        public string Subscription { get; set; }

        [JsonPropertyName("installment")]
        public string Installment { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime DueDate { get; set; }

        [JsonPropertyName("originalDueDate")]
        public DateTime OriginalDueDate { get; set; }

        [JsonPropertyName("paymentDate")]
        public DateTime? PaymentDate { get; set; }

        [JsonPropertyName("invoiceUrl")]
        public string InvoiceUrl { get; set; }

        [JsonPropertyName("bankSlipUrl")]
        public string BankSlipUrl { get; set; }

        [JsonPropertyName("transactionReceiptUrl")]
        public string TransactionReceiptUrl { get; set; }

        [JsonPropertyName("creditCard")]
        public AsaasWebhookCreditCardDto CreditCard { get; set; }
    }
}

