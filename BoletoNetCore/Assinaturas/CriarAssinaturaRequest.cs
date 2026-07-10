using BoletoNetCore.CartãoDeCredito;

namespace BoletoNetCore.Assinaturas
{
    public class CriarAssinaturaRequest
    {
        public string Customer { get; set; }
        public CustomerInfo CustomerInfo { get; set; } = new CustomerInfo();
        public string BillingType { get; set; }
        public decimal Value { get; set; }
        public string Cycle { get; set; }
        public string NextDueDate { get; set; }
        public string Description { get; set; }
        public string ExternalReference { get; set; }
        public PaymentCreditCard CreditCard { get; set; }
        public CreditCardHolderInfo CreditCardHolderInfo { get; set; }
        public string RemoteIp { get; set; }
    }
}
