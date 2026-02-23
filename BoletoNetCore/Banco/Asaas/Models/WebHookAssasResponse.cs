namespace BoletoNetCore
{
    public class Discount
    {
        public decimal value { get; set; }
        public object limitDate { get; set; }
        public int dueDateLimitDays { get; set; }
        public string type { get; set; }
    }

    public class Fine
    {
        public decimal value { get; set; }
        public string type { get; set; }
    }

    public class Interest
    {
        public decimal value { get; set; }
        public string type { get; set; }
    }

    public class Payment
    {
        public string @object { get; set; }
        public string id { get; set; }
        public string dateCreated { get; set; }
        public string customer { get; set; }
        public string subscription { get; set; }
        public string paymentLink { get; set; }
        public decimal value { get; set; }
        public double netValue { get; set; }
        public object originalValue { get; set; }
        public object interestValue { get; set; }
        public string description { get; set; }
        public string billingType { get; set; }
        public string confirmedDate { get; set; }
        public string pixTransaction { get; set; }
        public string pixQrCodeId { get; set; }
        public string status { get; set; }
        public string dueDate { get; set; }
        public string originalDueDate { get; set; }
        public string paymentDate { get; set; }
        public string clientPaymentDate { get; set; }
        public int? installmentNumber { get; set; }
        public string invoiceUrl { get; set; }
        public string invoiceNumber { get; set; }
        public string externalReference { get; set; }
        public bool deleted { get; set; }
        public bool anticipated { get; set; }
        public bool anticipable { get; set; }
        public string creditDate { get; set; }
        public string estimatedCreditDate { get; set; }
        public string transactionReceiptUrl { get; set; }
        public string nossoNumero { get; set; }
        public string bankSlipUrl { get; set; }
        public object lastInvoiceViewedDate { get; set; }
        public object lastBankSlipViewedDate { get; set; }
        public Discount discount { get; set; }
        public Fine fine { get; set; }
        public Interest interest { get; set; }
        public bool postalService { get; set; }
        public object custody { get; set; }
        public object refunds { get; set; }
    }
    public class WebHookAssasResponse
    {
        public string @event { get; set; }
        public Payment payment { get; set; }
    }
}
