using BoletoNetCore.CartãoDeCredito;

namespace BoletoNetCore.WebAPI.Models
{
    /// <summary>
    /// Request para geração de cobrança (cartão ou boleto API).
    /// </summary>
    public class RequestGerarCobranca
    {
        public RequestCobranca RequestCobranca { get; set; }
    }
}
