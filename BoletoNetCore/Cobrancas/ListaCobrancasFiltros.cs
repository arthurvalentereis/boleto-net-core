namespace BoletoNetCore.Cobrancas
{
    /// <summary>
    /// Contrato genérico de filtros para listagem de cobranças.
    /// Entrada normalizada; cada banco traduz estes filtros para os parâmetros da sua API
    /// (ex.: Asaas usa em GET /v3/payments).
    /// </summary>
    public class ListaCobrancasFiltros
    {
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public string Customer { get; set; }
        public string CustomerGroupName { get; set; }
        public string BillingType { get; set; }
        public string Status { get; set; }
        public string Subscription { get; set; }
        public string Installment { get; set; }
        public string ExternalReference { get; set; }
        public string PaymentDate { get; set; }
        public string InvoiceStatus { get; set; }
        public string EstimatedCreditDate { get; set; }
        public bool? Anticipated { get; set; }
        public bool? Anticipable { get; set; }
        public string DateCreatedGe { get; set; }
        public string DateCreatedLe { get; set; }
        public string PaymentDateGe { get; set; }
        public string PaymentDateLe { get; set; }
        public string EstimatedCreditDateGe { get; set; }
        public string EstimatedCreditDateLe { get; set; }
        public string DueDateGe { get; set; }
        public string DueDateLe { get; set; }
        public string User { get; set; }
        public string CheckoutSession { get; set; }
        public string PixQrCodeId { get; set; }
    }
}
