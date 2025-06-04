using System;
using System.Text.Json.Serialization;

namespace Mychurch.Common.WebClients.Asaas.Models.Responses
{
    public class AsaasPaymentResponseDto
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("customer")]
        public string Customer { get; set; }

        [JsonPropertyName("checkoutSession")]
        public string? CheckoutSession { get; set; }

        [JsonPropertyName("paymentLink")]
        public string? PaymentLink { get; set; }

        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("netValue")]
        public decimal NetValue { get; set; }

        [JsonPropertyName("originalValue")]
        public decimal? OriginalValue { get; set; }

        [JsonPropertyName("interestValue")]
        public decimal? InterestValue { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("billingType")]
        public string BillingType { get; set; }

        [JsonPropertyName("pixTransaction")]
        public string? PixTransaction { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime DueDate { get; set; }

        [JsonPropertyName("originalDueDate")]
        public DateTime OriginalDueDate { get; set; }

        [JsonPropertyName("paymentDate")]
        public DateTime? PaymentDate { get; set; }

        [JsonPropertyName("clientPaymentDate")]
        public DateTime? ClientPaymentDate { get; set; }

        [JsonPropertyName("installmentNumber")]
        public int? InstallmentNumber { get; set; }

        [JsonPropertyName("invoiceUrl")]
        public string? InvoiceUrl { get; set; }

        [JsonPropertyName("invoiceNumber")]
        public string? InvoiceNumber { get; set; }

        [JsonPropertyName("externalReference")]
        public string? ExternalReference { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("anticipated")]
        public bool Anticipated { get; set; }

        [JsonPropertyName("anticipable")]
        public bool Anticipable { get; set; }

        [JsonPropertyName("creditDate")]
        public DateTime? CreditDate { get; set; }

        [JsonPropertyName("estimatedCreditDate")]
        public DateTime? EstimatedCreditDate { get; set; }

        [JsonPropertyName("transactionReceiptUrl")]
        public string? TransactionReceiptUrl { get; set; }

        [JsonPropertyName("nossoNumero")]
        public string? NossoNumero { get; set; }

        [JsonPropertyName("bankSlipUrl")]
        public string? BankSlipUrl { get; set; }

        [JsonPropertyName("lastInvoiceViewedDate")]
        public DateTime? LastInvoiceViewedDate { get; set; }

        [JsonPropertyName("lastBankSlipViewedDate")]
        public DateTime? LastBankSlipViewedDate { get; set; }

        [JsonPropertyName("discount")]
        public DiscountDto Discount { get; set; }

        [JsonPropertyName("fine")]
        public FineDto Fine { get; set; }

        [JsonPropertyName("interest")]
        public InterestDto Interest { get; set; }

        [JsonPropertyName("postalService")]
        public bool PostalService { get; set; }

        [JsonPropertyName("custody")]
        public string? Custody { get; set; }

        [JsonPropertyName("escrow")]
        public string? Escrow { get; set; }

        [JsonPropertyName("refunds")]
        public string? Refunds { get; set; }
    }

    public class DiscountDto
    {
        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("limitDate")]
        public DateTime? LimitDate { get; set; }

        [JsonPropertyName("dueDateLimitDays")]
        public int DueDateLimitDays { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }

    public class FineDto
    {
        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }

    public class InterestDto
    {
        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}