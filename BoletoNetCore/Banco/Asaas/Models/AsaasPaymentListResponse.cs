using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoletoNetCore
{
    /// <summary>
    /// Resposta da API Asaas para GET /v3/payments (lista paginada de cobranças).
    /// DTO específico do banco Asaas; fica em Banco/Asaas/Models para organização.
    /// </summary>
    public class AsaasPaymentListResponse
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("data")]
        public List<AsaasPaymentListItem> Data { get; set; }
    }
}
