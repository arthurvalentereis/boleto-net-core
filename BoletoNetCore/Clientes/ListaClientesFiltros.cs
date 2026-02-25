namespace BoletoNetCore.Clientes
{
    /// <summary>
    /// Contrato genérico de filtros para listagem de clientes.
    /// Entrada normalizada; cada banco traduz estes filtros para os parâmetros da sua API
    /// (ex.: Asaas usa em GET /v3/customers).
    /// </summary>
    public class ListaClientesFiltros
    {
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CpfCnpj { get; set; }
        public string GroupName { get; set; }
        public string ExternalReference { get; set; }
    }
}
