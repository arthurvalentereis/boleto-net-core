using Newtonsoft.Json;

namespace BoletoNetCore
{
    /// <summary>
    /// Item de cobrança retornado pela API Asaas na listagem GET /v3/payments.
    /// DTO específico do banco Asaas; fica em Banco/Asaas/Models para organização.
    /// </summary>
    public class AsaasPaymentListItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("dateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("customer")]
        public string Customer { get; set; }

        [JsonProperty("subscription")]
        public string Subscription { get; set; }

        [JsonProperty("installment")]
        public string Installment { get; set; }

        [JsonProperty("checkoutSession")]
        public string CheckoutSession { get; set; }

        [JsonProperty("paymentLink")]
        public string PaymentLink { get; set; }

        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("netValue")]
        public decimal NetValue { get; set; }

        [JsonProperty("originalValue")]
        public decimal? OriginalValue { get; set; }

        [JsonProperty("interestValue")]
        public decimal? InterestValue { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("billingType")]
        public string BillingType { get; set; }

        [JsonProperty("canBePaidAfterDueDate")]
        public bool CanBePaidAfterDueDate { get; set; }

        [JsonProperty("pixTransaction")]
        public string PixTransaction { get; set; }

        [JsonProperty("pixQrCodeId")]
        public string PixQrCodeId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("dueDate")]
        public string DueDate { get; set; }

        [JsonProperty("originalDueDate")]
        public string OriginalDueDate { get; set; }

        [JsonProperty("paymentDate")]
        public string PaymentDate { get; set; }

        [JsonProperty("clientPaymentDate")]
        public string ClientPaymentDate { get; set; }

        [JsonProperty("installmentNumber")]
        public int? InstallmentNumber { get; set; }

        [JsonProperty("invoiceUrl")]
        public string InvoiceUrl { get; set; }

        [JsonProperty("invoiceNumber")]
        public string InvoiceNumber { get; set; }

        [JsonProperty("externalReference")]
        public string ExternalReference { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("anticipated")]
        public bool Anticipated { get; set; }

        [JsonProperty("anticipable")]
        public bool Anticipable { get; set; }

        [JsonProperty("creditDate")]
        public string CreditDate { get; set; }

        [JsonProperty("estimatedCreditDate")]
        public string EstimatedCreditDate { get; set; }

        [JsonProperty("transactionReceiptUrl")]
        public string TransactionReceiptUrl { get; set; }

        [JsonProperty("nossoNumero")]
        public string NossoNumero { get; set; }

        [JsonProperty("bankSlipUrl")]
        public string BankSlipUrl { get; set; }

        [JsonProperty("discount")]
        public Discount Discount { get; set; }

        [JsonProperty("fine")]
        public Fine Fine { get; set; }

        [JsonProperty("interest")]
        public Interest Interest { get; set; }
    }
}
