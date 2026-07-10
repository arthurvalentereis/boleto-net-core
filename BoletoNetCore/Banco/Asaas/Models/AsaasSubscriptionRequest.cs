using BoletoNetCore.CartãoDeCredito;

namespace BoletoNetCore
{
    public class AsaasSubscriptionRequest
    {
        public string customer { get; set; }
        public string billingType { get; set; }
        public decimal value { get; set; }
        public string nextDueDate { get; set; }
        public string cycle { get; set; }
        public string description { get; set; }
        public string externalReference { get; set; }
        public PaymentCreditCard creditCard { get; set; }
        public CreditCardHolderInfo creditCardHolderInfo { get; set; }
        public string remoteIp { get; set; }
    }
}
