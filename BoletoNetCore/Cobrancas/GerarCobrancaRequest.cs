using BoletoNetCore.CartãoDeCredito;
using BoletoNetCore.LinkPagamento;

namespace BoletoNetCore.Cobrancas
{
    /// <summary>
    /// Request unificado para geração de cobrança.
    /// Contém os dados necessários conforme o tipo de cobrança.
    /// </summary>
    public class GerarCobrancaRequest
    {
        /// <summary>Dados para geração de link de pagamento (usado quando TipoCobranca = LINK).</summary>
        public LinkPagamentoRequest LinkPagamento { get; set; }

        /// <summary>Dados para cobrança via cartão ou boleto API (usado quando TipoCobranca = CREDIT_CARD, BOLETO ou BOLETO_PIX).</summary>
        public RequestCobranca RequestCobranca { get; set; }
    }
}
