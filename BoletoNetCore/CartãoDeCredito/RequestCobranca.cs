namespace BoletoNetCore.CartãoDeCredito
{
    public class RequestCobranca
    {
        public string Customer { get; set; }  //Required
        /// <summary>
        /// Allowed:
        ///  UNDEFINED
        ///  BOLETO
        ///  CREDIT_CARD
        ///  PIX
        /// </summary>
        public string BillingType { get; set; }  //Required
        public decimal Value { get; set; }  //required
        public string DueDate { get; set; }  //required
        public string Description { get; set; }  //opcional
        public string ExternalReference { get; set; }  //opcional
        public int? InstallmentCount { get; set; }  //opcional
        public decimal? InstallmentValue { get; set; }  //opcional
        public bool? PostalService { get; set; } = false;//opcional
        public Discount Discount { get; set; } = new Discount(); //opcional
        public Fine Fine { get; set; } = new Fine();  //opcional
        public Interest Interest { get; set; } = new Interest();   //opcional
        public CallBack CallBack { get; set; } = new CallBack();//opcional
        public PaymentCreditCard CreditCard { get; set; } = new PaymentCreditCard();//opcional
        public CustomerInfo CustomerInfo { get; set; } = new CustomerInfo();//opcional
        public CreditCardHolderInfo CreditCardHolderInfo { get; set; } = new CreditCardHolderInfo();//opcional
        public string RemoteIp { get; set; }
    }
    public class Discount
    {
        public decimal Value { get; set; }  //
        public int DueDateLimitDays { get; set; }  //
        public string Type { get; set; }  //Fixed ou Percentege

    }
    public class Fine
    {
        public decimal Value { get; set; }  //
        public string Type { get; set; }  //Fixed ou Percentege

    }
    public class Interest
    {
        public decimal Value { get; set; }  //
    }
    public class CallBack
    {
        public string SuccessUrl { get; set; }  //
    }
    public class PaymentCreditCard
    {
        public string HolderName { get; set;}
        public string Number { get; set;}
        public string ExpiryMonth { get; set;}
        public string ExpiryYear { get; set;}
        public string Ccv { get; set;}
    }
    public class CustomerInfo
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string CpfCnpj { get; set; }
        public string PostalCode { get; set; }
        public string AddressNumber { get; set; }
        public string AddressComplement { get; set; }
        public string Phone { get; set; }
        public bool NotificationDisabled { get; set; } = true;
    }
    public class CreditCardHolderInfo
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string CpfCnpj { get; set; }
        public string PostalCode { get; set; }
        public string AddressNumber { get; set; }
        public string AddressComplement { get; set; }
        public string Phone { get; set; }
    }
}
