using System.Collections.Generic;

namespace BoletoNetCore.Assinaturas
{
    public class ListaAssinaturasResponse
    {
        public bool HasMore { get; set; }
        public int TotalCount { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public List<AssinaturaItemResponse> Data { get; set; } = new List<AssinaturaItemResponse>();
    }
}
