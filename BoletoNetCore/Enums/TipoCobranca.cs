namespace BoletoNetCore.Enums
{
    /// <summary>
    /// Define o tipo de cobrança a ser gerada.
    /// </summary>
    public enum TipoCobranca
    {
        /// <summary>Link de pagamento (checkout)</summary>
        LINK,

        /// <summary>Cartão de crédito</summary>
        CREDIT_CARD,

        /// <summary>Boleto via API do banco</summary>
        BOLETO_PIX,

        /// <summary>Boleto tradicional (CNAB)</summary>
        BOLETO,

        /// <summary>PIX</summary>
        PIX
    }
}
