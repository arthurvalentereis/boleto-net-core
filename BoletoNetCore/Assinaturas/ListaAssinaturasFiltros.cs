namespace BoletoNetCore.Assinaturas
{
    public class ListaAssinaturasFiltros
    {
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public string Customer { get; set; }
        public string Status { get; set; }
        public string ExternalReference { get; set; }
    }
}
