using System;
using BoletoNetCore;

namespace BoletoNetCore.Cobrancas
{
    /// <summary>
    /// Contrato genérico de item de cobrança na listagem.
    /// Entidade normalizada; cada banco (ex.: Asaas em Banco/Asaas/Models) mapeia seu response para este DTO.
    /// </summary>
    public class CobrancaItemDto
    {
        public string Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Customer { get; set; }
        public string Subscription { get; set; }
        public string Installment { get; set; }
        public string CheckoutSession { get; set; }
        public string PaymentLink { get; set; }
        public decimal Value { get; set; }
        public decimal NetValue { get; set; }
        public decimal? OriginalValue { get; set; }
        public decimal? InterestValue { get; set; }
        public string Description { get; set; }
        public string BillingType { get; set; }
        public bool CanBePaidAfterDueDate { get; set; }
        public string PixTransaction { get; set; }
        public string PixQrCodeId { get; set; }
        public string Status { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? OriginalDueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? ClientPaymentDate { get; set; }
        public int? InstallmentNumber { get; set; }
        public string InvoiceUrl { get; set; }
        public string InvoiceNumber { get; set; }
        public string ExternalReference { get; set; }
        public bool Deleted { get; set; }
        public bool Anticipated { get; set; }
        public bool Anticipable { get; set; }
        public DateTime? CreditDate { get; set; }
        public DateTime? EstimatedCreditDate { get; set; }
        public string TransactionReceiptUrl { get; set; }
        public string NossoNumero { get; set; }
        public string BankSlipUrl { get; set; }
        public Discount Discount { get; set; }
        public Fine Fine { get; set; }
        public Interest Interest { get; set; }
    }
}
