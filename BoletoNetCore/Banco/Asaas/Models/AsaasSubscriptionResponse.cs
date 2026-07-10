namespace BoletoNetCore
{
    public class AsaasSubscriptionResponse
    {
        public string id { get; set; }
        public string customer { get; set; }
        public string status { get; set; }
        public string billingType { get; set; }
        public string cycle { get; set; }
        public decimal value { get; set; }
        public string nextDueDate { get; set; }
        public string description { get; set; }
        public string externalReference { get; set; }
    }
}
