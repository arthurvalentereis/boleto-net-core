using System.Collections.Generic;

namespace BoletoNetCore.Cobrancas
{
    /// <summary>
    /// Contrato genérico de resposta da listagem de cobranças.
    /// Entidade normalizada para falar uma única linguagem entre os bancos; cada banco (Asaas, etc.)
    /// mapeia sua API para este modelo em IBancoOnlineRest.ListarCobrancas.
    /// </summary>
    public class ListaCobrancasResponse
    {
        public bool HasMore { get; set; }
        public int TotalCount { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public List<CobrancaItemDto> Data { get; set; } = new List<CobrancaItemDto>();
    }
}
