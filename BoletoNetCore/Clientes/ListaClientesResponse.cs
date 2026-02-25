using System.Collections.Generic;

namespace BoletoNetCore.Clientes
{
    /// <summary>
    /// Contrato genérico de resposta da listagem de clientes.
    /// Entidade normalizada para falar uma única linguagem entre os bancos; cada banco (Asaas, etc.)
    /// mapeia sua API para este modelo em IBancoOnlineRest.ListarClientes.
    /// </summary>
    public class ListaClientesResponse
    {
        public bool HasMore { get; set; }
        public int TotalCount { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public List<Customer> Data { get; set; } = new List<Customer>();
    }
}
