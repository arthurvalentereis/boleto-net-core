using System;

namespace BoletoNetCore.CartãoDeCredito
{
    public class CreditCardResponse
    {
        public string CreditCardNumber { get; set; }
        public string CreditCardBrand { get; set; }
        public string CreditCardToken { get; set; }
    }
    public class PaymentBase
    {
        public string Object { get; set; }
        public string Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Customer { get; set; }
        public string PaymentLink { get; set; }
        public decimal Value { get; set; }
        public decimal NetValue { get; set; }
        public decimal? OriginalValue { get; set; }
        public decimal? InterestValue { get; set; }
        public string Description { get; set; }
        public string BillingType { get; set; }
        public bool CanBePaidAfterDueDate { get; set; }
        public string PixTransaction { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime OriginalDueDate { get; set; }
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
        public DateTime? LastInvoiceViewedDate { get; set; }
        public DateTime? LastBankSlipViewedDate { get; set; }
        public Discount Discount { get; set; }
        public Fine Fine { get; set; }
        public Interest Interest { get; set; }
        public bool PostalService { get; set; }
        public object Custody { get; set; }
        public object Refunds { get; set; }
        public Pix Pix { get; set; }
    }
    public class PaymentCreditCardResponse : PaymentBase
    {
        public DateTime? ConfirmedDate { get; set; }
        public CreditCardResponse CreditCardResponse { get; set; }
    }
    public class Pix
    {
        public string EncodedImage { get; set; }
        public string Payload { get; set; }
        public string ExpirationDate { get; set; }
    }
    public class BankSlip : PaymentBase
    {
        
    }
}
