namespace BoletoNetCore.Assinaturas
{
    public class AssinaturaItemResponse
    {
        public string Id { get; set; }
        public string Customer { get; set; }
        public string Status { get; set; }
        public string BillingType { get; set; }
        public string Cycle { get; set; }
        public decimal Value { get; set; }
        public string NextDueDate { get; set; }
        public string Description { get; set; }
        public string ExternalReference { get; set; }
        public string InvoiceUrl { get; set; }
        public string BankSlipUrl { get; set; }
        public string PaymentUrl { get; set; }
        public string FirstPaymentId { get; set; }
    }
}
