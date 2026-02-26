using System.ComponentModel;
using System;

namespace BoletoNetCore
{
    [Serializable]
    [Browsable(false)]
    public class LinkPagamentoRequest
    {
        /// <summary>
        /// Construtor sem parâmetros para deserialização (ex.: API).
        /// </summary>
        public LinkPagamentoRequest()
        {
        }

        /// <summary>
        /// Construtor da Classe LinkPagamento
        /// </summary>
        /// <param name="banco"></param>
        public LinkPagamentoRequest(IBanco banco)
        {
            Banco = banco;
        }

        public IBanco Banco { get; set; }
        public string NomeLinkCobranca { get; set; }
        public string Descricao { get; set; }

        /// <summary>
        /// Boleto, CREDIT_CARD, PIX
        /// </summary>
        public string TipoCobranca { get; set; }

        /// <summary>
        /// DETACHED=Destacado,RECURRENT=Recorrente, INSTALLMENT=Parcelado
        /// </summary>
        public string FormaCobranca { get; set; }
        public decimal Valor { get; set; }

        /// <summary>
        /// Em caso de boleto, define a quantidade de dias úteis que o seu cliente poderá pagar o boleto após gerado
        /// </summary>
        public string DataVencimentoLimite { get; set; }
        public DateTime DataFinalLink { get; set; }

        /// <summary>
        /// WEEKLY, BIWEEKLY, MONTHLY, QUARTERLY, SEMIANNUALLY, YEARLY
        /// </summary>
        public string PeriodicidadeCobranca { get; set; }
        public string QuantidadeMaximaParcelamento { get; set; }
        public bool HabilitaNotificacao { get; set; }
        public string UrlPagamentoSucesso { get; set; }
        public bool RedicionarAutomaticamente { get; set; }
    }
}
